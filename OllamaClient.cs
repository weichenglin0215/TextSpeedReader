using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TextSpeedReader
{
    /// <summary>
    /// 與本機 Ollama (http://localhost:11434) 溝通的輕量級用戶端。
    /// 完全以標準 HttpClient + System.Text.Json 實作，不需任何額外套件或後台程序，
    /// 因為 Ollama 安裝後本身即以 HTTP REST 伺服器常駐 (ollama serve)。
    ///
    /// 使用的端點：
    ///   GET  /api/tags      → 取得已下載的模型清單
    ///   POST /api/generate  → 產生文字 (支援流式 stream)
    /// </summary>
    public static class OllamaClient
    {
        // 預設 (fallback) 位址：本機。
        // 使用 127.0.0.1 而非 localhost：在 Windows 上 localhost 會先解析到 IPv6 (::1)，
        // 若 Ollama 只監聽 IPv4，用戶端會先嘗試 ::1 失敗再退回 IPv4，造成明顯延遲。
        public const string DefaultBaseUrl = "http://127.0.0.1:11434";

        // 目前實際使用的位址。啟動時會優先嘗試 Windows 系統變數 OLLAMA_HOST；
        // 若未設定或無法正常使用，會退回 DefaultBaseUrl。
        public static string BaseUrl { get; private set; } = ResolveBaseUrl(out _envHostRaw);

        // 讀到的原始 OLLAMA_HOST 值 (供 LOG 顯示；null 表示未設定)
        private static readonly string? _envHostRaw;
        public static string? EnvHostRaw => _envHostRaw;

        // 是否正在使用來自環境變數的位址 (false 表示使用預設值)
        public static bool UsingEnvHost =>
            !string.Equals(BaseUrl, DefaultBaseUrl, StringComparison.OrdinalIgnoreCase);

        // 是否已因連線失敗而退回預設值 (避免重複退回)
        private static bool _fellBackToDefault;
        public static bool FellBackToDefault => _fellBackToDefault;

        /// <summary>
        /// 依 Windows 系統變數 OLLAMA_HOST 解析出 Ollama 的基底網址。
        /// 支援格式：
        ///   "127.0.0.1:11434"、"192.168.0.10"、"http://host:port"、":11500" 等。
        /// 若未設定或格式無法解析，回傳 DefaultBaseUrl。
        /// </summary>
        private static string ResolveBaseUrl(out string? rawValue)
        {
            // 依序檢查 Process / User / Machine 範圍
            string? raw = Environment.GetEnvironmentVariable("OLLAMA_HOST")
                ?? Environment.GetEnvironmentVariable("OLLAMA_HOST", EnvironmentVariableTarget.User)
                ?? Environment.GetEnvironmentVariable("OLLAMA_HOST", EnvironmentVariableTarget.Machine);

            rawValue = string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
            if (rawValue == null) return DefaultBaseUrl;

            try
            {
                string value = rawValue;

                // 補上 scheme
                if (!value.Contains("://")) value = "http://" + value;

                var uri = new Uri(value, UriKind.Absolute);

                // 取出主機；0.0.0.0 / 空白 對用戶端而言不可連線，改用 127.0.0.1
                string host = uri.Host;
                if (string.IsNullOrEmpty(host) || host == "0.0.0.0" || host == "::")
                    host = "127.0.0.1";

                int port = uri.IsDefaultPort || uri.Port <= 0 ? 11434 : uri.Port;
                string scheme = string.IsNullOrEmpty(uri.Scheme) ? "http" : uri.Scheme;

                return $"{scheme}://{host}:{port}";
            }
            catch
            {
                // 格式無法解析 → 退回預設值
                return DefaultBaseUrl;
            }
        }

        // 連線失敗時，若目前用的是環境變數位址，退回預設值 (只退一次)。回傳 true 表示有退回、可重試。
        private static bool TryFallbackToDefault()
        {
            if (_fellBackToDefault) return false;
            if (!UsingEnvHost) return false;
            BaseUrl = DefaultBaseUrl;
            _fellBackToDefault = true;
            return true;
        }

        // 共用一個 HttpClient (WinForms 生命週期內長存)；逾時放寬，因為大模型生成可能很久。
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan
        };

        /// <summary>
        /// AI 大模型的取樣參數 (對應 /api/generate 的 options 物件)。
        /// 由「AI參數下拉選單」的參數表填入。
        /// </summary>
        public class GenerateOptions
        {
            public bool Stream { get; set; } = true;
            public double Temperature { get; set; } = 0.9;
            public int NumPredict { get; set; } = 2048;
            public int NumCtx { get; set; } = 8192;
            public double RepeatPenalty { get; set; } = 1.1;
            public int TopK { get; set; } = 40;
            public double TopP { get; set; } = 0.9;

            // 保留大模型於 VRAM 的時間 (-1 = 永久保留，"10m" = 10 分鐘)
            public string KeepAlive { get; set; } = "10m";

            // 是否啟用「思考(thinking)」推理。null = 不指定 (用模型預設)；
            // 對支援思考的模型 (如 qwen3.5) 設 false 可略過思考、大幅加速。
            public bool? Think { get; set; } = null;
        }

        // 實際發出 GET /api/tags 並回傳原始 JSON 字串
        private static async Task<string> GetTagsJsonAsync(CancellationToken cancel)
        {
            using var resp = await _http.GetAsync($"{BaseUrl}/api/tags", cancel).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        }

        /// <summary>
        /// 呼叫 GET /api/tags，取得目前 Ollama 已下載的模型名稱清單。
        /// </summary>
        public static async Task<List<string>> GetModelsAsync(CancellationToken cancel = default)
        {
            string json;
            try
            {
                json = await GetTagsJsonAsync(cancel).ConfigureAwait(false);
            }
            catch (Exception ex) when (
                ex is HttpRequestException or TaskCanceledException
                && !cancel.IsCancellationRequested
                && TryFallbackToDefault())
            {
                // 環境變數位址連不上 → 已退回 localhost，重試一次
                json = await GetTagsJsonAsync(cancel).ConfigureAwait(false);
            }

            var result = new List<string>();

            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("models", out JsonElement models)
                && models.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement m in models.EnumerateArray())
                {
                    if (m.TryGetProperty("name", out JsonElement name) && name.ValueKind == JsonValueKind.String)
                    {
                        string? n = name.GetString();
                        if (!string.IsNullOrEmpty(n)) result.Add(n);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 已載入 (running) 模型的資源占用資訊 (來自 /api/ps)。
        /// </summary>
        public class LoadedModelInfo
        {
            public string Name = "";
            public long Size;      // 模型執行時總大小 (bytes)
            public long SizeVram;  // 其中位於 GPU VRAM 的大小 (bytes)

            // 位於 GPU 的百分比 (0~100)；100 表示完全在 GPU，0 表示完全在 CPU
            public int GpuPercent => Size <= 0 ? 0 : (int)Math.Round(SizeVram * 100.0 / Size);
        }

        /// <summary>
        /// 呼叫 GET /api/ps，取得目前常駐 (已載入) 模型的 CPU/GPU 放置情況。
        /// 用來診斷「高 CPU / 慢 prefill」是否因模型被卸載到 CPU 造成。
        /// </summary>
        public static async Task<List<LoadedModelInfo>> GetLoadedModelsAsync(CancellationToken cancel = default)
        {
            var result = new List<LoadedModelInfo>();
            using var resp = await _http.GetAsync($"{BaseUrl}/api/ps", cancel).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            string json = await resp.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            using JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("models", out JsonElement models)
                && models.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement m in models.EnumerateArray())
                {
                    var info = new LoadedModelInfo();
                    if (m.TryGetProperty("name", out var n) && n.ValueKind == JsonValueKind.String)
                        info.Name = n.GetString() ?? "";
                    if (m.TryGetProperty("size", out var s) && s.TryGetInt64(out long sv))
                        info.Size = sv;
                    if (m.TryGetProperty("size_vram", out var v) && v.TryGetInt64(out long vv))
                        info.SizeVram = vv;
                    result.Add(info);
                }
            }
            return result;
        }

        /// <summary>
        /// 卸載指定模型 (POST /api/generate 且 keep_alive=0、prompt 為空)。
        /// 模型卸載時其 KV cache (含上一次請求殘留的前綴快取) 會一併釋放，
        /// 因此可用來確保下一篇分析從「完全空白」的狀態重新開始。
        /// </summary>
        public static async Task UnloadModelAsync(string model, CancellationToken cancel = default)
        {
            var payload = new Dictionary<string, object?>
            {
                ["model"] = model,
                ["prompt"] = "",
                ["stream"] = false,
                ["keep_alive"] = 0
            };

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/generate")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            using var resp = await _http.SendAsync(req, cancel).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// /api/generate 完成時回傳的計時統計 (單位：奈秒 ns)。用來精確定位效能瓶頸。
        /// </summary>
        public class GenerateResult
        {
            public string Text = "";
            public long TotalDurationNs;      // 總耗時
            public long LoadDurationNs;       // 模型載入耗時
            public long PromptEvalCount;      // 提示詞 token 數
            public long PromptEvalDurationNs; // 提示詞評估 (prefill) 耗時
            public long EvalCount;            // 生成 token 數
            public long EvalDurationNs;       // 生成耗時

            public double TotalSeconds => TotalDurationNs / 1e9;
            public double LoadSeconds => LoadDurationNs / 1e9;
            public double PromptEvalSeconds => PromptEvalDurationNs / 1e9;
            public double EvalSeconds => EvalDurationNs / 1e9;
            public double PromptTokensPerSec => PromptEvalDurationNs > 0 ? PromptEvalCount / (PromptEvalDurationNs / 1e9) : 0;
            public double EvalTokensPerSec => EvalDurationNs > 0 ? EvalCount / (EvalDurationNs / 1e9) : 0;
        }

        /// <summary>
        /// 呼叫 POST /api/generate 以流式方式產生文字。
        /// 每收到一段文字就透過 <paramref name="onToken"/> 回呼 (可用於即時追加到 LOG 與結果)。
        /// 回傳完整文字與伺服器端的計時統計。
        /// </summary>
        /// <param name="model">模型名稱 (例如 "huihui_ai/qwen3.5-abliterated:9b")。</param>
        /// <param name="prompt">完整提示詞 (文章類型 .md + 使用者指令 + 文章本文)。</param>
        /// <param name="options">取樣參數。</param>
        /// <param name="seed">隨機種子 (用於 LOG 顯示與重現)。</param>
        /// <param name="onToken">每段流式文字的回呼。</param>
        /// <param name="cancel">取消權杖。</param>
        public static async Task<GenerateResult> GenerateStreamAsync(
            string model,
            string prompt,
            GenerateOptions options,
            int seed,
            Action<string> onToken,
            Action<string>? onThinking = null,
            CancellationToken cancel = default)
        {
            // 組出請求主體 (手動組 JSON 物件，確保 options 內欄位名稱與 Ollama 一致)
            var payload = new Dictionary<string, object?>
            {
                ["model"] = model,
                ["prompt"] = prompt,
                ["stream"] = options.Stream,
                ["keep_alive"] = options.KeepAlive,
                ["options"] = new Dictionary<string, object?>
                {
                    ["temperature"] = options.Temperature,
                    ["num_predict"] = options.NumPredict,
                    ["num_ctx"] = options.NumCtx,
                    ["repeat_penalty"] = options.RepeatPenalty,
                    ["top_k"] = options.TopK,
                    ["top_p"] = options.TopP,
                    ["seed"] = seed
                }
            };

            // think 為 /api/generate 的頂層參數 (不在 options 內)；只在明確指定時才送出
            if (options.Think.HasValue) payload["think"] = options.Think.Value;

            string body = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/generate")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            // ResponseHeadersRead：一收到標頭就回傳，之後逐行讀取串流，達成即時流式
            using var resp = await _http.SendAsync(
                req, HttpCompletionOption.ResponseHeadersRead, cancel).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();

            var full = new StringBuilder();
            var stats = new GenerateResult();
            using Stream stream = await resp.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Ollama 以 NDJSON (一行一個 JSON 物件) 回傳
            string? line;
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                cancel.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(line);
                    JsonElement root = doc.RootElement;

                    // 思考(thinking) token：即時回報但不併入最終文字 (儲存檔只留分析結果)
                    if (root.TryGetProperty("thinking", out JsonElement thinkTok)
                        && thinkTok.ValueKind == JsonValueKind.String)
                    {
                        string tp = thinkTok.GetString() ?? string.Empty;
                        if (tp.Length > 0) onThinking?.Invoke(tp);
                    }

                    if (root.TryGetProperty("response", out JsonElement respTok)
                        && respTok.ValueKind == JsonValueKind.String)
                    {
                        string piece = respTok.GetString() ?? string.Empty;
                        if (piece.Length > 0)
                        {
                            full.Append(piece);
                            onToken?.Invoke(piece);
                        }
                    }

                    if (root.TryGetProperty("done", out JsonElement done)
                        && done.ValueKind == JsonValueKind.True)
                    {
                        // 完成訊息內含伺服器端計時統計
                        stats.TotalDurationNs = GetLong(root, "total_duration");
                        stats.LoadDurationNs = GetLong(root, "load_duration");
                        stats.PromptEvalCount = GetLong(root, "prompt_eval_count");
                        stats.PromptEvalDurationNs = GetLong(root, "prompt_eval_duration");
                        stats.EvalCount = GetLong(root, "eval_count");
                        stats.EvalDurationNs = GetLong(root, "eval_duration");
                        break;
                    }
                }
                catch (JsonException)
                {
                    // 單行解析失敗不中斷整體流程 (極少數殘缺行)
                }
            }

            stats.Text = full.ToString();
            return stats;
        }

        private static long GetLong(JsonElement obj, string name)
            => obj.TryGetProperty(name, out var e) && e.TryGetInt64(out long v) ? v : 0;
    }
}

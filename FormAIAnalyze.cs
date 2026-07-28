using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// AI 分析文章選項彈窗：利用地端 Ollama + AI 大模型，分析 listViewFile 內的所有文章。
    /// 溝通方式為標準 HttpClient 直連 http://localhost:11434 (見 OllamaClient)，不需任何後台程序。
    /// </summary>
    public partial class FormAIAnalyze : Form
    {
        // 待分析的文章完整路徑清單 (由主視窗依 listViewFile 收集後注入)
        private readonly List<string> _articles;

        // Prompt 目錄 (存放各「文章類型」的 .md 提示詞)，位於執行檔旁
        private readonly string _promptDir;

        // 內建的 AI 參數預設表 (對應需求提供的參數表 CSV)
        private readonly List<ParamPreset> _paramPresets;

        // 執行中的取消來源
        private CancellationTokenSource? _cts;
        private bool _running;

        private readonly Random _rnd = new Random();

        // 防止「全選/清除/反選」批次操作期間，ItemCheck 逐一觸發造成計數頻繁更新
        private bool _suppressItemCheck;

        // LOG 緩衝區與定時刷新計時器 (降低 UI 更新頻率，避免閃爍與 CPU 飆高)
        private readonly StringBuilder _logBuffer = new StringBuilder();
        private readonly object _logLock = new object();
        private System.Windows.Forms.Timer? _logTimer;

        /// <summary>
        /// 檔案清單項目：顯示檔名，內部保留完整路徑。
        /// </summary>
        private class FileItem
        {
            public string FullPath = "";
            public string Display = "";
            public override string ToString() => Display;
        }

        /// <summary>
        /// AI 參數預設 (對應 /api/generate 的取樣參數)。
        /// </summary>
        private class ParamPreset
        {
            public string Name = "";
            public bool Stream;
            public double Temperature;
            public int NumPredict;
            public int NumCtx;
            public double RepeatPenalty;
            public int TopK;
            public double TopP;
            public override string ToString() => Name;
        }

        /// <summary>
        /// 建構子。
        /// </summary>
        /// <param name="articleFilePaths">要分析的文章完整路徑清單。</param>
        public FormAIAnalyze(List<string> articleFilePaths)
        {
            InitializeComponent();
            _articles = articleFilePaths ?? new List<string>();
            _promptDir = Path.Combine(AppContext.BaseDirectory, "Prompt");
            _paramPresets = BuildParamPresets();
        }

        private async void FormAIAnalyze_Load(object? sender, EventArgs e)
        {
            // 啟動 LOG 定時刷新計時器 (每 120ms 一次)
            _logTimer = new System.Windows.Forms.Timer { Interval = 120 };
            _logTimer.Tick += (s, ev) => FlushLog();
            _logTimer.Start();
            this.FormClosed += (s, ev) =>
            {
                _logTimer?.Stop();
                _cts?.Cancel();
            };

            // 高度設為螢幕工作區的 90% (寬度沿用設計時的 200% 版面)；調整後重新置中
            Rectangle wa = (Screen.FromControl(this) ?? Screen.PrimaryScreen!).WorkingArea;
            this.Height = (int)(wa.Height * 0.9);
            this.Left = wa.Left + (wa.Width - this.Width) / 2;
            this.Top = wa.Top + (wa.Height - this.Height) / 2;

            // 填入右側檔案勾選列表 (預設全部勾選)
            _suppressItemCheck = true;
            checkedListBoxFiles.Items.Clear();
            foreach (string path in _articles)
            {
                checkedListBoxFiles.Items.Add(
                    new FileItem { FullPath = path, Display = Path.GetFileName(path) }, true);
            }
            _suppressItemCheck = false;
            UpdateFileCount();

            // 填入 AI 參數下拉選單
            comboBoxParams.Items.Clear();
            foreach (var p in _paramPresets) comboBoxParams.Items.Add(p);
            if (comboBoxParams.Items.Count > 0) comboBoxParams.SelectedIndex = 0;

            // 填入文章類型下拉選單 (掃描 Prompt\*.md)
            LoadPromptTypes(null);

            AppendLog($"待分析文章數量：{_articles.Count} 篇");
            AppendLog($"Prompt 目錄：{_promptDir}");
            if (OllamaClient.EnvHostRaw != null)
                AppendLog($"偵測到系統變數 OLLAMA_HOST = {OllamaClient.EnvHostRaw}");
            else
                AppendLog("未偵測到系統變數 OLLAMA_HOST，使用預設位址。");
            AppendLog($"Ollama 位址：{OllamaClient.BaseUrl}");

            // 載入 Ollama 已下載的模型
            await RefreshModelsAsync();
        }

        // 依需求提供的參數表建立預設清單
        private static List<ParamPreset> BuildParamPresets()
        {
            // name, stream, temperature, num_predict, num_ctx, repeat_penalty, top_k, top_p
            string[] rows =
            {
                "LLM大模型參數表-8192-低溫純分析,true,0.4,2048,8192,1.1,40,0.9",
                "LLM大模型參數表-8192-標準,true,0.9,2048,8192,1.1,40,0.9",
                "LLM大模型參數表-8192-熱情,true,1.2,2048,8192,1.2,80,1",
                "LLM大模型參數表-16384-低溫純分析,true,0.4,4096,16384,1.1,40,0.9",
                "LLM大模型參數表-16384-標準,true,0.9,4096,16384,1.1,40,0.9",
                "LLM大模型參數表-16384-熱情,true,1.2,4096,16384,1.1,80,1",
                "LLM大模型參數表-32768-低溫純分析,true,0.4,4096,32768,1.1,40,0.9",
                "LLM大模型參數表-32768-標準,true,0.9,4096,32768,1.1,40,0.9",
                "LLM大模型參數表-32768-熱情,true,1.2,4096,32768,1.1,80,1",
                "LLM大模型參數表-65536-低溫純分析,true,0.4,4096,65536,1.1,40,0.9",
                "LLM大模型參數表-65536-標準,true,0.9,4096,65536,1.1,40,0.9",
                "LLM大模型參數表-65536-熱情,true,1.2,4096,65536,1.1,80,1",
            };

            var list = new List<ParamPreset>();
            var ci = CultureInfo.InvariantCulture;
            foreach (string row in rows)
            {
                string[] c = row.Split(',');
                list.Add(new ParamPreset
                {
                    Name = c[0],
                    Stream = bool.Parse(c[1]),
                    Temperature = double.Parse(c[2], ci),
                    NumPredict = int.Parse(c[3], ci),
                    NumCtx = int.Parse(c[4], ci),
                    RepeatPenalty = double.Parse(c[5], ci),
                    TopK = int.Parse(c[6], ci),
                    TopP = double.Parse(c[7], ci),
                });
            }
            return list;
        }

        // 掃描 Prompt 目錄，填入文章類型下拉選單；selectName 可指定載入後要選取的檔名 (不含副檔名)
        private void LoadPromptTypes(string? selectName)
        {
            comboBoxPromptType.Items.Clear();
            try
            {
                if (!Directory.Exists(_promptDir))
                    Directory.CreateDirectory(_promptDir);

                foreach (string file in Directory.GetFiles(_promptDir, "*.md"))
                {
                    comboBoxPromptType.Items.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[錯誤] 讀取 Prompt 目錄失敗：{ex.Message}");
            }

            if (comboBoxPromptType.Items.Count > 0)
            {
                int idx = 0;
                if (!string.IsNullOrEmpty(selectName))
                {
                    int found = comboBoxPromptType.Items.IndexOf(selectName);
                    if (found >= 0) idx = found;
                }
                comboBoxPromptType.SelectedIndex = idx;
            }
        }

        // 「重新整理」模型清單
        private async void buttonRefreshModels_Click(object? sender, EventArgs e)
        {
            await RefreshModelsAsync();
        }

        private async Task RefreshModelsAsync()
        {
            buttonRefreshModels.Enabled = false;
            try
            {
                AppendLog($">> 正在向 Ollama ({OllamaClient.BaseUrl}) 取得已下載的模型清單...");
                List<string> models = await OllamaClient.GetModelsAsync();

                // 若剛才因環境變數位址連不上而退回預設值，提示使用者
                if (OllamaClient.FellBackToDefault)
                    AppendLog($">> 系統變數位址無法連線，已改用預設位址：{OllamaClient.BaseUrl}");

                string? previous = comboBoxModel.SelectedItem as string;
                comboBoxModel.Items.Clear();
                foreach (string m in models) comboBoxModel.Items.Add(m);

                if (comboBoxModel.Items.Count > 0)
                {
                    int idx = previous != null ? comboBoxModel.Items.IndexOf(previous) : -1;
                    comboBoxModel.SelectedIndex = idx >= 0 ? idx : 0;
                    AppendLog($">> 取得 {models.Count} 個模型。");
                }
                else
                {
                    AppendLog(">> 未取得任何模型，請確認 Ollama 是否已啟動並下載模型。");
                }
            }
            catch (Exception ex)
            {
                AppendLog($"[錯誤] 無法連線 Ollama ({OllamaClient.BaseUrl})：{ex.Message}");
                MessageBox.Show(
                    $"無法連線到 Ollama ({OllamaClient.BaseUrl})。\n請確認 Ollama 已啟動 (ollama serve)。\n\n{ex.Message}",
                    "連線失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                buttonRefreshModels.Enabled = true;
            }
        }

        // 「編輯」文章類型 .md
        private void buttonEditPrompt_Click(object? sender, EventArgs e)
        {
            if (comboBoxPromptType.SelectedItem is not string name)
            {
                MessageBox.Show("請先選擇一個文章類型。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = Path.Combine(_promptDir, name + ".md");
            using var editor = new FormMarkdownEditor(path, _promptDir);
            if (editor.ShowDialog(this) == DialogResult.OK && editor.SavedFilePath != null)
            {
                string savedName = Path.GetFileNameWithoutExtension(editor.SavedFilePath);
                LoadPromptTypes(savedName);
                AppendLog($">> 已儲存提示詞：{editor.SavedFilePath}");
            }
        }

        // 「🗑清除」：清空 LOG 文字框
        private void buttonClearLog_Click(object? sender, EventArgs e)
        {
            // 連同尚未刷新的緩衝區一起清掉，避免清除後又被下一次 FlushLog 補寫回來
            lock (_logLock)
            {
                _logBuffer.Clear();
            }
            textBoxLog.Clear();
        }

        // 「全選」
        private void buttonSelectAll_Click(object? sender, EventArgs e) => SetAllChecked(true);

        // 「清除選取」
        private void buttonClearSelection_Click(object? sender, EventArgs e) => SetAllChecked(false);

        // 「反選」
        private void buttonInvertSelection_Click(object? sender, EventArgs e)
        {
            _suppressItemCheck = true;
            for (int i = 0; i < checkedListBoxFiles.Items.Count; i++)
                checkedListBoxFiles.SetItemChecked(i, !checkedListBoxFiles.GetItemChecked(i));
            _suppressItemCheck = false;
            UpdateFileCount();
        }

        private void SetAllChecked(bool value)
        {
            _suppressItemCheck = true;
            for (int i = 0; i < checkedListBoxFiles.Items.Count; i++)
                checkedListBoxFiles.SetItemChecked(i, value);
            _suppressItemCheck = false;
            UpdateFileCount();
        }

        // 使用者手動勾/取消勾選 → 更新計數 (ItemCheck 在狀態變更前觸發，故延後讀取)
        private void checkedListBoxFiles_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            if (_suppressItemCheck) return;
            BeginInvoke(new Action(UpdateFileCount));
        }

        private void UpdateFileCount()
        {
            labelFileCount.Text = $"已勾選 {checkedListBoxFiles.CheckedItems.Count} / {checkedListBoxFiles.Items.Count}";
        }

        // 「執行AI分析」
        private async void buttonRun_Click(object? sender, EventArgs e)
        {
            if (_running) return;

            if (comboBoxModel.SelectedItem is not string model || string.IsNullOrWhiteSpace(model))
            {
                MessageBox.Show("請先選擇 AI 大模型。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (comboBoxParams.SelectedItem is not ParamPreset preset)
            {
                MessageBox.Show("請先選擇 AI 參數。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (comboBoxPromptType.SelectedItem is not string promptType)
            {
                MessageBox.Show("請先選擇文章類型。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // 收集勾選的檔案
            var selected = new List<string>();
            foreach (FileItem item in checkedListBoxFiles.CheckedItems)
                selected.Add(item.FullPath);

            if (selected.Count == 0)
            {
                MessageBox.Show("請至少勾選一篇要分析的文章。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 讀取文章類型提示詞內容
            string promptTypePath = Path.Combine(_promptDir, promptType + ".md");
            string promptTypeContent;
            try
            {
                promptTypeContent = File.ReadAllText(promptTypePath, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取提示詞失敗：\n{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string userInstruction = textBoxUserInstruction.Text;

            SetRunning(true);
            _cts = new CancellationTokenSource();

            try
            {
                await RunAnalysisAsync(selected, model, preset, promptTypeContent, userInstruction, _cts.Token);
                AppendLog("==================================================");
                AppendLog(">> 全部文章分析完成。");
            }
            catch (OperationCanceledException)
            {
                AppendLog(">> 已取消。");
            }
            catch (Exception ex)
            {
                AppendLog($"[錯誤] 分析中止：{ex.Message}");
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                SetRunning(false);
            }
        }

        private async Task RunAnalysisAsync(
            List<string> articles, string model, ParamPreset preset, string promptTypeContent,
            string userInstruction, CancellationToken cancel)
        {
            int index = 0;
            foreach (string articlePath in articles)
            {
                cancel.ThrowIfCancellationRequested();
                index++;

                // 每篇都重新建立參數物件，避免上一篇的自動調整 (例如下修 num_predict) 影響本篇
                var options = new OllamaClient.GenerateOptions
                {
                    Stream = preset.Stream,
                    Temperature = preset.Temperature,
                    NumPredict = preset.NumPredict,
                    NumCtx = preset.NumCtx,
                    RepeatPenalty = preset.RepeatPenalty,
                    TopK = preset.TopK,
                    TopP = preset.TopP,
                    KeepAlive = "10m",
                    Think = checkBoxThink.Checked  // 由使用者勾選決定是否啟用思考
                };

                string fileName = Path.GetFileName(articlePath);
                AppendLog("==================================================");
                AppendLog($"[{Now()}] ({index}/{articles.Count}) 分析「{fileName}」");
                UpdateStatus($"分析中 ({index}/{articles.Count})：{fileName}");

                // 讀取文章內容 (自動偵測編碼)
                string articleContent;
                try
                {
                    Encoding enc = JTextFileLib.DetectEncoding(articlePath);
                    articleContent = await Task.Run(() => File.ReadAllText(articlePath, enc), cancel);
                }
                catch (Exception ex)
                {
                    AppendLog($"[錯誤] 讀取「{fileName}」失敗，略過：{ex.Message}");
                    continue;
                }

                // 組出完整提示詞：文章類型 .md + 使用者指令 + 文章本文
                var sb = new StringBuilder();
                sb.Append(promptTypeContent);
                if (!string.IsNullOrWhiteSpace(userInstruction))
                {
                    sb.Append("\n\n## 使用者額外指令\n");
                    sb.Append(userInstruction.Trim());
                }
                sb.Append("\n\n## 待分析文章內容\n");
                sb.Append(articleContent);
                string prompt = sb.ToString();

                int seed = _rnd.Next(1, int.MaxValue);

                // === 每篇獨立：先卸載模型，清空 KV cache 與前綴快取 ===
                // Ollama 會沿用上一次請求的 KV cache 做前綴比對 (本功能每篇的提示詞範本前綴完全相同)，
                // 卸載模型是唯一能保證下一篇「從完全空白開始」的作法。
                if (checkBoxFreshContext.Checked)
                {
                    try
                    {
                        AppendLog(">> 正在清除模型記憶 (卸載模型以確保本篇獨立分析)...");
                        await OllamaClient.UnloadModelAsync(model, cancel);
                        await Task.Delay(300, cancel);   // 給伺服器一點時間完成釋放
                        AppendLog(">> 模型記憶已清除，本篇將從全新狀態開始分析。");
                    }
                    catch (Exception ex)
                    {
                        AppendLog($">> [警告] 清除模型記憶失敗 (不影響繼續執行)：{ex.Message}");
                    }
                }

                // === 上下文預算檢查 ===
                // 提示詞 token + 生成 token 若超過 num_ctx，llama.cpp 會啟動 context shifting，
                // 把最舊的 token (也就是你的分析指令與輸出格式要求) 丟棄，
                // 造成格式錯亂、重複輸出、內容失控。這裡先估算並在必要時自動下修 num_predict。
                int estPromptTokens = EstimateTokens(prompt);
                int budget = options.NumCtx - estPromptTokens - 256;  // 保留 256 緩衝
                if (budget < options.NumPredict)
                {
                    int original = options.NumPredict;
                    if (budget < 512)
                    {
                        AppendLog($">> [警告] 提示詞估計約 {estPromptTokens} tokens，已接近或超出上下文視窗 " +
                                  $"({options.NumCtx})，本篇極可能發生格式錯亂或內容重複。" +
                                  "建議改用 num_ctx 更大的參數預設，或縮短文章。");
                    }
                    else
                    {
                        options.NumPredict = budget;
                        AppendLog($">> [自動調整] 提示詞估計約 {estPromptTokens} tokens，" +
                                  $"為避免超出上下文視窗 ({options.NumCtx}) 導致格式錯亂，" +
                                  $"num_predict 由 {original} 下修為 {options.NumPredict}。");
                    }
                }

                // 印出參數資訊 (仿需求 LOG 範例格式)
                AppendLog(">> 正在呼叫 Ollama 產生分析結果 (請稍候)...");
                AppendLog($">>>> 模型: {model}");
                AppendLog($">>>> 以流式回傳結果: {options.Stream}");
                AppendLog($">>>> VRAM保有大模型(keep_alive): {options.KeepAlive}");
                AppendLog($">>>> 溫度(Temperature): {options.Temperature}");
                AppendLog($">>>> 預測長度(num_predict): {options.NumPredict}");
                AppendLog($">>>> 上下文視窗(num_ctx): {options.NumCtx}");
                AppendLog($">>>> 重複懲罰(repeat_penalty): {options.RepeatPenalty}");
                AppendLog($">>>> Top-K: {options.TopK}");
                AppendLog($">>>> Top-P: {options.TopP}");
                AppendLog($">>>> 隨機種子(seed): {seed}");
                AppendLog($">>>> 提示詞字數(Prompt Length): {prompt.Length} characters");
                AppendLog("");
                AppendLog("=== 傳遞給 AI 的完整提示詞 ===");
                AppendLog(prompt);
                AppendLog("=====================");
                AppendLog(">>>> 正在發送 POST 請求至 Ollama...");
                AppendLog("(O)(O)(O)(O)(O)(O)(O)(O)(O)(O)流式生成文字開始(O)(O)(O)(O)(O)(O)(O)(O)(O)(O)");

                // 量測「首個 token 回應時間」與總耗時，用以判斷效能瓶頸
                var sw = Stopwatch.StartNew();
                bool firstAny = true;
                bool thinkingHeaderShown = false;
                bool responseHeaderShown = false;

                void OnFirstAny()
                {
                    if (!firstAny) return;
                    firstAny = false;
                    AppendLog($"\n>> 首個 token 回應時間 (實測)：{sw.Elapsed.TotalSeconds:F1} 秒");
                    _ = LogModelPlacementAsync();  // 診斷：模型跑在 GPU 還是 CPU
                }

                OllamaClient.GenerateResult r = await OllamaClient.GenerateStreamAsync(
                    model, prompt, options, seed,
                    onToken: token =>
                    {
                        OnFirstAny();
                        if (!responseHeaderShown)
                        {
                            responseHeaderShown = true;
                            AppendLog("\n────────── AI 分析結果 ──────────");
                        }
                        AppendLogRaw(token);
                    },
                    onThinking: think =>
                    {
                        OnFirstAny();
                        if (!thinkingHeaderShown)
                        {
                            thinkingHeaderShown = true;
                            AppendLog("\n────────── 思考過程 (thinking) ──────────");
                        }
                        AppendLogRaw(think);
                    },
                    cancel: cancel);

                string result = r.Text;
                sw.Stop();
                AppendLog("");
                AppendLog("(O)(O)(O)(O)(O)(O)(O)(O)(O)(O)流式生成文字結束(O)(O)(O)(O)(O)(O)(O)(O)(O)(O)");

                // === Ollama 伺服器端計時分解 (精確定位瓶頸) ===
                AppendLog("---------- 伺服器端計時分解 ----------");
                AppendLog($">> 模型載入 (load)      ：{r.LoadSeconds:F1} 秒");
                AppendLog($">> 提示詞評估 (prefill) ：{r.PromptEvalSeconds:F1} 秒 " +
                          $"／ {r.PromptEvalCount} tokens ／ {r.PromptTokensPerSec:F0} tok/s");
                AppendLog($">> 生成 (eval)          ：{r.EvalSeconds:F1} 秒 " +
                          $"／ {r.EvalCount} tokens ／ {r.EvalTokensPerSec:F0} tok/s");
                AppendLog($">> 伺服器總耗時 (total) ：{r.TotalSeconds:F1} 秒");
                AppendLog("--------------------------------------");

                // 儲存分析結果
                string savePath = BuildOutputPath(articlePath);
                try
                {
                    await Task.Run(() => File.WriteAllText(savePath, result, new UTF8Encoding(false)), cancel);
                    AppendLog($">>  [{Now()}] 儲存「{Path.GetFileName(savePath)}」");
                }
                catch (Exception ex)
                {
                    AppendLog($"[錯誤] 儲存分析結果失敗：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 粗略估算文字的 token 數量 (用於上下文預算檢查，非精確值)。
        /// 中日韓字元約 1 字 ≒ 1 token；其餘 (英數、標點) 約 4 字元 ≒ 1 token。
        /// </summary>
        private static int EstimateTokens(string text)
        {
            int cjk = 0, other = 0;
            foreach (char c in text)
            {
                // CJK 統一表意文字、擴充區、日文假名、全形標點
                if ((c >= 0x4E00 && c <= 0x9FFF) || (c >= 0x3400 && c <= 0x4DBF) ||
                    (c >= 0x3040 && c <= 0x30FF) || (c >= 0xFF00 && c <= 0xFFEF) ||
                    (c >= 0x3000 && c <= 0x303F))
                    cjk++;
                else
                    other++;
            }
            return cjk + (other / 4);
        }

        /// <summary>
        /// 產生輸出檔名：原檔名 + "_AI分析.txt"，若已存在則加 "_01"、"_02"...
        /// </summary>
        private static string BuildOutputPath(string articlePath)
        {
            string dir = Path.GetDirectoryName(articlePath) ?? AppContext.BaseDirectory;
            string baseName = Path.GetFileNameWithoutExtension(articlePath) + "_AI分析";

            string candidate = Path.Combine(dir, baseName + ".txt");
            if (!File.Exists(candidate)) return candidate;

            for (int i = 1; i < 1000; i++)
            {
                candidate = Path.Combine(dir, $"{baseName}_{i:00}.txt");
                if (!File.Exists(candidate)) return candidate;
            }
            // 極端情況：回傳含時間戳的檔名
            return Path.Combine(dir, $"{baseName}_{DateTime.Now:yyyyMMddHHmmss}.txt");
        }

        // 診斷：查詢 /api/ps，把目前載入模型的 GPU/CPU 放置比例印到 LOG。
        // 若 GPU 百分比明顯低於 100%，代表模型被卸載到 CPU，這就是「高 CPU / 慢 prefill」的主因。
        private async Task LogModelPlacementAsync()
        {
            try
            {
                var loaded = await OllamaClient.GetLoadedModelsAsync();
                foreach (var m in loaded)
                {
                    double gb = 1024.0 * 1024 * 1024;
                    string where = m.GpuPercent >= 99 ? "全部在 GPU"
                                 : m.GpuPercent <= 1 ? "全部在 CPU"
                                 : $"GPU {m.GpuPercent}% / CPU {100 - m.GpuPercent}%";
                    AppendLog($">> [診斷] 模型「{m.Name}」載入位置：{where} " +
                              $"(VRAM {m.SizeVram / gb:F2} GB / 總計 {m.Size / gb:F2} GB)");
                    if (m.GpuPercent < 99)
                        AppendLog(">> [診斷] ⚠ 模型未完全在 GPU，正使用 CPU 運算 → 這是高 CPU 與慢速的主因。" +
                                  "常見原因：num_ctx 太大導致 VRAM 不足。請改用較小 num_ctx 的參數預設再試。");
                }
            }
            catch (Exception ex)
            {
                AppendLog($">> [診斷] 無法取得模型放置資訊：{ex.Message}");
            }
        }

        // 「取消」：執行中則取消工作，否則關閉視窗
        private void buttonCancel_Click(object? sender, EventArgs e)
        {
            if (_running)
            {
                _cts?.Cancel();
                AppendLog(">> 正在取消...");
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void SetRunning(bool running)
        {
            _running = running;
            buttonRun.Enabled = !running;
            buttonRefreshModels.Enabled = !running;
            buttonEditPrompt.Enabled = !running;
            comboBoxModel.Enabled = !running;
            comboBoxParams.Enabled = !running;
            comboBoxPromptType.Enabled = !running;
            checkedListBoxFiles.Enabled = !running;
            buttonSelectAll.Enabled = !running;
            buttonClearSelection.Enabled = !running;
            buttonInvertSelection.Enabled = !running;
            checkBoxThink.Enabled = !running;
            checkBoxFreshContext.Enabled = !running;
            buttonCancel.Text = running ? "取消(&C)" : "關閉(&C)";
            if (!running) UpdateStatus("就緒");
        }

        private static string Now() => DateTime.Now.ToString("tt hh:mm:ss", new CultureInfo("zh-TW"));

        // 附加一行 LOG (自動換行)
        private void AppendLog(string text) => AppendLogRaw(text + "\n");

        // 附加原始文字到 LOG。流式 token 會先累積到緩衝區，由計時器每隔一段時間
        // 才一次寫入 TextBox，避免「一個字一個字」導致的畫面閃爍與 CPU 飆高。
        private void AppendLogRaw(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            lock (_logLock)
            {
                _logBuffer.Append(text);
            }
        }

        // 計時器 Tick (UI 執行緒)：把緩衝區內容一次寫入 LOG 文字框
        private void FlushLog()
        {
            if (textBoxLog.IsDisposed) return;

            string pending;
            lock (_logLock)
            {
                if (_logBuffer.Length == 0) return;
                pending = _logBuffer.ToString();
                _logBuffer.Clear();
            }

            // TextBox (Multiline) 只認得 CRLF，AI 回傳多為單獨的 LF，
            // 需正規化成 \r\n 才會換行顯示 (否則整段擠成一行)。
            pending = pending.Replace("\r\n", "\n").Replace("\n", "\r\n");
            textBoxLog.AppendText(pending);
        }

        private void UpdateStatus(string text)
        {
            if (labelStatus.IsDisposed) return;
            if (labelStatus.InvokeRequired)
                labelStatus.BeginInvoke(new Action(() => labelStatus.Text = text));
            else
                labelStatus.Text = text;
        }
    }
}

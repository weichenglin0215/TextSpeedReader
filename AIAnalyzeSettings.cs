using System;
using System.IO;
using System.Text;

namespace TextSpeedReader
{
    /// <summary>
    /// 「AI分析文章選項」彈窗的參數記憶。獨立於主程式 AppSettings，
    /// 儲存於執行檔旁的 INI 檔，讓使用者調整過的「AI大模型」「AI參數」
    /// 「文章類型」「使用者指令」在下次開啟彈窗時自動還原。
    /// </summary>
    internal static class AIAnalyzeSettings
    {
        private const string SettingsFileName = @".\AIAnalyze_Settings.ini";
        private const string SectionName = "AIAnalyze";

        /// <summary>上次選擇的 AI 大模型名稱。</summary>
        public static string LastModel { get; set; } = "";

        /// <summary>上次選擇的 AI 參數預設名稱。</summary>
        public static string LastParamPresetName { get; set; } = "";

        /// <summary>上次選擇的文章類型 (Prompt 檔名，不含副檔名)。</summary>
        public static string LastPromptType { get; set; } = "";

        /// <summary>上次輸入的使用者指令 (可能包含多行)。</summary>
        public static string LastUserInstruction { get; set; } = "";

        /// <summary>從設定檔載入。若檔案不存在或讀取失敗，靜默使用預設值 (不中斷程式)。</summary>
        public static void Load()
        {
            if (!File.Exists(SettingsFileName)) return;

            try
            {
                string[] lines = File.ReadAllLines(SettingsFileName);
                bool inSection = false;

                foreach (string raw in lines)
                {
                    string line = raw.Trim();
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        inSection = (line == $"[{SectionName}]");
                        continue;
                    }
                    if (!inSection || !line.Contains('=')) continue;

                    string[] parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length != 2) continue;
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    switch (key)
                    {
                        case "LastModel": LastModel = value; break;
                        case "LastParamPresetName": LastParamPresetName = value; break;
                        case "LastPromptType": LastPromptType = value; break;
                        case "LastUserInstruction": LastUserInstruction = DecodeMultiline(value); break;
                    }
                }
            }
            catch
            {
                // 讀取失敗時靜默使用預設值
            }
        }

        /// <summary>將目前記憶的值寫回設定檔。若寫入失敗，靜默忽略 (不中斷程式)。</summary>
        public static void Save()
        {
            try
            {
                using var writer = new StreamWriter(SettingsFileName, false, new UTF8Encoding(false));
                writer.WriteLine($"[{SectionName}]");
                writer.WriteLine($"LastModel={LastModel}");
                writer.WriteLine($"LastParamPresetName={LastParamPresetName}");
                writer.WriteLine($"LastPromptType={LastPromptType}");
                // 使用者指令可能包含換行與 '=' 等符號，以 Base64 編碼儲存避免破壞 INI 格式
                writer.WriteLine($"LastUserInstruction={EncodeMultiline(LastUserInstruction)}");
            }
            catch
            {
                // 儲存失敗不影響程式運作
            }
        }

        private static string EncodeMultiline(string s) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(s ?? ""));

        private static string DecodeMultiline(string s)
        {
            try { return Encoding.UTF8.GetString(Convert.FromBase64String(s)); }
            catch { return s; }  // 相容舊格式或非預期內容時，直接視為明文
        }
    }
}

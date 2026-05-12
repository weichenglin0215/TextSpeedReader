using System;
using System.IO;

namespace TextSpeedReader
{
    /// <summary>
    /// 應用程式設定管理類別。
    /// 使用 INI 文件格式（純文字鍵值對）保存所有使用者設定，
    /// 與現有的 TextSpeedReader.ini 風格一致。
    /// 設定檔路徑：.\TextSpeedReader_Settings.ini
    /// </summary>
    public class AppSettings
    {
        // 設定檔路徑（與執行檔相同目錄）
        private const string SettingsFileName = @".\TextSpeedReader_Settings.ini";
        // INI 區段名稱
        private const string SectionName = "Settings";

        // ─── 一般設定 ────────────────────────────────────────

        /// <summary>啟動時是否自動展開並切換到上次使用的目錄。</summary>
        public bool AutoOpenLastDirectory { get; set; } = true;

        /// <summary>上次使用的目錄完整路徑，用於自動開啟功能。</summary>
        public string LastDirectory { get; set; } = "";

        /// <summary>是否在下次啟動時還原上次使用的字型與大小。</summary>
        public bool KeepFontSize { get; set; } = true;

        /// <summary>上次使用的字型名稱，搭配 KeepFontSize 使用。</summary>
        public string LastFontFamily { get; set; } = "Microsoft JhengHei";

        /// <summary>上次使用的字型大小（Points），搭配 KeepFontSize 使用。</summary>
        public float LastFontSize { get; set; } = 12.0f;

        // ─── 文字處理設定 ────────────────────────────────────

        /// <summary>「增加行首空白」功能每行開頭插入的空白字元數。</summary>
        public int AddSpaceChrCount { get; set; } = 4;

        /// <summary>識別新段落開始的判定字串（用於特殊格式處理）。</summary>
        public string NewLineStartJudgment { get; set; } = "/*新咒語開始------------------- */";

        /// <summary>識別新段落結束的判定字串（用於特殊格式處理）。</summary>
        public string NewLineEndJudgment { get; set; } = "/*新咒語結束-------------------- */";

        // ─── 插入/註解字串設定 ───────────────────────────────

        /// <summary>「插入行首/行尾字串」功能中，插入到每行行首的字串，預設為雙引號。</summary>
        public string InsertBeginingText { get; set; } = "\"";

        /// <summary>「插入行首/行尾字串」功能中，插入到每行行尾的字串，預設為雙引號加逗號空格。</summary>
        public string InsertEndText { get; set; } = "\", ";

        /// <summary>批次加入/移除註解功能使用的開頭字串，預設為 C 風格 /*。</summary>
        public string AnnotationBegin { get; set; } = "/*";

        /// <summary>批次加入/移除註解功能使用的結尾字串，預設為 C 風格 */。</summary>
        public string AnnotationEnd { get; set; } = "*/";

        // ─── 歷史紀錄 ────────────────────────────────────────

        /// <summary>最近開啟的檔案完整路徑清單，最多保留 MaxHistoryCount 筆。</summary>
        public System.Collections.Generic.List<string> HistoryFiles { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>最近瀏覽的目錄完整路徑清單，最多保留 MaxHistoryCount 筆。</summary>
        public System.Collections.Generic.List<string> HistoryDirectories { get; set; } = new System.Collections.Generic.List<string>();

        /// <summary>歷史紀錄（檔案與目錄）最大保留筆數。</summary>
        public const int MaxHistoryCount = 10;

        // ─────────────────────────────────────────────────────

        /// <summary>
        /// 從設定檔載入所有設定值。
        /// 若設定檔不存在，則以預設值建立新設定檔。
        /// 若讀取過程發生例外，則靜默使用預設值（不中斷程式）。
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(SettingsFileName))
            {
                // 設定檔不存在時，以目前預設值建立新設定檔
                SaveSettings();
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(SettingsFileName);
                bool inSettingsSection = false;

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    // 偵測區段標頭（如 [Settings]）
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        inSettingsSection = (trimmedLine == $"[{SectionName}]");
                        continue;
                    }

                    // 在目標區段內解析鍵值對
                    if (inSettingsSection && trimmedLine.Contains("="))
                    {
                        // 以第一個 '=' 分割鍵與值（值本身可能含有 '='）
                        string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key)
                            {
                                case "AutoOpenLastDirectory":
                                    if (bool.TryParse(value, out bool autoOpen))
                                        AutoOpenLastDirectory = autoOpen;
                                    break;
                                case "LastDirectory":
                                    LastDirectory = value;
                                    break;
                                case "KeepFontSize":
                                    if (bool.TryParse(value, out bool keepFont))
                                        KeepFontSize = keepFont;
                                    break;
                                case "LastFontFamily":
                                    LastFontFamily = value;
                                    break;
                                case "LastFontSize":
                                    if (float.TryParse(value, out float fontSize))
                                        LastFontSize = fontSize;
                                    break;
                                case "AddSpaceChrCount":
                                    if (int.TryParse(value, out int spaceCount))
                                        AddSpaceChrCount = spaceCount;
                                    break;
                                case "NewLineStartJudgment":
                                    NewLineStartJudgment = value;
                                    break;
                                case "NewLineEndJudgment":
                                    NewLineEndJudgment = value;
                                    break;
                                case "InsertBeginingText":
                                    InsertBeginingText = value;
                                    break;
                                case "InsertEndText":
                                    InsertEndText = value;
                                    break;
                                case "AnnotationBegin":
                                    AnnotationBegin = value;
                                    break;
                                case "AnnotationEnd":
                                    AnnotationEnd = value;
                                    break;
                                case "HistoryFiles":
                                    if (!string.IsNullOrEmpty(value))
                                        HistoryFiles = new System.Collections.Generic.List<string>(
                                            value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                                    break;
                                case "HistoryDirectories":
                                    if (!string.IsNullOrEmpty(value))
                                        HistoryDirectories = new System.Collections.Generic.List<string>(
                                            value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 讀取失敗時靜默使用預設值，記錄錯誤訊息供偵錯參考
                Console.WriteLine($"讀取設定檔失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 將目前所有設定值儲存到設定檔。
        /// 若寫入失敗，會靜默記錄錯誤（不中斷程式）。
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    writer.WriteLine($"[{SectionName}]");
                    writer.WriteLine($"AutoOpenLastDirectory={AutoOpenLastDirectory}");
                    writer.WriteLine($"LastDirectory={LastDirectory}");
                    writer.WriteLine($"KeepFontSize={KeepFontSize}");
                    writer.WriteLine($"LastFontFamily={LastFontFamily}");
                    writer.WriteLine($"LastFontSize={LastFontSize}");
                    writer.WriteLine($"AddSpaceChrCount={AddSpaceChrCount}");
                    writer.WriteLine($"NewLineStartJudgment={NewLineStartJudgment}");
                    writer.WriteLine($"NewLineEndJudgment={NewLineEndJudgment}");
                    writer.WriteLine($"InsertBeginingText={InsertBeginingText}");
                    writer.WriteLine($"InsertEndText={InsertEndText}");
                    writer.WriteLine($"AnnotationBegin={AnnotationBegin}");
                    writer.WriteLine($"AnnotationEnd={AnnotationEnd}");
                    // 多筆歷史紀錄以 '|' 分隔儲存在同一行
                    writer.WriteLine($"HistoryFiles={string.Join("|", HistoryFiles)}");
                    writer.WriteLine($"HistoryDirectories={string.Join("|", HistoryDirectories)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"儲存設定檔失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 將指定檔案路徑加入最近開啟的檔案歷史清單的最前面。
        /// 若已存在（不分大小寫），會先移除舊的再插入到最前面（MRU 順序）。
        /// 超過 MaxHistoryCount 上限的舊紀錄會自動移除。
        /// </summary>
        /// <param name="filePath">要加入歷史紀錄的檔案完整路徑。</param>
        public void AddHistoryFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            // 先移除重複項（不分大小寫比對），再插入到最前面（最近使用者優先）
            HistoryFiles.RemoveAll(f => f.Equals(filePath, StringComparison.OrdinalIgnoreCase));
            HistoryFiles.Insert(0, filePath);
            // 超出上限時移除尾端最舊的紀錄
            if (HistoryFiles.Count > MaxHistoryCount)
                HistoryFiles.RemoveRange(MaxHistoryCount, HistoryFiles.Count - MaxHistoryCount);
        }

        /// <summary>
        /// 將指定目錄路徑加入最近瀏覽的目錄歷史清單的最前面。
        /// 邏輯與 AddHistoryFile 相同。
        /// </summary>
        /// <param name="dirPath">要加入歷史紀錄的目錄完整路徑。</param>
        public void AddHistoryDirectory(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath)) return;
            HistoryDirectories.RemoveAll(d => d.Equals(dirPath, StringComparison.OrdinalIgnoreCase));
            HistoryDirectories.Insert(0, dirPath);
            if (HistoryDirectories.Count > MaxHistoryCount)
                HistoryDirectories.RemoveRange(MaxHistoryCount, HistoryDirectories.Count - MaxHistoryCount);
        }
    }
}

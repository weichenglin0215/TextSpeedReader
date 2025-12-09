using System;
using System.IO;

namespace TextSpeedReader
{
    /// <summary>
    /// 應用程式設定管理類別
    /// 使用 INI 文件格式保存設定，與現有的 TextSpeedReader.ini 風格一致
    /// </summary>
    public class AppSettings
    {
        private const string SettingsFileName = @".\TextSpeedReader_Settings.ini";
        private const string SectionName = "Settings";

        // 設定項目
        public bool AutoOpenLastDirectory { get; set; } = true; // 預設為勾選
        public string LastDirectory { get; set; } = ""; // 上次使用的目錄路徑
        public bool KeepFontSize { get; set; } = true; // 保留字型與大小
        public string LastFontFamily { get; set; } = "Microsoft JhengHei"; // 上次使用的字型
        public float LastFontSize { get; set; } = 12.0f; // 上次使用的字型大小
        public int AddSpaceChrCount { get; set; } = 4; // 每行行首增加空白字元數
        public string NewLineStartJudgment { get; set; } = "/*新咒語開始------------------- */"; // 新行開頭的判定字串
        public string NewLineEndJudgment { get; set; } = "/*新咒語結束-------------------- */"; // 新行結尾的判定字串

        /// <summary>
        /// 載入設定
        /// </summary>
        public void LoadSettings()
        {
            if (!File.Exists(SettingsFileName))
            {
                // 如果設定檔不存在，使用預設值
                SaveSettings(); // 創建預設設定檔
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(SettingsFileName);
                bool inSettingsSection = false;

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();

                    // 檢查是否進入設定區段
                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        inSettingsSection = (trimmedLine == $"[{SectionName}]");
                        continue;
                    }

                    // 如果在設定區段內，解析設定值
                    if (inSettingsSection && trimmedLine.Contains("="))
                    {
                        string[] parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();

                            switch (key)
                            {
                                case "AutoOpenLastDirectory":
                                    if (bool.TryParse(value, out bool autoOpen))
                                    {
                                        AutoOpenLastDirectory = autoOpen;
                                    }
                                    break;
                                case "LastDirectory":
                                    LastDirectory = value;
                                    break;
                                case "KeepFontSize":
                                    if (bool.TryParse(value, out bool keepFont))
                                    {
                                        KeepFontSize = keepFont;
                                    }
                                    break;
                                case "LastFontFamily":
                                    LastFontFamily = value;
                                    break;
                                case "LastFontSize":
                                    if (float.TryParse(value, out float fontSize))
                                    {
                                        LastFontSize = fontSize;
                                    }
                                    break;
                                case "AddSpaceChrCount":
                                    if (int.TryParse(value, out int spaceCount))
                                    {
                                        AddSpaceChrCount = spaceCount;
                                    }
                                    break;
                                case "NewLineStartJudgment":
                                    NewLineStartJudgment = value;
                                    break;
                                case "NewLineEndJudgment":
                                    NewLineEndJudgment = value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果讀取失敗，使用預設值
                Console.WriteLine($"讀取設定檔失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 儲存設定
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"儲存設定檔失敗: {ex.Message}");
            }
        }
    }
}


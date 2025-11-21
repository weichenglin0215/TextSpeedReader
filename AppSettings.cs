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
                                // 可以在這裡添加更多設定項目的解析
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
                    // 可以在這裡添加更多設定項目的保存
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"儲存設定檔失敗: {ex.Message}");
            }
        }
    }
}


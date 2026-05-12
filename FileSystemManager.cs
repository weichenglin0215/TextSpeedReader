using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace TextSpeedReader
{
    /// <summary>
    /// 管理檔案系統相關操作，包含目錄樹狀結構建立、最近閱讀清單讀寫、
    /// 以及將檔案移至資源回收桶等功能。
    /// </summary>
    public class FileSystemManager
    {
        /// <summary>檔案瀏覽器目前的根目錄路徑，預設為 C:/。</summary>
        public string FullPath { get; set; } = @"C:/";

        /// <summary>
        /// 檔案瀏覽器支援顯示的文字檔案副檔名清單。
        /// 只有在此清單中的副檔名才會顯示在 ListView 中。
        /// </summary>
        public string[] TextExtensions { get; } = { ".txt", ".cs", ".yaml", ".htm", ".html", ".js", ".py", ".md", ".css", ".json" };

        /// <summary>
        /// 最近閱讀紀錄的資料結構，記錄檔案路徑和上次閱讀到的字元位置，
        /// 以便下次開啟時自動捲動到上次讀到的位置。
        /// </summary>
        public struct RecentReadList
        {
            /// <summary>檔案完整路徑（含磁碟機代號）。</summary>
            public string FileFullName;
            /// <summary>上次閱讀結束時，可視區域左上角的字元索引位置。</summary>
            public int LastCharCount;
        }

        /// <summary>最近閱讀檔案的清單，程式關閉時會寫入 TextSpeedReader.ini。</summary>
        public List<RecentReadList> recentReadList = new List<RecentReadList>();

        /// <summary>
        /// 遞迴取得子目錄並建立對應的 TreeNode，可限制遍歷的最大深度。
        /// 無存取權限的目錄會靜默略過，不顯示錯誤。
        /// </summary>
        /// <param name="subDirs">要列舉的子目錄資訊陣列。</param>
        /// <param name="nodeToAddTo">要將子節點加入的父 TreeNode。</param>
        /// <param name="subFolderDepth">目前遍歷深度（從 0 開始）。</param>
        /// <param name="subFolderDepthLimited">允許的最大遍歷深度。</param>
        public void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo, int subFolderDepth, int subFolderDepthLimited)
        {
            foreach (DirectoryInfo subDir in subDirs)
            {
                TreeNode aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                try
                {
                    DirectoryInfo[] subSubDirs = subDir.GetDirectories();
                    // 若仍有子目錄且尚未達到深度上限，繼續遞迴
                    if (subSubDirs.Length != 0 && subFolderDepth < (subFolderDepthLimited - 1))
                        GetDirectories(subSubDirs, aNode, subFolderDepth + 1, subFolderDepthLimited);
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception)
                {
                    // 無存取權限或其他 IO 錯誤，靜默略過此目錄
                }
            }
        }

        /// <summary>
        /// 從 TextSpeedReader.ini 讀取最近閱讀清單。
        /// 檔案每兩行為一筆記錄：第一行是完整路徑，第二行是字元位置。
        /// 若 ini 檔不存在或讀取失敗，清單保持空白。
        /// </summary>
        public void LoadRecentReadList()
        {
            if (!File.Exists(@".\TextSpeedReader.ini"))
                return;

            recentReadList.Clear();
            try
            {
                using (StreamReader file = new StreamReader(@".\TextSpeedReader.ini"))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        RecentReadList record = new RecentReadList { FileFullName = line };
                        string countLine = file.ReadLine();
                        if (countLine != null)
                        {
                            // 解析上次閱讀的字元位置，格式錯誤時保留預設值 0
                            if (!int.TryParse(countLine, out record.LastCharCount))
                            {
                                Console.WriteLine($"無法解析閱讀位置: '{countLine}'");
                            }
                        }
                        recentReadList.Add(record);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(@".\TextSpeedReader.ini 啟始檔案無法讀取: " + e.Message);
            }
        }

        /// <summary>
        /// 將最近閱讀清單寫回 TextSpeedReader.ini。
        /// 每筆記錄佔兩行：路徑和字元位置。
        /// </summary>
        public void SaveRecentReadList()
        {
            using (StreamWriter file = new StreamWriter(@".\TextSpeedReader.ini"))
            {
                foreach (var item in recentReadList)
                {
                    file.WriteLine(item.FileFullName);
                    file.WriteLine(item.LastCharCount);
                }
            }
        }

        /// <summary>
        /// 將指定檔案移至資源回收桶。
        /// 若為 .htm/.html 檔案，同時將同名的 _files 資源目錄一起移到回收桶。
        /// </summary>
        /// <param name="fullPath">要刪除的檔案完整路徑。</param>
        /// <returns>成功移到資源回收桶回傳 true，檔案不存在回傳 false。</returns>
        public bool DeleteFileToRecycleBin(string fullPath)
        {
            if (!FileSystem.FileExists(fullPath))
                return false;

            FileSystem.DeleteFile(fullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

            // HTML 檔案通常有對應的 _files 資料夾存放圖片等資源，一併刪除
            string ext = Path.GetExtension(fullPath);
            if (ext == ".htm" || ext == ".html")
            {
                string? dir = Path.GetDirectoryName(fullPath);
                string baseName = Path.GetFileNameWithoutExtension(fullPath);
                if (dir != null)
                {
                    string assetsDir = Path.Combine(dir, baseName) + "_files";
                    if (FileSystem.DirectoryExists(assetsDir))
                    {
                        FileSystem.DeleteDirectory(assetsDir, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                }
            }
            return true;
        }
    }
}

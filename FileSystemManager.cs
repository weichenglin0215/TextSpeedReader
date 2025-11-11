using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace TextSpeedReader
{
    public class FileSystemManager
    {
        // 檔案瀏覽器根目錄路徑
        public string FullPath { get; set; } = @"C:/";
        // 支援的文字檔案類型
        public string[] TextExtensions { get; } = { ".txt", ".cs", ".htm", ".html" };

        // 最近閱讀檔案清單結構
        public struct RecentReadList
        {
            public string FileFullName;    // 檔案完整路徑
            public int LastCharCount;      // 上次閱讀位置(字元數)
        }

        // 最近閱讀檔案清單
        public List<RecentReadList> recentReadList = new List<RecentReadList>();

        // 遞迴獲取目錄結構，可限制遍歷深度
        public void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo, int subFolderDepth, int subFolderDepthLimited)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                try
                {
                    subSubDirs = subDir.GetDirectories();
                    if (subSubDirs.Length != 0 && subFolderDepth < (subFolderDepthLimited - 1))
                    {
                        GetDirectories(subSubDirs, aNode, subFolderDepth + 1, subFolderDepthLimited);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception)
                {
                    // 忽略無權限訪問的目錄
                }
            }
        }

        // 讀取最近閱讀清單
        public void LoadRecentReadList()
        {
            if (File.Exists(@".\TextSpeedReader.ini"))
            {
                recentReadList.Clear();
                try
                {
                    using (StreamReader file = new StreamReader(@".\TextSpeedReader.ini"))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            RecentReadList tmpRecentReadList = new RecentReadList
                            {
                                FileFullName = line
                            };

                            if ((line = file.ReadLine()) != null)
                            {
                                try
                                {
                                    tmpRecentReadList.LastCharCount = Int32.Parse(line);
                                }
                                catch (FormatException exc)
                                {
                                    Console.WriteLine($"Unable to parse '{line}'" + exc);
                                }
                            }
                            recentReadList.Add(tmpRecentReadList);
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(@".\TextSpeedReader.ini" + " 啟始檔案無法讀取");
                    Console.WriteLine(e.Message);
                }
            }
        }

        // 儲存最近閱讀清單
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

        // 刪除檔案到資源回收桶
        public bool DeleteFileToRecycleBin(string fullPath)
        {
            if (FileSystem.FileExists(fullPath))
            {
                FileSystem.DeleteFile(fullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                // 如果是網頁檔案，同時刪除相關的資源目錄
                if ((Path.GetExtension(fullPath) == ".htm" || Path.GetExtension(fullPath) == ".html") 
                    && FileSystem.DirectoryExists(Path.Combine(Path.GetDirectoryName(fullPath), 
                    Path.GetFileNameWithoutExtension(fullPath)) + "_files"))
                {
                    FileSystem.DeleteDirectory(
                        Path.Combine(Path.GetDirectoryName(fullPath), 
                        Path.GetFileNameWithoutExtension(fullPath)) + "_files", 
                        UIOption.OnlyErrorDialogs, 
                        RecycleOption.SendToRecycleBin);
                }
                return true;
            }
            return false;
        }
    }
} 
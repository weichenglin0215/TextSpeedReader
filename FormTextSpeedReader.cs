using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualBasic;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader : Form
    {
        #region 字段定義

        // 視窗最小尺寸設定
        public override System.Drawing.Size MinimumSize { get; set; }

        // 檔案系統管理器
        public FileSystemManager fileManager = new FileSystemManager();

        // 顯示管理器
        public DisplayManager displayManager = new DisplayManager();

        // 應用程式設定管理器
        private AppSettings appSettings = new AppSettings();

        // 系統字體列表
        private FontFamily[] m_FontFamilies = Array.Empty<FontFamily>();

        // 當前選中的樹狀目錄節點文字
        private string m_TreeViewSelectedNodeText = "";

        // 可選擇的字體大小列表
        private int[] m_FontSize = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36, 48, 66, 72, 100, 125, 150, 200, 300, 400, 500 };

        // 網頁瀏覽器縮放比例
        private int m_WebBrowserZoom = 100;

        // 網頁瀏覽器可選擇的縮放大小
        private int[] m_WebBrowserSize = { 10, 25, 33, 50, 66, 75, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500, 600 };

        // ListView 欄位排序器
        private ListViewColumnSorter m_LvwColumnSorter = new ListViewColumnSorter();

        // 目前顯示於 WebBrowser 的本機 HTML 完整路徑
        private string m_CurrentHtmlFilePath = "";

        // 臨時HTML檔案路徑（用於編碼轉換）
        private string m_TempHtmlFilePath = "";

        // 最近閱讀檔案清單
        public List<FileSystemManager.RecentReadList> m_RecentReadList = new List<FileSystemManager.RecentReadList>();

        // 當前閱讀檔案在清單中的索引
        public int m_RecentReadListIndex = -1;

        // 查找/替换对话框
        private FormFindReplace? m_FindReplaceDialog = null;

        // 標記是否為右鍵點擊（用於防止右鍵時打開檔案）
        private bool m_IsRightClick = false;
        // 記錄最後一次右鍵點擊的時間戳
        private DateTime m_LastRightClickTime = DateTime.MinValue;

        // 標記當前檔案是否被修改過（文件載入後是否有編輯）
        private bool m_IsCurrentFileModified = false;
        // 標記是否正在載入檔案（用於避免載入時觸發 TextChanged 事件設置修改標誌）
        private bool m_IsLoadingFile = false;

        #endregion

        #region 建構函式

        // 建構函式
        public FormTextSpeedReader()
        {
            InitializeComponent();

            // 載入應用程式設定
            appSettings.LoadSettings();

            // 註冊 TreeView 展開事件
            this.treeViewFolder.BeforeExpand += treeViewFolder_BeforeExpand;

            PopulateTreeViewAll(1);        // 初始化檔案樹狀圖（僅第一層以減少資源消耗）
            fileManager.LoadRecentReadList();           // 讀取最近閱讀清單
            GetSystemFonts();              // 獲取系統字體
            SetFormSize();                 // 設定視窗大小
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            webBrowser1.Visible = false;
            //建立鍵盤事件
            KeyPreview = true; // 讓表單能接收鍵盤事件
            this.KeyDown += FormTextSpeedReader_KeyDown;

            //建立建議右鍵選單
            //item.Click += toolStripMenuItemRemoveLineBreaks_Click;
            //menu.Items.Add(item);
            //richTextBoxText.ContextMenuStrip = menu;

            // 建立 richTextBoxText 的文字變更和選擇變更事件
            richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
            richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;

            // 建立 WebBrowser 文檔載入完成事件
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;

            // 添加編碼轉換選單項目
            AddEncodingConvertMenuItem();

            // 初始化菜單狀態
            UpdateMenuStatus();
        }

        #endregion

        #region 初始化與設定

        // 設定視窗大小和位置
        private void SetFormSize()
        {
            // 設定最小視窗大小
            MinimumSize = new Size(1320, 800);
            // 獲取螢幕工作區域大小
            Screen? primaryScreen = Screen.PrimaryScreen;
            if (primaryScreen != null)
            {
                System.Drawing.Rectangle workingRectangle = primaryScreen.WorkingArea;
                // 設定視窗大小略小於工作區域
                this.Size = new System.Drawing.Size(workingRectangle.Width - 10, workingRectangle.Height - 10);
                // 將視窗置中
                Point newPosition = new Point(0, 0);
                newPosition.X = (workingRectangle.Width - this.Width) / 2;
                newPosition.Y = (workingRectangle.Height - this.Height) / 2;
                this.Location = newPosition;
            }
        }

        // 獲取系統已安裝的字體
        private void GetSystemFonts()
        {
            // 獲取系統已安裝的字體集合
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            m_FontFamilies = installedFontCollection.Families;

            // 將所有字體名稱添加到下拉選單中
            int count = m_FontFamilies.Length;
            for (int j = 0; j < count; ++j)
            {
                toolStripComboBoxFonts.Items.Add(m_FontFamilies[j].Name);
            }
        }

        #endregion

        #region 快捷鍵與檔案操作

        // 快捷鍵事件
        private void FormTextSpeedReader_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveCurrentFile();
                e.SuppressKeyPress = true; // 防止發出系統聲音
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                ShowFindDialog();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.H)
            {
                ShowReplaceDialog();
                e.SuppressKeyPress = true;
            }
        }




        #endregion

        #region 檔案樹狀檢視



        /*
        // 遞迴獲取目錄結構，可限制遍歷深度
        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo, int subFolderDeepth, int subFolderDeepthLimited)
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
                    // 獲取子目錄
                    subSubDirs = subDir.GetDirectories();
                    // 如果有子目錄且未達到深度限制，則繼續遞迴
                    if (subSubDirs.Length != 0 && subFolderDeepth < (subFolderDeepthLimited - 1))
                    {
                        GetDirectories(subSubDirs, aNode, subFolderDeepth+1, subFolderDeepthLimited);
                    }
                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (Exception exc)
                {
                    // 忽略無權限訪問的目錄
                }
            }
        }
        */

        #endregion

        #region 檔案列表與選擇

        // 處理樹狀檢視節點選擇變更事件
        private void treeViewFolder_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            treeViewFolder_AfterSelect(sender, e, null);
        }

        // 處理樹狀檢視節點選擇變更事件（支援保持選中項目）
        private void treeViewFolder_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e, string? fileNameToSelect)
        {
            if (e.Node == null)
                return;

            // 記錄當前選中的檔案名稱（如果有的話）
            string? currentSelectedFileName = null;
            if (listViewFile.SelectedItems.Count > 0)
            {
                currentSelectedFileName = listViewFile.SelectedItems[0].Text;
            }

            // 如果指定了要選中的檔案名稱，優先使用它
            if (fileNameToSelect != null)
            {
                currentSelectedFileName = fileNameToSelect;
            }

            TreeNode newSelected = e.Node;
            m_TreeViewSelectedNodeText = e.Node.FullPath;
            listViewFile.Items.Clear();

            // 獲取選中節點的目錄資訊
            if (newSelected.Tag == null || !(newSelected.Tag is DirectoryInfo))
                return;

            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            if (newSelected.GetNodeCount(false) == 0)
            {
                fileManager.GetDirectories(nodeDirInfo.GetDirectories(), e.Node, 0, 1);
            }

            // 更新檔案列表顯示
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem? item = null;

            // 開始更新檔案列表
            listViewFile.BeginUpdate();
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                // 只顯示支援的檔案類型
                foreach (var itemExt in fileManager.TextExtensions)
                {
                    if (file.Extension.ToLower() == itemExt)
                    {
                        item = new ListViewItem(file.Name, 1);
                        subItems = new ListViewItem.ListViewSubItem[]
                           //{ new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToShortDateString())};
                           { new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"))};
                        item.SubItems.AddRange(subItems);
                        subItems = new ListViewItem.ListViewSubItem[]
                           { new ListViewItem.ListViewSubItem(item, file.Length.ToString("N0"))};
                        item.SubItems.AddRange(subItems); listViewFile.Items.Add(item);
                    }
                }
            }

            // 調整列寬並設定排序器
            listViewFile.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewFile.ListViewItemSorter = m_LvwColumnSorter;
            listViewFile.EndUpdate();

            // 如果有要選中的檔案名稱，重新選中它並保持滾動位置
            if (!string.IsNullOrEmpty(currentSelectedFileName))
            {
                // 使用 BeginInvoke 確保在 UI 更新完成後執行
                this.BeginInvoke(new Action(() =>
                {
                    RestoreSelectedFilePosition(currentSelectedFileName);
                }));
            }

            // 更新狀態欄顯示檔案數量和選取數量
            UpdateFileSelectionStatus();
        }

        // 恢復選中檔案的位置（不觸發文件載入）
        private void RestoreSelectedFilePosition(string fileName)
        {
            if (listViewFile.Items.Count == 0)
                return;

            try
            {
                // 暫時禁用 SelectedIndexChanged 事件，避免觸發文件載入
                listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;

                // 找到對應的檔案項目
                ListViewItem? targetItem = null;
                foreach (ListViewItem item in listViewFile.Items)
                {
                    if (item.Text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        targetItem = item;
                        break;
                    }
                }

                if (targetItem != null)
                {
                    // 選中該項目
                    targetItem.Selected = true;
                    targetItem.Focused = true;
                    // 確保項目可見（保持當前滾動位置）
                    targetItem.EnsureVisible();
                }

                // 重新啟用 SelectedIndexChanged 事件
                listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
            }
            catch
            {
                // 確保重新啟用事件（即使發生錯誤）
                try
                {
                    listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                }
                catch { }
            }
        }



        // 處理檔案列表欄位點擊排序
        private void listViewFile_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 檢查點擊的欄位是否為目前的排序欄位
            if (e.Column == m_LvwColumnSorter.SortColumn)
            {
                // 切換排序方向（升序/降序）
                if (m_LvwColumnSorter.Order == SortOrder.Ascending)
                {
                    m_LvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    m_LvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // 設定新的排序欄位，預設為升序
                m_LvwColumnSorter.SortColumn = e.Column;
                m_LvwColumnSorter.Order = SortOrder.Ascending;
            }

            // 執行排序
            this.listViewFile.Sort();
        }

        // 處理檔案列表的刪除鍵事件
        private void listViewFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteFiles();
                e.SuppressKeyPress = true;
            }
        }

        #endregion

        #region 網頁瀏覽器


        #endregion

        #region UI 控制項事件處理

        // 離開按鈕點擊事件
        private void QuitButton_Click(object sender, EventArgs e)
        {
            //SelectFolderPath();
        }

        // 選擇資料夾路徑
        private void SelectFolderPath()
        {
            try
            {
                string startupPath = Application.StartupPath;
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "請選取目錄";
                    dialog.ShowNewFolderButton = false;
                    //dialog.RootFolder = Environment.SpecialFolder.MyComputer; //沒用，.MyComputer只會取得空字串，可以試試MyDocument
                    dialog.SelectedPath = fileManager.FullPath;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        fileManager.FullPath = dialog.SelectedPath;
                        Console.WriteLine("dialog.RootFolder " + dialog.RootFolder);
                        Console.WriteLine("dialog.SelectedPath " + dialog.SelectedPath);
                        PopulateTreeView(Regex.Matches(fileManager.FullPath, @"\\").Count);
                        Console.WriteLine("\\ Count " + Regex.Matches(fileManager.FullPath, @"\\").Count);

                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Import failed because " + exc.Message + " , please try again later.");
            }
        }

        private void FolderPathButton_Click(object sender, EventArgs e)
        {

        }

        // 顯示資料夾按鈕點擊事件
        private void ShowFolderButton_Click(object sender, EventArgs e)
        {
            ToggleFolderPanel();
        }

        // 切換資料夾瀏覽面板顯示/隱藏
        private void ToggleFolderPanel()
        {
            splitContainerMain.Panel1Collapsed = !splitContainerMain.Panel1Collapsed;
        }


        // 程式關閉時儲存最近閱讀清單
        private void FormTSRClosing(object sender, FormClosingEventArgs e)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@".\TextSpeedReader.ini"))
            {
                // 將所有最近閱讀記錄寫入設定檔
                foreach (var item in m_RecentReadList)
                {
                    file.WriteLine(item.FileFullName);
                    file.WriteLine(item.LastCharCount);
                }
                file.Close();
            }

            // 儲存當前目錄到設定檔
            if (!string.IsNullOrEmpty(m_TreeViewSelectedNodeText))
            {
                appSettings.LastDirectory = m_TreeViewSelectedNodeText;
                appSettings.SaveSettings();
            }
        }

        /*
        // 讀取最近閱讀清單
        private void GetRecentReadList()
        {
            if (File.Exists(@".\TextSpeedReader.ini"))
            {
                int counter = 0;
                string line;
                m_RecentReadList.Clear();
                RecentReadList tmpRecentReadList = new RecentReadList();
                try
                {
                    // 從設定檔讀取最近閱讀記錄
                    using (StreamReader file = new StreamReader(@".\TextSpeedReader.ini"))
                    {
                        while ((line = file.ReadLine()) != null)
                        {
                            tmpRecentReadList.FileFullName = line;
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
                            m_RecentReadList.Add(tmpRecentReadList);

                            // 更新最近閱讀檔案列表顯示
                            ListViewItem item = new ListViewItem(m_RecentReadList[counter].FileFullName, 1);
                            ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[]
                               { new ListViewItem.ListViewSubItem(item, m_RecentReadList[counter].LastCharCount.ToString())};
                            item.SubItems.AddRange(subItems);
                            listViewRecentFiles.Items.Add(item);

                            counter++;
                        }
                        file.Close();
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(@".\TextSpeedReader.ini" + " 啟始檔案無法讀取");
                    Console.WriteLine(e.Message);
                }
            }
        }
        */

        #endregion

        #region 文字處理功能

        // 移除選取的文字斷行，將所選文字合併成同一行（右鍵選單）
        private void toolStripMenuItemRemoveLineBreaks_Click(object sender, EventArgs e)
        {
            RemoveSelectedTextLineBreaks();
        }

        // 移除選取的文字斷行，將所選文字合併成同一行
        private void RemoveSelectedTextLineBreaks()
        {
            string selectedText = richTextBoxText.SelectedText;
            if (!string.IsNullOrEmpty(selectedText))
            {
                // 判斷結尾斷行符號
                string lineBreak = "";
                if (selectedText.EndsWith("\r\n"))
                    lineBreak = "\r\n";
                else if (selectedText.EndsWith("\n"))
                    lineBreak = "\n";
                else if (selectedText.EndsWith("\r"))
                    lineBreak = "\r";

                string singleLine = selectedText.Replace("\r", "").Replace("\n", "");

                // 加回最後一個斷行符號（如果原本有）
                singleLine += lineBreak;

                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = singleLine;
                richTextBoxText.Select(selStart, singleLine.Length);
            }
        }

        // 自動選取直到空白行（右鍵選單）
        private void toolStripMenuItem_AutoSelectCR_Click(object sender, EventArgs e)
        {
            AutoSelectCR();
        }

        // 自動選取直到空白行
        private void AutoSelectCR()
        {
            int start = richTextBoxText.SelectionStart;
            string text = richTextBoxText.Text;
            int length = text.Length;
            //int end = start;
            int end = richTextBoxText.SelectionStart + richTextBoxText.SelectionLength;

            // 從目前選取範圍的結尾開始尋找兩個連續斷行符號，若一開始就遇到兩個連續斷行符號，則先選取這兩個符號，並將end+2
            // 檢查目前位置與下一個字元是否都是斷行符號
            char beginC1 = text[end];
            char beginC2 = text[end + 1];

            // 判斷 \r\n 或 \n\n 或 \r\r
            bool beginisLineBreak1 = (beginC1 == '\r' || beginC1 == '\n');
            bool beginisLineBreak2 = (beginC2 == '\r' || beginC2 == '\n');

            if (beginisLineBreak1 && beginisLineBreak2)
            {
                // 包含這兩個斷行符號
                end += 2;
            }


            while (end < length - 1)
            {
                // 檢查目前位置與下一個字元是否都是斷行符號
                char c1 = text[end];
                char c2 = text[end + 1];

                // 判斷 \r\n 或 \n\n 或 \r\r
                bool isLineBreak1 = (c1 == '\r' || c1 == '\n');
                bool isLineBreak2 = (c2 == '\r' || c2 == '\n');

                if (isLineBreak1 && isLineBreak2)
                {
                    // 不要包含這兩個斷行符號
                    //end += 2;
                    break;
                }
                end++;
            }

            // 若沒找到，選到結尾
            if (end > length) end = length;

            richTextBoxText.Select(start, end - start);
        }

        // 自動選取直到遇到空白行、或遇到該行最後一字為句點「。」（全形半形都要）或驚嘆號「！」（全形/半形）就停止（右鍵選單）
        private void toolStripMenuItem_AutoSelectWithPunctuation_Click(object sender, EventArgs e)
        {
            AutoSelectWithPunctuation();
        }

        // 自動選取直到遇到空白行、或遇到該行最後一字為句點「。」（全形半形都要）或驚嘆號「！」（全形/半形）就停止
        private void AutoSelectWithPunctuation()
        {
            int start = richTextBoxText.SelectionStart;
            string text = richTextBoxText.Text;
            int length = text.Length;
            int end = richTextBoxText.SelectionStart + richTextBoxText.SelectionLength;
            while (end < length)
            {
                int lineBreakPos = text.IndexOfAny(new char[] { '\r', '\n' }, end);
                if (lineBreakPos == -1) lineBreakPos = length;
                string line = text.Substring(end, lineBreakPos - end);
                // 空白行則停止
                if (string.IsNullOrWhiteSpace(line)) break;
                char lastChar = line.Length > 0 ? line[line.Length - 1] : '\0';
                // 若行末為標點，則將end設至該行結尾並中止迴圈
                if (lastChar == '.' || lastChar == '。' || lastChar == '!' || lastChar == '！' || lastChar == '?' || lastChar == '？')
                {
                    end = lineBreakPos;
                    if (end < length - 1 && text[end] == '\r' && text[end + 1] == '\n') end += 2;
                    else if (text[end] == '\r' || text[end] == '\n') end++;
                    break;
                }
                end = lineBreakPos;
                // 跳過 \r\n
                if (end < length - 1 && text[end] == '\r' && text[end + 1] == '\n') end += 2;
                else if (text[end] == '\r' || text[end] == '\n') end++;
            }
            richTextBoxText.Select(start, Math.Max(0, end - start));
        }



        // 處理單行的邏輯
        private string ProcessLine(string line, int firstNonSpaceTabPos, int lastNonSpaceTabPos,
                                   bool lineHasOnlySpacesAndTabs, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            string processedLine = line;

            // 規則2: 若整行僅空白或TAB，移除TAB，保留空白與換行符
            // 注意：對於只有空白/TAB的行，不應用行尾空白刪除規則，因為整行都是空白
            if (lineHasOnlySpacesAndTabs)
            {
                // 移除所有TAB，保留空格
                processedLine = line.Replace("\t", "");
                if (line != processedLine)
                {
                    hasChanges = true;
                }
                return processedLine;
            }

            // 規則3: 若該行最前面就是文字(非空白內容)，保留整行不變（但需要處理結尾空白）
            if (firstNonSpaceTabPos == 0)
            {
                // 行開頭就是非空白內容，只處理結尾空白
                // 新規則: 移除每一行最後的單一空白字元或連續空白字元
                processedLine = RemoveTrailingSpaces(processedLine, ref hasChanges);
                return processedLine;
            }

            // 規則1: 該行最前面有空白字符或TAB，移除開頭的連續空白字符或TAB字符
            if (firstNonSpaceTabPos > 0)
            {
                // 移除開頭的連續空白字符或TAB字符
                processedLine = line.Substring(firstNonSpaceTabPos);
                hasChanges = true;

                // 新規則: 移除每一行最後的單一空白字元或連續空白字元
                processedLine = RemoveTrailingSpaces(processedLine, ref hasChanges);
                return processedLine;
            }

            // 其他情況（不應該發生），也處理結尾空白
            processedLine = RemoveTrailingSpaces(processedLine, ref hasChanges);
            return processedLine;
        }

        // 移除行尾的單一空白字元或連續空白字元
        private string RemoveTrailingSpaces(string line, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            // 從行尾開始掃描，找到最後一個非空白字符的位置
            int lastNonSpaceIndex = line.Length - 1;
            while (lastNonSpaceIndex >= 0 && (line[lastNonSpaceIndex] == ' ' || line[lastNonSpaceIndex] == '\t'))
            {
                lastNonSpaceIndex--;
            }

            // 如果找到結尾有空白字符，移除它們
            if (lastNonSpaceIndex < line.Length - 1)
            {
                string processedLine = line.Substring(0, lastNonSpaceIndex + 1);
                hasChanges = true;
                return processedLine;
            }

            // 沒有結尾空白，返回原行
            return line;
        }

        #endregion

        #region 繁簡轉換功能

        // 編輯文字轉換為簡體（右鍵選單）
        private void toolStripMenuItem_EditTextCovertSimplified_Click(object sender, EventArgs e)
        {
            EditTextCovertSimplified();
        }

        // 編輯文字轉換為簡體
        private void EditTextCovertSimplified()
        {
            // 檢查是否有選取的文字，如果沒有則處理整個文檔
            bool processWholeDocument = (richTextBoxText.SelectionLength == 0);
            string textToConvert;

            if (processWholeDocument)
            {
                // 處理整個文檔
                textToConvert = richTextBoxText.Text;
            }
            else
            {
                // 處理選取的文字
                textToConvert = richTextBoxText.SelectedText;
            }

            if (string.IsNullOrEmpty(textToConvert))
            {
                MessageBox.Show("文字內容為空！", "提示");
                return;
            }

            try
            {
                // 保存選擇區域的位置
                int selStart = richTextBoxText.SelectionStart;
                int selLength = richTextBoxText.SelectionLength;

                // 使用Microsoft.VisualBasic.Strings.StrConv進行繁簡轉換
                // VbStrConv.SimplifiedChinese: 繁體轉簡體
                const int SimplifiedChineseLcid = 0x0804; // zh-CN
                string? simplifiedText = Strings.StrConv(textToConvert, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);
                if (string.IsNullOrEmpty(simplifiedText))
                {
                    MessageBox.Show("轉換結果為空，請檢查原始文字。", "錯誤");
                    return;
                }

                if (processWholeDocument)
                {
                    // 處理整個文檔
                    richTextBoxText.Text = simplifiedText;
                    // 恢復光標位置
                    richTextBoxText.SelectionStart = selStart;
                    richTextBoxText.SelectionLength = 0;
                }
                else
                {
                    // 處理選取的文字
                    richTextBoxText.SelectedText = simplifiedText;
                    // 重新選取轉換後的文字
                    richTextBoxText.Select(selStart, simplifiedText.Length);
                }

                // 標記檔案已被修改
                m_IsCurrentFileModified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("轉換時發生錯誤：\n" + ex.Message, "錯誤");
            }
        }

        // 編輯文字轉換為繁體（右鍵選單）
        private void toolStripMenuItem_EditTextCovertTraditional_Click(object sender, EventArgs e)
        {
            EditTextCovertTraditional();
        }

        // 編輯文字轉換為繁體
        private void EditTextCovertTraditional()
        {
            // 檢查是否有選取的文字，如果沒有則處理整個文檔
            bool processWholeDocument = (richTextBoxText.SelectionLength == 0);
            string textToConvert;

            if (processWholeDocument)
            {
                // 處理整個文檔
                textToConvert = richTextBoxText.Text;
            }
            else
            {
                // 處理選取的文字
                textToConvert = richTextBoxText.SelectedText;
            }

            if (string.IsNullOrEmpty(textToConvert))
            {
                MessageBox.Show("文字內容為空！", "提示");
                return;
            }

            try
            {
                // 保存選擇區域的位置
                int selStart = richTextBoxText.SelectionStart;
                int selLength = richTextBoxText.SelectionLength;

                // 使用Microsoft.VisualBasic.Strings.StrConv進行簡繁轉換
                // 注意：VbStrConv.TraditionalChinese 需要指定正確的 LCID
                // 對於簡體轉繁體，應該使用 zh-CN LCID，因為源文字是簡體中文
                const int SimplifiedChineseLcid = 0x0804; // zh-CN

                // 使用 zh-CN LCID 進行簡體到繁體的轉換
                // 這是關鍵：當源文字是簡體中文時，應該使用簡體中文的 LCID
                string? traditionalText = Strings.StrConv(textToConvert, VbStrConv.TraditionalChinese, SimplifiedChineseLcid);

                // 檢查轉換結果
                if (string.IsNullOrEmpty(traditionalText))
                {
                    MessageBox.Show("轉換結果為空，請檢查原始文字。", "錯誤");
                    return;
                }

                if (processWholeDocument)
                {
                    // 處理整個文檔
                    richTextBoxText.Text = traditionalText;
                    // 恢復光標位置
                    richTextBoxText.SelectionStart = selStart;
                    richTextBoxText.SelectionLength = 0;
                }
                else
                {
                    // 處理選取的文字
                    richTextBoxText.SelectedText = traditionalText;
                    // 重新選取轉換後的文字
                    richTextBoxText.Select(selStart, traditionalText.Length);
                }

                // 標記檔案已被修改
                m_IsCurrentFileModified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("轉換時發生錯誤：\n" + ex.Message, "錯誤");
            }
        }

        #endregion

        #region HTML 處理功能

        // 複製HTML選取文字並儲存為檔案（按鈕）
        private void toolStripButtonCopyHtmlSaveFile_Click(object sender, EventArgs e)
        {
            CopyHtmlSaveFile();
        }

        // 將選中的檔案滾動到 ListView 的中間位置（不重新選取，避免觸發文件載入）
        private void ScrollToSelectedFileCenter(string fileName)
        {
            if (listViewFile.Items.Count == 0)
                return;

            // 檢查當前選中的文件是否就是目標文件
            bool isCurrentlySelected = false;
            if (listViewFile.SelectedItems.Count > 0)
            {
                isCurrentlySelected = (listViewFile.SelectedItems[0].Text == fileName);
            }

            // 找到對應的檔案項目
            ListViewItem? targetItem = null;
            foreach (ListViewItem item in listViewFile.Items)
            {
                if (item.Text == fileName)
                {
                    targetItem = item;
                    break;
                }
            }

            if (targetItem == null)
                return;

            // 使用 BeginInvoke 確保在 UI 更新完成後執行滾動
            this.BeginInvoke(new Action(() =>
            {
                try
                {
                    // 暫時禁用 SelectedIndexChanged 事件，避免觸發文件載入
                    listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;

                    // 計算可見區域可以顯示的項目數量
                    int itemHeight = 20; // 預設項目高度
                    if (listViewFile.Items.Count > 0)
                    {
                        var firstItem = listViewFile.Items[0];
                        if (firstItem.Bounds.Height > 0)
                        {
                            itemHeight = firstItem.Bounds.Height;
                        }
                    }

                    int visibleItemCount = listViewFile.ClientSize.Height / itemHeight;
                    if (visibleItemCount <= 0)
                        visibleItemCount = 10; // 預設值

                    // 計算目標項目的索引
                    int targetIndex = targetItem.Index;

                    // 計算要滾動到的位置（讓目標項目在中間）
                    int scrollToIndex = targetIndex - (visibleItemCount / 2);
                    if (scrollToIndex < 0)
                        scrollToIndex = 0;
                    if (scrollToIndex >= listViewFile.Items.Count)
                        scrollToIndex = Math.Max(0, listViewFile.Items.Count - 1);

                    // 滾動到計算的位置
                    if (scrollToIndex < listViewFile.Items.Count && scrollToIndex >= 0)
                    {
                        listViewFile.TopItem = listViewFile.Items[scrollToIndex];
                    }

                    // 如果目標文件就是當前選中的文件，只需要恢復選中狀態（不會觸發事件因為已經禁用）
                    // 如果目標文件不是當前選中的文件，也選中它（但不會觸發事件）
                    if (isCurrentlySelected)
                    {
                        // 目標文件就是當前選中的文件，確保它仍然被選中
                        if (listViewFile.SelectedItems.Count == 0 || listViewFile.SelectedItems[0].Text != fileName)
                        {
                            targetItem.Selected = true;
                            targetItem.Focused = true;
                        }
                    }
                    else
                    {
                        // 目標文件不是當前選中的文件，選中它但不觸發事件
                        targetItem.Selected = true;
                        targetItem.Focused = true;
                    }

                    // 重新啟用 SelectedIndexChanged 事件
                    listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                }
                catch
                {
                    // 確保重新啟用事件（即使發生錯誤）
                    try
                    {
                        listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                    }
                    catch { }

                    // 如果計算失敗，至少確保項目可見
                    if (targetItem != null)
                    {
                        targetItem.EnsureVisible();
                    }
                }
            }));
        }

        #endregion

        #region 右鍵選單事件處理

        // 合併無空白（右鍵選單，未實作）
        private void toolStripMenuItem_MergeNoneSpace_Click(object sender, EventArgs e)
        {
            // 未實作
        }

        // 移除行首和行尾的空白字元（右鍵選單）
        private void toolStripMenuItem_RemoveLeadingAndTrailingSpaces_Click(object sender, EventArgs e)
        {
            RemoveLeadingAndTrailingSpaces();
        }

        // 自動移除目前文章中多餘的斷行（右鍵選單）
        private void toolStripMenuItem_AutoRemoveCRButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCR();
        }

        // 自動移除目前文章中多餘的斷行，不包含該行最後一個字是句點或驚嘆號的行（右鍵選單）
        private void toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark_Click(object sender, EventArgs e)
        {
            AutoRemoveCRWithoutDotAndExclamationMark();
        }

        // 將目前TXT繁體中文轉換成簡體中文並儲存（右鍵選單）
        private void toolStripMenuItem_ConvertToSimplified_Click(object sender, EventArgs e)
        {
            ConvertCurrentTxtToSimplifiedAndSave();
        }

        // 移除超過120字元的行（右鍵選單）
        private void toolStripMenuItem_RemoveMoreThan120Char_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("此功能會將每行超過100個字元的部分，自動在下一個句點、驚嘆號、問號處斷行，請注意備份原始檔案！", "提示");
            RemoveMoreThan120Char();
            //MessageBox.Show("RemoveMoreThan120Char()執行完畢", "提示");
        }

        // 檢查指定位置是否在"「"和"」"之間
        private bool IsInsideQuotationMarks(string line, int startPos, int pos)
        {
            // 從開始位置到標點符號位置之間查找引號
            int quoteStart = -1;
            for (int i = startPos; i < pos; i++)
            {
                if (line[i] == '「')
                {
                    quoteStart = i;
                }
                else if (line[i] == '」' && quoteStart >= 0)
                {
                    // 找到配對的結束引號，重置開始位置
                    quoteStart = -1;
                }
            }

            // 如果找到未配對的"「"，檢查標點符號是否在其後
            if (quoteStart >= 0)
            {
                // 檢查標點符號之後是否有"」"
                for (int i = pos + 1; i < line.Length; i++)
                {
                    if (line[i] == '」')
                    {
                        // 標點符號在"「"和"」"之間
                        return true;
                    }
                    else if (line[i] == '「')
                    {
                        // 遇到新的開始引號，說明之前的引號已經結束
                        break;
                    }
                }
            }

            return false;
        }

        // 移除超過120字元的行
        private void RemoveMoreThan120Char()
        {
            //超過100個字元時，尋找下一個句點、驚嘆號、問號，將該行截成兩行
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;
            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }
            if (string.IsNullOrEmpty(text))
                return;

            const int maxLineLength = 100; // 最大行長度
            // 標點符號：句點、驚嘆號、問號（全形和半形）
            char[] punctuationMarks = { '.', '。', '!', '！', '?', '？' };

            StringBuilder result = new StringBuilder();
            int currentPos = 0;
            int textLength = text.Length;
            // 預設換行符（當沒有找到換行符時使用）
            string defaultLineBreak = "\r\n";

            while (currentPos < textLength)
            {
                // 找到下一行的開始位置（下一個換行符或文本結尾）
                int lineStart = currentPos;
                int lineEnd = currentPos;
                string lineBreak = "";

                // 找到行尾（換行符或文本結尾）
                while (lineEnd < textLength)
                {
                    if (text[lineEnd] == '\r')
                    {
                        if (lineEnd + 1 < textLength && text[lineEnd + 1] == '\n')
                        {
                            lineBreak = "\r\n";
                            break;
                        }
                        else
                        {
                            lineBreak = "\r";
                            break;
                        }
                    }
                    else if (text[lineEnd] == '\n')
                    {
                        lineBreak = "\n";
                        break;
                    }
                    lineEnd++;
                }

                // 如果沒找到換行符，則到文本結尾
                if (lineEnd >= textLength)
                {
                    lineEnd = textLength;
                }

                // 取得當前行的內容（不含換行符）
                string line = text.Substring(lineStart, lineEnd - lineStart);

                // 如果行長度超過100個字符，需要處理
                if (line.Length > maxLineLength)
                {
                    int processedPos = 0;
                    // 如果沒有找到換行符，使用預設換行符
                    string currentLineBreak = string.IsNullOrEmpty(lineBreak) ? defaultLineBreak : lineBreak;

                    while (processedPos < line.Length)
                    {
                        int remainingLength = line.Length - processedPos;

                        // 如果剩餘部分小於最大長度，這是最後一段，直接添加（不需要斷行）
                        if (remainingLength < maxLineLength)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 如果剩餘部分正好等於最大長度，也需要檢查是否需要斷行
                        // 只有在這是最後一段且沒有更多內容時才直接添加
                        if (remainingLength == maxLineLength && processedPos + maxLineLength >= line.Length)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 從當前位置開始的第100個字符位置開始，尋找下一個標點符號
                        int searchStart = processedPos + maxLineLength;
                        int punctuationPos = -1;
                        const int maxSearchLength = 300; // 最多搜尋300個字

                        // 在剩餘文本中尋找標點符號（從第100個字符位置開始，最多搜尋300個字）
                        int searchEnd = Math.Min(searchStart + maxSearchLength, line.Length);
                        bool foundPunctuationIn300Chars = false;
                        for (int i = searchStart; i < searchEnd; i++)
                        {
                            if (Array.IndexOf(punctuationMarks, line[i]) >= 0)
                            {
                                // 規則4：檢查標點符號是否在"「"和"」"之間，如果是則跳過
                                if (!IsInsideQuotationMarks(line, processedPos, i))
                                {
                                    punctuationPos = i;
                                    foundPunctuationIn300Chars = true;
                                    break;
                                }
                            }
                        }

                        // 規則1：如果超過300個字都沒有找到句點、問號和驚嘆號，在第120個字之後找逗點
                        if (!foundPunctuationIn300Chars && searchEnd - searchStart >= maxSearchLength)
                        {
                            // 從第120個字之後（processedPos + 120）尋找逗點
                            int commaSearchStart = processedPos + 120;
                            int commaSearchEnd = Math.Min(commaSearchStart + maxSearchLength, line.Length);
                            for (int i = commaSearchStart; i < commaSearchEnd; i++)
                            {
                                if (line[i] == ',' || line[i] == '，')
                                {
                                    // 規則4：檢查逗點是否在"「"和"」"之間，如果是則跳過
                                    if (!IsInsideQuotationMarks(line, processedPos, i))
                                    {
                                        punctuationPos = i;
                                        break;
                                    }
                                }
                            }
                        }

                        // 計算分割後剩餘的字符數
                        int remainingAfterSplit = -1;
                        if (punctuationPos >= 0)
                        {
                            remainingAfterSplit = line.Length - (punctuationPos + 1);
                        }
                        else
                        {
                            remainingAfterSplit = line.Length - (processedPos + maxLineLength);
                        }

                        // 如果剩餘部分少於400字，且沒找到標點符號，不執行分割，直接添加剩餘內容
                        // 這樣可以避免在非符號處斷行
                        if (punctuationPos < 0 && remainingLength < (maxLineLength + maxSearchLength))
                        {
                            // 剩餘部分少於400字（100 + 300），且沒找到標點符號
                            // 不執行分割，直接添加剩餘內容，避免在非符號處斷行
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 如果分割後剩餘部分少於100個字符，不執行分割，直接添加剩餘內容
                        if (remainingAfterSplit < maxLineLength && remainingAfterSplit > 0)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 如果分割後剩餘部分正好等於100個字符，不執行分割，直接添加剩餘內容
                        // 這樣可以避免在非符號處斷行
                        if (remainingAfterSplit == maxLineLength)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 如果找到標點符號，在標點符號後截斷
                        if (punctuationPos >= 0)
                        {
                            int actualCutPos = punctuationPos + 1;

                            // 規則2：如果找到的句點是連續兩個以上的句點或驚嘆號或問號，從最後一個符號之後去添加斷行符號
                            while (actualCutPos < line.Length && Array.IndexOf(punctuationMarks, line[actualCutPos]) >= 0)
                            {
                                actualCutPos++;
                            }

                            // 規則3：如果是句點或驚嘆號或問號之後有"」"，從"」"之後去添加斷行符號
                            if (actualCutPos < line.Length && line[actualCutPos] == '」')
                            {
                                actualCutPos++;
                            }

                            // 檢查標點符號後面是否有空白字符，如果有則跳過，避免產生空白行
                            while (actualCutPos < line.Length && char.IsWhiteSpace(line[actualCutPos]))
                            {
                                actualCutPos++;
                            }

                            // 如果跳過空白字符後已經到行尾，說明標點符號後面只有空白字符，不在此處分割
                            if (actualCutPos >= line.Length)
                            {
                                // 不分割，直接添加剩餘內容
                                result.Append(line.Substring(processedPos));
                                break;
                            }

                            // 計算跳過連續標點符號和空白字符後的剩餘部分長度
                            int remainingAfterActualCut = line.Length - actualCutPos;

                            // 如果剩餘部分正好是100個字符或更少，不執行分割，直接添加剩餘內容
                            // 這樣可以避免在非符號處斷行
                            if (remainingAfterActualCut <= maxLineLength)
                            {
                                result.Append(line.Substring(processedPos));
                                break;
                            }

                            // 添加從當前位置到標點符號（包含標點符號）的內容
                            string segment = line.Substring(processedPos, punctuationPos - processedPos + 1);

                            // 規則1：如果原本是逗點，改成句號
                            if (segment.Length > 0)
                            {
                                char lastChar = segment[segment.Length - 1];
                                if (lastChar == ',' || lastChar == '，')
                                {
                                    // 將最後一個逗點改成句號
                                    segment = segment.Substring(0, segment.Length - 1) + '。';
                                }
                            }

                            result.Append(segment);
                            result.Append(currentLineBreak); // 插入換行符
                            processedPos = actualCutPos; // 從跳過空白字符後的位置繼續處理
                        }
                        else
                        {
                            // 如果沒找到標點符號，檢查是否應該強制截斷
                            int cutPos = processedPos + maxLineLength;

                            // 在最大長度處強制截斷
                            // 檢查截斷位置後面是否有空白字符，如果有則跳過
                            int actualCutPos = cutPos;

                            // 規則4：如果截斷位置在"「"和"」"之間，繼續向後查找直到引號外
                            while (actualCutPos < line.Length && IsInsideQuotationMarks(line, processedPos, actualCutPos))
                            {
                                actualCutPos++;
                            }

                            while (actualCutPos < line.Length && char.IsWhiteSpace(line[actualCutPos]))
                            {
                                actualCutPos++;
                            }

                            // 如果跳過空白字符後已經到行尾，不在此處分割
                            if (actualCutPos >= line.Length)
                            {
                                result.Append(line.Substring(processedPos));
                                break;
                            }

                            // 計算跳過引號和空白字符後的剩餘部分長度
                            int remainingAfterCut = line.Length - actualCutPos;

                            // 如果剩餘部分正好是100個字符或更少，不執行分割，直接添加剩餘內容
                            // 這樣可以避免在非符號處斷行
                            if (remainingAfterCut <= maxLineLength)
                            {
                                result.Append(line.Substring(processedPos));
                                break;
                            }

                            // 使用 actualCutPos 來計算截斷長度，確保截斷位置正確
                            result.Append(line.Substring(processedPos, actualCutPos - processedPos));
                            result.Append(currentLineBreak); // 插入換行符
                            processedPos = actualCutPos; // 從跳過空白字符後的位置繼續處理
                        }
                    }
                    // 長行分割後，最後一段後面添加原有的換行符（如果有的話）
                    if (!string.IsNullOrEmpty(lineBreak))
                    {
                        result.Append(lineBreak);
                    }
                }
                else
                {
                    // 行長度不超過100，直接添加
                    result.Append(line);
                    // 添加原有的換行符（如果有的話）
                    if (!string.IsNullOrEmpty(lineBreak))
                    {
                        result.Append(lineBreak);
                    }
                }

                // 更新位置
                if (!string.IsNullOrEmpty(lineBreak))
                {
                    currentPos = lineEnd + lineBreak.Length;
                }
                else
                {
                    currentPos = lineEnd;
                }
            }

            // 套用結果
            if (processWholeDocument)
            {
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                richTextBoxText.ScrollToCaret();
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }

        #endregion

        #region 狀態欄更新


        // 更新狀態欄顯示檔案資訊
        private void UpdateStatusLabel()
        {
            if (!richTextBoxText.Visible)
            {
                // 如果不是文字檔案模式，清空狀態欄（HTML檔案會單獨處理）
                // toolStripStatusLabelFixed.Text = "";
                return;
            }

            // 獲取當前檔案名稱（無路徑）
            string fileName = "";
            string fullPath = "";
            if (m_RecentReadListIndex >= 0 && m_RecentReadListIndex < m_RecentReadList.Count)
            {
                fullPath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                fileName = Path.GetFileName(fullPath);
            }

            // 更新檔案名稱到 toolStripStatusLabelFileName
            // 如果路徑長度超過 40 個字，顯示前3個字+"..."+最後34個字
            if (!string.IsNullOrEmpty(fullPath) && fullPath.Length > 36)
            {
                toolStripStatusLabelFileName.Text = fullPath.Substring(0, 3) + "..." + fullPath.Substring(fullPath.Length - 30);
            }
            else
            {
                toolStripStatusLabelFileName.Text = fullPath;
            }

            // 獲取總字數和選取的字數
            int totalChars = richTextBoxText.Text.Length;
            int selectedChars = richTextBoxText.SelectionLength;

            // 更新總字數和選取字數到 toolStripStatusLabelFixed
            toolStripStatusLabelFixed.Text = $"總字數: {totalChars:N0} | 選取字數: {selectedChars:N0}";
        }

        // 更新菜單項的啟用/禁用狀態
        private void UpdateMenuStatus()
        {
            // 檢查是否有開啟TXT檔案（richTextBoxText 可見且有檔案索引）
            bool hasTxtFileOpen = richTextBoxText.Visible &&
                                  m_RecentReadListIndex >= 0 &&
                                  m_RecentReadListIndex < m_RecentReadList.Count;

            // 更新TXT相關菜單項的狀態
            toolStripMenuItem_ConvertToSimplified.Enabled = hasTxtFileOpen;
            toolStripMenuItem_SaveTxtFile.Enabled = hasTxtFileOpen;
            toolStripMenuItem_SaveTxtAsNewFile.Enabled = hasTxtFileOpen;

            // 檢查是否開啟HTML檔案（webBrowser1 可見）
            bool hasHtmlFileOpen = webBrowser1.Visible;

            // 更新HTML相關菜單項的狀態
            toolStripMenuItem_CopyHtmlSaveFile.Enabled = hasHtmlFileOpen;
            toolStripMenuItem_CopyHtmlSaveFileSimplified.Enabled = hasHtmlFileOpen;
        }

        // 處理 richTextBoxText 文字變更事件
        private void RichTextBoxText_TextChanged(object? sender, EventArgs e)
        {
            UpdateStatusLabel();

            // 如果正在載入檔案，不設置修改標誌（避免載入文件時觸發）
            if (!m_IsLoadingFile && m_RecentReadListIndex >= 0)
            {
                // 文件載入後，使用者編輯了內容，設置修改標誌
                m_IsCurrentFileModified = true;
            }
        }

        // 處理 richTextBoxText 選擇變更事件
        private void RichTextBoxText_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateStatusLabel();
        }

        #endregion

        private void toolStripMenuItem_ConvertTraditional_Click(object sender, EventArgs e)
        {
            BatchConvertTxtFilesToTraditionalAndSave();
        }

        private void toolStripMenuItem_KeepTwoCRBetweenLines_Click(object sender, EventArgs e)
        {
            KeepTwoCRBetweenLines();
        }


        private void toolStripMenuItem_WithoutCRBetweenLines_Click(object sender, EventArgs e)
        {
            WithoutCRBetweenLines();
        }


        private void toolStripMenuItem_DelFiles_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }

        private void toolStripMenuItem_SelectedTextSaveAsNew_Click(object sender, EventArgs e)
        {
            SelectedTextSaveAsNew();
        }


        private void toolStripMenuItem_WholeTextSaveAsNew_Click(object sender, EventArgs e)
        {
            WholeTextSaveAsNew();
        }


        private void toolStripMenuItem_RemoveMoreThan120CharB_Click(object sender, EventArgs e)
        {
            RemoveMoreThan120Char();
        }

        private void toolStripMenuItem_RenameFile_Click(object sender, EventArgs e)
        {
            RenameFile();
        }



        private void toolStripMenuItem_SearchFiles_Click(object sender, EventArgs e)
        {
            SearchFiles();
        }



        private void toolStripMenuItem_SaveTxtFile_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void toolStripMenuItem_SaveTxtAsNewFile_Click(object sender, EventArgs e)
        {
            SaveTxtAsNewFile();
        }
        private void SaveTxtAsNewFile()
        {
            // 獲取 RichTextBox 的內容
            string content = richTextBoxText.Text;

            // 檢查內容是否為空
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("檔案內容為空，無法儲存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 使用 SaveFileDialog 讓使用者選擇儲存位置和檔名
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.Title = "另存新檔";
                saveFileDialog.DefaultExt = "txt";

                string? initialDirectory = null;
                string suggestedFileName = "";

                // 如果有當前開啟的檔案，使用其目錄和檔名作為初始值
                if (m_RecentReadListIndex >= 0 && m_RecentReadList.Count > 0)
                {
                    string currentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                    string? directory = Path.GetDirectoryName(currentFilePath);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentFilePath);
                    string extension = Path.GetExtension(currentFilePath);

                    if (!string.IsNullOrEmpty(directory))
                    {
                        initialDirectory = directory;
                        saveFileDialog.InitialDirectory = directory;
                    }

                    // 設置建議檔名（原檔名）
                    suggestedFileName = fileNameWithoutExtension + extension;
                    saveFileDialog.FileName = suggestedFileName;
                }
                else
                {
                    // 如果沒有當前開啟的檔案，使用預設檔名
                    suggestedFileName = "新文字檔.txt";
                    saveFileDialog.FileName = suggestedFileName;
                }

                // Windows SaveFileDialog 會自動選中文件名（不包括擴展名）當設置 FileName 時
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 儲存檔案，使用 UTF-8 編碼以避免中文亂碼
                        File.WriteAllText(saveFileDialog.FileName, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                        MessageBox.Show($"已成功儲存至：\n{saveFileDialog.FileName}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 檢查儲存的新檔案是否位於目前目錄
                        string? savedDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                        if (!string.IsNullOrEmpty(savedDirectory) &&
                            !string.IsNullOrEmpty(m_TreeViewSelectedNodeText) &&
                            Path.GetFullPath(savedDirectory).Equals(Path.GetFullPath(m_TreeViewSelectedNodeText), StringComparison.OrdinalIgnoreCase))
                        {
                            // 記錄當前選中的檔案名稱
                            string? currentSelectedFileName = null;
                            if (listViewFile.SelectedItems.Count > 0)
                            {
                                currentSelectedFileName = listViewFile.SelectedItems[0].Text;
                            }

                            // 重新載入目前資料夾檔案列表，並保持當前選中位置
                            if (treeViewFolder.SelectedNode != null)
                            {
                                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"儲存檔案時發生錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR_Click(object sender, EventArgs e)
        {
            RemoveLeadingAndTrailingSpaces();
        }

        private void toolStripMenuItem_EndingAddDot_Click(object sender, EventArgs e)
        {
            EndingAddDot();
        }


        private void toolStripButton_Option_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }

        private void ShowOptionsDialog()
        {
            using (FormOptions optionsDialog = new FormOptions(appSettings))
            {
                optionsDialog.Owner = this;
                if (optionsDialog.ShowDialog() == DialogResult.OK)
                {
                    // 設定已保存（在 FormOptions 中已處理）
                    // 可以在這裡添加需要立即生效的設定處理
                }
            }
        }

        private void toolStripMenuItem_RenameDirectory_Click(object sender, EventArgs e)
        {
            RenameDirectory();
        }

        private void toolStripMenuItem_OpenFileManager_Click(object sender, EventArgs e)
        {
            OpenFileManager();
        }

        private void OpenFileManager()
        {
            if (treeViewFolder.SelectedNode == null || treeViewFolder.SelectedNode.Tag is not DirectoryInfo)
                return;

            DirectoryInfo directoryInfo = (DirectoryInfo)treeViewFolder.SelectedNode.Tag;
            try
            {
                if (Directory.Exists(directoryInfo.FullName))
                {
                    System.Diagnostics.Process.Start("explorer.exe", directoryInfo.FullName);
                }
                else
                {
                    MessageBox.Show("目錄不存在。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("無法開啟檔案總管: " + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItem_AutoRemoveCRButton2_Click(object sender, EventArgs e)
        {
            AutoRemoveCR();
        }

        private void toolStripMenuItem_SaveTxtFile2_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void toolStripMenuItem_SaveTxtAsNewFile2_Click(object sender, EventArgs e)
        {
            SaveTxtAsNewFile();
        }
    }
}

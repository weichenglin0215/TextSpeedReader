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

        // 儲存目前檔案
        private void SaveCurrentFile()
        {
            SaveCurrentFile(true);
        }

        // 儲存目前檔案（可選擇是否顯示訊息）
        private void SaveCurrentFile(bool showMessage)
        {
            if (m_RecentReadListIndex >= 0)
            {
                string filePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                string content = richTextBoxText.Text;
                bool result = JTextFileLib.Instance().SaveTxtFile(filePath, content, false);
                if (result)
                {
                    // 儲存成功，重置修改標誌
                    m_IsCurrentFileModified = false;

                    // 重新載入目前資料夾檔案列表
                    if (treeViewFolder.SelectedNode != null)
                    {
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                    }
                    if (showMessage)
                    {
                        MessageBox.Show("儲存成功！", "提示");
                    }
                }
                else
                {
                    if (showMessage)
                    {
                        MessageBox.Show("儲存失敗！", "錯誤");
                    }
                }
            }
        }

        // 另存新檔
        private void SaveCurrentFileAs()
        {
            if (m_RecentReadListIndex >= 0)
            {
                string currentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                string? directory = Path.GetDirectoryName(currentFilePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentFilePath);
                string extension = Path.GetExtension(currentFilePath);

                // 如果沒有目錄，使用應用程式目錄
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Application.StartupPath;
                }

                // 使用 SaveFileDialog 讓使用者輸入檔名
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    saveFileDialog.FileName = fileNameWithoutExtension + extension;
                    saveFileDialog.InitialDirectory = directory;
                    saveFileDialog.Title = "另存新檔";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // 儲存檔案，使用 UTF-8 編碼以避免中文亂碼
                            File.WriteAllText(saveFileDialog.FileName, richTextBoxText.Text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                            // 另存新檔成功，重置修改標誌
                            m_IsCurrentFileModified = false;

                            MessageBox.Show($"已成功儲存至：\n{saveFileDialog.FileName}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // 重新載入目前資料夾檔案列表
                            if (treeViewFolder.SelectedNode != null)
                            {
                                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"儲存失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        // 顯示查找對話框
        private void ShowFindDialog()
        {
            // 只有在文字編輯模式下才顯示查找對話框
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (m_FindReplaceDialog == null || m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog = new FormFindReplace(richTextBoxText, true);
                m_FindReplaceDialog.Owner = this;
            }
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }

        // 顯示替換對話框
        private void ShowReplaceDialog()
        {
            // 只有在文字編輯模式下才顯示替換對話框
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 如果对话框已存在，先关闭它
            if (m_FindReplaceDialog != null && !m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog.Close();
                m_FindReplaceDialog.Dispose();
            }

            // 创建新的替换对话框
            m_FindReplaceDialog = new FormFindReplace(richTextBoxText, false);
            m_FindReplaceDialog.Owner = this;
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }

        #endregion

        #region 檔案樹狀檢視

        // 初始化並填入指定目錄的樹狀檢視資料
        private void PopulateTreeView(int subFolderDeepthLimited)
        {
            treeViewFolder.Nodes.Clear();
            TreeNode rootNode;

            // 建立目錄資訊物件
            DirectoryInfo info = new DirectoryInfo(fileManager.FullPath);
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name.Substring(0, info.Name.Length - 1));
                rootNode.Tag = info;
                // 遞迴獲取子目錄，限制遍歷深度
                fileManager.GetDirectories(info.GetDirectories(), rootNode, 0, subFolderDeepthLimited);
                treeViewFolder.Nodes.Add(rootNode);
                treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];
                m_TreeViewSelectedNodeText = treeViewFolder.SelectedNode.FullPath;
                treeViewFolder.SelectedNode.Expand();
            }
        }

        // 初始化並填入所有磁碟機的樹狀檢視資料
        private void PopulateTreeViewAll(int subFolderDeepthLimited)
        {
            treeViewFolder.Nodes.Clear();
            // 所有可能的磁碟機路徑
            string[] AllDisk = { @"C:\\", @"D:\\", @"E:\\", @"F:\\", @"G:\\", @"H:\\", @"I:\\", @"J:\\", @"K:\\" };
            foreach (var item in AllDisk)
            {
                TreeNode rootNode;
                DirectoryInfo info = new DirectoryInfo(item);
                if (info.Exists)
                {
                    // 建立磁碟機節點
                    rootNode = new TreeNode(info.Name.Substring(0, info.Name.Length - 1));
                    rootNode.Tag = info;
                    // 僅加載第一層子目錄（減少資源消耗）
                    fileManager.GetDirectories(info.GetDirectories(), rootNode, 0, 1);
                    treeViewFolder.Nodes.Add(rootNode);
                }
            }

            // 首先選擇第一個節點（確保有選中的節點）
            if (treeViewFolder.Nodes.Count > 0)
            {
                treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];
                m_TreeViewSelectedNodeText = treeViewFolder.SelectedNode.FullPath;
                treeViewFolder.SelectedNode.Expand();
            }

            // 如果啟用自動開啟上次目錄，逐步展開到上次的目錄
            if (appSettings.AutoOpenLastDirectory && !string.IsNullOrEmpty(appSettings.LastDirectory))
            {
                ExpandToLastDirectory(appSettings.LastDirectory);
            }
        }

        // 根據 LastDirectory 逐步展開到目標目錄
        private void ExpandToLastDirectory(string targetPath)
        {
            if (string.IsNullOrEmpty(targetPath))
                return;

            try
            {
                // 解析路徑成分
                string[] pathParts = targetPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (pathParts.Length == 0)
                    return;

                // 第一部分應該是磁碟機代號（如 "D:"）
                string driveLetter = pathParts[0];
                if (!driveLetter.EndsWith(":"))
                    driveLetter += ":";

                // 查找對應的磁碟機節點
                TreeNode? driveNode = null;
                foreach (TreeNode node in treeViewFolder.Nodes)
                {
                    if (node.Text.Equals(driveLetter, StringComparison.OrdinalIgnoreCase))
                    {
                        driveNode = node;
                        break;
                    }
                }

                if (driveNode == null)
                    return;

                // 選擇磁碟機節點
                treeViewFolder.SelectedNode = driveNode;
                m_TreeViewSelectedNodeText = driveNode.FullPath;
                driveNode.Expand();
                driveNode.EnsureVisible();

                // 手動觸發 AfterSelect 事件
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(driveNode));

                // 從第二個部分開始逐步展開（跳過磁碟機代號）
                TreeNode? currentNode = driveNode;
                for (int i = 1; i < pathParts.Length; i++)
                {
                    string partName = pathParts[i];
                    TreeNode? foundChild = null;

                    // 在當前節點的子節點中查找
                    foreach (TreeNode child in currentNode.Nodes)
                    {
                        if (child.Text.Equals(partName, StringComparison.OrdinalIgnoreCase))
                        {
                            foundChild = child;
                            break;
                        }
                    }

                    if (foundChild == null)
                    {
                        // 如果找不到子節點，嘗試展開當前節點並重新查找
                        currentNode.Expand();
                        foreach (TreeNode child in currentNode.Nodes)
                        {
                            if (child.Text.Equals(partName, StringComparison.OrdinalIgnoreCase))
                            {
                                foundChild = child;
                                break;
                            }
                        }

                        // 如果還是找不到，停止展開
                        if (foundChild == null)
                            break;
                    }

                    // 選擇找到的子節點
                    treeViewFolder.SelectedNode = foundChild;
                    m_TreeViewSelectedNodeText = foundChild.FullPath;
                    foundChild.Expand();
                    foundChild.EnsureVisible();

                    // 手動觸發 AfterSelect 事件
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(foundChild));

                    currentNode = foundChild;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"展開到上次目錄時發生錯誤: {ex.Message}");
            }
        }

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
            if (e.Node == null)
                return;

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

            // 更新狀態欄顯示檔案數量和選取數量
            UpdateFileSelectionStatus();
        }

        // 處理檔案列表滑鼠按下事件（用於區分左右鍵）
        private void ListViewFile_MouseDown(object sender, MouseEventArgs e)
        {
            // 記錄是否為右鍵點擊
            if (e.Button == MouseButtons.Right)
            {
                m_IsRightClick = true;
                m_LastRightClickTime = DateTime.Now;
            }
            else
            {
                m_IsRightClick = false;
            }
        }

        // 處理檔案列表滑鼠點擊事件（用於區分左右鍵）
        private void ListViewFile_MouseClick(object sender, MouseEventArgs e)
        {
            // 記錄是否為右鍵點擊
            if (e.Button == MouseButtons.Right)
            {
                m_IsRightClick = true;
                m_LastRightClickTime = DateTime.Now;
            }
            else
            {
                m_IsRightClick = false;
            }
        }

        // 處理檔案列表選擇變更事件
        private void ListViewFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 更新狀態欄顯示檔案數量和選取數量
            UpdateFileSelectionStatus();

            // 如果沒有選中任何檔案，更新菜單狀態並返回
            if (this.listViewFile.SelectedItems.Count == 0)
            {
                UpdateMenuStatus();
                return;
            }

            // 檢查是否為右鍵點擊（檢查標記和時間戳，如果在最近500毫秒內有右鍵點擊，則不打開檔案）
            if (m_IsRightClick || (DateTime.Now - m_LastRightClickTime).TotalMilliseconds < 500)
            {
                m_IsRightClick = false; // 重置標記
                return;
            }

            // 檢查是否為文字檔案
            Console.WriteLine("this.listViewFile.SelectedItems.Count " + this.listViewFile.SelectedItems.Count);
            //Console.WriteLine("this.listViewFile.SelectedItems[0].Text " + this.listViewFile.SelectedItems[0].Text);
            if (this.listViewFile.SelectedItems.Count > 0 && Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".txt")
            {
                string tmpFullFileName = m_TreeViewSelectedNodeText + @"\" + this.listViewFile.SelectedItems[0].Text;

                // 使用編碼檢測讀取檔案內容
                string tmpString = ReadFileWithEncodingDetection(tmpFullFileName);

                // 檢查選中的檔案是否是當前正在編輯的檔案
                // 需要檢查 m_RecentReadListIndex 是否有效，以及對應的檔案是否仍然存在
                if (m_RecentReadListIndex >= 0 &&
                    m_RecentReadListIndex < m_RecentReadList.Count &&
                    m_RecentReadList[m_RecentReadListIndex].FileFullName == tmpFullFileName &&
                    File.Exists(tmpFullFileName))
                {
                    // 檢查當前檔案是否有未保存的修改
                    if (m_IsCurrentFileModified)
                    {
                        // 有未保存的修改，詢問使用者是否放棄編輯內容
                        DialogResult result = MessageBox.Show(
                            $"檔案「{Path.GetFileName(tmpFullFileName)}」有未保存的修改，是否放棄編輯內容並重新載入檔案？",
                            "確認重新載入",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result != DialogResult.Yes)
                        {
                            // 使用者選擇不重新載入，取消選擇
                            if (listViewFile.SelectedItems.Count > 0)
                            {
                                listViewFile.SelectedItems[0].Selected = false;
                            }
                            return; // 取消操作
                        }
                        // 使用者確認重新載入，繼續執行後續的載入邏輯
                    }
                    else
                    {
                        // 內容沒有修改，直接返回，不需要重新載入
                        return;
                    }
                }
                else
                {
                    // 選中的是其他檔案，檢查當前檔案是否有未保存的修改
                    if (m_RecentReadListIndex >= 0 && richTextBoxText.Visible)
                    {
                        // 檢查當前檔案是否有未保存的修改
                        if (m_IsCurrentFileModified)
                        {
                            string currentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;

                            // 有未保存的修改，顯示確認對話框
                            using (FormSaveConfirm saveConfirmDialog = new FormSaveConfirm(Path.GetFileName(currentFilePath)))
                            {
                                saveConfirmDialog.Owner = this;
                                if (saveConfirmDialog.ShowDialog() == DialogResult.OK)
                                {
                                    switch (saveConfirmDialog.SelectedOption)
                                    {
                                        case FormSaveConfirm.SaveOption.No:
                                            // 不保存，直接載入新檔案
                                            break;

                                        case FormSaveConfirm.SaveOption.SaveAs:
                                            // 另存新檔
                                            SaveCurrentFileAs();
                                            break;

                                        case FormSaveConfirm.SaveOption.Save:
                                            // 儲存檔案（覆蓋目前檔案），不顯示訊息（因為即將切換檔案）
                                            SaveCurrentFile(false);
                                            break;
                                    }
                                }
                                else
                                {
                                    // 使用者取消對話框，取消選擇
                                    if (listViewFile.SelectedItems.Count > 0)
                                    {
                                        listViewFile.SelectedItems[0].Selected = false;
                                    }
                                    return; // 取消操作
                                }
                            }
                        }
                    }
                }

                // 讀取文字檔案內容
                if (JTextFileLib.Instance().ReadTxtFile(tmpFullFileName, ref tmpString, false))
                {
                    // 如果有歷史記錄，更新當前檔案的閱讀位置
                    if (m_RecentReadListIndex >= 0)
                    {
                        FileSystemManager.RecentReadList tmpRecentReadList = m_RecentReadList[m_RecentReadListIndex];
                        // 獲取第一個可見字元的索引
                        int firstVisibleChar = richTextBoxText.GetCharIndexFromPosition(new Point(0, 0));
                        tmpRecentReadList.LastCharCount = firstVisibleChar;
                        m_RecentReadList[m_RecentReadListIndex] = tmpRecentReadList;

                        // 更新最近閱讀檔案列表的顯示
                        listViewRecentFiles.Items[m_RecentReadListIndex].SubItems[1].Text =
                            m_RecentReadList[m_RecentReadListIndex].LastCharCount.ToString();
                    }

                    // 檢查新選擇的檔案是否在最近閱讀清單中
                    int tmpCount = 0;
                    m_RecentReadListIndex = -1;
                    foreach (var item in m_RecentReadList)
                    {
                        if (item.FileFullName == tmpFullFileName)
                        {
                            m_RecentReadListIndex = tmpCount;
                            break;
                        }
                        tmpCount++;
                    }

                    // 如果是新檔案，添加到最近閱讀清單
                    if (m_RecentReadListIndex == -1)
                    {
                        FileSystemManager.RecentReadList tmpRecentReadList = new FileSystemManager.RecentReadList();
                        tmpRecentReadList.FileFullName = tmpFullFileName;
                        tmpRecentReadList.LastCharCount = 0;
                        m_RecentReadList.Add(tmpRecentReadList);
                        m_RecentReadListIndex = m_RecentReadList.Count - 1;

                        // 添加到最近閱讀檔案列表顯示
                        ListViewItem item = new ListViewItem(m_RecentReadList[m_RecentReadListIndex].FileFullName, 1);
                        ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[]
                           { new ListViewItem.ListViewSubItem(item, m_RecentReadList[m_RecentReadListIndex].LastCharCount.ToString())};
                        item.SubItems.AddRange(subItems);
                        listViewRecentFiles.Items.Add(item);
                    }

                    // 顯示文字內容並定位到上次閱讀位置
                    this.richTextBoxText.Visible = true;
                    webBrowser1.Visible = false;

                    // 設置載入標誌，避免 TextChanged 事件設置修改標誌
                    m_IsLoadingFile = true;
                    this.richTextBoxText.Text = tmpString;
                    m_IsLoadingFile = false;
                    // 文件載入完成，重置修改標誌
                    m_IsCurrentFileModified = false;

                    richTextBoxText.SelectionStart = m_RecentReadList[m_RecentReadListIndex].LastCharCount;
                    richTextBoxText.SelectionLength = 0; // 確保選取字數為0
                    richTextBoxText.ScrollToCaret();

                    // 更新狀態欄顯示檔案資訊
                    UpdateStatusLabel();

                    // 更新菜單狀態
                    UpdateMenuStatus();
                }
            }

            // 處理HTML檔案
            if (this.listViewFile.SelectedItems.Count > 0 && (Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".html" || Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".htm"))
            {
                try
                {
                    // 切換到網頁瀏覽器顯示模式
                    this.richTextBoxText.Visible = false;
                    webBrowser1.Visible = true;
                    // 載入HTML檔案
                    m_CurrentHtmlFilePath = Path.Combine(m_TreeViewSelectedNodeText, this.listViewFile.SelectedItems[0].Text);

                    // 讀取並處理HTML檔案內容，確保正確編碼顯示
                    LoadHtmlFileWithEncoding(m_CurrentHtmlFilePath);

                    // 更新狀態欄：顯示HTML檔案名稱，清空固定狀態欄
                    string htmlFileName = Path.GetFileName(m_CurrentHtmlFilePath);
                    toolStripStatusLabelFileName.Text = htmlFileName;
                    toolStripStatusLabelFixed.Text = "";

                    // 更新菜單狀態
                    UpdateMenuStatus();
                }
                catch (System.UriFormatException exc)
                {
                    Console.WriteLine(exc);
                    return;
                }
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

        // 處理網頁導航
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;

            // 確保網址格式正確
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                webBrowser1.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        // 處理網頁導航完成事件
        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //toolStripTextBox1.Text = webBrowser1.Url.ToString();
        }

        // 處理 WebBrowser 文檔載入完成事件
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ApplyWebBrowserDefaultStyle();

            // 清理臨時HTML檔案
            if (!string.IsNullOrEmpty(m_TempHtmlFilePath) && File.Exists(m_TempHtmlFilePath))
            {
                try
                {
                    // 延遲刪除，確保WebBrowser完全載入
                    System.Threading.Timer timer = null;
                    timer = new System.Threading.Timer((state) =>
                    {
                        try
                        {
                            if (File.Exists(m_TempHtmlFilePath))
                            {
                                File.Delete(m_TempHtmlFilePath);
                            }
                            m_TempHtmlFilePath = "";
                        }
                        catch { }
                        finally
                        {
                            timer?.Dispose();
                        }
                    }, null, 1000, System.Threading.Timeout.Infinite);
                }
                catch { }
            }
        }

        // 應用 WebBrowser 預設樣式（背景顏色、字型、字體尺寸）
        private void ApplyWebBrowserDefaultStyle()
        {
            if (webBrowser1.Document == null)
                return;

            try
            {
                // 獲取當前 richTextBoxText 的字體設定作為參考
                Font currentFont = richTextBoxText.Font;
                string fontFamily = currentFont.FontFamily.Name;
                float fontSize = currentFont.SizeInPoints;

                // 設定背景顏色（例如：白色 #FFFFFF，或淺灰色 #F5F5F5）
                // 可以根據需要修改顏色值
                //string backgroundColor = "#FFFFFF"; // 白色背景
                string backgroundColor = "#000000"; // 黑色背景

                // 設定字型家族（處理字體名稱中的特殊字符）
                string escapedFontFamily = fontFamily.Replace("'", "\\'");

                // 設定文字顏色
                string textColor = "#FFFFFF"; // 白色文字

                // 方法1：直接設置 body 樣式（簡單但可能被 HTML 內聯樣式覆蓋）
                if (webBrowser1.Document.Body != null)
                {
                    string style = $"font-family: '{escapedFontFamily}', sans-serif; font-size: {fontSize}pt; background-color: {backgroundColor}; color: {textColor};";
                    webBrowser1.Document.Body.Style = style;
                }

                // 方法2：注入 CSS 樣式到 head（更可靠，優先級更高）
                HtmlElement? headElement = webBrowser1.Document.GetElementsByTagName("head")[0];
                if (headElement != null)
                {
                    HtmlElement? styleElement = webBrowser1.Document.CreateElement("style");
                    if (styleElement != null)
                    {
                        // 使用 !important 確保樣式優先級
                        string css = $@"
                            body {{
                                font-family: '{escapedFontFamily}', sans-serif !important;
                                font-size: {fontSize}pt !important;
                                background-color: {backgroundColor} !important;
                                color: {textColor} !important;
                            }}
                        ";
                        styleElement.SetAttribute("type", "text/css");
                        styleElement.InnerHtml = css;
                        headElement.AppendChild(styleElement);
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果設置樣式失敗，不影響其他功能
                Console.WriteLine($"設置 WebBrowser 樣式時發生錯誤: {ex.Message}");
            }
        }

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

        // 增加字體大小
        private void FontSizeAdd(object sender, EventArgs e)
        {
            if (richTextBoxText.Visible)
            {
                Font tmpFont = richTextBoxText.Font;
                float tmpFontSize = richTextBoxText.Font.Size;
                // 在預設字體大小列表中尋找下一個較大的尺寸
                for (int i = 0; i < m_FontSize.Length; i++)
                {
                    if (m_FontSize[i] > tmpFontSize)
                    {
                        tmpFontSize = m_FontSize[i];
                        break;
                    }
                }
                // 套用新字體大小
                Font newFont = new Font(tmpFont.FontFamily, tmpFontSize);
                richTextBoxText.Font = newFont;
            }
            else if (webBrowser1.Visible)
            {
                // 在預設縮放比例列表中尋找下一個較大的比例
                for (int i = 0; i < m_WebBrowserSize.Length; i++)
                {
                    if (m_WebBrowserSize[i] > m_WebBrowserZoom)
                    {
                        m_WebBrowserZoom = m_WebBrowserSize[i];
                        break;
                    }
                }
                // 套用新縮放比例
                if (webBrowser1.Document?.Body != null)
                {
                    webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
                }
            }
        }

        // 減少字體大小按鈕點擊事件
        private void FontSizeReduce(object sender, EventArgs e)
        {
            if (richTextBoxText.Visible)
            {
                Font tmpFont = richTextBoxText.Font;
                float tmpFontSize = richTextBoxText.Font.Size;
                // 在預設字體大小列表中尋找下一個較小的尺寸
                for (int i = m_FontSize.Length - 1; i >= 0; i--)
                {
                    if (m_FontSize[i] < tmpFontSize)
                    {
                        tmpFontSize = m_FontSize[i];
                        break;
                    }
                }
                // 套用新字體大小
                Font newFont = new Font(tmpFont.FontFamily, tmpFontSize);
                richTextBoxText.Font = newFont;
            }
            else if (webBrowser1.Visible)
            {
                // 在預設縮放比例列表中尋找下一個較小的比例
                for (int i = m_WebBrowserSize.Length - 1; i >= 0; i--)
                {
                    if (m_WebBrowserSize[i] < m_WebBrowserZoom)
                    {
                        m_WebBrowserZoom = m_WebBrowserSize[i];
                        break;
                    }
                }
                // 套用新縮放比例
                if (webBrowser1.Document?.Body != null)
                {
                    webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
                }
            }
        }
        // 變更文字字體
        private void ChangeFont(object sender, EventArgs e)
        {
            int count = m_FontFamilies.Length;
            // 尋找選擇的字體並套用
            object? selectedItem = toolStripComboBoxFonts.SelectedItem;
            if (selectedItem == null)
                return;

            for (int j = 0; j < count; ++j)
            {
                if (m_FontFamilies[j] != null && m_FontFamilies[j].Name == (string)selectedItem)
                {
                    Font newFont = new Font(m_FontFamilies[j], richTextBoxText.Font.Size, richTextBoxText.Font.Style);
                    richTextBoxText.Font = newFont;
                }
            }
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

        // 自動移除目前文章中多餘的斷行（按鈕）
        private void AutoRemoveCRButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCR();
        }

        // 自動移除目前文章中多餘的斷行
        private void AutoRemoveCR()
        {
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
            //如果選擇的字串最後一個字是斷行或分行符號就少選一個字元
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
                richTextBoxText.SelectionLength = richTextBoxText.SelectionLength - 2;
                //MessageBox.Show("選取內容最後有斷行與新行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
                //MessageBox.Show("選取內容最後有斷行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
                //MessageBox.Show("選取內容最後有新行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            int textLength = text.Length;
            int currentPos = 0;
            StringBuilder result = new StringBuilder();
            while (currentPos < textLength)
            {
                int paragraphStart = currentPos;
                int paragraphEnd = currentPos;
                int doubleBreakStart = -1;
                // 搜尋當前段落結束（兩個連續斷行符號）
                while (paragraphEnd < textLength)
                {
                    if (text[paragraphEnd] == '\r' || text[paragraphEnd] == '\n')
                    {
                        int firstBreakEnd = paragraphEnd + 1;
                        if (paragraphEnd < textLength - 1 && text[paragraphEnd] == '\r' && text[paragraphEnd + 1] == '\n')
                            firstBreakEnd = paragraphEnd + 2;
                        if (firstBreakEnd < textLength && (text[firstBreakEnd] == '\r' || text[firstBreakEnd] == '\n'))
                        {
                            doubleBreakStart = paragraphEnd;
                            break;
                        }
                    }
                    paragraphEnd++;
                }
                // 沒找到則延伸到結尾
                if (doubleBreakStart == -1)
                    paragraphEnd = textLength;
                else
                    paragraphEnd = doubleBreakStart;
                if (paragraphEnd > paragraphStart)
                {
                    string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);
                    // 移除斷行合併
                    string mergedLine = "";
                    mergedLine = paragraph.Replace("\r", "").Replace("\n", "");
                    result.Append(mergedLine);
                }
                // 處理段落分隔符
                if (doubleBreakStart >= 0)
                {
                    // 包含分隔斷行符們
                    int sepEnd = doubleBreakStart;
                    while (sepEnd < textLength && (text[sepEnd] == '\r' || text[sepEnd] == '\n'))
                        sepEnd++;
                    result.Append(text.Substring(doubleBreakStart, sepEnd - doubleBreakStart));
                    currentPos = sepEnd;
                }
                else
                {
                    currentPos = paragraphEnd;
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

        // 自動移除目前文章中多餘的斷行，不包含該行最後一個字是句點或驚嘆號的行（按鈕）
        private void AutoRemoveCRWithoutDotAndExclamationMarkButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCRWithoutDotAndExclamationMark();
        }

        // 自動移除目前文章中多餘的斷行，不包含該行最後一個字是句點或驚嘆號的行
        private void AutoRemoveCRWithoutDotAndExclamationMark()
        {
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
            //如果選擇的字串最後一個字是斷行或分行符號就少選一個字元
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
                richTextBoxText.SelectionLength = richTextBoxText.SelectionLength - 2;
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
            }
            int textLength = text.Length;
            int currentPos = 0;
            StringBuilder result = new StringBuilder();
            while (currentPos < textLength)
            {
                int paragraphStart = currentPos;
                int paragraphEnd = currentPos;
                int doubleBreakStart = -1;
                // 搜尋當前段落結束（兩個連續斷行符號）
                while (paragraphEnd < textLength)
                {
                    if (text[paragraphEnd] == '\r' || text[paragraphEnd] == '\n')
                    {
                        int firstBreakEnd = paragraphEnd + 1;
                        if (paragraphEnd < textLength - 1 && text[paragraphEnd] == '\r' && text[paragraphEnd + 1] == '\n')
                            firstBreakEnd = paragraphEnd + 2;
                        if (firstBreakEnd < textLength && (text[firstBreakEnd] == '\r' || text[firstBreakEnd] == '\n'))
                        {
                            doubleBreakStart = paragraphEnd;
                            break;
                        }
                    }
                    paragraphEnd++;
                }
                // 沒找到則延伸到結尾
                if (doubleBreakStart == -1)
                    paragraphEnd = textLength;
                else
                    paragraphEnd = doubleBreakStart;
                if (paragraphEnd > paragraphStart)
                {
                    string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);
                    // 處理段落內的斷行，移除斷行但保留以句點或驚嘆號結尾的行的斷行
                    StringBuilder mergedParagraph = new StringBuilder();
                    int lineStart = 0;
                    while (lineStart < paragraph.Length)
                    {
                        int lineBreakPos = paragraph.IndexOfAny(new char[] { '\r', '\n' }, lineStart);
                        if (lineBreakPos == -1) lineBreakPos = paragraph.Length;
                        string line = paragraph.Substring(lineStart, lineBreakPos - lineStart);
                        // 檢查該行最後一個字是否為句點或驚嘆號
                        char lastChar = line.Length > 0 ? line[line.Length - 1] : '\0';
                        bool shouldKeepLineBreak = (lastChar == '.' || lastChar == '。' || lastChar == '!' || lastChar == '！' || lastChar == '?' || lastChar == '？');
                        // 移除行內的斷行符號，但保留行內容
                        string lineWithoutBreaks = line.Replace("\r", "").Replace("\n", "");
                        mergedParagraph.Append(lineWithoutBreaks);
                        // 如果該行以句點或驚嘆號結尾，保留斷行符號
                        if (shouldKeepLineBreak && lineBreakPos < paragraph.Length)
                        {
                            // 保留斷行符號
                            if (lineBreakPos < paragraph.Length - 1 && paragraph[lineBreakPos] == '\r' && paragraph[lineBreakPos + 1] == '\n')
                            {
                                mergedParagraph.Append("\r\n");
                                lineStart = lineBreakPos + 2;
                            }
                            else if (paragraph[lineBreakPos] == '\r' || paragraph[lineBreakPos] == '\n')
                            {
                                mergedParagraph.Append(paragraph[lineBreakPos]);
                                lineStart = lineBreakPos + 1;
                            }
                            else
                            {
                                lineStart = lineBreakPos;
                            }
                        }
                        else
                        {
                            // 不保留斷行符號，繼續處理下一行
                            lineStart = lineBreakPos;
                            if (lineStart < paragraph.Length)
                            {
                                if (lineStart < paragraph.Length - 1 && paragraph[lineStart] == '\r' && paragraph[lineStart + 1] == '\n')
                                    lineStart += 2;
                                else if (paragraph[lineStart] == '\r' || paragraph[lineStart] == '\n')
                                    lineStart++;
                            }
                        }
                    }
                    result.Append(mergedParagraph.ToString());
                }
                // 處理段落分隔符
                if (doubleBreakStart >= 0)
                {
                    // 包含分隔斷行符們
                    int sepEnd = doubleBreakStart;
                    while (sepEnd < textLength && (text[sepEnd] == '\r' || text[sepEnd] == '\n'))
                        sepEnd++;
                    result.Append(text.Substring(doubleBreakStart, sepEnd - doubleBreakStart));
                    currentPos = sepEnd;
                }
                else
                {
                    currentPos = paragraphEnd;
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

        // 自動移除目前文章中多餘的斷行，包含句點與驚嘆號標點符號
        private void AutoRemoveCRWithDotAndExclamationMark()
        {
            string text = richTextBoxText.Text;
            int length = text.Length;
            int start = richTextBoxText.SelectionStart;
            int end = richTextBoxText.SelectionStart + richTextBoxText.SelectionLength;
            while (end < length)
            {
                int lineBreakPos = text.IndexOfAny(new char[] { '\r', '\n' }, end);
                if (lineBreakPos == -1) lineBreakPos = length;
                string line = text.Substring(end, lineBreakPos - end);
                char lastChar = line.Length > 0 ? line[line.Length - 1] : '\0';
                if (lastChar == '.' || lastChar == '。' || lastChar == '!' || lastChar == '！')
                {
                    end = lineBreakPos;
                }
                else
                {
                    end = lineBreakPos;
                }
                end = lineBreakPos;
                // 跳過 \r\n
                if (end < length - 1 && text[end] == '\r' && text[end + 1] == '\n') end += 2;
                else if (text[end] == '\r' || text[end] == '\n') end++;
            }
            richTextBoxText.Select(start, Math.Max(0, end - start));
        }

        // 移除行首和行尾的空白字元（按鈕）
        private void RemoveLeadSpace_Click(object sender, EventArgs e)
        {
            RemoveLeadingAndTrailingSpaces();
        }

        // 移除行首和行尾的空白字元
        private void RemoveLeadingAndTrailingSpaces()
        {
            //若該行只有空白字元或TAB字元，請移除該行文字開頭的空白或TAB符號

            // 獲取選定文本
            string selectedText = richTextBoxText.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                // 如果沒有選定文本，處理整個文檔
                selectedText = richTextBoxText.Text;
                if (string.IsNullOrEmpty(selectedText))
                    return;

                // 處理整個文檔
                ProcessRemoveLeadingSpaces(selectedText, true);
                selectedText = richTextBoxText.Text; //更新選取的文字
                ProcessRemoveEndingSpaces(selectedText, true); //處理整個文檔的行尾空白
            }
            else
            {
                // 處理選定範圍
                ProcessRemoveLeadingSpaces(selectedText, false);
                selectedText = richTextBoxText.SelectedText; //更新選取的文字
                ProcessRemoveEndingSpaces(selectedText, false); //處理選定範圍的行尾空白
            }
        }

        // (1) 移除行首連續的空白/TAB/全形空白；(2) 移除行尾連續的空白/TAB/全形空白
        private void ProcessRemoveLeadingSpaces(string text, bool processWholeDocument)
        {
            if (string.IsNullOrEmpty(text))
                return;

            StringBuilder result = new StringBuilder();
            bool hasChanges = false;
            int textLength = text.Length;
            int i = 0;

            while (i < textLength)
            {
                int lineStart = i;
                bool lineEndedWithBreak = false;

                while (i < textLength)
                {
                    char c = text[i];
                    // 包括半形空白, TAB, 全形空白(U+3000)
                    if (c == '\r' || c == '\n')
                    {
                        lineEndedWithBreak = true;
                        string line = text.Substring(lineStart, i - lineStart);
                        string processedLine = ProcessLineForLead(line, ref hasChanges);
                        result.Append(processedLine);

                        // 處理 \r\n 換行
                        if (c == '\r' && i + 1 < textLength && text[i + 1] == '\n')
                        {
                            result.Append("\r\n");
                            i += 2;
                        }
                        else
                        {
                            result.Append(c);
                            i++;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (i >= textLength && !lineEndedWithBreak && lineStart < textLength)
                {
                    string line = text.Substring(lineStart, textLength - lineStart);
                    string processedLine = ProcessLineForLead(line, ref hasChanges);
                    result.Append(processedLine);
                    break;
                }
            }
            if (hasChanges)
            {
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
                    int selStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectedText = result.ToString();
                    int newLength = result.Length;
                    richTextBoxText.Select(selStart, newLength);
                }
            }
        }

        private string ProcessLineForLead(string line, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            var trimmedLine = RemoveLeadingFullWhitespace(line);
            if (!object.ReferenceEquals(line, trimmedLine))
                hasChanges = true;
            return trimmedLine;
        }
        private string RemoveLeadingFullWhitespace(string line)
        {
            int i = 0;
            while (i < line.Length && (line[i] == ' ' || line[i] == '\t' || line[i] == '\u3000'))
            {
                i++;
            }
            return line.Substring(i);
        }

        private void ProcessRemoveEndingSpaces(string text, bool processWholeDocument)
        {
            if (string.IsNullOrEmpty(text))
                return;
            StringBuilder result = new StringBuilder();
            bool hasChanges = false;
            int textLength = text.Length;
            int i = 0;

            while (i < textLength)
            {
                int lineStart = i;
                bool lineEndedWithBreak = false;
                while (i < textLength)
                {
                    char c = text[i];
                    if (c == '\r' || c == '\n')
                    {
                        lineEndedWithBreak = true;
                        string line = text.Substring(lineStart, i - lineStart);
                        string processedLine = RemoveTrailingSpacesFull(line, ref hasChanges);
                        result.Append(processedLine);
                        if (c == '\r' && i + 1 < textLength && text[i + 1] == '\n')
                        {
                            result.Append("\r\n");
                            i += 2;
                        }
                        else
                        {
                            result.Append(c);
                            i++;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (i >= textLength && !lineEndedWithBreak && lineStart < textLength)
                {
                    string line = text.Substring(lineStart, textLength - lineStart);
                    string processedLine = RemoveTrailingSpacesFull(line, ref hasChanges);
                    result.Append(processedLine);
                    break;
                }
            }
            if (hasChanges)
            {
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
                    int selStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectedText = result.ToString();
                    int newLength = result.Length;
                    richTextBoxText.Select(selStart, newLength);
                }
            }
        }
        private string RemoveTrailingSpacesFull(string line, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            int lastIndex = line.Length - 1;
            while (lastIndex >= 0 && (line[lastIndex] == ' ' || line[lastIndex] == '\t' || line[lastIndex] == '\u3000'))
            {
                lastIndex--;
            }
            if (lastIndex < line.Length - 1)
            {
                hasChanges = true;
                return line.Substring(0, lastIndex + 1);
            }
            return line;
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

        // 將目前TXT繁體中文轉換成簡體中文並儲存（按鈕）
        private void buttonConvertToSimplified_Click(object sender, EventArgs e)
        {
            ConvertCurrentTxtToSimplifiedAndSave();
        }

        // 將目前TXT繁體中文轉換成簡體中文並儲存
        private void ConvertCurrentTxtToSimplifiedAndSave()
        {
            // 檢查是否有當前打開的檔案
            if (m_RecentReadListIndex < 0)
            {
                MessageBox.Show("請先打開一個文字檔案！", "提示");
                return;
            }

            string originalFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;

            // 檢查是否為文字檔案
            if (Path.GetExtension(originalFilePath).ToLower() != ".txt")
            {
                MessageBox.Show("目前檔案不是.TXT檔案！", "提示");
                return;
            }

            // 獲取當前文字內容
            string traditionalText = richTextBoxText.Text;

            if (string.IsNullOrEmpty(traditionalText))
            {
                MessageBox.Show("檔案內容為空！", "提示");
                return;
            }

            try
            {
                // 使用Microsoft.VisualBasic.Strings.StrConv進行繁簡轉換
                // VbStrConv.SimplifiedChinese: 繁體轉簡體
                const int SimplifiedChineseLcid = 0x0804; // zh-CN
                string? simplifiedText = Strings.StrConv(traditionalText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);
                if (string.IsNullOrEmpty(simplifiedText))
                {
                    MessageBox.Show("轉換結果為空，請檢查原始文字。", "錯誤");
                    return;
                }

                // 生成新檔案路徑：原檔名_簡體.txt
                string? directory = Path.GetDirectoryName(originalFilePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                //後面添加的簡體1只是為了在ComfyUI的LoadPromptsFromDir避開錯誤。將來可能需要修改。
                string newFilePath = Path.Combine(directory ?? "", fileNameWithoutExtension + "_簡體1.txt");

                // 檢查新檔案是否已存在
                if (File.Exists(newFilePath))
                {
                    DialogResult result = MessageBox.Show(
                        "檔案 " + Path.GetFileName(newFilePath) + " 已存在，是否要覆蓋？",
                        "確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                // 儲存簡體中文檔案，強制使用 UTF-8 以避免簡體中文字元被轉成問號
                bool saveResult;
                try
                {
                    File.WriteAllText(newFilePath, simplifiedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                    saveResult = true;
                }
                catch
                {
                    saveResult = false;
                }

                if (saveResult)
                {
                    MessageBox.Show("簡體中文檔案已成功儲存至：\n" + newFilePath, "成功");

                    // 重新載入目前資料夾檔案列表
                    if (treeViewFolder.SelectedNode != null)
                    {
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                    }
                }
                else
                {
                    MessageBox.Show("儲存失敗！", "錯誤");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("轉換或儲存時發生錯誤：\n" + ex.Message, "錯誤");
            }
        }

        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為簡體並另存為 _簡體.txt（按鈕）
        private void toolStripButtonFileConvertToSimplified_Click(object sender, EventArgs e)
        {
            BatchConvertTxtFilesToSimplifiedAndSave();
        }

        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為簡體並另存為 _簡體.txt
        private void BatchConvertTxtFilesToSimplifiedAndSave()
        {
            if (listViewFile.SelectedItems.Count == 0)
            {
                MessageBox.Show("請先在檔案清單中選取要轉換的 .TXT 檔案。", "提示");
                return;
            }

            int successCount = 0;
            int skipCount = 0;
            int failCount = 0;

            foreach (ListViewItem item in listViewFile.SelectedItems)
            {
                try
                {
                    string fileName = item.Text;
                    string fullPath = Path.Combine(m_TreeViewSelectedNodeText, fileName);

                    // 僅處理 .txt
                    if (Path.GetExtension(fullPath).ToLower() != ".txt")
                    {
                        skipCount++;
                        continue;
                    }

                    // 讀取原始文字（依偵測編碼）
                    string traditionalText = "";
                    if (!JTextFileLib.Instance().ReadTxtFile(fullPath, ref traditionalText, false))
                    {
                        failCount++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(traditionalText))
                    {
                        // 空檔案仍然會建立對應簡體檔
                    }

                    // 繁轉簡（指定 zh-CN LCID）
                    const int SimplifiedChineseLcid = 0x0804; // zh-CN
                    string? simplifiedText = Strings.StrConv(traditionalText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);
                    if (string.IsNullOrEmpty(simplifiedText))
                    {
                        simplifiedText = ""; // 空檔案時使用空字串
                    }

                    // 產生新檔名
                    string? dir = Path.GetDirectoryName(fullPath);
                    string nameNoExt = Path.GetFileNameWithoutExtension(fullPath);
                    //後面添加的簡體1只是為了在ComfyUI的LoadPromptsFromDir避開錯誤。將來可能需要修改。
                    string newPath = Path.Combine(dir ?? "", nameNoExt + "_簡體1.txt");

                    // 寫入 UTF-8 (BOM) 以避免 ? 字元
                    File.WriteAllText(newPath, simplifiedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                    successCount++;
                }
                catch (Exception)
                {
                    failCount++;
                }
            }

            // 刷新清單
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
            }

            MessageBox.Show(
                $"轉換完成：成功 {successCount}，略過 {skipCount}，失敗 {failCount}。",
                "批次轉換結果");
        }

        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為繁體並另存為 _繁體.txt（右鍵選單）
        private void toolStripMenuItem_FileConvertToTraditional_Click(object sender, EventArgs e)
        {
            BatchConvertTxtFilesToTraditionalAndSave();
        }

        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為繁體並另存為 _繁體.txt
        private void BatchConvertTxtFilesToTraditionalAndSave()
        {
            if (listViewFile.SelectedItems.Count == 0)
            {
                MessageBox.Show("請先在檔案清單中選取要轉換的 .TXT 檔案。", "提示");
                return;
            }

            int successCount = 0;
            int skipCount = 0;
            int failCount = 0;

            foreach (ListViewItem item in listViewFile.SelectedItems)
            {
                try
                {
                    string fileName = item.Text;
                    string fullPath = Path.Combine(m_TreeViewSelectedNodeText, fileName);

                    // 僅處理 .txt
                    if (Path.GetExtension(fullPath).ToLower() != ".txt")
                    {
                        skipCount++;
                        continue;
                    }

                    // 讀取原始文字（依偵測編碼）
                    string sourceText = "";
                    if (!JTextFileLib.Instance().ReadTxtFile(fullPath, ref sourceText, false))
                    {
                        failCount++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(sourceText))
                    {
                        // 空檔案仍然會建立對應繁體檔
                    }

                    // 轉換為繁體中文
                    // 關鍵：當源文字是簡體中文時，應該使用 zh-CN LCID 進行轉換
                    // 這樣才能正確將簡體字符（如"这"）轉換為繁體字符（如"這"）
                    const int SimplifiedChineseLcid = 0x0804; // zh-CN
                    string? traditionalText = Strings.StrConv(sourceText, VbStrConv.TraditionalChinese, SimplifiedChineseLcid);
                    if (string.IsNullOrEmpty(traditionalText))
                    {
                        traditionalText = ""; // 空檔案時使用空字串
                    }

                    // 產生新檔名
                    string? dir = Path.GetDirectoryName(fullPath);
                    string nameNoExt = Path.GetFileNameWithoutExtension(fullPath);
                    string newPath = Path.Combine(dir ?? "", nameNoExt + "_繁體.txt");

                    // 儲存繁體中文檔案，強制使用 UTF-8 (BOM) 以避免繁體中文字元被轉成問號
                    // 使用與簡體轉換相同的編碼方式
                    File.WriteAllText(newPath, traditionalText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                    successCount++;
                }
                catch (Exception)
                {
                    failCount++;
                }
            }

            // 刷新清單
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
            }

            MessageBox.Show(
                $"轉換完成：成功 {successCount}，略過 {skipCount}，失敗 {failCount}。",
                "批次轉換結果");
        }

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

        // 複製HTML選取文字並儲存為檔案
        private void CopyHtmlSaveFile()
        {
            if (!webBrowser1.Visible)
            {
                MessageBox.Show("請先開啟一個 HTML 檔案並在頁面中選取文字。", "提示");
                return;
            }

            try
            {
                // 清空剪貼簿，避免殘留舊資料
                Clipboard.Clear();
            }
            catch { }

            try
            {
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.ExecCommand("Copy", false, "");
                }
            }
            catch { }

            Application.DoEvents();
            System.Threading.Thread.Sleep(60);

            string selectedText = "";
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                    selectedText = Clipboard.GetText(TextDataFormat.UnicodeText);
                else if (Clipboard.ContainsText())
                    selectedText = Clipboard.GetText();
            }
            catch { selectedText = ""; }

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                MessageBox.Show("未偵測到選取文字。請先在瀏覽器中選取（反白）文字後再按此按鈕。", "提示");
                return;
            }

            // 將選取的文字轉換為繁體中文
            try
            {
                const int SimplifiedChineseLcid = 0x0804; // zh-CN
                string? traditionalText = Strings.StrConv(selectedText, VbStrConv.TraditionalChinese, SimplifiedChineseLcid);

                if (!string.IsNullOrEmpty(traditionalText))
                {
                    selectedText = traditionalText;
                }
            }
            catch (Exception ex)
            {
                // 轉換失敗時使用原始文字，不顯示錯誤訊息
                Console.WriteLine($"文字轉換為繁體時發生錯誤：{ex.Message}");
            }

            string targetPath = "";
            if (!string.IsNullOrEmpty(m_CurrentHtmlFilePath) && File.Exists(m_CurrentHtmlFilePath))
            {
                string? dir = Path.GetDirectoryName(m_CurrentHtmlFilePath);
                string nameNoExt = Path.GetFileNameWithoutExtension(m_CurrentHtmlFilePath);
                targetPath = Path.Combine(dir ?? "", nameNoExt + ".txt");
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    sfd.FileName = "選取文字.txt";
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;
                    targetPath = sfd.FileName;
                }
            }

            // 檢查檔案是否已存在
            if (File.Exists(targetPath))
            {
                // 顯示檔案覆蓋確認對話框
                using (FormFileOverwriteConfirm overwriteDialog = new FormFileOverwriteConfirm(Path.GetFileName(targetPath)))
                {
                    overwriteDialog.Owner = this;
                    if (overwriteDialog.ShowDialog() == DialogResult.OK)
                    {
                        switch (overwriteDialog.SelectedOption)
                        {
                            case FormFileOverwriteConfirm.OverwriteOption.Cancel:
                                // 取消儲存
                                return;

                            case FormFileOverwriteConfirm.OverwriteOption.Overwrite:
                                // 覆蓋原有檔案，繼續執行儲存
                                break;

                            case FormFileOverwriteConfirm.OverwriteOption.SaveAs:
                                // 另存新檔，顯示另存新檔對話框
                                using (SaveFileDialog sfd = new SaveFileDialog())
                                {
                                    sfd.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                                    sfd.FileName = Path.GetFileName(targetPath);
                                    string? dir = Path.GetDirectoryName(targetPath);
                                    if (!string.IsNullOrEmpty(dir))
                                    {
                                        sfd.InitialDirectory = dir;
                                    }
                                    if (sfd.ShowDialog() != DialogResult.OK)
                                        return;
                                    targetPath = sfd.FileName;
                                }
                                break;
                        }
                    }
                    else
                    {
                        // 使用者取消對話框，取消儲存
                        return;
                    }
                }
            }

            try
            {
                File.WriteAllText(targetPath, selectedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                MessageBox.Show("已儲存為：\n" + targetPath, "完成");

                // 保存當前選中的檔案名稱（如果有的話）
                string? selectedFileName = null;
                if (listViewFile.SelectedItems.Count > 0)
                {
                    selectedFileName = listViewFile.SelectedItems[0].Text;
                }

                if (treeViewFolder.SelectedNode != null)
                {
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));

                    // 恢復選中狀態並滾動到中間位置
                    if (!string.IsNullOrEmpty(selectedFileName))
                    {
                        ScrollToSelectedFileCenter(selectedFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：\n" + ex.Message, "錯誤");
            }
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

        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為簡體並另存為 _簡體.txt（右鍵選單）
        /// <summary>
        /// 編碼轉換對話框
        /// </summary>
        private void toolStripMenuItem_EncodingConvert_Click(object sender, EventArgs e)
        {
            if (listViewFile.SelectedItems.Count == 0)
            {
                MessageBox.Show("請先選擇要轉換編碼的檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (Form encodingDialog = new Form())
            {
                encodingDialog.Text = "編碼轉換";
                encodingDialog.Size = new Size(300, 200);
                encodingDialog.StartPosition = FormStartPosition.CenterParent;
                encodingDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                encodingDialog.MaximizeBox = false;
                encodingDialog.MinimizeBox = false;

                Label label = new Label();
                label.Text = "選擇來源編碼：";
                label.Location = new Point(20, 20);
                label.AutoSize = true;

                ComboBox comboBox = new ComboBox();
                comboBox.Location = new Point(20, 45);
                comboBox.Size = new Size(240, 25);
                comboBox.Items.AddRange(new string[] {
                    "自動檢測",
                    "UTF-8",
                    "GB2312 (簡體中文)",
                    "Big5 (繁體中文)",
                    "ASCII"
                });
                comboBox.SelectedIndex = 0;

                CheckBox convertToTraditionalCheckBox = new CheckBox();
                convertToTraditionalCheckBox.Text = "同時轉換為繁體中文";
                convertToTraditionalCheckBox.Location = new Point(20, 80);
                convertToTraditionalCheckBox.AutoSize = true;
                convertToTraditionalCheckBox.Checked = true;

                Button okButton = new Button();
                okButton.Text = "確定";
                okButton.Location = new Point(80, 120);
                okButton.Size = new Size(75, 30);
                okButton.DialogResult = DialogResult.OK;

                Button cancelButton = new Button();
                cancelButton.Text = "取消";
                cancelButton.Location = new Point(165, 120);
                cancelButton.Size = new Size(75, 30);
                cancelButton.DialogResult = DialogResult.Cancel;

                encodingDialog.Controls.AddRange(new Control[] { label, comboBox, convertToTraditionalCheckBox, okButton, cancelButton });

                if (encodingDialog.ShowDialog() == DialogResult.OK)
                {
                    ConvertFileEncoding(comboBox.SelectedItem.ToString(), convertToTraditionalCheckBox.Checked);
                }
            }
        }

        /// <summary>
        /// 執行檔案編碼轉換
        /// </summary>
        private void ConvertFileEncoding(string sourceEncoding, bool convertToTraditional)
        {
            try
            {
                foreach (ListViewItem selectedItem in listViewFile.SelectedItems)
                {
                    string fileName = selectedItem.Text;
                    string filePath = Path.Combine(m_TreeViewSelectedNodeText, fileName);

                    if (!File.Exists(filePath))
                        continue;

                    Encoding encoding;
                    switch (sourceEncoding)
                    {
                        case "UTF-8":
                            encoding = Encoding.UTF8;
                            break;
                        case "GB2312 (簡體中文)":
                            encoding = Encoding.GetEncoding("GB2312");
                            break;
                        case "Big5 (繁體中文)":
                            encoding = Encoding.GetEncoding("Big5");
                            break;
                        case "ASCII":
                            encoding = Encoding.ASCII;
                            break;
                        default: // 自動檢測
                            encoding = DetectFileEncoding(filePath);
                            break;
                    }

                    // 讀取檔案內容
                    string content = File.ReadAllText(filePath, encoding);

                    // 如果需要，轉換為繁體中文
                    if (convertToTraditional && ContainsChineseCharacters(content))
                    {
                        content = ConvertSimplifiedToTraditional(content);
                    }

                    // 詢問是否要覆蓋原檔案或建立新檔案
                    DialogResult result = MessageBox.Show(
                        $"是否要覆蓋原檔案「{fileName}」？\n選擇「否」將建立新檔案。",
                        "儲存選項",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                        continue;

                    string savePath;
                    if (result == DialogResult.Yes)
                    {
                        savePath = filePath;
                    }
                    else
                    {
                        // 建立新檔案，檔名加上 "_converted"
                        string extension = Path.GetExtension(fileName);
                        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        savePath = Path.Combine(m_TreeViewSelectedNodeText, $"{fileNameWithoutExt}_converted{extension}");
                    }

                    // 儲存檔案為 UTF-8
                    File.WriteAllText(savePath, content, new UTF8Encoding(true));

                    MessageBox.Show($"檔案已儲存至：{Path.GetFileName(savePath)}", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 重新整理檔案列表
                ListViewFile_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"編碼轉換失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItem_FileConvertToSimplified_Click(object sender, EventArgs e)
        {
            BatchConvertTxtFilesToSimplifiedAndSave();
        }

        // 複製HTML選取文字並儲存為檔案（右鍵選單）
        private void toolStripMenuItem_CopyHtmlSaveFile_Click(object sender, EventArgs e)
        {
            CopyHtmlSaveFile();
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

        // 更新狀態欄顯示檔案數量和選取數量
        private void UpdateFileSelectionStatus()
        {
            int totalCount = listViewFile.Items.Count;
            int selectedCount = listViewFile.SelectedItems.Count;

            if (selectedCount > 0)
            {
                toolStripStatusLabelNews.Text = $"檔案數量: {totalCount:N0} | 已選取: {selectedCount:N0}";
            }
            else
            {
                toolStripStatusLabelNews.Text = $"檔案數量: {totalCount:N0}";
            }
        }

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
            if (m_RecentReadListIndex >= 0 && m_RecentReadListIndex < m_RecentReadList.Count)
            {
                string fullPath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                fileName = Path.GetFileName(fullPath);
            }

            // 更新檔案名稱到 toolStripStatusLabelFileName
            toolStripStatusLabelFileName.Text = fileName;

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

        private void KeepTwoCRBetweenLines()
        {
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

            // 識別換行符類型（優先使用 \r\n，然後是 \n，最後是 \r）
            string lineBreak = "\r\n";
            if (!text.Contains("\r\n"))
            {
                if (text.Contains("\n"))
                    lineBreak = "\n";
                else if (text.Contains("\r"))
                    lineBreak = "\r";
            }

            // 將所有換行符統一為標準格式以便處理
            string normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = normalizedText.Split('\n');

            StringBuilder result = new StringBuilder();

            // 收集所有非空行的索引
            List<int> nonEmptyLineIndices = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    nonEmptyLineIndices.Add(i);
                }
            }

            if (nonEmptyLineIndices.Count == 0)
            {
                // 如果沒有非空行，返回空字符串
                result.Append("");
            }
            else
            {
                // 處理每一對相鄰的非空行
                for (int idx = 0; idx < nonEmptyLineIndices.Count; idx++)
                {
                    int lineIndex = nonEmptyLineIndices[idx];

                    // 添加非空行內容
                    result.Append(lines[lineIndex]);

                    // 如果不是最後一個非空行，檢查與下一個非空行之間的空行數
                    if (idx < nonEmptyLineIndices.Count - 1)
                    {
                        int nextLineIndex = nonEmptyLineIndices[idx + 1];
                        int emptyLinesBetween = nextLineIndex - lineIndex - 1;

                        if (emptyLinesBetween == 0)
                        {
                            // 兩行之間沒有空白行，添加一個空白行
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                        else if (emptyLinesBetween == 1)
                        {
                            // 兩行之間已經有一個空白行，保留它
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                        else
                        {
                            // 兩行之間有多個空白行，只保留一個
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                    }
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

        private void toolStripMenuItem_WithoutCRBetweenLines_Click(object sender, EventArgs e)
        {
            WithoutCRBetweenLines();
        }

        private void WithoutCRBetweenLines()
        {
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

            // 識別換行符類型（優先使用 \r\n，然後是 \n，最後是 \r）
            string lineBreak = "\r\n";
            if (!text.Contains("\r\n"))
            {
                if (text.Contains("\n"))
                    lineBreak = "\n";
                else if (text.Contains("\r"))
                    lineBreak = "\r";
            }

            // 將所有換行符統一為標準格式以便處理
            string normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = normalizedText.Split('\n');

            StringBuilder result = new StringBuilder();

            // 收集所有非空行的索引
            List<int> nonEmptyLineIndices = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    nonEmptyLineIndices.Add(i);
                }
            }

            if (nonEmptyLineIndices.Count == 0)
            {
                // 如果沒有非空行，返回空字符串
                result.Append("");
            }
            else
            {
                // 處理每一對相鄰的非空行
                for (int idx = 0; idx < nonEmptyLineIndices.Count; idx++)
                {
                    int lineIndex = nonEmptyLineIndices[idx];

                    // 添加非空行內容
                    result.Append(lines[lineIndex]);

                    // 如果不是最後一個非空行，只添加一個換行符（沒有空白行）
                    if (idx < nonEmptyLineIndices.Count - 1)
                    {
                        result.Append(lineBreak);
                    }
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

        private void toolStripMenuItem_DelFiles_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }
        private void DeleteFiles()
        {
            if (listViewFile.SelectedItems.Count > 0) // 有選定的項目
            {
                // 檢查是否有要刪除的檔案是當前正在編輯的檔案
                bool isDeletingCurrentFile = false;
                string? currentEditingFile = null;
                if (m_RecentReadListIndex >= 0 && m_RecentReadList.Count > 0)
                {
                    currentEditingFile = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                    foreach (ListViewItem selectedItem in listViewFile.SelectedItems)
                    {
                        string tmpFullFileName = Path.Combine(m_TreeViewSelectedNodeText, selectedItem.Text);
                        if (tmpFullFileName.Equals(currentEditingFile, StringComparison.OrdinalIgnoreCase))
                        {
                            isDeletingCurrentFile = true;
                            break;
                        }
                    }
                }

                // 如果刪除的是當前編輯中的檔案，直接清除修改標誌，不詢問是否儲存
                if (isDeletingCurrentFile)
                {
                    m_IsCurrentFileModified = false;
                    // 重置當前編輯檔案索引，強制重新載入下一個選中的檔案
                    m_RecentReadListIndex = -1;
                }

                // 顯示確認對話框
                string message = "您確定要將 " + listViewFile.SelectedItems[0].Text + "   " +
                               listViewFile.SelectedItems.Count + " 個檔案移到資源回收桶？";
                string caption = "移到資源回收桶";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    int tmpIndex = 0;

                    // 暫時禁用 SelectedIndexChanged 事件，避免刪除過程中觸發文件載入
                    listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;

                    try
                    {
                        listViewFile.BeginUpdate();
                        // 從最後一個選擇項目往前逐一刪除
                        for (int i = listViewFile.SelectedItems.Count - 1; i >= 0; i--)
                        {
                            ListViewItem item = listViewFile.SelectedItems[i];
                            tmpIndex = item.Index;
                            string tmpFullFileName = Path.Combine(m_TreeViewSelectedNodeText, listViewFile.SelectedItems[i].Text);

                            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(tmpFullFileName))
                            {
                                // 將檔案移至資源回收桶
                                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(tmpFullFileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                                // 如果是網頁檔案，同時刪除相關的資源目錄
                                if (Path.GetExtension(tmpFullFileName) == ".htm"
                                    || Path.GetExtension(tmpFullFileName) == ".html")
                                {
                                    string? dir = Path.GetDirectoryName(tmpFullFileName);
                                    if (dir != null && Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(Path.Combine(dir,
                                        Path.GetFileNameWithoutExtension(tmpFullFileName)) + "_files"))
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                                            Path.Combine(dir,
                                            Path.GetFileNameWithoutExtension(tmpFullFileName)) + "_files",
                                            UIOption.OnlyErrorDialogs,
                                            RecycleOption.SendToRecycleBin);
                                    }
                                }
                                listViewFile.Items.Remove(item); // 從列表中移除項目
                            }
                            else
                            {
                                MessageBox.Show(tmpFullFileName + " 已不存在！");
                            }
                        }
                        listViewFile.EndUpdate();

                        // 刪除檔案後，重置索引以強制重新載入下一個選中的檔案
                        m_RecentReadListIndex = -1;

                        // 重新啟用 SelectedIndexChanged 事件
                        listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;

                        // 選取下一個項目
                        if (listViewFile.Items.Count > tmpIndex)
                        {
                            // 直接調用選項變更事件處理函數，確保檔案會被載入
                            listViewFile.Items[tmpIndex].Selected = true;
                            ListViewFile_SelectedIndexChanged(listViewFile, new EventArgs());
                        }
                        else if (listViewFile.Items.Count > 0)
                        {
                            // 直接調用選項變更事件處理函數，確保檔案會被載入
                            listViewFile.Items[listViewFile.Items.Count - 1].Selected = true;
                            ListViewFile_SelectedIndexChanged(listViewFile, new EventArgs());
                        }
                        else
                        {
                            richTextBoxText.Text = ""; // 沒有檔案時才清空文字框
                            // 更新菜單狀態（沒有檔案時禁用相關菜單）
                            UpdateMenuStatus();
                        }
                    }
                    catch
                    {
                        // 確保重新啟用事件（即使發生錯誤）
                        listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                        throw;
                    }
                }
            }
        }

        private void toolStripMenuItem_SelectedTextSaveAsNew_Click(object sender, EventArgs e)
        {
            SelectedTextSaveAsNew();
        }

        private void SelectedTextSaveAsNew()
        {
            // 檢查是否有選中的文字
            if (richTextBoxText.SelectionLength == 0)
            {
                MessageBox.Show("請先選取要儲存的文字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 檢查是否有當前打開的檔案
            if (m_RecentReadListIndex < 0)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 獲取選中的文字
            string selectedText = richTextBoxText.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                MessageBox.Show("選取的文字為空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 獲取當前檔案路徑
            string currentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
            string? directory = Path.GetDirectoryName(currentFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentFilePath);
            string extension = Path.GetExtension(currentFilePath);

            // 如果沒有目錄，使用應用程式目錄
            if (string.IsNullOrEmpty(directory))
            {
                directory = Application.StartupPath;
            }

            // 尋找下一個可用的序號
            int nextSequence = 1;
            string suggestedFileName = "";
            while (true)
            {
                string sequenceStr = nextSequence.ToString("D3"); // 格式化為三位數，如 001, 002, 003
                string testFileName = $"{fileNameWithoutExtension}-{sequenceStr}{extension}";
                string testFilePath = Path.Combine(directory, testFileName);

                if (!File.Exists(testFilePath))
                {
                    suggestedFileName = testFileName;
                    break;
                }
                nextSequence++;

                // 防止無限循環（最多檢查到 999）
                if (nextSequence > 999)
                {
                    MessageBox.Show("序號已達到上限（999），請手動指定檔名。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    suggestedFileName = $"{fileNameWithoutExtension}-999{extension}";
                    break;
                }
            }

            // 使用 SaveFileDialog 讓使用者確認或修改檔名和位置
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                saveFileDialog.FileName = suggestedFileName;
                saveFileDialog.InitialDirectory = directory;
                saveFileDialog.Title = "儲存選取的文字";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 儲存檔案，使用 UTF-8 編碼以避免中文亂碼
                        File.WriteAllText(saveFileDialog.FileName, selectedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                        MessageBox.Show($"已成功儲存至：\n{saveFileDialog.FileName}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 重新載入目前資料夾檔案列表
                        if (treeViewFolder.SelectedNode != null)
                        {
                            treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"儲存失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void toolStripMenuItem_WholeTextSaveAsNew_Click(object sender, EventArgs e)
        {
            WholeTextSaveAsNew();
        }

        private void WholeTextSaveAsNew()
        {
            // 檢查是否有當前打開的檔案
            if (m_RecentReadListIndex < 0)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 獲取整篇文字
            string fullText = richTextBoxText.Text;
            if (string.IsNullOrEmpty(fullText))
            {
                MessageBox.Show("檔案內容為空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 獲取當前檔案路徑
            string currentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
            string? directory = Path.GetDirectoryName(currentFilePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentFilePath);
            string extension = Path.GetExtension(currentFilePath);

            // 如果沒有目錄，使用應用程式目錄
            if (string.IsNullOrEmpty(directory))
            {
                directory = Application.StartupPath;
            }

            const int chunkSize = 3000; // 每個檔案3000個字
            const int maxRemainingSize = 4000; // 最後剩餘4000字以內就存成一個檔案

            int textLength = fullText.Length;
            int currentPosition = 0;
            int fileSequence = 1;
            int successCount = 0;
            int skipCount = 0;

            // 識別換行符類型（優先使用 \r\n，然後是 \n，最後是 \r）
            string lineBreak = "\r\n";
            if (!fullText.Contains("\r\n"))
            {
                if (fullText.Contains("\n"))
                    lineBreak = "\n";
                else if (fullText.Contains("\r"))
                    lineBreak = "\r";
            }

            while (currentPosition < textLength)
            {
                int remainingLength = textLength - currentPosition;

                // 如果剩餘長度在4000字以內，全部存成一個檔案
                if (remainingLength <= maxRemainingSize)
                {
                    // 生成檔案名
                    string sequenceStr = fileSequence.ToString("D3");
                    string newFileName = $"{fileNameWithoutExtension}-{sequenceStr}{extension}";
                    string newFilePath = Path.Combine(directory, newFileName);

                    // 檢查檔案是否存在
                    if (File.Exists(newFilePath))
                    {
                        DialogResult result = MessageBox.Show(
                            $"檔案「{newFileName}」已存在，是否要覆蓋？",
                            "確認覆蓋",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result != DialogResult.Yes)
                        {
                            skipCount++;
                            break; // 使用者選擇不覆蓋，停止處理
                        }
                    }

                    // 準備檔案內容：第一行是檔名（無副檔名），第二行是空行，然後是文字內容
                    StringBuilder fileContent = new StringBuilder();
                    fileContent.Append(fileNameWithoutExtension + "-" + sequenceStr);
                    fileContent.Append(lineBreak);
                    fileContent.Append(lineBreak);
                    fileContent.Append(fullText.Substring(currentPosition));

                    try
                    {
                        // 儲存檔案，使用 UTF-8 編碼以避免中文亂碼
                        File.WriteAllText(newFilePath, fileContent.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"儲存檔案「{newFileName}」時發生錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break; // 發生錯誤，停止處理
                    }

                    break; // 處理完最後一部分，結束
                }
                else
                {
                    // 需要分割，先取3000個字，然後找到完整的一行
                    int targetPosition = currentPosition + chunkSize;

                    // 如果目標位置已經超過文字長度，直接取到末尾
                    if (targetPosition >= textLength)
                    {
                        targetPosition = textLength;
                    }
                    else
                    {
                        // 從目標位置開始，向後查找換行符，確保包含完整的一行
                        int lineEndPosition = targetPosition;

                        // 查找換行符（\r\n, \n, 或 \r）
                        while (lineEndPosition < textLength)
                        {
                            if (fullText[lineEndPosition] == '\r')
                            {
                                // 檢查是否是 \r\n
                                if (lineEndPosition + 1 < textLength && fullText[lineEndPosition + 1] == '\n')
                                {
                                    lineEndPosition += 2; // 包含 \r\n
                                }
                                else
                                {
                                    lineEndPosition += 1; // 只包含 \r
                                }
                                break; // 找到換行符，結束查找
                            }
                            else if (fullText[lineEndPosition] == '\n')
                            {
                                lineEndPosition += 1; // 包含 \n
                                break; // 找到換行符，結束查找
                            }
                            lineEndPosition++;
                        }

                        // 如果沒找到換行符（到達文件末尾），就取到末尾
                        if (lineEndPosition >= textLength)
                        {
                            targetPosition = textLength;
                        }
                        else
                        {
                            targetPosition = lineEndPosition;
                        }
                    }

                    // 計算實際要取的長度
                    int chunkLength = targetPosition - currentPosition;
                    string chunkText = fullText.Substring(currentPosition, chunkLength);

                    // 生成檔案名
                    string sequenceStr = fileSequence.ToString("D3");
                    string newFileName = $"{fileNameWithoutExtension}-{sequenceStr}{extension}";
                    string newFilePath = Path.Combine(directory, newFileName);

                    // 檢查檔案是否存在
                    if (File.Exists(newFilePath))
                    {
                        DialogResult result = MessageBox.Show(
                            $"檔案「{newFileName}」已存在，是否要覆蓋？",
                            "確認覆蓋",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result != DialogResult.Yes)
                        {
                            skipCount++;
                            fileSequence++;
                            currentPosition = targetPosition;
                            continue; // 使用者選擇不覆蓋，跳過這個檔案
                        }
                    }

                    // 準備檔案內容：第一行是檔名（無副檔名），第二行是空行，然後是文字內容
                    StringBuilder fileContent = new StringBuilder();
                    fileContent.Append(fileNameWithoutExtension + "-" + sequenceStr);
                    fileContent.Append(lineBreak);
                    fileContent.Append(lineBreak);
                    fileContent.Append(chunkText);

                    try
                    {
                        // 儲存檔案，使用 UTF-8 編碼以避免中文亂碼
                        File.WriteAllText(newFilePath, fileContent.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"儲存檔案「{newFileName}」時發生錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break; // 發生錯誤，停止處理
                    }

                    currentPosition = targetPosition;
                    fileSequence++;
                }
            }

            // 顯示處理結果
            string message = $"處理完成：成功儲存 {successCount} 個檔案";
            if (skipCount > 0)
            {
                message += $"，略過 {skipCount} 個檔案";
            }
            MessageBox.Show(message, "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 重新載入目前資料夾檔案列表
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
            }
        }

        private void toolStripMenuItem_RemoveMoreThan120CharB_Click(object sender, EventArgs e)
        {
            RemoveMoreThan120Char();
        }

        private void toolStripMenuItem_RenameFile_Click(object sender, EventArgs e)
        {
            RenameFile();
        }

        private void RenameFile()
        {
            if (listViewFile.SelectedItems.Count <= 0)
            {
                MessageBox.Show("請先選取要更名的檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool isMulti = listViewFile.SelectedItems.Count > 1;

            // 無論單檔或多檔，預設值都包含副檔名
            string defaultInputName = listViewFile.SelectedItems[0].Text;

            FormRenameInput renameDialog = new FormRenameInput(
                isMulti
                    ? "請輸入新檔名（可含副檔名）。\r\n多檔將使用：新檔名-001, -002, ..."
                    : "請輸入新檔名（可含副檔名）。",
                "更名檔案",
                defaultInputName,
                isMulti);

            if (renameDialog.ShowDialog(this) != DialogResult.OK)
            {
                return; // 取消
            }

            string input = renameDialog.InputText;
            if (string.IsNullOrWhiteSpace(input))
            {
                return; // 取消
            }

            input = input.Trim();
            string inputExt = Path.GetExtension(input);
            string inputBaseName = Path.GetFileNameWithoutExtension(input);

            // 記錄更名後的檔案名稱列表，用於重新選取
            List<string> renamedFiles = new List<string>();

            int index = 1;
            foreach (ListViewItem lvItem in listViewFile.SelectedItems)
            {
                string srcName = lvItem.Text;
                string srcPath = Path.Combine(m_TreeViewSelectedNodeText, srcName);
                string srcExt = Path.GetExtension(srcName);

                string newName;
                if (isMulti)
                {
                    // 多選時：新檔名-001 + 副檔名（若輸入含副檔名則用輸入的，否則沿用各自原副檔名）
                    string numberSuffix = "-" + index.ToString("D3");
                    string extToUse = !string.IsNullOrEmpty(inputExt) ? inputExt : srcExt;
                    newName = inputBaseName + numberSuffix + extToUse;
                }
                else
                {
                    // 單檔直接使用輸入的新檔名
                    newName = input;
                }

                string destPath = Path.Combine(m_TreeViewSelectedNodeText, newName);
                string destExt = Path.GetExtension(newName);

                // 副檔名變更確認
                if (!string.Equals(destExt, srcExt, StringComparison.OrdinalIgnoreCase))
                {
                    DialogResult extChange = MessageBox.Show(
                        $"檔案「{srcName}」的副檔名將由「{srcExt}」變更為「{destExt}」，是否繼續？",
                        "確認變更副檔名",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (extChange != DialogResult.Yes)
                    {
                        if (isMulti)
                        {
                            index++;
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                // 同名檔案覆蓋確認
                if (File.Exists(destPath))
                {
                    DialogResult overwrite = MessageBox.Show(
                        $"目的檔已存在：\r\n{newName}\r\n是否覆蓋？",
                        "確認覆蓋",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    if (overwrite != DialogResult.Yes)
                    {
                        if (isMulti)
                        {
                            index++;
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                try
                {
                    // .NET 6+ 支援指定 overwrite 參數
                    File.Move(srcPath, destPath, true);
                    // 記錄成功更名的檔案名稱
                    renamedFiles.Add(newName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"更名失敗：{srcName}\r\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (!isMulti)
                    {
                        return;
                    }
                }

                index++;
            }

            // 重新載入目前資料夾檔案列表
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));

                // 重新選取更名後的檔案
                if (renamedFiles.Count > 0)
                {
                    listViewFile.BeginUpdate();
                    // 先清除所有選取
                    listViewFile.SelectedItems.Clear();
                    // 根據檔名選取對應的項目
                    foreach (string fileName in renamedFiles)
                    {
                        foreach (ListViewItem item in listViewFile.Items)
                        {
                            if (item.Text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                            {
                                item.Selected = true;
                                item.EnsureVisible();
                                break;
                            }
                        }
                    }
                    listViewFile.EndUpdate();
                }
            }
        }

        private void toolStripMenuItem_SearchFiles_Click(object sender, EventArgs e)
        {
            SearchFiles();
        }

        private void SearchFiles()
        {
            // 顯示輸入對話框
            string searchText = Microsoft.VisualBasic.Interaction.InputBox(
                "請輸入要搜尋的檔名關鍵字（部分符合即可）：",
                "尋找檔案",
                "");

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return; // 取消或空輸入
            }

            searchText = searchText.Trim();

            // 在 listViewFile 中搜尋並選取符合條件的檔案
            listViewFile.BeginUpdate();

            // 先清除所有選取
            listViewFile.SelectedItems.Clear();

            // 搜尋並選取符合條件的項目（不區分大小寫）
            int matchCount = 0;
            ListViewItem? firstMatch = null;
            foreach (ListViewItem item in listViewFile.Items)
            {
                if (item.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    item.Selected = true;
                    matchCount++;
                    if (firstMatch == null)
                    {
                        firstMatch = item;
                    }
                }
            }

            listViewFile.EndUpdate();

            // 如果有找到符合的檔案，滾動到第一個符合的項目
            if (firstMatch != null)
            {
                firstMatch.EnsureVisible();
            }

            // 顯示搜尋結果訊息
            if (matchCount > 0)
            {
                MessageBox.Show(
                    $"找到 {matchCount:N0} 個符合「{searchText}」的檔案。",
                    "搜尋完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"未找到符合「{searchText}」的檔案。",
                    "搜尋結果",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
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
                            // 重新載入目前資料夾檔案列表
                            if (treeViewFolder.SelectedNode != null)
                            {
                                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));

                                // 選中剛儲存的檔案
                                string savedFileName = Path.GetFileName(saveFileDialog.FileName);
                                ScrollToSelectedFileCenter(savedFileName);
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

        private void EndingAddDot()
        {
            // 檢查每行結尾不是 "，。！？」… "符號(包括全形與半形字元)，請在行尾加上"。"(全形)

            // 獲取選定文本或整個文檔
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

            // 定義不需要添加句號的結尾符號（包括全形與半形）
            char[] punctuationMarks = new char[]
            {
                ',', '，', '.', '。', '!', '！', '?', '？', '"', '"', '」', '…', ' ', '\u3000', '\t'
            };

            // 分割成行並處理每一行
            string[] lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 如果不是最後一行或不是空行，檢查行結尾
                if (!string.IsNullOrEmpty(line))
                {
                    // 檢查行結尾是否已經有不需要的符號
                    bool endsWithPunctuation = false;
                    if (line.Length > 0)
                    {
                        char lastChar = line[line.Length - 1];
                        endsWithPunctuation = punctuationMarks.Contains(lastChar);
                    }

                    // 如果沒有結尾符號，添加全形句號
                    if (!endsWithPunctuation)
                    {
                        line += "。";
                    }
                }

                // 添加行到結果
                result.Append(line);

                // 添加換行符（除了最後一行）
                if (i < lines.Length - 1)
                {
                    result.Append("\r\n");
                }
            }

            // 更新文本
            if (processWholeDocument)
            {
                // 處理整個文檔
                richTextBoxText.Text = result.ToString();
                richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.SelectionLength = 0;
            }
            else
            {
                // 處理選定範圍
                richTextBoxText.SelectedText = result.ToString();
                richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.SelectionLength = result.Length;
            }

            // 滾動到游標位置
            richTextBoxText.ScrollToCaret();
        }

        private void toolStripButton_Option_Click(object sender, EventArgs e)
        {
            ShowOptionsDialog();
        }
        /// <summary>
        /// 檢測檔案編碼
        /// </summary>
        private Encoding DetectFileEncoding(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 讀取檔案開頭的位元組來檢測編碼
                    byte[] buffer = new byte[1024];
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);

                    // 檢查 BOM
                    if (bytesRead >= 3 && buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                        return Encoding.UTF8;
                    if (bytesRead >= 2 && buffer[0] == 0xFE && buffer[1] == 0xFF)
                        return Encoding.BigEndianUnicode;
                    if (bytesRead >= 2 && buffer[0] == 0xFF && buffer[1] == 0xFE)
                        return Encoding.Unicode;

                    // 先嘗試用UTF-8解碼，檢查是否為有效的UTF-8編碼
                    try
                    {
                        string utf8Test = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // 如果UTF-8解碼成功且包含中文字符，優先判斷為UTF-8
                        if (ContainsChineseCharacters(utf8Test))
                        {
                            // 進一步驗證UTF-8的有效性
                            byte[] utf8Bytes = Encoding.UTF8.GetBytes(utf8Test);
                            if (utf8Bytes.Length >= bytesRead * 0.8) // 如果重編碼後的位元組數量合理
                            {
                                return Encoding.UTF8;
                            }
                        }
                    }
                    catch { }

                    // 如果不是UTF-8，嘗試檢測是否為 GB2312/GBK
                    // 統計高位位元組的數量
                    int highByteCount = 0;
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] > 127)
                            highByteCount++;
                    }

                    // 如果高位位元組超過一定比例，可能是中文編碼
                    if (highByteCount > bytesRead * 0.1)
                    {
                        // 嘗試用 GB2312 解碼，看是否能得到合理的中文字符
                        try
                        {
                            string testText = Encoding.GetEncoding("GB2312").GetString(buffer, 0, Math.Min(100, bytesRead));
                            // 如果包含中文字符，且UTF-8解碼不成功，可能是 GB2312
                            if (ContainsChineseCharacters(testText))
                                return Encoding.GetEncoding("GB2312");
                        }
                        catch { }
                    }

                    // 預設返回 UTF-8
                    return Encoding.UTF8;
                }
            }
            catch
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// 檢查字串是否包含中文字符
        /// </summary>
        private bool ContainsChineseCharacters(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x4E00 && c <= 0x9FFF) // 中文字符範圍
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 將簡體中文轉換為繁體中文
        /// </summary>
        private string ConvertSimplifiedToTraditional(string simplifiedText)
        {
            try
            {
                // 使用 Microsoft.VisualBasic.Strings.StrConv 進行簡繁轉換
                return Microsoft.VisualBasic.Strings.StrConv(simplifiedText,
                    Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
            }
            catch
            {
                // 如果轉換失敗，返回原文
                return simplifiedText;
            }
        }

        /// <summary>
        /// 讀取檔案內容並自動處理編碼
        /// </summary>
        private string ReadFileWithEncodingDetection(string filePath)
        {
            try
            {
                // 檢測檔案編碼
                Encoding detectedEncoding = DetectFileEncoding(filePath);

                // 讀取檔案內容
                string content = File.ReadAllText(filePath, detectedEncoding);

                // 只對真正的簡體中文編碼檔案詢問轉換，UTF-8檔案不詢問
                if (detectedEncoding.CodePage == Encoding.GetEncoding("GB2312").CodePage ||
                    detectedEncoding.CodePage == Encoding.GetEncoding("GBK").CodePage)
                {
                    if (ContainsChineseCharacters(content))
                    {
                        DialogResult result = MessageBox.Show(
                            "檢測到此檔案可能是簡體中文編碼。是否將內容轉換為繁體中文？",
                            "編碼檢測",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            content = ConvertSimplifiedToTraditional(content);
                        }
                    }
                }
                // UTF-8檔案（包括已經轉換過的繁體中文檔案）直接返回，不詢問轉換

                return content;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取檔案時發生錯誤: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        /// <summary>
        /// 動態添加編碼轉換選單項目
        /// </summary>
        private void AddEncodingConvertMenuItem()
        {
            try
            {
                // 在檔案儲存下拉按鈕中添加編碼轉換選項
                if (toolStripDropDownButtonSave != null)
                {
                    // 創建編碼轉換選單項目
                    ToolStripMenuItem encodingConvertItem = new ToolStripMenuItem("編碼轉換(&E)");
                    encodingConvertItem.Click += toolStripMenuItem_EncodingConvert_Click;

                    // 添加分隔符和編碼轉換項目
                    toolStripDropDownButtonSave.DropDownItems.Add(new ToolStripSeparator());
                    toolStripDropDownButtonSave.DropDownItems.Add(encodingConvertItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加編碼轉換選單失敗: {ex.Message}");
            }
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

        private void toolStripMenuItem_CopyHtmlSaveFileSimplified_Click(object sender, EventArgs e)
        {
            CopyHtmlSaveFileSimplified();
        }

        /// <summary>
        /// 載入HTML檔案並確保正確編碼顯示
        /// </summary>
        private void LoadHtmlFileWithEncoding(string htmlFilePath)
        {
            string tempFilePath = null;
            try
            {
                // 先讀取檔案開頭檢查是否有charset宣告
                string firstBytes = "";
                using (FileStream fs = new FileStream(htmlFilePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead = fs.Read(buffer, 0, buffer.Length);
                    firstBytes = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }

                // 檢查是否已經有charset宣告
                bool hasCharset = firstBytes.IndexOf("charset=", StringComparison.OrdinalIgnoreCase) >= 0;

                // 如果已經有charset宣告，直接載入原始檔案
                if (hasCharset)
                {
                    webBrowser1.Navigate("file:///" + htmlFilePath);
                    return;
                }

                // 如果沒有charset宣告，需要檢測編碼並處理
                // 嘗試多種編碼方式，找到能正確解碼的編碼
                Encoding detectedEncoding = null;
                string htmlContent = null;
                bool isGB2312OrGBK = false;

                // 先嘗試GB2312編碼（簡體中文常見編碼）
                try
                {
                    string testContent = File.ReadAllText(htmlFilePath, Encoding.GetEncoding("GB2312"));
                    // 如果包含中文字符，就認為是GB2312編碼
                    // 不依賴IsLikelySimplifiedChinese，因為它可能不夠準確
                    if (ContainsChineseCharacters(testContent))
                    {
                        detectedEncoding = Encoding.GetEncoding("GB2312");
                        htmlContent = testContent;
                        isGB2312OrGBK = true;
                    }
                }
                catch { }

                // 如果GB2312不適用，嘗試GBK
                if (detectedEncoding == null)
                {
                    try
                    {
                        string testContent = File.ReadAllText(htmlFilePath, Encoding.GetEncoding("GBK"));
                        if (ContainsChineseCharacters(testContent))
                        {
                            detectedEncoding = Encoding.GetEncoding("GBK");
                            htmlContent = testContent;
                            isGB2312OrGBK = true;
                        }
                    }
                    catch { }
                }

                // 如果還是不適用，使用自動檢測
                if (detectedEncoding == null)
                {
                    detectedEncoding = DetectFileEncoding(htmlFilePath);
                    htmlContent = File.ReadAllText(htmlFilePath, detectedEncoding);
                    isGB2312OrGBK = detectedEncoding.CodePage == Encoding.GetEncoding("GB2312").CodePage ||
                                   detectedEncoding.CodePage == Encoding.GetEncoding("GBK").CodePage;
                }

                // 添加UTF-8 charset宣告
                string charsetDeclaration = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">";

                // 查找<head>標籤的位置
                int headIndex = htmlContent.IndexOf("<head>", StringComparison.OrdinalIgnoreCase);
                if (headIndex >= 0)
                {
                    // 在<head>標籤後插入charset宣告
                    int insertPosition = headIndex + "<head>".Length;
                    htmlContent = htmlContent.Insert(insertPosition, "\n    " + charsetDeclaration);
                }
                else
                {
                    // 如果沒有<head>標籤，在文檔開頭添加完整的head區塊
                    string headBlock = "<!DOCTYPE html>\n<html>\n<head>\n    " + charsetDeclaration + "\n</head>\n<body>\n";
                    htmlContent = headBlock + htmlContent;

                    // 在文檔結尾添加</body></html>
                    if (!htmlContent.Contains("</body>", StringComparison.OrdinalIgnoreCase))
                    {
                        htmlContent += "\n</body>\n</html>";
                    }
                }

                // 如果是UTF-8編碼，直接使用DocumentText載入（不需要臨時檔案）
                if (!isGB2312OrGBK)
                {
                    webBrowser1.DocumentText = htmlContent;
                    return;
                }

                // 如果是GB2312/GBK編碼，需要轉換為UTF-8並使用臨時檔案
                // 清理之前的臨時檔案
                if (!string.IsNullOrEmpty(m_TempHtmlFilePath) && File.Exists(m_TempHtmlFilePath))
                {
                    try
                    {
                        File.Delete(m_TempHtmlFilePath);
                    }
                    catch { }
                }

                // 創建臨時檔案，以UTF-8編碼寫入
                tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".html");
                m_TempHtmlFilePath = tempFilePath;
                
                // 使用UTF-8編碼（無BOM）寫入檔案
                // 這樣可以確保WebBrowser正確識別UTF-8編碼
                UTF8Encoding utf8NoBom = new UTF8Encoding(false);
                File.WriteAllText(tempFilePath, htmlContent, utf8NoBom);

                // 使用Navigate載入臨時檔案
                webBrowser1.Navigate("file:///" + tempFilePath.Replace('\\', '/'));
            }
            catch (Exception ex)
            {
                // 清理臨時檔案
                try
                {
                    if (tempFilePath != null && File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                catch { }

                // 如果處理失敗，嘗試直接載入原始檔案
                try
                {
                    webBrowser1.Navigate("file:///" + htmlFilePath);
                }
                catch
                {
                    // 最後的錯誤處理
                    string errorHtml = $"<html><head><meta charset=\"utf-8\"></head><body><p>無法載入HTML檔案：{ex.Message}</p></body></html>";
                    try
                    {
                        webBrowser1.DocumentText = errorHtml;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 判斷文字是否可能是簡體中文
        /// </summary>
        private bool IsLikelySimplifiedChinese(string text)
        {
            // 簡化的判斷邏輯：檢查是否包含常見的簡體中文特有字符
            // 這裡可以根據需要擴展更複雜的判斷邏輯
            string[] simplifiedIndicators = new string[] { "的", "了", "是", "不", "在", "有", "和", "这", "那", "我", "你", "他" };

            foreach (string indicator in simplifiedIndicators)
            {
                if (text.Contains(indicator))
                {
                    return true;
                }
            }

            return false;
        }

        private void CopyHtmlSaveFileSimplified()
        {
            if (!webBrowser1.Visible)
            {
                MessageBox.Show("請先開啟一個 HTML 檔案並在頁面中選取文字。", "提示");
                return;
            }

            try
            {
                // 清空剪貼簿，避免殘留舊資料
                Clipboard.Clear();
            }
            catch { }

            try
            {
                if (webBrowser1.Document != null)
                {
                    webBrowser1.Document.ExecCommand("Copy", false, "");
                }
            }
            catch { }

            Application.DoEvents();
            System.Threading.Thread.Sleep(60);

            string selectedText = "";
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                    selectedText = Clipboard.GetText(TextDataFormat.UnicodeText);
                else if (Clipboard.ContainsText())
                    selectedText = Clipboard.GetText();
            }
            catch { selectedText = ""; }

            if (string.IsNullOrWhiteSpace(selectedText))
            {
                MessageBox.Show("未偵測到選取文字。請先在瀏覽器中選取（反白）文字後再按此按鈕。", "提示");
                return;
            }

            // 將選取的文字轉換為簡體中文
            try
            {
                const int SimplifiedChineseLcid = 0x0804; // zh-CN
                string? simplifiedText = Strings.StrConv(selectedText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);

                if (!string.IsNullOrEmpty(simplifiedText))
                {
                    selectedText = simplifiedText;
                }
                else
                {
                    MessageBox.Show("文字轉換為簡體失敗，使用原始文字。", "警告");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("文字轉換為簡體時發生錯誤：\n" + ex.Message + "\n\n使用原始文字繼續儲存。", "警告");
            }

            string targetPath = "";
            if (!string.IsNullOrEmpty(m_CurrentHtmlFilePath) && File.Exists(m_CurrentHtmlFilePath))
            {
                string? dir = Path.GetDirectoryName(m_CurrentHtmlFilePath);
                string nameNoExt = Path.GetFileNameWithoutExtension(m_CurrentHtmlFilePath);
                targetPath = Path.Combine(dir ?? "", nameNoExt + "_簡體.txt");
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "簡體文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    sfd.FileName = "選取文字_簡體.txt";
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;
                    targetPath = sfd.FileName;
                }
            }

            // 檢查檔案是否已存在
            if (File.Exists(targetPath))
            {
                // 顯示檔案覆蓋確認對話框
                using (FormFileOverwriteConfirm overwriteDialog = new FormFileOverwriteConfirm(Path.GetFileName(targetPath)))
                {
                    overwriteDialog.Owner = this;
                    if (overwriteDialog.ShowDialog() == DialogResult.OK)
                    {
                        switch (overwriteDialog.SelectedOption)
                        {
                            case FormFileOverwriteConfirm.OverwriteOption.Cancel:
                                // 取消儲存
                                return;

                            case FormFileOverwriteConfirm.OverwriteOption.Overwrite:
                                // 覆蓋原有檔案，繼續執行儲存
                                break;

                            case FormFileOverwriteConfirm.OverwriteOption.SaveAs:
                                // 另存新檔，顯示另存新檔對話框
                                using (SaveFileDialog sfd = new SaveFileDialog())
                                {
                                    sfd.Filter = "簡體文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                                    sfd.FileName = Path.GetFileName(targetPath);
                                    string? dir = Path.GetDirectoryName(targetPath);
                                    if (!string.IsNullOrEmpty(dir))
                                    {
                                        sfd.InitialDirectory = dir;
                                    }
                                    if (sfd.ShowDialog() != DialogResult.OK)
                                        return;
                                    targetPath = sfd.FileName;
                                }
                                break;
                        }
                    }
                    else
                    {
                        // 使用者取消對話框，取消儲存
                        return;
                    }
                }
            }

            try
            {
                // 以GB2312編碼儲存簡體中文檔案
                Encoding gb2312 = Encoding.GetEncoding("GB2312");
                File.WriteAllText(targetPath, selectedText, gb2312);
                MessageBox.Show("已儲存為簡體中文檔案：\n" + targetPath, "完成");

                // 保存當前選中的檔案名稱（如果有的話）
                string? selectedFileName = null;
                if (listViewFile.SelectedItems.Count > 0)
                {
                    selectedFileName = listViewFile.SelectedItems[0].Text;
                }

                if (treeViewFolder.SelectedNode != null)
                {
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));

                    // 恢復選中狀態並滾動到中間位置
                    if (!string.IsNullOrEmpty(selectedFileName))
                    {
                        ScrollToSelectedFileCenter(selectedFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：\n" + ex.Message, "錯誤");
            }
        }
    }
}

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

        // 最近閱讀檔案清單
        public List<FileSystemManager.RecentReadList> m_RecentReadList = new List<FileSystemManager.RecentReadList>();

        // 當前閱讀檔案在清單中的索引
        public int m_RecentReadListIndex = -1;

        #endregion

        #region 建構函式

        // 建構函式
        public FormTextSpeedReader()
        {
            InitializeComponent();
            PopulateTreeViewAll(1);        // 初始化檔案樹狀圖
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
        }

        // 儲存目前檔案
        private void SaveCurrentFile()
        {
            if (m_RecentReadListIndex >= 0)
            {
                string filePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
                string content = richTextBoxText.Text;
                bool result = JTextFileLib.Instance().SaveTxtFile(filePath, content, false);
                if (result)
                {
                    // 重新載入目前資料夾檔案列表
                    if (treeViewFolder.SelectedNode != null)
                    {
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                    }
                    MessageBox.Show("儲存成功！", "提示");
                }
                else
                    MessageBox.Show("儲存失敗！", "錯誤");
            }
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
                    // 遞迴獲取子目錄
                    fileManager.GetDirectories(info.GetDirectories(), rootNode, 0, subFolderDeepthLimited);
                    treeViewFolder.Nodes.Add(rootNode);
                    treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];
                    m_TreeViewSelectedNodeText = treeViewFolder.SelectedNode.FullPath;
                    treeViewFolder.SelectedNode.Expand();
                }
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
        }

        // 處理檔案列表選擇變更事件
        private void ListViewFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 檢查是否為文字檔案
            Console.WriteLine("this.listViewFile.SelectedItems.Count " + this.listViewFile.SelectedItems.Count);
            //Console.WriteLine("this.listViewFile.SelectedItems[0].Text " + this.listViewFile.SelectedItems[0].Text);
            if (this.listViewFile.SelectedItems.Count > 0 && Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".txt")
            {
                string tmpString = "";
                string tmpFullFileName = m_TreeViewSelectedNodeText + @"\" + this.listViewFile.SelectedItems[0].Text;

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
                    this.richTextBoxText.Text = tmpString;
                    richTextBoxText.SelectionStart = m_RecentReadList[m_RecentReadListIndex].LastCharCount;
                    richTextBoxText.ScrollToCaret();
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
                    webBrowser1.Navigate("file:///" + m_CurrentHtmlFilePath);
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
                if (listViewFile.SelectedItems.Count > 0) // 有選定的項目
                {
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

                        // 選取下一個項目
                        if (listViewFile.Items.Count > tmpIndex)
                        {
                            listViewFile.Items[tmpIndex].Selected = true;
                        }
                        else if (listViewFile.Items.Count > 0)
                        {
                            listViewFile.Items[listViewFile.Items.Count - 1].Selected = true;
                        }
                        else
                        {
                            richTextBoxText.Text = ""; // 清空文字框
                        }
                    }
                }
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

        #endregion

        #region UI 控制項事件處理

        // 離開按鈕點擊事件
        private void QuitButton_Click(object sender, EventArgs e)
        {
            SelectFolderPath();
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
                string newFilePath = Path.Combine(directory ?? "", fileNameWithoutExtension + "_簡體.txt");

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
                    string newPath = Path.Combine(dir ?? "", nameNoExt + "_簡體.txt");

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
            // 獲取目前文字內容
            string traditionalText = richTextBoxText.SelectedText;
            if (string.IsNullOrEmpty(traditionalText))
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
                string? simplifiedText = Strings.StrConv(traditionalText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);
                if (string.IsNullOrEmpty(simplifiedText))
                {
                    MessageBox.Show("轉換結果為空，請檢查原始文字。", "錯誤");
                    return;
                }

                // 將轉換後的簡體文字顯示在文字框中
                richTextBoxText.SelectedText = simplifiedText;

                // 重新選取轉換後的文字，保持原來的選擇區域
                richTextBoxText.Select(selStart, simplifiedText.Length);
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
            // 獲取目前文字內容
            string simplifiedText = richTextBoxText.SelectedText;
            if (string.IsNullOrEmpty(simplifiedText))
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
                string? traditionalText = Strings.StrConv(simplifiedText, VbStrConv.TraditionalChinese, SimplifiedChineseLcid);

                // 檢查轉換結果
                if (string.IsNullOrEmpty(traditionalText))
                {
                    MessageBox.Show("轉換結果為空，請檢查原始文字。", "錯誤");
                    return;
                }

                // 將轉換後的繁體文字顯示在文字框中
                richTextBoxText.SelectedText = traditionalText;

                // 重新選取轉換後的文字，保持原來的選擇區域
                richTextBoxText.Select(selStart, traditionalText.Length);
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

            try
            {
                File.WriteAllText(targetPath, selectedText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                MessageBox.Show("已儲存為：\n" + targetPath, "完成");
                if (treeViewFolder.SelectedNode != null)
                {
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：\n" + ex.Message, "錯誤");
            }
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

                        // 如果剩餘部分小於等於最大長度，這是最後一段，直接添加
                        if (remainingLength <= maxLineLength)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 從當前位置開始的第100個字符位置開始，尋找下一個標點符號
                        int searchStart = processedPos + maxLineLength;
                        int punctuationPos = -1;

                        // 在剩餘文本中尋找標點符號（從第100個字符位置開始）
                        for (int i = searchStart; i < line.Length; i++)
                        {
                            if (Array.IndexOf(punctuationMarks, line[i]) >= 0)
                            {
                                punctuationPos = i;
                                break;
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

                        // 如果分割後剩餘部分少於100個字符，不執行分割，直接添加剩餘內容
                        if (remainingAfterSplit < maxLineLength && remainingAfterSplit > 0)
                        {
                            result.Append(line.Substring(processedPos));
                            break;
                        }

                        // 如果找到標點符號，在標點符號後截斷
                        if (punctuationPos >= 0)
                        {
                            // 檢查標點符號後面是否有空白字符，如果有則跳過，避免產生空白行
                            int actualCutPos = punctuationPos + 1;
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

                            // 添加從當前位置到標點符號（包含標點符號）的內容，但不包含後面的空白字符
                            result.Append(line.Substring(processedPos, punctuationPos - processedPos + 1));
                            result.Append(currentLineBreak); // 插入換行符
                            processedPos = actualCutPos; // 從跳過空白字符後的位置繼續處理
                        }
                        else
                        {
                            // 如果沒找到標點符號，在最大長度處強制截斷
                            // 檢查截斷位置後面是否有空白字符，如果有則跳過
                            int cutPos = processedPos + maxLineLength;
                            int actualCutPos = cutPos;
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

                            result.Append(line.Substring(processedPos, cutPos - processedPos));
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

        private void toolStripMenuItem_ConvertTraditional_Click(object sender, EventArgs e)
        {
            BatchConvertTxtFilesToTraditionalAndSave();
        }
    }
}

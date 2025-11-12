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
        // 視窗最小尺寸設定
        public override System.Drawing.Size MinimumSize { get; set; }
        // 系統字體列表
        public FontFamily[] m_FontFamilies;
        // 檔案系統管理器
        public FileSystemManager fileManager = new FileSystemManager();
        // 顯示管理器
        public DisplayManager displayManager = new DisplayManager();
        // ListView 欄位排序器
        private ListViewColumnSorter lvwColumnSorter;
        // 當前選中的樹狀目錄節點文字
        private string treeViewSelectedNodeText = "";
        // 當前閱讀檔案在清單中的索引
        private int recentReadListIndex = -1;
        // 檔案瀏覽器根目錄路徑
        //string m_FullPath = @"C:/";
        // 當前選中的樹狀目錄節點文字
        string m_TreeViewSelectedNodeText = "";
        // 支援的文字檔案類型
        //string[] m_TexExtensions = { ".txt", ".cs", ".htm", ".html" };
        // 可選擇的字體大小列表
        int[] m_FontSize = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36, 48, 66, 72, 100, 125, 150, 200, 300, 400, 500 };
        // 網頁瀏覽器縮放比例
        int m_WebBrowserZoom = 100;
        // 網頁瀏覽器可選擇的縮放大小
        int[] m_WebBrowserSize = { 10, 25, 33, 50, 66, 75, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500, 600 };
        // ListView 欄位排序器
        ListViewColumnSorter m_LvwColumnSorter = new ListViewColumnSorter();
        // 目前顯示於 WebBrowser 的本機 HTML 完整路徑
        private string m_CurrentHtmlFilePath = "";

        //建議右鍵選單
        //ContextMenuStrip menu = new ContextMenuStrip();
        //ToolStripMenuItem item = new ToolStripMenuItem("移除選取文字斷行");



        // 最近閱讀檔案清單結構
        //public struct RecentReadList
        //{
        //    public string FileFullName;    // 檔案完整路徑
        //    public int LastCharCount;      // 上次閱讀位置(字元數)
        //}
        // 最近閱讀檔案清單
        public List<FileSystemManager.RecentReadList> m_RecentReadList = new List<FileSystemManager.RecentReadList>();
        // 當前閱讀檔案在清單中的索引
        public int m_RecentReadListIndex = -1;

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


        // 設定視窗大小和位置
        private void SetFormSize()
        {
            // 設定最小視窗大小
            MinimumSize = new Size(1320, 800);
            // 獲取螢幕工作區域大小
            System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            // 設定視窗大小略小於工作區域
            this.Size = new System.Drawing.Size(workingRectangle.Width - 10, workingRectangle.Height - 10);
            // 將視窗置中
            Point newPosition = new Point(0, 0);
            newPosition.X = (workingRectangle.Width - this.Width) / 2;
            newPosition.Y = (workingRectangle.Height - this.Height) / 2;
            this.Location = newPosition;
        }

        // 快捷鍵事件
        private void FormTextSpeedReader_KeyDown(object sender, KeyEventArgs e)
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
                bool result = JTextFileLib.instance.SaveTxtFile(filePath, content, false);
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

        // 處理樹狀檢視節點選擇變更事件
        private void treeViewFolder_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            TreeNode newSelected = e.Node;
            m_TreeViewSelectedNodeText = e.Node.FullPath;
            listViewFile.Items.Clear();

            // 獲取選中節點的目錄資訊
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            if (newSelected.GetNodeCount(false) == 0)
            {
                fileManager.GetDirectories(nodeDirInfo.GetDirectories(), e.Node, 0, 1);
            }

            // 更新檔案列表顯示
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

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

        //滑鼠點擊後，這方法不好，先取消。
        void treeViewFolder_NodeMouseClick2(object sender, TreeNodeMouseClickEventArgs e)
        {

        }

        //離開
        private void QuitButton_Click(object sender, EventArgs e)
        {
            const string message = "確定要離開?";
            const string caption = "結束本程式";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
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
                if (JTextFileLib.instance.ReadTxtFile(tmpFullFileName, ref tmpString, false))
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

        // Updates the URL in TextBoxAddress upon navigation.
        private void webBrowser1_Navigated(object sender,
            WebBrowserNavigatedEventArgs e)
        {
            //toolStripTextBox1.Text = webBrowser1.Url.ToString();
        }

        private void FolderPathButton_Click(object sender, EventArgs e)
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
                webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
            }
        }

        // 減少字體大小
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
                webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
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

        // 切換資料夾瀏覽面板顯示/隱藏
        private void ShowFolderButton_Click(object sender, EventArgs e)
        {
            splitContainerMain.Panel1Collapsed = !splitContainerMain.Panel1Collapsed;
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

        // 變更文字字體
        private void ChangeFont(object sender, EventArgs e)
        {
            int count = m_FontFamilies.Length;
            // 尋找選擇的字體並套用
            for (int j = 0; j < count; ++j)
            {
                if (m_FontFamilies[j].Name == (string)toolStripComboBoxFonts.SelectedItem)
                {
                    Font newFont = new Font(m_FontFamilies[j], richTextBoxText.Font.Size, richTextBoxText.Font.Style);
                    richTextBoxText.Font = newFont;
                }
            }
        }
        //移除選取的文字斷行，將所選文字合併成同一行，richTextBoxText的右鍵選單
        private void toolStripMenuItemRemoveLineBreaks_Click(object sender, EventArgs e)
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
        //自動選取直到空白行，richTextBoxText的右鍵選單
        private void toolStripMenuItem_AutoSelectCR_Click(object sender, EventArgs e)
        {
            AutoSelectCR();
        }
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

        //自動移除目前文章中多餘的斷行，按鍵
        private void AutoRemoveCRButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCR();
        }

        //自動移除目前文章中多餘的斷行
        private void AutoRemoveCR()
        {
            //整合toolStripMenuItem_AutoSelectCR_Click跟toolStripMenuItemRemoveLineBreaks_Click功能，
            //能自動從文章起始處搜尋直到兩個連續斷行或新行符號為止，並將選取範圍內的文字合併成同一行。
            //若該行只有空白或是TAB符號，則不進行合併。

            string text = richTextBoxText.Text;
            if (string.IsNullOrEmpty(text))
                return;

            int textLength = text.Length;
            int currentPos = 0;  // 從文章起始處開始
            StringBuilder result = new StringBuilder();
            bool hasChanges = false;

            while (currentPos < textLength)
            {
                int paragraphStart = currentPos;
                int paragraphEnd = currentPos;
                int doubleBreakStart = -1;  // 兩個連續斷行符號的起始位置

                // 搜尋當前段落的結束位置（兩個連續斷行符號）
                while (paragraphEnd < textLength)
                {
                    // 檢查是否遇到斷行符號
                    if (text[paragraphEnd] == '\r' || text[paragraphEnd] == '\n')
                    {
                        // 找到第一個斷行符號，檢查後面是否還有斷行符號
                        int firstBreakEnd = paragraphEnd + 1;

                        // 處理 \r\n 組合
                        if (paragraphEnd < textLength - 1 && text[paragraphEnd] == '\r' && text[paragraphEnd + 1] == '\n')
                        {
                            firstBreakEnd = paragraphEnd + 2;
                        }

                        // 檢查下一個位置是否也是斷行符號
                        if (firstBreakEnd < textLength && (text[firstBreakEnd] == '\r' || text[firstBreakEnd] == '\n'))
                        {
                            // 找到兩個連續斷行符號，段落在此結束（不包含第一個斷行符號）
                            doubleBreakStart = paragraphEnd;
                            break;
                        }
                    }
                    paragraphEnd++;
                }

                // 如果沒找到連續斷行符號，段落延伸到文件結尾
                if (doubleBreakStart == -1)
                    paragraphEnd = textLength;
                else
                    paragraphEnd = doubleBreakStart;  // 段落結束於第一個斷行符號之前

                // 取得段落內容（不包含段落分隔符）
                if (paragraphEnd > paragraphStart)
                {
                    string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);

                    // 移除段落內的所有斷行符號，合併成一行
                    string mergedLine = paragraph.Replace("\r", "").Replace("\n", "");

                    // 檢查該行是否只包含空白或TAB符號
                    if (!string.IsNullOrWhiteSpace(mergedLine))
                    {
                        // 不是空白行，進行合併
                        result.Append(mergedLine);
                        if (paragraph != mergedLine)
                        {
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        // 只包含空白或TAB，保留原樣（不進行合併）
                        result.Append(paragraph);
                    }
                }

                // 處理段落分隔符（兩個連續斷行符號）
                if (doubleBreakStart >= 0)
                {
                    // 保留所有連續的斷行符號作為段落分隔
                    int breakStart = doubleBreakStart;
                    while (breakStart < textLength && (text[breakStart] == '\r' || text[breakStart] == '\n'))
                    {
                        if (breakStart < textLength - 1 && text[breakStart] == '\r' && text[breakStart + 1] == '\n')
                        {
                            result.Append("\r\n");
                            breakStart += 2;
                        }
                        else
                        {
                            result.Append(text[breakStart]);
                            breakStart++;
                        }
                    }
                    currentPos = breakStart;
                }
                else
                {
                    // 已到達文件結尾
                    currentPos = textLength;
                }
            }

            // 如果有變更，更新文字內容
            if (hasChanges)
            {
                int originalSelectionStart = richTextBoxText.SelectionStart;
                string newText = result.ToString();
                richTextBoxText.Text = newText;

                // 恢復游標位置（盡可能接近原位置）
                if (originalSelectionStart < newText.Length)
                {
                    richTextBoxText.SelectionStart = originalSelectionStart;
                }
                else
                {
                    richTextBoxText.SelectionStart = newText.Length;
                }
                richTextBoxText.ScrollToCaret();
            }
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
                                    || Path.GetExtension(tmpFullFileName) == ".html"
                                    && Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(Path.Combine(Path.GetDirectoryName(tmpFullFileName),
                                    Path.GetFileNameWithoutExtension(tmpFullFileName)) + "_files"))
                                {
                                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(
                                        Path.Combine(Path.GetDirectoryName(tmpFullFileName),
                                        Path.GetFileNameWithoutExtension(tmpFullFileName)) + "_files",
                                        UIOption.OnlyErrorDialogs,
                                        RecycleOption.SendToRecycleBin);
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

        private void RemoveLeadSpace_Click(object sender, EventArgs e)
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
                int firstNonSpaceTabPos = -1;
                bool lineHasOnlySpacesAndTabs = true;
                int lastNonSpaceTabPos = -1;
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

        // 將目前TXT繁體中文轉換成簡體中文並儲存
        private void buttonConvertToSimplified_Click(object sender, EventArgs e)
        {
            ConvertCurrentTxtToSimplifiedAndSave();
        }
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
                string simplifiedText = Strings.StrConv(traditionalText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);

                // 生成新檔案路徑：原檔名_簡體.txt
                string directory = Path.GetDirectoryName(originalFilePath);
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
                string newFilePath = Path.Combine(directory, fileNameWithoutExtension + "_簡體.txt");

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


        // 將 listViewFile 中被選取的 TXT 檔案批次轉換為簡體並另存為 _簡體.txt
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
                    if (!JTextFileLib.instance.ReadTxtFile(fullPath, ref traditionalText, false))
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
                    string simplifiedText = Strings.StrConv(traditionalText, VbStrConv.SimplifiedChinese, SimplifiedChineseLcid);

                    // 產生新檔名
                    string dir = Path.GetDirectoryName(fullPath);
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

        private void toolStripButtonCopyHtmlSaveFile_Click(object sender, EventArgs e)
        {
            CopyHtmlSaveFile();
        }

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
                    webBrowser1.Document.ExecCommand("Copy", false, null);
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
                string dir = Path.GetDirectoryName(m_CurrentHtmlFilePath);
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

        private void toolStripMenuItem_MergeNoneSpace_Click(object sender, EventArgs e)
        {

        }
    }
}

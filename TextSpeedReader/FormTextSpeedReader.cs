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
        //string[] m_TexExtensions = { ".txt", ".cs", ".html" };
        // 可選擇的字體大小列表
        int[] m_FontSize = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22,24,26,28,32,36,48,66,72,100,125,150,200,300,400,500 };
        // 網頁瀏覽器縮放比例
        int m_WebBrowserZoom = 100;
        // 網頁瀏覽器可選擇的縮放大小
        int[] m_WebBrowserSize = { 10, 25, 33, 50, 66, 75, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500, 600 };
        // ListView 欄位排序器
        ListViewColumnSorter m_LvwColumnSorter = new ListViewColumnSorter();

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
            string[] AllDisk = {@"C:\\", @"D:\\", @"E:\\", @"F:\\", @"G:\\", @"H:\\", @"I:\\", @"J:\\", @"K:\\" };
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
                           { new ListViewItem.ListViewSubItem(item, file.LastWriteTime.ToShortDateString())};
                        item.SubItems.AddRange(subItems);
                        subItems = new ListViewItem.ListViewSubItem[]
                           { new ListViewItem.ListViewSubItem(item, file.Length.ToString())};
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
            if (this.listViewFile.SelectedItems.Count > 0 && Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".html")
            {
                try
                {
                    // 切換到網頁瀏覽器顯示模式
                    this.richTextBoxText.Visible = false;
                    webBrowser1.Visible = true;
                    // 載入HTML檔案
                    webBrowser1.Navigate("file:///" + m_TreeViewSelectedNodeText + @"\" + this.listViewFile.SelectedItems[0].Text);
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
                            
                            if (FileSystem.FileExists(tmpFullFileName))
                            {
                                // 將檔案移至資源回收桶
                                FileSystem.DeleteFile(tmpFullFileName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                                
                                // 如果是網頁檔案，同時刪除相關的資源目錄
                                if (Path.GetExtension(tmpFullFileName) == ".htm" 
                                    || Path.GetExtension(tmpFullFileName) == ".html" 
                                    && FileSystem.DirectoryExists(Path.Combine(Path.GetDirectoryName(tmpFullFileName), 
                                    Path.GetFileNameWithoutExtension(tmpFullFileName)) + "_files"))
                                {
                                    FileSystem.DeleteDirectory(
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
    }
}

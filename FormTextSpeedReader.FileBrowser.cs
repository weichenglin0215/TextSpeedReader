using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
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
                
                // 檢查是否有子目錄，若有則添加 Dummy 節點
                if (HasSubDirectories(info))
                {
                    rootNode.Nodes.Add("Dummy");
                }
                
                treeViewFolder.Nodes.Add(rootNode);
                treeViewFolder.SelectedNode = treeViewFolder.Nodes[0];
                m_TreeViewSelectedNodeText = treeViewFolder.SelectedNode.FullPath;
                treeViewFolder.SelectedNode.Expand(); // 這會觸發 BeforeExpand 並載入子目錄
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
                DirectoryInfo info = new DirectoryInfo(item);
                if (info.Exists)
                {
                    // 建立磁碟機節點
                    TreeNode rootNode = new TreeNode(info.Name.Substring(0, info.Name.Length - 1));
                    rootNode.Tag = info;
                    
                    // 檢查是否有子目錄，若有則添加 Dummy 節點
                    if (HasSubDirectories(info))
                    {
                        rootNode.Nodes.Add("Dummy");
                    }
                    
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

        // 載入指定節點的子目錄 (包含孫目錄檢查)
        private void LoadDirectories(TreeNode parentNode)
        {
            if (parentNode.Tag is not DirectoryInfo dirInfo) return;

            try
            {
                parentNode.Nodes.Clear(); // 清除可能存在的 Dummy 節點

                foreach (DirectoryInfo subDir in dirInfo.GetDirectories())
                {
                    try
                    {
                        TreeNode childNode = new TreeNode(subDir.Name);
                        childNode.Tag = subDir;
                        childNode.ImageKey = "folder";
                        
                        // 檢查是否有子目錄 (孫目錄)，若有則添加 Dummy 節點以顯示 "+"
                        if (HasSubDirectories(subDir))
                        {
                            childNode.Nodes.Add("Dummy"); 
                        }
                        
                        parentNode.Nodes.Add(childNode);
                    }
                    catch { /* 忽略個別目錄錯誤 */ }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading directories for {dirInfo.FullName}: {ex.Message}");
            }
        }

        // 檢查目錄是否有子目錄
        private bool HasSubDirectories(DirectoryInfo dir)
        {
            try
            {
                return dir.GetDirectories().Length > 0;
            }
            catch
            {
                return false;
            }
        }

        // TreeView 展開前事件處理 (延遲載入)
        private void treeViewFolder_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (e.Node != null && e.Node.Nodes.Count > 0 && e.Node.Nodes[0].Text == "Dummy")
            {
                // 暫停重繪以避免閃爍 (可選)
                treeViewFolder.BeginUpdate();
                LoadDirectories(e.Node);
                treeViewFolder.EndUpdate();
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

        // 處理 TreeView 的 MouseDown，用於右鍵選取目錄項目並記錄按鍵
        private void treeViewFolder_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode? node = treeViewFolder.GetNodeAt(e.Location);
            if (node != null)
            {
                // 當右鍵點擊某個節點時，將該節點設為選中（使右鍵選單等操作針對該節點）
                if (e.Button == MouseButtons.Right)
                {
                    treeViewFolder.SelectedNode = node;
                    m_LastTreeMouseWasRight = true;
                }
                else
                {
                    m_LastTreeMouseWasRight = false;
                }
            }
            else
            {
                // 如果沒有點擊到節點，重置標記
                m_LastTreeMouseWasRight = false;
            }
        }

        // 處理 TreeView 的 NodeMouseClick，用於檢查重複點擊同一節點時目錄是否存在
        private void treeViewFolder_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            // 只處理左鍵點擊
            if (e.Button != MouseButtons.Left)
                return;

            TreeNode clickedNode = e.Node;
            if (clickedNode == null)
                return;

            // 檢查是否點擊的是當前已選中的節點
            bool isAlreadySelected = (treeViewFolder.SelectedNode == clickedNode);

            if (isAlreadySelected)
            {
                // 點擊已選中的節點，檢查目錄是否仍然存在
                if (clickedNode.Tag is DirectoryInfo dirInfo)
                {
                    if (!Directory.Exists(dirInfo.FullName))
                    {
                        // 目錄不存在，手動觸發處理
                        HandleMissingDirectory(clickedNode);
                    }
                    else
                    {
                        // 目錄存在，手動觸發 AfterSelect 以重新載入檔案列表
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(clickedNode));
                    }
                }
            }
            // 如果不是已選中的節點，AfterSelect 事件會自動處理
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
            if (this.listViewFile.SelectedItems.Count > 0 && Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".txt" || Path.GetExtension(this.listViewFile.SelectedItems[0].Text).ToLower() == ".yaml")
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

                    // 加入歷史紀錄
                    appSettings.AddHistoryFile(tmpFullFileName);
                    string? dir = Path.GetDirectoryName(tmpFullFileName);
                    if (dir != null) appSettings.AddHistoryDirectory(dir);
                    UpdateHistoryMenu();
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

                    // 更新狀態欄：顯示HTML檔案名稱到 toolStripStatusLabelFileName，清空固定狀態欄
                    string htmlFileName = Path.GetFileName(m_CurrentHtmlFilePath);
                    string htmlFileFullName = m_CurrentHtmlFilePath;
                    // 更新檔案名稱到 toolStripStatusLabelFileName
                    // 如果路徑長度超過 50 個字，顯示前10個字+"..."+最後37個字
                    if (!string.IsNullOrEmpty(htmlFileFullName) && htmlFileFullName.Length > 45)
                    {
                        toolStripStatusLabelFileName.Text = htmlFileFullName.Substring(0, 12) + "..." + htmlFileFullName.Substring(htmlFileFullName.Length - 30,30);
                    }
                    else
                    {
                        toolStripStatusLabelFileName.Text = htmlFileFullName;
                    }
                    toolStripStatusLabelFixed.Text = "";

                    // 更新菜單狀態
                    UpdateMenuStatus();

                    // 加入歷史紀錄
                    appSettings.AddHistoryFile(m_CurrentHtmlFilePath);
                    string? dir = Path.GetDirectoryName(m_CurrentHtmlFilePath);
                    if (dir != null) appSettings.AddHistoryDirectory(dir);
                    UpdateHistoryMenu();
                }
                catch (System.UriFormatException exc)
                {
                    Console.WriteLine(exc);
                    return;
                }
            }
        }

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

        // 比較節點的目前子目錄與實際檔案系統，若不同則重新載入該節點的子目錄
        private void RefreshChildNodesIfChanged(TreeNode node)
        {
            if (node == null || node.Tag == null || !(node.Tag is DirectoryInfo dirInfo))
                return;

            try
            {
                // 取得實際的第一層子目錄名稱集合
                string[] actualSubDirs = Array.Empty<string>();
                try
                {
                    actualSubDirs = Directory.GetDirectories(dirInfo.FullName)
                        .Select(d => Path.GetFileName(d) ?? d).ToArray();
                }
                catch { /* 權限或其他錯誤時忽略 */ }

                // 取得目前 TreeNode 下的子節點名稱集合（忽略 Dummy）
                var nodeNames = new List<string>();
                foreach (TreeNode child in node.Nodes)
                {
                    if (child.Text == "Dummy") continue;
                    nodeNames.Add(child.Text);
                }

                bool different = false;
                if (nodeNames.Count != actualSubDirs.Length)
                {
                    different = true;
                }
                else
                {
                    // 比對名稱清單（不分大小寫）
                    var setA = new HashSet<string>(nodeNames, StringComparer.OrdinalIgnoreCase);
                    foreach (var sd in actualSubDirs)
                    {
                        if (!setA.Contains(sd))
                        {
                            different = true;
                            break;
                        }
                    }
                }

                if (different)
                {
                    // 重新載入該節點的子目錄
                    node.Nodes.Clear();
                    LoadDirectories(node);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshChildNodesIfChanged error: {ex.Message}");
            }
        }
    }
}

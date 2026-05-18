using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        #region 拖曳開啟檔案功能

        /// <summary>
        /// 處理拖曳進入事件：檢查拖曳資料是否為檔案路徑，是則允許拖放，否則拒絕。
        /// </summary>
        private void FormTextSpeedReader_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;    // 允許拖曳複製（顯示「複製」游標）
            else
                e.Effect = DragDropEffects.None;    // 不接受此類型的拖曳
        }

        /// <summary>
        /// 處理拖曳放下事件：
        /// - 若拖入的是檔案，展開目錄樹至該檔案所在目錄，並在 ListView 中選取該檔案。
        /// - 若拖入的是資料夾，直接展開目錄樹至該資料夾。
        /// 目前只處理第一個拖曳的項目（files[0]）。
        /// </summary>
        private void FormTextSpeedReader_DragDrop(object? sender, DragEventArgs e)
        {
            try
            {
                if (e.Data is null) return;

                string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0) return;

                string filePath = files[0];

                if (File.Exists(filePath))
                {
                    // 拖入的是檔案：展開至所在目錄，再在 ListView 中選取該檔案
                    string? directoryPath = Path.GetDirectoryName(filePath);
                    string fileName = Path.GetFileName(filePath);

                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        // 展開目錄樹（會同時更新 ListView 的檔案清單）
                        ExpandToLastDirectory(directoryPath);

                        // 在 ListView 中找到並選取該檔案，選取後會觸發 SelectedIndexChanged 載入內容
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
                }
                else if (Directory.Exists(filePath))
                {
                    // 拖入的是資料夾：直接展開目錄樹至該資料夾
                    ExpandToLastDirectory(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"拖曳開啟失敗: {ex.Message}");
            }
        }

        #endregion

        #region 拖曳 ListView 檔案至 TreeView 目錄功能

        /// <summary>
        /// listViewFile 開始拖曳：收集所有選取檔案的完整路徑，以 FileDrop 格式啟動拖曳操作。
        /// </summary>
        private void listViewFile_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (listViewFile.SelectedItems.Count == 0) return;
            if (string.IsNullOrEmpty(m_TreeViewSelectedNodeText)) return;

            var filePaths = new List<string>();
            foreach (ListViewItem item in listViewFile.SelectedItems)
            {
                string fullPath = Path.Combine(m_TreeViewSelectedNodeText, item.Text);
                if (File.Exists(fullPath))
                    filePaths.Add(fullPath);
            }
            if (filePaths.Count == 0) return;

            DataObject data = new DataObject(DataFormats.FileDrop, filePaths.ToArray());
            listViewFile.DoDragDrop(data, DragDropEffects.Move | DragDropEffects.Copy);
        }

        /// <summary>
        /// 拖曳進入 treeViewFolder：確認資料為 FileDrop 才允許放下，
        /// 依 Ctrl 鍵狀態決定效果（複製或移動）。
        /// </summary>
        private void treeViewFolder_DragEnter_FromListView(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = (e.KeyState & 8) != 0 ? DragDropEffects.Copy : DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 拖曳在 treeViewFolder 上移動：高亮游標下方的目錄節點，
        /// 按住 Ctrl = 複製效果，否則 = 移動效果。
        /// </summary>
        private void treeViewFolder_DragOver_FromListView(object? sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            Point clientPoint = treeViewFolder.PointToClient(new Point(e.X, e.Y));
            TreeNode? targetNode = treeViewFolder.GetNodeAt(clientPoint);

            // 切換高亮節點
            if (targetNode != m_DragOverNode)
            {
                if (m_DragOverNode != null)
                {
                    m_DragOverNode.BackColor = Color.Empty;
                    m_DragOverNode.ForeColor = Color.Empty;
                }
                m_DragOverNode = targetNode;
                if (m_DragOverNode != null)
                {
                    m_DragOverNode.BackColor = SystemColors.Highlight;
                    m_DragOverNode.ForeColor = SystemColors.HighlightText;
                }
            }

            e.Effect = (targetNode?.Tag is DirectoryInfo)
                ? ((e.KeyState & 8) != 0 ? DragDropEffects.Copy : DragDropEffects.Move)
                : DragDropEffects.None;
        }

        /// <summary>
        /// 拖曳離開 treeViewFolder：清除節點高亮。
        /// </summary>
        private void treeViewFolder_DragLeave_FromListView(object? sender, EventArgs e)
        {
            if (m_DragOverNode != null)
            {
                m_DragOverNode.BackColor = Color.Empty;
                m_DragOverNode.ForeColor = Color.Empty;
                m_DragOverNode = null;
            }
        }

        /// <summary>
        /// 放下至 treeViewFolder 節點：依效果執行搬移或複製，
        /// 搬移後自動從 listViewFile 移除已搬走的項目。
        /// </summary>
        private void treeViewFolder_DragDrop_FromListView(object? sender, DragEventArgs e)
        {
            // 清除高亮
            if (m_DragOverNode != null)
            {
                m_DragOverNode.BackColor = Color.Empty;
                m_DragOverNode.ForeColor = Color.Empty;
                m_DragOverNode = null;
            }

            if (e.Data == null) return;
            string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files == null || files.Length == 0) return;

            Point clientPoint = treeViewFolder.PointToClient(new Point(e.X, e.Y));
            TreeNode? targetNode = treeViewFolder.GetNodeAt(clientPoint);
            if (targetNode == null || targetNode.Tag is not DirectoryInfo targetDir) return;

            string targetPath = targetDir.FullName;
            bool isCopy = (e.Effect == DragDropEffects.Copy);
            var errors = new List<string>();
            int successCount = 0;

            foreach (string sourcePath in files)
            {
                if (!File.Exists(sourcePath)) continue;

                string fileName = Path.GetFileName(sourcePath);
                string destPath = Path.Combine(targetPath, fileName);

                if (File.Exists(destPath))
                {
                    string action = isCopy ? "複製" : "搬移";
                    var choice = MessageBox.Show(
                        $"{action}時發現目標目錄已存在「{fileName}」，是否覆蓋？",
                        "確認覆蓋",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    if (choice != DialogResult.Yes) continue;
                }

                try
                {
                    if (isCopy)
                        File.Copy(sourcePath, destPath, true);
                    else
                        File.Move(sourcePath, destPath, true);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{fileName}: {ex.Message}");
                }
            }

            // 搬移後從 listViewFile 移除已不存在的項目
            if (!isCopy && successCount > 0)
            {
                var toRemove = new List<ListViewItem>();
                foreach (ListViewItem item in listViewFile.Items)
                {
                    string fullPath = Path.Combine(m_TreeViewSelectedNodeText, item.Text);
                    if (!File.Exists(fullPath))
                        toRemove.Add(item);
                }
                listViewFile.BeginUpdate();
                foreach (var item in toRemove)
                    listViewFile.Items.Remove(item);
                listViewFile.EndUpdate();
                UpdateFileSelectionStatus();
            }

            if (errors.Count > 0)
                MessageBox.Show($"部分檔案操作失敗：\n{string.Join("\n", errors)}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // 若目標節點已展開，刷新其子目錄顯示
            if (targetNode.IsExpanded)
                RefreshChildNodesIfChanged(targetNode);
        }

        #endregion
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        #region 拖曳功能

        // 處理拖曳進入事件
        private void FormTextSpeedReader_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data is not null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        // 處理拖曳放下事件
        private void FormTextSpeedReader_DragDrop(object? sender, DragEventArgs e)
        {
            try
            {
                if (e.Data is null) return;

                // 獲取拖曳的檔案列表
                string[]? files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    string filePath = files[0];
                    if (File.Exists(filePath))
                    {
                        string? directoryPath = Path.GetDirectoryName(filePath);
                        string fileName = Path.GetFileName(filePath);

                        if (!string.IsNullOrEmpty(directoryPath))
                        {
                            // 1. 展開樹狀結構至該目錄 (這會觸發檔案列表更新)
                            ExpandToLastDirectory(directoryPath);

                            // 2. 在檔案列表中選擇該檔案
                            foreach (ListViewItem item in listViewFile.Items)
                            {
                                if (item.Text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                                {
                                    item.Selected = true;
                                    item.EnsureVisible();
                                    // 設定 Selected = true 會觸發 SelectedIndexChanged 事件，進而載入檔案內容
                                    break;
                                }
                            }
                        }
                    }
                    else if (Directory.Exists(filePath))
                    {
                        // 如果是資料夾，直接展開至該目錄
                        ExpandToLastDirectory(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"拖曳開啟失敗: {ex.Message}");
            }
        }

        #endregion
    }
}

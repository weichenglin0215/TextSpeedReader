using System;
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
    }
}

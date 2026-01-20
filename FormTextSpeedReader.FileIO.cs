using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;
using System.Drawing;
using Microsoft.VisualBasic.FileIO;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        private void RenameDirectory()
        {
            if (treeViewFolder.SelectedNode == null)
            {
                MessageBox.Show("請先選擇要更名的資料夾。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode selectedNode = treeViewFolder.SelectedNode;
            DirectoryInfo? dirInfo = selectedNode.Tag as DirectoryInfo;

            // 如果 Tag 為空或不是 DirectoryInfo，嘗試從 FullPath 解析
            if (dirInfo == null)
            {
                return;
            }

            string oldName = dirInfo.Name;
            string fullPath = dirInfo.FullName;
            string? parentPath = Directory.GetParent(fullPath)?.FullName;

            // 檢查是否為根目錄
            if (parentPath == null || Path.GetPathRoot(fullPath) == fullPath)
            {
                MessageBox.Show("無法更名磁碟機根目錄。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormRenameInput renameDialog = new FormRenameInput(
                "請輸入新資料夾名稱：",
                "更名資料夾",
                oldName,
                false);

            if (renameDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string newName = renameDialog.InputText.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            // 名稱未變更
            if (newName.Equals(oldName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // 檢查無效字元
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (newName.IndexOfAny(invalidChars) >= 0)
            {
                MessageBox.Show("名稱包含無效字元。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string newPath = Path.Combine(parentPath, newName);

            if (Directory.Exists(newPath))
            {
                MessageBox.Show("該名稱的資料夾已存在。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Directory.Move(fullPath, newPath);

                // 更新節點資訊
                selectedNode.Text = newName;
                DirectoryInfo newDirInfo = new DirectoryInfo(newPath);
                selectedNode.Tag = newDirInfo;
                m_TreeViewSelectedNodeText = newPath; // 更新當前選中路徑

                // 重新載入並選取以刷新列表
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(selectedNode));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更名失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteDirectory()
        {
            // 刪除該目錄，並重新整理跟該目錄相關的treeViewFolder的樹狀結構項目。
            if (treeViewFolder.SelectedNode == null)
            {
                MessageBox.Show("請先選擇要刪除的資料夾。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode selectedNode = treeViewFolder.SelectedNode;
            DirectoryInfo? dirInfo = selectedNode.Tag as DirectoryInfo;
            if (dirInfo == null)
            {
                MessageBox.Show("所選項目不是資料夾。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fullPath = dirInfo.FullName;

            // 不允許刪除根目錄
            if (Path.GetPathRoot(fullPath).Equals(fullPath, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("無法刪除磁碟機根目錄。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dr = MessageBox.Show($"確定要將資料夾\n\"{fullPath}\"\n移到資源回收筒嗎？", "刪除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
                return;

            try
            {
                // 使用資源回收筒刪除
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory(fullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);

                // 更新 tree view：移除節點並選取父節點
                TreeNode parent = selectedNode.Parent;
                if (parent != null)
                {
                    parent.Nodes.Remove(selectedNode);
                    treeViewFolder.SelectedNode = parent;
                    // 更新 m_TreeViewSelectedNodeText
                    if (parent.Tag is DirectoryInfo pd)
                        m_TreeViewSelectedNodeText = pd.FullName;
                    else
                        m_TreeViewSelectedNodeText = parent.FullPath;

                    // 重新載入父節點下的檔案列表
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(parent));
                }
                else
                {
                    // 如果沒有父節點（應該很少發生），直接從根移除並清空檔案列表
                    treeViewFolder.Nodes.Remove(selectedNode);
                    m_TreeViewSelectedNodeText = string.Empty;
                    listViewFile.Items.Clear();
                    richTextBoxText.Text = string.Empty;
                }
            }
            catch (OperationCanceledException)
            {
                // 使用者取消或系統拒絕，無需處理
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刪除失敗：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
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
                
                // 記錄當前選中的檔案名稱
                string? currentSelectedFileName = null;
                if (listViewFile.SelectedItems.Count > 0)
                {
                    currentSelectedFileName = listViewFile.SelectedItems[0].Text;
                }
                // 如果沒有選中項目，使用當前開啟的檔案名稱
                if (string.IsNullOrEmpty(currentSelectedFileName))
                {
                    currentSelectedFileName = Path.GetFileName(filePath);
                }
                
                bool result = JTextFileLib.Instance().SaveTxtFile(filePath, content, false);
                if (result)
                {
                    // 儲存成功，重置修改標誌
                    m_IsCurrentFileModified = false;

                    // 重新載入目前資料夾檔案列表，並保持當前選中位置
                    if (treeViewFolder.SelectedNode != null)
                    {
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);
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

            // 記錄當前選中的檔案名稱
            string? currentSelectedFileName = null;
            if (listViewFile.SelectedItems.Count > 0)
            {
                currentSelectedFileName = listViewFile.SelectedItems[0].Text;
            }

            // 檢查儲存的檔案是否位於目前目錄
            if (!string.IsNullOrEmpty(directory) &&
                !string.IsNullOrEmpty(m_TreeViewSelectedNodeText) &&
                Path.GetFullPath(directory).Equals(Path.GetFullPath(m_TreeViewSelectedNodeText), StringComparison.OrdinalIgnoreCase))
            {
                // 重新載入目前資料夾檔案列表，並保持當前選中位置
                if (treeViewFolder.SelectedNode != null)
                {
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);
                }
            }
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

                    // 記錄當前選中的檔案名稱
                    string? currentSelectedFileName = null;
                    if (listViewFile.SelectedItems.Count > 0)
                    {
                        currentSelectedFileName = listViewFile.SelectedItems[0].Text;
                    }
                    // 如果沒有選中項目，使用原始檔案名稱
                    if (string.IsNullOrEmpty(currentSelectedFileName))
                    {
                        currentSelectedFileName = Path.GetFileName(originalFilePath);
                    }

                    // 重新載入目前資料夾檔案列表，並保持當前選中位置
                    if (treeViewFolder.SelectedNode != null)
                    {
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);
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

            // 記錄當前選中的檔案名稱
            string? currentSelectedFileName = null;
            if (listViewFile.SelectedItems.Count > 0)
            {
                currentSelectedFileName = listViewFile.SelectedItems[0].Text;
            }

            // 刷新清單，並保持當前選中位置
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);
            }

            MessageBox.Show(
                $"轉換完成：成功 {successCount}，略過 {skipCount}，失敗 {failCount}。",
                "批次轉換結果");
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

                // 檢查儲存的檔案是否位於目前目錄
                string? savedDirectory = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(savedDirectory) &&
                    !string.IsNullOrEmpty(m_TreeViewSelectedNodeText) &&
                    Path.GetFullPath(savedDirectory).Equals(Path.GetFullPath(m_TreeViewSelectedNodeText), StringComparison.OrdinalIgnoreCase))
                {
                    if (treeViewFolder.SelectedNode != null)
                    {
                        // 重新載入目前資料夾檔案列表，並保持當前選中位置
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), selectedFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("儲存失敗：\n" + ex.Message, "錯誤");
            }
        }



        // 執行檔案編碼轉換
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



        // 檢測檔案編碼
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

        // 檢查字串是否包含中文字符
        private bool ContainsChineseCharacters(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x4E00 && c <= 0x9FFF) // 中文字符範圍
                    return true;
            }
            return false;
        }

        // 將簡體中文轉換為繁體中文
        private string ConvertSimplifiedToTraditional(string simplifiedText)
        {
            try
            {
                // 使用 Microsoft.VisualBasic.Strings.StrConv 進行簡繁轉換
                // 使用 0x0804 (zh-CN) 作為 LCID 確保能正確從簡體字集中辨識並轉換
                return Microsoft.VisualBasic.Strings.StrConv(simplifiedText,
                    Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0x0804);
            }
            catch
            {
                // 如果轉換失敗，返回原文
                return simplifiedText;
            }
        }

        // 讀取檔案內容並自動處理編碼
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

        // 動態添加編碼轉換選單項目
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


        // 載入HTML檔案並確保正確編碼顯示
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

        // 判斷文字是否可能是簡體中文
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

        private void RenameFile()
        {
            if (listViewFile.SelectedItems.Count <= 0)
            {
                MessageBox.Show("請先選取要更名的檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool isMulti = listViewFile.SelectedItems.Count > 1;

            // 無論單檔或多檔，預設值都包含副檔名
            string FullInputName = listViewFile.SelectedItems[0].Text;
            string inputNameWithoutExt = Path.GetFileNameWithoutExtension(FullInputName);
            string extension = Path.GetExtension(FullInputName);
            string defaultInputName = listViewFile.SelectedItems[0].Text;
            //目前選取的項目的全檔名
            string tmpSelectedFullFileName = m_TreeViewSelectedNodeText + @"\" + this.listViewFile.SelectedItems[0].Text;
            //目前正在編輯的檔案的全檔名，只能是TXT檔，HTML檔不會列入編輯檔案清單。
            string tmpcurrentFilePath = "";
            if(m_RecentReadList.Count > 0)
            {
                tmpcurrentFilePath = m_RecentReadList[m_RecentReadListIndex].FileFullName;
            }

            //MessageBox.Show(tmpSelectedFullFileName + "\n\r" + tmpcurrentFilePath);

            if (richTextBoxText.SelectedText.Length > 0 && tmpcurrentFilePath == tmpSelectedFullFileName)
            {
                inputNameWithoutExt = richTextBoxText.SelectedText.Trim(m_AllIllegalFileName).Replace("","").Replace("　","");
                if (inputNameWithoutExt.Length > 30)
                {
                    inputNameWithoutExt = inputNameWithoutExt.Substring(0,30);
                }
                defaultInputName = inputNameWithoutExt + extension;
            }

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

            // 記錄當前選中的檔案名稱（更名後的新名稱）
            string? currentSelectedFileName = null;
            if (renamedFiles.Count > 0)
            {
                // 使用第一個更名後的檔案名稱
                currentSelectedFileName = renamedFiles[0];
            }
            else if (listViewFile.SelectedItems.Count > 0)
            {
                // 如果沒有更名，使用當前選中的檔案名稱
                currentSelectedFileName = listViewFile.SelectedItems[0].Text;
            }

            // 重新載入目前資料夾檔案列表，並保持當前選中位置
            if (treeViewFolder.SelectedNode != null)
            {
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), currentSelectedFileName);

                // 如果是多檔更名，需要選取所有更名後的檔案
                if (renamedFiles.Count > 1)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            // 暫時禁用 SelectedIndexChanged 事件
                            listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;
                            
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
                    }));
                }
            }
        }
        /// <summary>
        /// 針對 TreeView 中選取的目錄進行名稱編碼修正
        /// </summary>
        private void ReCodeFolderName()
        {
            if (treeViewFolder.SelectedNode == null || treeViewFolder.SelectedNode.Tag is not DirectoryInfo)
            {
                MessageBox.Show("請先選擇要變更編碼的目錄。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode node = treeViewFolder.SelectedNode;
            DirectoryInfo dirInfo = (DirectoryInfo)node.Tag;
            string srcName = dirInfo.Name;

            // 磁碟機根目錄不允許更名
            if (dirInfo.Parent == null)
            {
                MessageBox.Show("無法變更磁碟機根目錄的編碼。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FormReCodeFileName recodeDialog = new FormReCodeFileName(srcName))
            {
                recodeDialog.Text = "變更目錄名稱編碼";
                if (recodeDialog.ShowDialog(this) == DialogResult.OK &&
                    recodeDialog.SelectedCorrectEncoding != null &&
                    recodeDialog.SelectedWrongEncoding != null)
                {
                    Encoding correct = recodeDialog.SelectedCorrectEncoding;
                    Encoding wrong = recodeDialog.SelectedWrongEncoding;
                    bool sim2Trad = recodeDialog.IsSim2TradChecked;

                    try
                    {
                        byte[] bytes = wrong.GetBytes(srcName);
                        string newName = correct.GetString(bytes);

                        if (sim2Trad)
                        {
                            newName = ConvertSimplifiedToTraditional(newName);
                        }

                        if (newName == srcName) return;

                        string oldPath = dirInfo.FullName;
                        string newPath = Path.Combine(dirInfo.Parent.FullName, newName);

                        if (Directory.Exists(newPath))
                        {
                            if (MessageBox.Show($"目的地目錄「{newName}」已存在。是否嘗試合併？\n(這可能會覆蓋現有同名檔案)", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                return;
                        }

                        Directory.Move(oldPath, newPath);

                        // 更新節點資訊
                        node.Text = newName;
                        node.Tag = new DirectoryInfo(newPath);

                        // 刷新此節點的子目錄結構 (因為路徑已變)
                        node.Nodes.Clear();
                        if (HasSubDirectories((DirectoryInfo)node.Tag))
                        {
                            node.Nodes.Add("Dummy");
                        }

                        if (node.IsExpanded)
                        {
                            node.Collapse();
                            node.Expand();
                        }

                        // 同時刷新 ListView
                        treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(node));
                        
                        toolStripStatusLabelFixed.Text = $"目錄編碼已修正：{newName}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"變更目錄編碼失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        /// <summary>
        /// 針對 ListView 中選取的檔案或目錄進行名稱編碼修正
        /// </summary>
        private void ReCodeFileName()
        {
            if (this.listViewFile.SelectedItems.Count <= 0)
            {
                MessageBox.Show("請先選取要更碼的檔案或目錄。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 使用第一個選取項目的名稱作為預覽範本
            string sampleName = this.listViewFile.SelectedItems[0].Text;

            using (FormReCodeFileName recodeDialog = new FormReCodeFileName(sampleName))
            {
                if (recodeDialog.ShowDialog(this) != DialogResult.OK ||
                    recodeDialog.SelectedCorrectEncoding == null ||
                    recodeDialog.SelectedWrongEncoding == null)
                {
                    return;
                }

                Encoding correct = recodeDialog.SelectedCorrectEncoding;
                Encoding wrong = recodeDialog.SelectedWrongEncoding;
                bool sim2Trad = recodeDialog.IsSim2TradChecked;

                List<string> renamedItems = new List<string>();
                bool anyDirectoryRenamed = false;

                this.listViewFile.BeginUpdate();

                foreach (ListViewItem lvItem in this.listViewFile.SelectedItems)
                {
                    string srcName = lvItem.Text;
                    string srcPath = Path.Combine(m_TreeViewSelectedNodeText, srcName);

                    try
                    {
                        byte[] bytes = wrong.GetBytes(srcName);
                        string newName = correct.GetString(bytes);

                        if (sim2Trad)
                        {
                            newName = ConvertSimplifiedToTraditional(newName);
                        }

                        if (newName == srcName) continue;

                        string destPath = Path.Combine(m_TreeViewSelectedNodeText, newName);

                        bool isFile = File.Exists(srcPath);
                        bool isDir = !isFile && Directory.Exists(srcPath);

                        if (!isFile && !isDir) continue;

                        if (File.Exists(destPath) || Directory.Exists(destPath))
                        {
                            if (MessageBox.Show($"目的地「{newName}」已存在。是否覆蓋或合併？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                continue;
                        }

                        if (isFile)
                        {
                            File.Move(srcPath, destPath, true);
                        }
                        else if (isDir)
                        {
                            Directory.Move(srcPath, destPath);
                            anyDirectoryRenamed = true;
                        }

                        renamedItems.Add(newName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"更新「{srcName}」名稱時發生錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                this.listViewFile.EndUpdate();

                if (treeViewFolder.SelectedNode != null)
                {
                    // 如果有目錄被更名，通知 TreeView 刷新
                    if (anyDirectoryRenamed)
                    {
                        RefreshChildNodesIfChanged(treeViewFolder.SelectedNode);
                    }

                    // 刷新 ListView 內容
                    string? firstRenamed = renamedItems.Count > 0 ? renamedItems[0] : null;
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), firstRenamed);

                    // 重新選取所有更名後的項目
                    if (renamedItems.Count > 1)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;
                                listViewFile.BeginUpdate();
                                listViewFile.SelectedItems.Clear();
                                foreach (string name in renamedItems)
                                {
                                    foreach (ListViewItem item in listViewFile.Items)
                                    {
                                        if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase))
                                        {
                                            item.Selected = true;
                                            break;
                                        }
                                    }
                                }
                                listViewFile.EndUpdate();
                                listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                                UpdateFileSelectionStatus();
                            }
                            catch { /* 忽略 UI 更新錯誤 */ }
                        }));
                    }
                }
                
                toolStripStatusLabelFixed.Text = $"已修正 {renamedItems.Count} 個項目的編碼";
            }
        }

        private void ReCodeFullFoldersFilesName()
        {
            if (treeViewFolder.SelectedNode == null || treeViewFolder.SelectedNode.Tag is not DirectoryInfo)
            {
                MessageBox.Show("請先選擇要批量變更名稱編碼的目錄。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode rootNode = treeViewFolder.SelectedNode;
            DirectoryInfo rootDir = (DirectoryInfo)rootNode.Tag;

            using (FormReCodeFull recodeDialog = new FormReCodeFull(rootDir.Name))
            {
                if (recodeDialog.ShowDialog(this) != DialogResult.OK ||
                    recodeDialog.SelectedCorrectEncoding == null ||
                    recodeDialog.SelectedWrongEncoding == null)
                {
                    return;
                }

                Encoding correct = recodeDialog.SelectedCorrectEncoding;
                Encoding wrong = recodeDialog.SelectedWrongEncoding;
                bool sim2Trad = recodeDialog.IsSim2TradChecked;

                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    
                    // 執行遞迴處理
                    ProcessReCodeFull(rootDir, correct, wrong, sim2Trad);

                    // 處理根目錄自身的更名 (如果有需要)
                    string newRootName = ReCodeString(rootDir.Name, correct, wrong);
                    if (sim2Trad) newRootName = ConvertSimplifiedToTraditional(newRootName);

                    if (newRootName != rootDir.Name)
                    {
                        DirectoryInfo? parent = rootDir.Parent;
                        if (parent != null)
                        {
                            string newRootPath = Path.Combine(parent.FullName, newRootName);
                            if (!Directory.Exists(newRootPath))
                            {
                                Directory.Move(rootDir.FullName, newRootPath);
                                rootNode.Text = newRootName;
                                rootNode.Tag = new DirectoryInfo(newRootPath);
                                m_TreeViewSelectedNodeText = newRootPath;
                            }
                        }
                    }

                    // 重新整理 UI
                    RefreshChildNodesIfChanged(rootNode);
                    treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(rootNode));

                    MessageBox.Show("批量變更名稱編碼完成。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabelFixed.Text = "批量變更名稱編碼已完成";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"批量變更名稱時發生錯誤：\n{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void ProcessReCodeFull(DirectoryInfo dir, Encoding correct, Encoding wrong, bool sim2Trad)
        {
            // 1. 處理當前目錄下的所有檔案
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    string oldName = file.Name;
                    string newName = ReCodeString(oldName, correct, wrong);
                    if (sim2Trad) newName = ConvertSimplifiedToTraditional(newName);

                    if (newName != oldName)
                    {
                        string newPath = Path.Combine(dir.FullName, newName);
                        if (!File.Exists(newPath))
                        {
                            file.MoveTo(newPath);
                        }
                    }
                }
                catch { /* 忽略單一檔案錯誤，繼續處理 */ }
            }

            // 2. 處理當前目錄下的所有子目錄
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                try
                {
                    // 先遞迴處理子目錄內部的內容
                    ProcessReCodeFull(subDir, correct, wrong, sim2Trad);

                    // 處理子目錄自身的名稱
                    string oldName = subDir.Name;
                    string newName = ReCodeString(oldName, correct, wrong);
                    if (sim2Trad) newName = ConvertSimplifiedToTraditional(newName);

                    if (newName != oldName)
                    {
                        string newPath = Path.Combine(dir.FullName, newName);
                        if (!Directory.Exists(newPath))
                        {
                            subDir.MoveTo(newPath);
                        }
                    }
                }
                catch { /* 忽略單一目錄錯誤，繼續處理 */ }
            }
        }

        private string ReCodeString(string input, Encoding correct, Encoding wrong)
        {
            try
            {
                byte[] bytes = wrong.GetBytes(input);
                return correct.GetString(bytes);
            }
            catch { return input; }
        }

        /// <summary>
        /// 將 ListView 中選取的檔案或目錄名稱由簡體中文轉換為繁體中文
        /// </summary>
        private void FileNameSim2Trad()
        {
            if (this.listViewFile.SelectedItems.Count <= 0)
            {
                MessageBox.Show("請先選取要轉換名稱的檔案或目錄。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<string> renamedItems = new List<string>();
            bool anyDirectoryRenamed = false;

            this.listViewFile.BeginUpdate();

            foreach (ListViewItem lvItem in this.listViewFile.SelectedItems)
            {
                string srcName = lvItem.Text;
                string srcPath = Path.Combine(m_TreeViewSelectedNodeText, srcName);

                try
                {
                    string newName = ConvertSimplifiedToTraditional(srcName);

                    // 如果轉換結果與原文相同 (包括大小寫)，則跳過
                    if (string.Equals(newName, srcName, StringComparison.Ordinal)) continue;

                    string destPath = Path.Combine(m_TreeViewSelectedNodeText, newName);

                    bool isFile = File.Exists(srcPath);
                    bool isDir = !isFile && Directory.Exists(srcPath);

                    if (!isFile && !isDir) continue;

                    // 檢查目標是否已存在 (且不是原檔案的僅大小寫差異)
                    if (!string.Equals(srcPath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(destPath) || Directory.Exists(destPath))
                        {
                            if (MessageBox.Show($"目的地「{newName}」已存在。是否覆蓋或合併？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                                continue;
                        }
                    }

                    if (isFile)
                    {
                        // File.Move 在 .NET Core 支援同路徑大小寫變更，但在某些情況下仍需小心
                        File.Move(srcPath, destPath, true);
                    }
                    else if (isDir)
                    {
                        Directory.Move(srcPath, destPath);
                        anyDirectoryRenamed = true;
                    }

                    renamedItems.Add(newName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"轉換名稱失敗！\n來源：{srcName}\n錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.listViewFile.EndUpdate();

            if (treeViewFolder.SelectedNode != null)
            {
                if (anyDirectoryRenamed)
                {
                    RefreshChildNodesIfChanged(treeViewFolder.SelectedNode);
                }

                string? firstRenamed = renamedItems.Count > 0 ? renamedItems[0] : null;
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(treeViewFolder.SelectedNode), firstRenamed);
                
                // 重新選取所有轉換後的項
                if (renamedItems.Count > 1)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            listViewFile.SelectedIndexChanged -= ListViewFile_SelectedIndexChanged;
                            listViewFile.BeginUpdate();
                            listViewFile.SelectedItems.Clear();
                            foreach (string name in renamedItems)
                            {
                                foreach (ListViewItem item in listViewFile.Items)
                                {
                                    if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        item.Selected = true;
                                        break;
                                    }
                                }
                            }
                            listViewFile.EndUpdate();
                            listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
                            UpdateFileSelectionStatus();
                        }
                        catch { }
                    }));
                }
            }
            
            toolStripStatusLabelFixed.Text = $"已將 {renamedItems.Count} 個項目轉換為繁體";
        }

        /// <summary>
        /// 將 TreeView 中選取的目錄名稱由簡體中文轉換為繁體中文
        /// </summary>
        private void FolderNameSim2Trad()
        {
            if (treeViewFolder.SelectedNode == null || treeViewFolder.SelectedNode.Tag is not DirectoryInfo)
            {
                MessageBox.Show("請先選擇要轉換名稱的目錄。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TreeNode node = treeViewFolder.SelectedNode;
            DirectoryInfo dirInfo = (DirectoryInfo)node.Tag;
            string srcName = dirInfo.Name;

            if (dirInfo.Parent == null)
            {
                MessageBox.Show("無法轉換磁碟機根目錄的名稱。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string newName = ConvertSimplifiedToTraditional(srcName);

                if (string.Equals(newName, srcName, StringComparison.Ordinal)) return;

                string oldPath = dirInfo.FullName;
                string newPath = Path.Combine(dirInfo.Parent.FullName, newName);

                // 檢查目標是否已存在 (且不是僅大小寫差異)
                if (!string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase))
                {
                    if (Directory.Exists(newPath))
                    {
                        if (MessageBox.Show($"目的地目錄「{newName}」已存在。是否嘗試合併？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                            return;
                    }
                }

                Directory.Move(oldPath, newPath);

                node.Text = newName;
                node.Tag = new DirectoryInfo(newPath);

                node.Nodes.Clear();
                if (HasSubDirectories((DirectoryInfo)node.Tag))
                {
                    node.Nodes.Add("Dummy");
                }

                if (node.IsExpanded)
                {
                    node.Collapse();
                    node.Expand();
                }

                // 更新選中路徑變數並刷新列表
                m_TreeViewSelectedNodeText = newPath;
                treeViewFolder_AfterSelect(treeViewFolder, new TreeViewEventArgs(node));
                
                toolStripStatusLabelFixed.Text = $"目錄名稱已轉換為繁體：{newName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"轉換目錄名稱失敗！\n來源：{srcName}\n錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

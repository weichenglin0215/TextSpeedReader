using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// Markdown 檔案編輯彈窗：左邊為 Markdown 簡易預覽，右邊為純文字編輯欄。
    /// 預設開啟為螢幕工作區的 90% 寬與 90% 高。
    /// 右下方提供「取消」「另存新檔」「儲存」三個按鈕。
    /// 採用 RichTextBox 內建簡易渲染 (零外部套件依賴)。
    /// </summary>
    public partial class FormMarkdownEditor : Form
    {
        // 目前編輯中的檔案完整路徑 (儲存時寫回此路徑)
        private string _currentPath;

        // Prompt 目錄 (另存新檔的預設目錄)
        private readonly string _promptDir;

        // 每一個編輯欄來源行 (依索引) 對應到 richTextBoxPreview 中的字元位置，
        // 由 RenderPreview 逐行渲染時建立，供捲動同步使用。
        private readonly List<int> _linePreviewOffsets = new List<int>();

        /// <summary>
        /// 若使用者有儲存或另存，會記錄最後成功寫入的檔案路徑；否則為 null。
        /// 呼叫端可據此重新整理「文章類型」下拉選單。
        /// </summary>
        public string? SavedFilePath { get; private set; }

        /// <summary>
        /// 建構子。
        /// </summary>
        /// <param name="filePath">要編輯的 .md 檔案完整路徑。</param>
        /// <param name="promptDir">Prompt 目錄 (另存新檔的預設位置)。</param>
        public FormMarkdownEditor(string filePath, string promptDir)
        {
            InitializeComponent();
            _currentPath = filePath;
            _promptDir = promptDir;
        }

        private void FormMarkdownEditor_Load(object? sender, EventArgs e)
        {
            // 預設開啟為螢幕工作區的 90% 大小
            Rectangle wa = (Screen.FromControl(this) ?? Screen.PrimaryScreen!).WorkingArea;
            this.Size = new Size((int)(wa.Width * 0.9), (int)(wa.Height * 0.9));

            // 讀取檔案內容 (UTF-8)
            string content = string.Empty;
            try
            {
                if (File.Exists(_currentPath))
                {
                    content = File.ReadAllText(_currentPath, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取檔案失敗：\n{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            textBoxEditor.Text = content;
            labelFileName.Text = "檔案：" + _currentPath;
            RenderPreview();
        }

        // 編輯內容變動 → 即時更新左側預覽
        private void textBoxEditor_TextChanged(object? sender, EventArgs e)
        {
            // 記錄目前的插入點位置 (RenderPreview 操作 richTextBoxPreview 的過程中，
            // Win32 rich-edit 控制項會悄悄搶走輸入焦點，即使沒有呼叫 Focus()，
            // 導致 textBoxEditor 的插入點游標消失不見)。
            bool wasFocused = textBoxEditor.Focused;
            int selStart = textBoxEditor.SelectionStart;
            int selLength = textBoxEditor.SelectionLength;

            RenderPreview();

            // 渲染完畢後把焦點與插入點明確移回編輯欄，恢復游標顯示。
            if (wasFocused)
            {
                textBoxEditor.Focus();
                textBoxEditor.SelectionStart = selStart;
                textBoxEditor.SelectionLength = selLength;
            }

            // RenderPreview 會 Clear() 整個預覽框再重新 AppendText，捲動位置因此重置回頂端。
            // 這裡依「編輯欄目前所在的原始碼行」對應到預覽框中同一來源行的位置，
            // 讓預覽自動捲動到對應區塊，而不是永遠停在文章開頭。
            int caretLine = textBoxEditor.GetLineFromCharIndex(textBoxEditor.SelectionStart);
            if (caretLine >= 0 && caretLine < _linePreviewOffsets.Count)
            {
                int previewPos = Math.Min(_linePreviewOffsets[caretLine], richTextBoxPreview.TextLength);
                richTextBoxPreview.SelectionStart = previewPos;
                richTextBoxPreview.SelectionLength = 0;
                richTextBoxPreview.ScrollToCaret();
            }
        }

        /// <summary>
        /// 內建的簡易 Markdown 渲染：將編輯欄內容依基本語法著色/加粗後顯示於 RichTextBox。
        /// 支援：# 標題 (1~3 級)、粗體 **text**、清單 (- / * / 數字.)、程式碼區塊 ```、水平線 ---。
        /// </summary>
        private void RenderPreview()
        {
            richTextBoxPreview.SuspendLayout();
            richTextBoxPreview.Clear();
            _linePreviewOffsets.Clear();

            Font baseFont = richTextBoxPreview.Font;
            string[] lines = textBoxEditor.Text.Replace("\r\n", "\n").Split('\n');
            bool inCodeBlock = false;

            foreach (string raw in lines)
            {
                // 記錄這一來源行渲染前，預覽框目前的字元位置 (供捲動同步使用)
                _linePreviewOffsets.Add(richTextBoxPreview.TextLength);

                string line = raw;

                // 程式碼區塊圍籬
                if (line.TrimStart().StartsWith("```"))
                {
                    inCodeBlock = !inCodeBlock;
                    continue;
                }

                if (inCodeBlock)
                {
                    AppendText(line + "\n", new Font("Consolas", baseFont.Size), Color.DarkSlateGray);
                    continue;
                }

                // 水平線
                string trimmed = line.Trim();
                if (trimmed == "---" || trimmed == "***" || trimmed == "___")
                {
                    AppendText("──────────────────────────────\n", baseFont, Color.Silver);
                    continue;
                }

                // 標題
                if (line.StartsWith("### "))
                {
                    AppendText(line.Substring(4) + "\n",
                        new Font(baseFont.FontFamily, baseFont.Size + 2, FontStyle.Bold), Color.FromArgb(60, 60, 120));
                    continue;
                }
                if (line.StartsWith("## "))
                {
                    AppendText(line.Substring(3) + "\n",
                        new Font(baseFont.FontFamily, baseFont.Size + 4, FontStyle.Bold), Color.FromArgb(40, 40, 100));
                    continue;
                }
                if (line.StartsWith("# "))
                {
                    AppendText(line.Substring(2) + "\n",
                        new Font(baseFont.FontFamily, baseFont.Size + 7, FontStyle.Bold), Color.FromArgb(20, 20, 80));
                    continue;
                }

                // 清單項目 (- / * / 數字.)
                string listTrim = line.TrimStart();
                if (listTrim.StartsWith("- ") || listTrim.StartsWith("* "))
                {
                    int indent = line.Length - listTrim.Length;
                    AppendText(new string(' ', indent) + "  • ", baseFont, Color.DimGray);
                    AppendInline(listTrim.Substring(2) + "\n", baseFont);
                    continue;
                }

                // 一般段落 (含粗體 inline 解析)
                AppendInline(line + "\n", baseFont);
            }

            richTextBoxPreview.ResumeLayout();
        }

        // 解析一行內的粗體 **text**，其餘為一般文字
        private void AppendInline(string text, Font baseFont)
        {
            int i = 0;
            while (i < text.Length)
            {
                int start = text.IndexOf("**", i, StringComparison.Ordinal);
                if (start < 0)
                {
                    AppendText(text.Substring(i), baseFont, richTextBoxPreview.ForeColor);
                    break;
                }
                int end = text.IndexOf("**", start + 2, StringComparison.Ordinal);
                if (end < 0)
                {
                    AppendText(text.Substring(i), baseFont, richTextBoxPreview.ForeColor);
                    break;
                }

                // 粗體前的一般文字
                if (start > i)
                    AppendText(text.Substring(i, start - i), baseFont, richTextBoxPreview.ForeColor);

                // 粗體內容
                string boldText = text.Substring(start + 2, end - start - 2);
                AppendText(boldText, new Font(baseFont, FontStyle.Bold), richTextBoxPreview.ForeColor);

                i = end + 2;
            }
        }

        // 以指定字型與顏色附加文字到預覽框
        private void AppendText(string text, Font font, Color color)
        {
            richTextBoxPreview.SelectionStart = richTextBoxPreview.TextLength;
            richTextBoxPreview.SelectionLength = 0;
            richTextBoxPreview.SelectionFont = font;
            richTextBoxPreview.SelectionColor = color;
            richTextBoxPreview.AppendText(text);
        }

        // 「儲存」：寫回目前檔案路徑
        private void buttonSave_Click(object? sender, EventArgs e)
        {
            if (WriteToFile(_currentPath))
            {
                SavedFilePath = _currentPath;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // 「另存新檔」：以 SaveFileDialog 選擇新路徑
        private void buttonSaveAs_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "Markdown 檔案 (*.md)|*.md|所有檔案 (*.*)|*.*",
                DefaultExt = "md",
                InitialDirectory = Directory.Exists(_promptDir)
                    ? _promptDir : Path.GetDirectoryName(_currentPath),
                FileName = Path.GetFileName(_currentPath)
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            if (WriteToFile(dlg.FileName))
            {
                _currentPath = dlg.FileName;
                labelFileName.Text = "檔案：" + _currentPath;
                SavedFilePath = _currentPath;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // 「取消」：不儲存關閉
        private void buttonCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 實際寫檔 (UTF-8, 無 BOM)
        private bool WriteToFile(string path)
        {
            try
            {
                File.WriteAllText(path, textBoxEditor.Text, new UTF8Encoding(false));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"儲存檔案失敗：\n{ex.Message}", "錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}

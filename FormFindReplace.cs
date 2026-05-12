using System;
using System.Drawing;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 尋找與取代對話框。
    /// 可工作於兩種模式：
    /// - 查找模式（findMode=true）：只顯示「尋找」相關控制項。
    /// - 取代模式（findMode=false）：同時顯示「取代」相關控制項。
    /// 支援「區分大小寫」和「全字匹配」選項。
    /// </summary>
    public partial class FormFindReplace : Form
    {
        // 目標 RichTextBox，所有查找/取代操作都作用於此控制項
        private RichTextBox targetRichTextBox;
        // 上一次查找結束的字元位置，用於「繼續查找下一個」
        private int lastSearchPosition = 0;
        // true = 查找模式，false = 取代模式
        private bool isFindMode = true;

        /// <summary>
        /// 建構子：初始化對話框並依模式顯示/隱藏取代相關控制項。
        /// </summary>
        /// <param name="richTextBox">要進行查找/取代的 RichTextBox 控制項。</param>
        /// <param name="findMode">true 為查找模式，false 為取代模式。</param>
        public FormFindReplace(RichTextBox richTextBox, bool findMode = true)
        {
            InitializeComponent();
            targetRichTextBox = richTextBox;
            isFindMode = findMode;

            if (isFindMode)
            {
                // 查找模式：隱藏取代相關的控制項
                this.Text = "尋找";
                labelReplace.Visible = false;
                textBoxReplace.Visible = false;
                buttonReplace.Visible = false;
                buttonReplaceAll.Visible = false;
            }
            else
            {
                this.Text = "尋找與取代";
            }

            // 若目標 RichTextBox 中已有選取文字，自動填入查找框
            if (targetRichTextBox.SelectionLength > 0)
                textBoxFind.Text = targetRichTextBox.SelectedText;

            // 查找文字改變時，重置查找起始位置，避免從上次位置繼續造成混淆
            textBoxFind.TextChanged += (s, e) => { lastSearchPosition = 0; };
        }

        // 查找/取代完成後，將焦點還給主視窗的 RichTextBox
        private void ReturnFocusToOwner()
        {
            if (this.Owner != null && !this.Owner.IsDisposed)
            {
                // 使用 BeginInvoke 確保在目前事件處理完成後再切換焦點
                this.BeginInvoke(new Action(() =>
                {
                    if (this.Owner != null && !this.Owner.IsDisposed)
                    {
                        this.Owner.Activate();
                        if (targetRichTextBox != null && !targetRichTextBox.IsDisposed && targetRichTextBox.Visible)
                            targetRichTextBox.Focus();
                    }
                }));
            }
        }

        private void buttonFindNext_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            Replace();
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            ReplaceAll();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
            ReturnFocusToOwner();
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Enter：查找模式執行查找，取代模式執行取代
                if (isFindMode) FindNext(); else Replace();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                ReturnFocusToOwner();
            }
        }

        private void textBoxReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Replace();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Hide();
                ReturnFocusToOwner();
            }
        }

        /// <summary>
        /// 從上一次查找位置開始，在 targetRichTextBox 中查找下一個符合的文字。
        /// 若到達結尾未找到，會從頭再查找一次（循環查找）。
        /// </summary>
        private void FindNext()
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
            {
                MessageBox.Show("請輸入要尋找的文字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxFind.Focus();
                return;
            }

            string searchText = textBoxFind.Text;
            bool matchCase = checkBoxMatchCase.Checked;
            bool wholeWord = checkBoxWholeWord.Checked;

            // 若目前有選取文字，從選取範圍的結尾開始查找（避免重複找到同一個）
            int startPos = lastSearchPosition;
            if (targetRichTextBox.SelectionLength > 0)
                startPos = targetRichTextBox.SelectionStart + targetRichTextBox.SelectionLength;

            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int foundPos = -1;

            if (wholeWord)
            {
                // 全字匹配：確認找到的位置前後都不是字母、數字或底線
                string text = targetRichTextBox.Text;
                int searchLength = searchText.Length;

                foundPos = FindWholeWord(text, searchText, startPos, comparison, searchLength);

                // 若從 startPos 起找不到，從頭再找一次（循環）
                if (foundPos == -1 && startPos > 0)
                    foundPos = FindWholeWord(text, searchText, 0, comparison, searchLength);
            }
            else
            {
                // 普通查找：使用 IndexOf
                foundPos = targetRichTextBox.Text.IndexOf(searchText, startPos, comparison);
                if (foundPos == -1 && startPos > 0)
                    foundPos = targetRichTextBox.Text.IndexOf(searchText, 0, comparison);
            }

            if (foundPos >= 0)
            {
                // 找到：選取並捲動到找到的位置
                targetRichTextBox.SelectionStart = foundPos;
                targetRichTextBox.SelectionLength = searchText.Length;
                targetRichTextBox.ScrollToCaret();
                targetRichTextBox.Focus();
                lastSearchPosition = foundPos + searchText.Length;
            }
            else
            {
                MessageBox.Show($"找不到「{searchText}」。", "尋找", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lastSearchPosition = 0;
            }
        }

        // 在文字中從指定位置開始，以全字匹配方式查找第一個符合的位置
        private int FindWholeWord(string text, string searchText, int startPos, StringComparison comparison, int searchLength)
        {
            for (int i = startPos; i <= text.Length - searchLength; i++)
            {
                if (string.Compare(text.Substring(i, searchLength), searchText, comparison) != 0)
                    continue;

                // 檢查前後是否為字詞邊界（非字母、數字、底線）
                bool frontBoundary = (i == 0) || !(char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '_');
                bool backBoundary = (i + searchLength >= text.Length) || !(char.IsLetterOrDigit(text[i + searchLength]) || text[i + searchLength] == '_');

                if (frontBoundary && backBoundary)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 若目前選取的文字符合查找條件，則取代之，然後繼續查找下一個。
        /// </summary>
        private void Replace()
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
            {
                MessageBox.Show("請輸入要尋找的文字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxFind.Focus();
                return;
            }

            // 若目前選取的文字符合查找條件，先取代，再繼續查找
            if (targetRichTextBox.SelectionLength > 0)
            {
                string selectedText = targetRichTextBox.SelectedText;
                string searchText = textBoxFind.Text;
                bool matchCase = checkBoxMatchCase.Checked;

                bool isMatch = matchCase
                    ? selectedText == searchText
                    : string.Equals(selectedText, searchText, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                {
                    int selStart = targetRichTextBox.SelectionStart;
                    targetRichTextBox.SelectedText = textBoxReplace.Text;
                    // 取代後選取剛插入的文字，方便使用者確認
                    targetRichTextBox.SelectionStart = selStart;
                    targetRichTextBox.SelectionLength = textBoxReplace.Text.Length;
                    targetRichTextBox.ScrollToCaret();
                }
            }

            // 繼續查找下一個
            FindNext();
        }

        /// <summary>
        /// 取代所有符合的文字（支援全字匹配和大小寫選項）。
        /// 完成後顯示取代數量。
        /// </summary>
        private void ReplaceAll()
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
            {
                MessageBox.Show("請輸入要尋找的文字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxFind.Focus();
                return;
            }

            string searchText = textBoxFind.Text;
            string replaceText = textBoxReplace.Text;
            bool matchCase = checkBoxMatchCase.Checked;
            bool wholeWord = checkBoxWholeWord.Checked;
            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            int replaceCount = 0;
            string text = targetRichTextBox.Text;

            if (wholeWord)
            {
                // 全字匹配取代：逐段掃描並重建字串
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                int currentPos = 0;

                while (currentPos < text.Length)
                {
                    int foundPos = text.IndexOf(searchText, currentPos, comparison);
                    if (foundPos == -1)
                    {
                        // 沒有更多符合的位置，附加剩餘文字並結束
                        result.Append(text.Substring(currentPos));
                        break;
                    }

                    // 驗證是否為全字邊界
                    bool frontBoundary = (foundPos == 0) || !(char.IsLetterOrDigit(text[foundPos - 1]) || text[foundPos - 1] == '_');
                    bool backBoundary = (foundPos + searchText.Length >= text.Length) || !(char.IsLetterOrDigit(text[foundPos + searchText.Length]) || text[foundPos + searchText.Length] == '_');

                    if (frontBoundary && backBoundary)
                    {
                        // 符合全字邊界：取代
                        result.Append(text.Substring(currentPos, foundPos - currentPos));
                        result.Append(replaceText);
                        replaceCount++;
                        currentPos = foundPos + searchText.Length;
                    }
                    else
                    {
                        // 不符合全字邊界：保留並繼續
                        result.Append(text.Substring(currentPos, foundPos + searchText.Length - currentPos));
                        currentPos = foundPos + searchText.Length;
                    }
                }

                targetRichTextBox.Text = result.ToString();
            }
            else
            {
                // 普通取代：先計算次數，再一次性取代
                int index = 0;
                while ((index = text.IndexOf(searchText, index, comparison)) != -1)
                {
                    replaceCount++;
                    index += searchText.Length;
                }
                targetRichTextBox.Text = text.Replace(searchText, replaceText, comparison);
            }

            MessageBox.Show($"已取代 {replaceCount} 個項目。", "取代", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lastSearchPosition = 0;
        }

        // 使用者按視窗關閉按鈕時，改為隱藏對話框（而非真正關閉），下次可直接重新顯示
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                ReturnFocusToOwner();
            }
        }
    }
}

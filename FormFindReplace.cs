using System;
using System.Drawing;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormFindReplace : Form
    {
        private RichTextBox targetRichTextBox;
        private int lastSearchPosition = 0;
        private bool isFindMode = true; // true = 查找模式, false = 替换模式

        public FormFindReplace(RichTextBox richTextBox, bool findMode = true)
        {
            InitializeComponent();
            targetRichTextBox = richTextBox;
            isFindMode = findMode;
            
            if (isFindMode)
            {
                this.Text = "尋找";
                //this.Height = 150;
                labelReplace.Visible = false;
                textBoxReplace.Visible = false;
                buttonReplace.Visible = false;
                buttonReplaceAll.Visible = false;
            }
            else
            {
                this.Text = "尋找與取代";
                //this.Height = 200;
            }

            // 如果 RichTextBox 有选中的文本，将其设为查找文本
            if (targetRichTextBox.SelectionLength > 0)
            {
                textBoxFind.Text = targetRichTextBox.SelectedText;
            }

            // 当查找文本改变时，重置搜索位置
            textBoxFind.TextChanged += (s, e) => { lastSearchPosition = 0; };
        }

        // 將焦點返回到主視窗的輔助方法
        private void ReturnFocusToOwner()
        {
            if (this.Owner != null && !this.Owner.IsDisposed)
            {
                // 使用 BeginInvoke 確保在當前事件處理完成後執行
                this.BeginInvoke(new Action(() =>
                {
                    if (this.Owner != null && !this.Owner.IsDisposed)
                    {
                        this.Owner.Activate();
                        if (targetRichTextBox != null && !targetRichTextBox.IsDisposed && targetRichTextBox.Visible)
                        {
                            targetRichTextBox.Focus();
                        }
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
                if (isFindMode)
                    FindNext();
                else
                    Replace();
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

            int startPos = lastSearchPosition;
            if (targetRichTextBox.SelectionLength > 0)
            {
                startPos = targetRichTextBox.SelectionStart + targetRichTextBox.SelectionLength;
            }

            StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int foundPos = -1;

            if (wholeWord)
            {
                // 全字匹配查找
                string text = targetRichTextBox.Text;
                int searchLength = searchText.Length;
                
                for (int i = startPos; i <= text.Length - searchLength; i++)
                {
                    if (string.Compare(text.Substring(i, searchLength), searchText, comparison) == 0)
                    {
                        // 检查前后字符是否为单词边界
                        bool isWordBoundary = true;
                        if (i > 0)
                        {
                            char prevChar = text[i - 1];
                            if (char.IsLetterOrDigit(prevChar) || prevChar == '_')
                                isWordBoundary = false;
                        }
                        if (i + searchLength < text.Length)
                        {
                            char nextChar = text[i + searchLength];
                            if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                isWordBoundary = false;
                        }
                        
                        if (isWordBoundary)
                        {
                            foundPos = i;
                            break;
                        }
                    }
                }
                
                // 如果没找到，从头开始查找
                if (foundPos == -1 && startPos > 0)
                {
                    for (int i = 0; i <= text.Length - searchLength; i++)
                    {
                        if (string.Compare(text.Substring(i, searchLength), searchText, comparison) == 0)
                        {
                            bool isWordBoundary = true;
                            if (i > 0)
                            {
                                char prevChar = text[i - 1];
                                if (char.IsLetterOrDigit(prevChar) || prevChar == '_')
                                    isWordBoundary = false;
                            }
                            if (i + searchLength < text.Length)
                            {
                                char nextChar = text[i + searchLength];
                                if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                                    isWordBoundary = false;
                            }
                            
                            if (isWordBoundary)
                            {
                                foundPos = i;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // 普通查找
                foundPos = targetRichTextBox.Text.IndexOf(searchText, startPos, comparison);
                
                // 如果没找到，从头开始查找
                if (foundPos == -1 && startPos > 0)
                {
                    foundPos = targetRichTextBox.Text.IndexOf(searchText, 0, comparison);
                }
            }

            if (foundPos >= 0)
            {
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

        private void Replace()
        {
            if (string.IsNullOrEmpty(textBoxFind.Text))
            {
                MessageBox.Show("請輸入要尋找的文字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBoxFind.Focus();
                return;
            }

            // 如果当前选中的文本匹配查找文本，则替换
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
                    targetRichTextBox.SelectionStart = selStart;
                    targetRichTextBox.SelectionLength = textBoxReplace.Text.Length;
                    targetRichTextBox.ScrollToCaret();
                }
            }

            // 查找下一个
            FindNext();
        }

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

            int replaceCount = 0;
            string text = targetRichTextBox.Text;
            int currentPos = 0;

            if (wholeWord)
            {
                // 全字匹配替换
                StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                System.Text.StringBuilder result = new System.Text.StringBuilder();
                
                while (currentPos < text.Length)
                {
                    int foundPos = text.IndexOf(searchText, currentPos, comparison);
                    if (foundPos == -1)
                    {
                        result.Append(text.Substring(currentPos));
                        break;
                    }
                    
                    // 检查是否为单词边界
                    bool isWordBoundary = true;
                    if (foundPos > 0)
                    {
                        char prevChar = text[foundPos - 1];
                        if (char.IsLetterOrDigit(prevChar) || prevChar == '_')
                            isWordBoundary = false;
                    }
                    if (foundPos + searchText.Length < text.Length)
                    {
                        char nextChar = text[foundPos + searchText.Length];
                        if (char.IsLetterOrDigit(nextChar) || nextChar == '_')
                            isWordBoundary = false;
                    }
                    
                    if (isWordBoundary)
                    {
                        result.Append(text.Substring(currentPos, foundPos - currentPos));
                        result.Append(replaceText);
                        replaceCount++;
                        currentPos = foundPos + searchText.Length;
                    }
                    else
                    {
                        result.Append(text.Substring(currentPos, foundPos + searchText.Length - currentPos));
                        currentPos = foundPos + searchText.Length;
                    }
                }
                
                targetRichTextBox.Text = result.ToString();
            }
            else
            {
                // 普通替换
                StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                
                // 先计算替换次数
                int count = 0;
                int index = 0;
                while ((index = text.IndexOf(searchText, index, comparison)) != -1)
                {
                    count++;
                    index += searchText.Length;
                }
                replaceCount = count;
                
                // 执行替换
                targetRichTextBox.Text = text.Replace(searchText, replaceText, comparison);
            }

            MessageBox.Show($"已取代 {replaceCount} 個項目。", "取代", MessageBoxButtons.OK, MessageBoxIcon.Information);
            lastSearchPosition = 0;
        }

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


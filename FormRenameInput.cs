using System;
using System.IO;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 輸入新名稱的通用對話框，用於重新命名檔案或資料夾。
    /// 開啟時自動選取文字框中不含副檔名的部分，方便使用者直接輸入新名稱。
    /// </summary>
    public partial class FormRenameInput : Form
    {
        /// <summary>使用者確認後輸入的新名稱（已 Trim）。</summary>
        public string InputText { get; private set; } = "";

        // 初始預設值，用於計算要自動選取的長度（不含副檔名）
        private string m_DefaultValue = "";

        /// <summary>
        /// 建構子：設定對話框標題、提示文字和預設輸入值。
        /// </summary>
        /// <param name="prompt">顯示在文字框上方的說明文字。</param>
        /// <param name="title">對話框視窗標題。</param>
        /// <param name="defaultValue">文字框的預設值（通常為原始檔名）。</param>
        /// <param name="isMultiFile">是否為多檔案批次更名模式（目前保留備用）。</param>
        public FormRenameInput(string prompt, string title, string defaultValue, bool isMultiFile)
        {
            InitializeComponent();
            this.Text = title;
            labelPrompt.Text = prompt;
            textBoxFileName.Text = defaultValue;
            m_DefaultValue = defaultValue;

            // 在 Shown 事件中執行文字選取（確保 TextBox 已完全初始化）
            this.Shown += FormRenameInput_Shown;
        }

        // 對話框顯示後：自動選取檔名中不含副檔名的部分並設定焦點
        private void FormRenameInput_Shown(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(m_DefaultValue))
            {
                string extension = Path.GetExtension(m_DefaultValue);
                if (!string.IsNullOrEmpty(extension))
                {
                    // 選取不含副檔名的部分（例如 "document.txt" 只選取 "document"）
                    int selectLength = m_DefaultValue.Length - extension.Length;
                    textBoxFileName.Select(0, selectLength);
                }
                else
                {
                    // 無副檔名則全選
                    textBoxFileName.SelectAll();
                }
            }
            textBoxFileName.Focus();
        }

        // 「確定」按鈕：驗證輸入不為空，設定回傳值並關閉
        private void buttonOK_Click(object sender, EventArgs e)
        {
            InputText = textBoxFileName.Text.Trim();
            if (string.IsNullOrEmpty(InputText))
            {
                MessageBox.Show("檔名不能為空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「取消」按鈕：放棄輸入並關閉
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Enter 鍵確認，Escape 鍵取消
        private void textBoxFileName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonOK_Click(sender, e);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                buttonCancel_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}

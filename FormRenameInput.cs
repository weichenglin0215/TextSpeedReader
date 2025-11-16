using System;
using System.IO;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormRenameInput : Form
    {
        public string InputText { get; private set; } = "";

        private string m_DefaultValue = "";

        public FormRenameInput(string prompt, string title, string defaultValue, bool isMultiFile)
        {
            InitializeComponent();
            this.Text = title;
            labelPrompt.Text = prompt;
            textBoxFileName.Text = defaultValue;
            m_DefaultValue = defaultValue;

            // 在表單顯示後選取文字，確保 TextBox 已完全初始化
            this.Shown += FormRenameInput_Shown;
        }

        private void FormRenameInput_Shown(object sender, EventArgs e)
        {
            // 自動選取不含副檔名的部分
            if (!string.IsNullOrEmpty(m_DefaultValue))
            {
                string extension = Path.GetExtension(m_DefaultValue);
                if (!string.IsNullOrEmpty(extension))
                {
                    int selectLength = m_DefaultValue.Length - extension.Length;
                    textBoxFileName.Select(0, selectLength);
                }
                else
                {
                    textBoxFileName.SelectAll();
                }
            }
            // 將焦點設定到 TextBox
            textBoxFileName.Focus();
        }

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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

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


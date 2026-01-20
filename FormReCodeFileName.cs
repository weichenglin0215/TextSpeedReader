using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormReCodeFileName : Form
    {
        public string SelectedName { get; private set; } = "";
        public Encoding? SelectedCorrectEncoding { get; private set; }
        public Encoding? SelectedWrongEncoding { get; private set; }
        private string m_OriginalName;

        public FormReCodeFileName(string originalName)
        {
            InitializeComponent();
            m_OriginalName = originalName;
            labelOriginal.Text = "目前檔名: " + originalName;
            PopulatePreviews();
        }

        private void PopulatePreviews()
        {
            listBoxPreviews.Items.Clear();

            // 取得常見編碼
            Encoding utf8 = new UTF8Encoding(false);
            Encoding gbk = Encoding.GetEncoding(936);
            Encoding big5 = Encoding.GetEncoding(950);
            Encoding unicode = Encoding.Unicode;

            // 根據使用者要求的內容添加 preview
            AddPreview("Big5 -> GBK", big5, gbk);
            AddPreview("Big5 -> UTF-8", big5, utf8);
            AddPreview("Big5 -> Unicode", big5, unicode);
            AddPreview("GBK -> Big5", gbk, big5);
            AddPreview("GBK -> UTF-8", gbk, utf8);
            AddPreview("GBK -> Unicode", gbk, unicode);
            AddPreview("UTF-8 -> Big5", utf8, big5);
            AddPreview("UTF-8 -> GBK", utf8, gbk);
            AddPreview("UTF-8 -> Unicode", utf8, unicode);
            AddPreview("Unicode -> Big5", unicode, big5);
            AddPreview("Unicode -> GBK", unicode, gbk);
            AddPreview("Unicode -> UTF-8", unicode, utf8);
        }

        private void AddPreview(string label, Encoding correct, Encoding wrong)
        {
            try
            {
                byte[] bytes = wrong.GetBytes(m_OriginalName);
                string fixedName = correct.GetString(bytes);
                
                if (fixedName != m_OriginalName)
                {
                    listBoxPreviews.Items.Add(new PreviewItem(label, fixedName, correct, wrong));
                }
            }
            catch
            {
                // 忽略失敗的轉換
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (listBoxPreviews.SelectedItem is PreviewItem selected)
            {
                SelectedName = selected.FixedName;
                SelectedCorrectEncoding = selected.CorrectEncoding;
                SelectedWrongEncoding = selected.WrongEncoding;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("請先選擇一個正確的顯示結果。", "提示");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listBoxPreviews_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxPreviews.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                buttonOK_Click(sender, e);
            }
        }

        private class PreviewItem
        {
            public string Label { get; }
            public string FixedName { get; }
            public Encoding CorrectEncoding { get; }
            public Encoding WrongEncoding { get; }

            public PreviewItem(string label, string fixedName, Encoding correct, Encoding wrong)
            {
                Label = label;
                FixedName = fixedName;
                CorrectEncoding = correct;
                WrongEncoding = wrong;
            }

            public override string ToString()
            {
                return $"[{Label}] {FixedName}";
            }
        }
    }
}

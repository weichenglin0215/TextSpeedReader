using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormReCodeFull : Form
    {
        public Encoding? SelectedCorrectEncoding { get; private set; }
        public Encoding? SelectedWrongEncoding { get; private set; }
        public bool IsSim2TradChecked => checkBoxSim2Trad.Checked;
        private string m_OriginalName;

        public FormReCodeFull(string originalName)
        {
            InitializeComponent();
            m_OriginalName = originalName;
            labelOriginal.Text = "根目錄名稱: " + originalName;
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
            Encoding gb18030 = Encoding.GetEncoding(54936);

            // 修正後的預覽邏輯：(標籤, 正確編碼, 錯誤編碼)
            // 目標是將目前的「亂碼文字」還原回「原始位元組」，再用「正確編碼」重新解碼。
            // 例如："目前: Big5 -> 正確: GBK" 表示目前看到的是用 Big5 誤開的 GBK 檔名。
            AddPreview("目前: Big5 -> 轉成: GB18030", gb18030, big5);
            AddPreview("目前: Big5 -> 轉成: GBK", gbk, big5);
            AddPreview("目前: Big5 -> 轉成: UTF-8", utf8, big5);
            AddPreview("目前: Big5 -> 轉成: Unicode", unicode, big5);

            AddPreview("目前: GB18030 -> 轉成: Big5", big5, gb18030);

            AddPreview("目前: GBK -> 轉成: Big5", big5, gbk);
            AddPreview("目前: GBK -> 轉成: UTF-8", utf8, gbk);
            AddPreview("目前: GBK -> 轉成: Unicode", unicode, gbk);

            AddPreview("目前: UTF-8 -> 轉成: Big5", big5, utf8);
            AddPreview("目前: UTF-8 -> 轉成: GBK", gbk, utf8);
            AddPreview("目前: UTF-8 -> 轉成: GB18030", gb18030, utf8);
            AddPreview("目前: UTF-8 -> 轉成: Unicode", unicode, utf8);

            AddPreview("目前: Unicode -> 轉成: Big5", big5, unicode);
            AddPreview("目前: Unicode -> 轉成: GBK", gbk, unicode);
            AddPreview("目前: Unicode -> 轉成: UTF-8", utf8, unicode);
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 批量編碼修正對話框（目錄全量版）：以目錄名稱為預覽範本，列出各種「錯誤編碼→正確編碼」的轉換結果，
    /// 供使用者選取後對整個目錄樹下的所有檔案與子目錄套用相同的編碼修正。
    /// 與 FormReCodeFileName 的差異：此對話框不回傳修正後的名稱，只回傳使用者選定的編碼組合。
    /// </summary>
    public partial class FormReCodeFull : Form
    {
        /// <summary>使用者選定的「正確」目標編碼（用來重新解碼原始位元組）。</summary>
        public Encoding? SelectedCorrectEncoding { get; private set; }
        /// <summary>使用者選定的「錯誤」來源編碼（用來還原出原始位元組）。</summary>
        public Encoding? SelectedWrongEncoding { get; private set; }
        /// <summary>是否同時執行簡→繁轉換。</summary>
        public bool IsSim2TradChecked => checkBoxSim2Trad.Checked;
        private string m_OriginalName;

        public FormReCodeFull()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構子：傳入根目錄名稱作為預覽範本，並立即產生所有編碼轉換預覽。
        /// </summary>
        /// <param name="originalName">根目錄的當前名稱（作為預覽範本，可能含亂碼）。</param>
        public FormReCodeFull(string originalName) : this()
        {
            m_OriginalName = originalName;
            labelOriginal.Text = "根目錄名稱: " + originalName;
            PopulatePreviews();
        }

        // 列舉所有常見的編碼組合，將轉換結果加入 listBoxPreviews
        private void PopulatePreviews()
        {
            listBoxPreviews.Items.Clear();

            // 取得常見編碼
            Encoding utf8 = new UTF8Encoding(false);
            Encoding utf8BOM = new UTF8Encoding(true);
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
            AddPreview("目前: Big5 -> 轉成: UTF-8-BOM", utf8BOM, big5);
            AddPreview("目前: Big5 -> 轉成: Unicode", unicode, big5);

            AddPreview("目前: GB18030 -> 轉成: Big5", big5, gb18030);

            AddPreview("目前: GBK -> 轉成: Big5", big5, gbk);
            AddPreview("目前: GBK -> 轉成: UTF-8", utf8, gbk);
            AddPreview("目前: GBK -> 轉成: UTF-8-BOM", utf8BOM, gbk);
            AddPreview("目前: GBK -> 轉成: Unicode", unicode, gbk);

            AddPreview("目前: UTF-8 -> 轉成: Big5", big5, utf8);
            AddPreview("目前: UTF-8 -> 轉成: GBK", gbk, utf8);
            AddPreview("目前: UTF-8 -> 轉成: GB18030", gb18030, utf8);
            AddPreview("目前: UTF-8 -> 轉成: Unicode", unicode, utf8);

            AddPreview("目前: UTF-8-BOM -> 轉成: Big5", big5, utf8BOM);
            AddPreview("目前: UTF-8-BOM -> 轉成: GBK", gbk, utf8BOM);
            AddPreview("目前: UTF-8-BOM -> 轉成: GB18030", gb18030, utf8BOM);
            AddPreview("目前: UTF-8-BOM -> 轉成: Unicode", unicode, utf8BOM);

            AddPreview("目前: Unicode -> 轉成: Big5", big5, unicode);
            AddPreview("目前: Unicode -> 轉成: GBK", gbk, unicode);
            AddPreview("目前: Unicode -> 轉成: UTF-8", utf8, unicode);
            AddPreview("目前: Unicode -> 轉成: UTF-8-BOM", utf8BOM, unicode);
        }

        // 嘗試一種編碼轉換；結果不同才加入清單
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

        // 「確定」按鈕：回傳使用者選取的編碼組合（不回傳修正後的名稱）
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

        // 「取消」按鈕
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 雙擊清單項目相當於按下「確定」按鈕
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 編碼修正對話框（檔名／文字預覽版）：將目前字串以各種「錯誤編碼→正確編碼」組合重新解碼，
    /// 列出所有與原始結果不同的預覽供使用者選取。
    /// 主要用途：修正因編碼誤判而顯示為亂碼的檔名或文字片段。
    /// </summary>
    public partial class FormReCodeFileName : Form
    {
        /// <summary>使用者確認選取後，修正後的新名稱（或文字）。</summary>
        public string SelectedName { get; private set; } = "";
        /// <summary>使用者選定的「正確」目標編碼（用來重新解碼原始位元組）。</summary>
        public Encoding? SelectedCorrectEncoding { get; private set; }
        /// <summary>使用者選定的「錯誤」來源編碼（用來還原出原始位元組）。</summary>
        public Encoding? SelectedWrongEncoding { get; private set; }
        /// <summary>是否同時執行簡→繁轉換。</summary>
        public bool IsSim2TradChecked => checkBoxSim2Trad.Checked;
        private string m_OriginalName;

        public FormReCodeFileName()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 建構子：傳入要修正的原始名稱，並立即產生所有編碼轉換預覽。
        /// </summary>
        /// <param name="originalName">目前顯示的名稱（可能含亂碼）。</param>
        public FormReCodeFileName(string originalName) : this()
        {
            m_OriginalName = originalName;
            labelOriginal.Text = "目前檔名: " + originalName;
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

        // 嘗試一種編碼轉換：以 wrong 編碼取得位元組，再以 correct 解碼；結果不同才加入清單
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
                // 忽略失敗的轉換（例如：原始字串無法以 wrong 編碼表示）
            }
        }

        // 「確定」按鈕：回傳使用者選取的預覽結果
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

        // 「取消」按鈕：放棄選取並關閉
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 雙擊清單項目：相當於按下「確定」按鈕
        private void listBoxPreviews_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBoxPreviews.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                buttonOK_Click(sender, e);
            }
        }

        // 預覽項目資料結構：儲存標籤、轉換後名稱，以及使用的編碼組合
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

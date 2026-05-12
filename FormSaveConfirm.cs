using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 儲存確認對話框：當使用者要切換到其他檔案，但目前檔案有未保存的修改時顯示。
    /// 提供三種選擇：
    /// - 不儲存：直接放棄修改並繼續。
    /// - 另存新檔：開啟另存對話框後繼續。
    /// - 儲存檔案：覆蓋原始檔案後繼續。
    /// </summary>
    public partial class FormSaveConfirm : Form
    {
        /// <summary>使用者選擇的儲存動作。</summary>
        public enum SaveOption
        {
            No,      // 不儲存，直接放棄修改
            SaveAs,  // 另存新檔
            Save     // 覆蓋儲存目前檔案
        }

        /// <summary>使用者最終選擇的動作，預設為「不儲存」。</summary>
        public SaveOption SelectedOption { get; private set; } = SaveOption.No;

        /// <summary>
        /// 建構子：顯示包含檔案名稱的確認訊息。
        /// </summary>
        /// <param name="fileName">有未保存修改的檔案名稱（僅檔名，不含路徑）。</param>
        public FormSaveConfirm(string fileName)
        {
            InitializeComponent();
            labelMessage.Text = $"檔案「{fileName}」有未保存的修改，是否先儲存目前編輯內容？";
        }

        // 「不儲存」按鈕：放棄修改，直接繼續操作
        private void buttonNo_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.No;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「另存新檔」按鈕：開啟另存對話框後再繼續
        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.SaveAs;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「儲存檔案」按鈕：覆蓋原始檔案後繼續
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.Save;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

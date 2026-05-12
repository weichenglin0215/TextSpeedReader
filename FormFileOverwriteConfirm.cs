using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 檔案覆蓋確認對話框：當要儲存的目標檔案已存在時顯示。
    /// 提供三種選擇：
    /// - 取消：放棄儲存操作。
    /// - 覆蓋原有檔案：直接覆蓋已存在的同名檔案。
    /// - 另存新檔：開啟另存對話框以指定不同的路徑或名稱。
    /// </summary>
    public partial class FormFileOverwriteConfirm : Form
    {
        /// <summary>使用者選擇的覆蓋動作。</summary>
        public enum OverwriteOption
        {
            Cancel,    // 取消儲存，不執行任何動作
            Overwrite, // 覆蓋已存在的同名檔案
            SaveAs     // 另存新檔，選擇不同路徑或名稱
        }

        /// <summary>使用者最終選擇的動作，預設為「取消」。</summary>
        public OverwriteOption SelectedOption { get; private set; } = OverwriteOption.Cancel;

        /// <summary>
        /// 建構子：顯示包含衝突檔案名稱的確認訊息。
        /// </summary>
        /// <param name="fileName">已存在的衝突檔案名稱（僅檔名，不含路徑）。</param>
        public FormFileOverwriteConfirm(string fileName)
        {
            InitializeComponent();
            labelMessage.Text = $"檔案「{fileName}」已存在，請選擇處理方式：";
        }

        // 「取消」按鈕：放棄本次儲存操作
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.Cancel;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「覆蓋」按鈕：直接覆蓋同名檔案
        private void buttonOverwrite_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.Overwrite;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「另存新檔」按鈕：開啟另存對話框以選擇新位置
        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.SaveAs;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

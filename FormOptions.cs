using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 選項對話框：讓使用者修改應用程式設定（AppSettings）中的各項參數。
    /// 按下「確定」時將設定值寫回 AppSettings 並儲存到設定檔。
    /// 按下「取消」時放棄修改。
    /// </summary>
    public partial class FormOptions : Form
    {
        // 對應到主程式的 AppSettings 參考，修改後直接反映到設定物件
        private AppSettings appSettings;

        /// <summary>
        /// 建構子：傳入 AppSettings 物件並以目前設定值初始化介面。
        /// </summary>
        /// <param name="settings">主程式的 AppSettings 執行個體。</param>
        public FormOptions(AppSettings settings)
        {
            InitializeComponent();
            appSettings = settings;

            // 將目前設定值填入介面控制項
            checkBoxAutoOpenLastDirectory.Checked = appSettings.AutoOpenLastDirectory;
            checkBoxKeepFontSize.Checked = appSettings.KeepFontSize;
            textBoxNewLineStartJudgment.Text = appSettings.NewLineStartJudgment;
            textBoxNewLineEndJudgment.Text = appSettings.NewLineEndJudgment;
            numericUpDown_AddSpaceChrCount.Value = appSettings.AddSpaceChrCount;
            textBoxInsertBeginingText.Text = appSettings.InsertBeginingText;
            textBoxInsertEndText.Text = appSettings.InsertEndText;
            textBoxAnnotationBegin.Text = appSettings.AnnotationBegin;
            textBoxAnnotationEnd.Text = appSettings.AnnotationEnd;
        }

        // 「確定」按鈕：將介面值寫回 AppSettings 並儲存設定檔
        private void buttonOK_Click(object sender, EventArgs e)
        {
            appSettings.AutoOpenLastDirectory = checkBoxAutoOpenLastDirectory.Checked;
            appSettings.KeepFontSize = checkBoxKeepFontSize.Checked;
            appSettings.AddSpaceChrCount = (int)numericUpDown_AddSpaceChrCount.Value;
            appSettings.NewLineStartJudgment = textBoxNewLineStartJudgment.Text;
            appSettings.NewLineEndJudgment = textBoxNewLineEndJudgment.Text;
            appSettings.InsertBeginingText = textBoxInsertBeginingText.Text;
            appSettings.InsertEndText = textBoxInsertEndText.Text;
            appSettings.AnnotationBegin = textBoxAnnotationBegin.Text;
            appSettings.AnnotationEnd = textBoxAnnotationEnd.Text;

            // 立即儲存設定到 INI 檔案
            appSettings.SaveSettings();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 「取消」按鈕：放棄所有修改，關閉對話框
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 「清除歷史紀錄」按鈕：清空檔案與目錄的歷史清單（需要使用者確認）
        private void buttonClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定要清除所有歷史紀錄(檔案與目錄)嗎？",
                "清除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                appSettings.HistoryFiles.Clear();
                appSettings.HistoryDirectories.Clear();
                MessageBox.Show("歷史紀錄已清除。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}

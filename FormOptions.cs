using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormOptions : Form
    {
        private AppSettings appSettings;

        public FormOptions(AppSettings settings)
        {
            InitializeComponent();
            appSettings = settings;

            // 載入當前設定值
            checkBoxAutoOpenLastDirectory.Checked = appSettings.AutoOpenLastDirectory;
            checkBoxKeepFontSize.Checked = appSettings.KeepFontSize;

            // 載入判定字串
            textBoxNewLineStartJudgment.Text = appSettings.NewLineStartJudgment;
            textBoxNewLineEndJudgment.Text = appSettings.NewLineEndJudgment;

            numericUpDown_AddSpaceChrCount.Value = appSettings.AddSpaceChrCount;

            // 載入插入字串
            textBoxInsertBeginingText.Text = appSettings.InsertBeginingText;
            textBoxInsertEndText.Text = appSettings.InsertEndText;

            // 載入註解字串
            textBoxAnnotationBegin.Text = appSettings.AnnotationBegin;
            textBoxAnnotationEnd.Text = appSettings.AnnotationEnd;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 保存設定
            appSettings.AutoOpenLastDirectory = checkBoxAutoOpenLastDirectory.Checked;
            appSettings.KeepFontSize = checkBoxKeepFontSize.Checked;
            appSettings.AddSpaceChrCount = (int)numericUpDown_AddSpaceChrCount.Value;

            // 保存判定字串
            appSettings.NewLineStartJudgment = textBoxNewLineStartJudgment.Text;
            appSettings.NewLineEndJudgment = textBoxNewLineEndJudgment.Text;

            // 保存插入字串
            appSettings.InsertBeginingText = textBoxInsertBeginingText.Text;
            appSettings.InsertEndText = textBoxInsertEndText.Text;

            // 保存註解字串
            appSettings.AnnotationBegin = textBoxAnnotationBegin.Text;
            appSettings.AnnotationEnd = textBoxAnnotationEnd.Text;

            appSettings.SaveSettings();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定要清除所有歷史紀錄(檔案與目錄)嗎？", "清除確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                appSettings.HistoryFiles.Clear();
                appSettings.HistoryDirectories.Clear();
                MessageBox.Show("歷史紀錄已清除。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


    }
}


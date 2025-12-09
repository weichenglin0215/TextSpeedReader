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
            
            appSettings.SaveSettings();
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}


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
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // 保存設定
            appSettings.AutoOpenLastDirectory = checkBoxAutoOpenLastDirectory.Checked;
            appSettings.KeepFontSize = checkBoxKeepFontSize.Checked;
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


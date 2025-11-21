using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormFileOverwriteConfirm : Form
    {
        public enum OverwriteOption
        {
            Cancel,      // 取消儲存
            Overwrite,   // 覆蓋原有檔案
            SaveAs       // 另存新檔
        }

        public OverwriteOption SelectedOption { get; private set; } = OverwriteOption.Cancel;

        public FormFileOverwriteConfirm(string fileName)
        {
            InitializeComponent();
            labelMessage.Text = $"檔案「{fileName}」已存在，請選擇處理方式：";
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.Cancel;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonOverwrite_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.Overwrite;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            SelectedOption = OverwriteOption.SaveAs;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}


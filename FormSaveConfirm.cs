using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormSaveConfirm : Form
    {
        public enum SaveOption
        {
            No,           // 否
            SaveAs,       // 另存新檔
            Save          // 儲存檔案
        }

        public SaveOption SelectedOption { get; private set; } = SaveOption.No;

        public FormSaveConfirm(string fileName)
        {
            InitializeComponent();
            labelMessage.Text = $"檔案「{fileName}」有未保存的修改，是否先儲存目前編輯內容？";
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.No;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.SaveAs;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SelectedOption = SaveOption.Save;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}



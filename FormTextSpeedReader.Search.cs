using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        // 顯示查找對話框
        private void ShowFindDialog()
        {
            // 只有在文字編輯模式下才顯示查找對話框
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (m_FindReplaceDialog == null || m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog = new FormFindReplace(richTextBoxText, true);
                m_FindReplaceDialog.Owner = this;
            }
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }

        // 顯示替換對話框
        private void ShowReplaceDialog()
        {
            // 只有在文字編輯模式下才顯示替換對話框
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 如果对话框已存在，先关闭它
            if (m_FindReplaceDialog != null && !m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog.Close();
                m_FindReplaceDialog.Dispose();
            }

            // 创建新的替换对话框
            m_FindReplaceDialog = new FormFindReplace(richTextBoxText, false);
            m_FindReplaceDialog.Owner = this;
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }


    }
}

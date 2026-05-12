using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        /// <summary>
        /// 顯示「尋找」對話框（查找模式）。
        /// 若對話框已開啟則重新激活；若不是文字編輯模式則提示使用者先開啟檔案。
        /// </summary>
        private void ShowFindDialog()
        {
            // 只有在文字編輯模式下（richTextBoxText 可見）才顯示查找對話框
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 若對話框尚未建立或已被關閉（Disposed），重新建立
            if (m_FindReplaceDialog == null || m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog = new FormFindReplace(richTextBoxText, true);
                m_FindReplaceDialog.Owner = this;
            }
            // 顯示對話框並將焦點移至查找文字框
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }

        /// <summary>
        /// 顯示「尋找與取代」對話框（取代模式）。
        /// 每次呼叫都會重新建立對話框（確保以取代模式初始化），
        /// 若不是文字編輯模式則提示使用者先開啟檔案。
        /// </summary>
        private void ShowReplaceDialog()
        {
            if (!richTextBoxText.Visible)
            {
                MessageBox.Show("請先開啟一個文字檔案。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 每次顯示取代對話框時，關閉並重建，確保以「取代模式」初始化
            if (m_FindReplaceDialog != null && !m_FindReplaceDialog.IsDisposed)
            {
                m_FindReplaceDialog.Close();
                m_FindReplaceDialog.Dispose();
            }

            m_FindReplaceDialog = new FormFindReplace(richTextBoxText, false);
            m_FindReplaceDialog.Owner = this;
            m_FindReplaceDialog.Show();
            m_FindReplaceDialog.Activate();
            m_FindReplaceDialog.textBoxFind.Focus();
            m_FindReplaceDialog.textBoxFind.SelectAll();
        }
    }
}

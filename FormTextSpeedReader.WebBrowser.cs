using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        /// <summary>
        /// 以 WebBrowser 控制項導航到指定網址。
        /// 若網址未含通訊協定前綴，自動補上 http://。
        /// </summary>
        /// <param name="address">要導航的網址字串。</param>
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;

            // 若網址沒有 http:// 或 https:// 前綴，補上 http://
            if (!address.StartsWith("http://") && !address.StartsWith("https://"))
                address = "http://" + address;

            try
            {
                webBrowser1.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                // 網址格式不合法，靜默忽略
            }
        }

        // WebBrowser 導航完成後的事件（目前無需額外處理）
        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }

        /// <summary>
        /// WebBrowser 文件完全載入後的事件處理。
        /// 套用預設樣式（黑底白字、指定字型），並刪除暫存的 HTML 臨時檔案。
        /// 臨時檔案延遲 1 秒後刪除，確保 WebBrowser 已完全完成載入。
        /// </summary>
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // 套用使用者設定的顯示樣式（黑底白字、字型）
            ApplyWebBrowserDefaultStyle();

            // 清理先前建立的暫存 HTML 檔案
            if (!string.IsNullOrEmpty(m_TempHtmlFilePath) && File.Exists(m_TempHtmlFilePath))
            {
                try
                {
                    // 延遲 1 秒刪除，避免 WebBrowser 仍在讀取時刪除導致錯誤
                    System.Threading.Timer timer = null;
                    timer = new System.Threading.Timer((state) =>
                    {
                        try
                        {
                            if (File.Exists(m_TempHtmlFilePath))
                                File.Delete(m_TempHtmlFilePath);
                            m_TempHtmlFilePath = "";
                        }
                        catch { }
                        finally
                        {
                            timer?.Dispose();
                        }
                    }, null, 1000, System.Threading.Timeout.Infinite);
                }
                catch { }
            }
        }

        /// <summary>
        /// 對 WebBrowser 的 body 元素套用預設顯示樣式：
        /// 縮放比例、字型家族、字型大小、黑色背景、白色文字。
        /// 僅在「改變HTML字體底色」按鈕被選取時才套用。
        /// </summary>
        private void ApplyWebBrowserDefaultStyle()
        {
            if (webBrowser1.Document == null) return;
            if (!toolStripButtonHTMLChangeFontChecker.Checked) return;

            try
            {
                Font currentFont = m_Font;
                string fontFamily = currentFont.FontFamily.Name;
                float fontSize = currentFont.SizeInPoints;
                // 處理字型名稱中的單引號（CSS 需要跳脫）
                string escapedFontFamily = fontFamily.Replace("'", "\\'");

                if (webBrowser1.Document.Body != null)
                {
                    // 組合 CSS 樣式字串，一次設定所有屬性
                    string style = $"zoom: {m_WebBrowserZoom}%; font-family: '{escapedFontFamily}', sans-serif; font-size: {fontSize}pt; background-color: #000000; color: #FFFFFF;";
                    webBrowser1.Document.Body.Style = style;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"設置 WebBrowser 樣式時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新 WebBrowser body 元素的樣式，使用指定的縮放比例。
        /// 其他樣式屬性（字型、顏色）與 ApplyWebBrowserDefaultStyle 相同。
        /// 僅在「改變HTML字體底色」按鈕被選取時才套用。
        /// </summary>
        /// <param name="zoomRate">縮放比例字串（純數字，不含 %）。</param>
        private void ApplyWebBrowserFontStyle(string zoomRate)
        {
            if (webBrowser1.Document == null) return;
            if (!toolStripButtonHTMLChangeFontChecker.Checked) return;

            try
            {
                Font currentFont = m_Font;
                string fontFamily = currentFont.FontFamily.Name;
                float fontSize = currentFont.SizeInPoints;
                string escapedFontFamily = fontFamily.Replace("'", "\\'");

                if (webBrowser1.Document.Body != null)
                {
                    string style = $"zoom: {zoomRate}%; font-family: '{escapedFontFamily}', sans-serif; font-size: {fontSize}pt; background-color: #000000; color: #FFFFFF;";
                    webBrowser1.Document.Body.Style = style;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"設置 WebBrowser 樣式時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 「改變HTML字體底色」工具列按鈕的點擊事件。
        /// 選取時套用自訂樣式；取消選取時重新整理頁面以還原原始樣式。
        /// </summary>
        private void ToolStripButtonHTMLChangeFontChecker_Click(object sender, EventArgs e)
        {
            // 更新按鈕文字以反映目前勾選狀態
            toolStripButtonHTMLChangeFontChecker.Text = toolStripButtonHTMLChangeFontChecker.Checked
                ? "✔改變HTML字體底色"
                : "　改變HTML字體底色";

            if (toolStripButtonHTMLChangeFontChecker.Checked)
                ApplyWebBrowserDefaultStyle();
            else if (webBrowser1.Url != null)
                webBrowser1.Refresh(); // 重整以還原 HTML 頁面本身的原始樣式
        }
    }
}

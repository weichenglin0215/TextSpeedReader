using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        // 處理網頁導航
        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;

            // 確保網址格式正確
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                webBrowser1.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        // 處理網頁導航完成事件
        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //toolStripTextBox1.Text = webBrowser1.Url.ToString();
        }

        // 處理 WebBrowser 文檔載入完成事件
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ApplyWebBrowserDefaultStyle();

            // 清理臨時HTML檔案
            if (!string.IsNullOrEmpty(m_TempHtmlFilePath) && File.Exists(m_TempHtmlFilePath))
            {
                try
                {
                    // 延遲刪除，確保WebBrowser完全載入
                    System.Threading.Timer timer = null;
                    timer = new System.Threading.Timer((state) =>
                    {
                        try
                        {
                            if (File.Exists(m_TempHtmlFilePath))
                            {
                                File.Delete(m_TempHtmlFilePath);
                            }
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

        // 應用 WebBrowser 預設樣式（背景顏色、字型、字體尺寸）
        private void ApplyWebBrowserDefaultStyle()
        {
            if (webBrowser1.Document == null)
                return;

            try
            {
                // 獲取當前 richTextBoxText 的字體設定作為參考
                //Font currentFont = richTextBoxText.Font;
                Font currentFont = m_Font;
                string fontFamily = currentFont.FontFamily.Name;
                float fontSize = currentFont.SizeInPoints;

                // 設定背景顏色（例如：白色 #FFFFFF，或淺灰色 #F5F5F5）
                // 可以根據需要修改顏色值
                //string backgroundColor = "#FFFFFF"; // 白色背景
                string backgroundColor = "#000000"; // 黑色背景

                // 設定字型家族（處理字體名稱中的特殊字符）
                string escapedFontFamily = fontFamily.Replace("'", "\\'");

                // 設定文字顏色
                string textColor = "#FFFFFF"; // 白色文字

                // 方法1：直接設置 body 樣式（簡單但可能被 HTML 內聯樣式覆蓋）
                if (webBrowser1.Document.Body != null)
                {
                    string style = $"zoom: {m_WebBrowserZoom.ToString()}%; font-family: '{escapedFontFamily}', sans-serif; font-size: {fontSize}pt; background-color: {backgroundColor}; color: {textColor};";
                    webBrowser1.Document.Body.Style = style;
                }
                /*
                // 方法2：注入 CSS 樣式到 head（更可靠，優先級更高）
                HtmlElement? headElement = webBrowser1.Document.GetElementsByTagName("head")[0];
                if (headElement != null)
                {
                    HtmlElement? styleElement = webBrowser1.Document.CreateElement("style");
                    if (styleElement != null)
                    {
                        // 使用 !important 確保樣式優先級
                        string css = $@"
                            body {{
                                font-family: '{escapedFontFamily}', sans-serif !important;
                                font-size: {fontSize}pt !important;
                                background-color: {backgroundColor} !important;
                                color: {textColor} !important;
                            }}
                        ";
                        styleElement.SetAttribute("type", "text/css");
                        styleElement.InnerHtml = css;
                        headElement.AppendChild(styleElement);
                    }
                }
                */
            }
            catch (Exception ex)
            {
                // 如果設置樣式失敗，不影響其他功能
                Console.WriteLine($"設置 WebBrowser 樣式時發生錯誤: {ex.Message}");
            }
        }

        private void ApplyWebBrowserFontStyle(string zoomRate)
        {
            if (webBrowser1.Document == null)
                return;

            try
            {
                // 獲取當前 richTextBoxText 的字體設定作為參考
                //Font currentFont = richTextBoxText.Font;
                Font currentFont = m_Font;
                string fontFamily = currentFont.FontFamily.Name;
                float fontSize = currentFont.SizeInPoints;

                // 設定背景顏色（例如：白色 #FFFFFF，或淺灰色 #F5F5F5）
                // 可以根據需要修改顏色值
                //string backgroundColor = "#FFFFFF"; // 白色背景
                string backgroundColor = "#000000"; // 黑色背景

                // 設定字型家族（處理字體名稱中的特殊字符）
                string escapedFontFamily = fontFamily.Replace("'", "\\'");

                // 設定文字顏色
                string textColor = "#FFFFFF"; // 白色文字

                // 方法1：直接設置 body 樣式（簡單但可能被 HTML 內聯樣式覆蓋）
                if (webBrowser1.Document.Body != null)
                {
                    string style = $"zoom: {zoomRate}%; font-family: '{escapedFontFamily}', sans-serif; font-size: {fontSize}pt; background-color: {backgroundColor}; color: {textColor};";
                    webBrowser1.Document.Body.Style = style;
                }
            }
            catch (Exception ex)
            {
                // 如果設置樣式失敗，不影響其他功能
                Console.WriteLine($"設置 WebBrowser 樣式時發生錯誤: {ex.Message}");
            }
        }

    }
}

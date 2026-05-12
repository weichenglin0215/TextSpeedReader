using System.Drawing;
using System.Drawing.Text;

namespace TextSpeedReader
{
    /// <summary>
    /// 管理顯示相關的資料，包含系統字型清單、字型大小選項，
    /// 以及 WebBrowser 縮放比例的遞增/遞減邏輯。
    /// </summary>
    public class DisplayManager
    {
        /// <summary>目前系統已安裝的字型家族陣列，用於填充字型下拉選單。</summary>
        public FontFamily[] FontFamilies { get; private set; }

        /// <summary>
        /// 可選的字型大小預設清單（點數），涵蓋從 1pt 到 500pt 的常用尺寸。
        /// 字型大小調整功能會在此清單中尋找上一個或下一個尺寸。
        /// </summary>
        public int[] FontSizes { get; } = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36, 48, 66, 72, 100, 125, 150, 200, 300, 400, 500 };

        /// <summary>WebBrowser 目前的縮放比例（百分比），預設 100%。</summary>
        public int WebBrowserZoom { get; set; } = 100;

        /// <summary>
        /// WebBrowser 縮放比例的預設選項清單（百分比），
        /// 涵蓋從 10% 到 600% 的常用縮放比例。
        /// </summary>
        public int[] WebBrowserSizes { get; } = { 10, 25, 33, 50, 66, 75, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500, 600 };

        /// <summary>
        /// 建構子：初始化時立即載入系統字型清單。
        /// </summary>
        public DisplayManager()
        {
            LoadSystemFonts();
        }

        // 取得系統已安裝的所有字型家族
        private void LoadSystemFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontFamilies = installedFontCollection.Families;
        }

        /// <summary>
        /// 在 FontSizes 清單中找出比 currentSize 大的最小尺寸並回傳。
        /// 若已是最大尺寸，回傳 currentSize（不變）。
        /// </summary>
        /// <param name="currentSize">目前字型大小（點數）。</param>
        /// <returns>下一個較大的字型大小，或 currentSize（若已是最大值）。</returns>
        public float GetNextLargerFontSize(float currentSize)
        {
            for (int i = 0; i < FontSizes.Length; i++)
            {
                if (FontSizes[i] > currentSize)
                    return FontSizes[i];
            }
            return currentSize;
        }

        /// <summary>
        /// 在 FontSizes 清單中找出比 currentSize 小的最大尺寸並回傳。
        /// 若已是最小尺寸，回傳 currentSize（不變）。
        /// </summary>
        /// <param name="currentSize">目前字型大小（點數）。</param>
        /// <returns>下一個較小的字型大小，或 currentSize（若已是最小值）。</returns>
        public float GetNextSmallerFontSize(float currentSize)
        {
            for (int i = FontSizes.Length - 1; i >= 0; i--)
            {
                if (FontSizes[i] < currentSize)
                    return FontSizes[i];
            }
            return currentSize;
        }

        /// <summary>
        /// 在 WebBrowserSizes 清單中找出比 WebBrowserZoom 大的最小縮放比例並回傳。
        /// 若已是最大縮放，回傳目前 WebBrowserZoom（不變）。
        /// </summary>
        /// <returns>下一個較大的縮放比例（百分比），或目前縮放（若已是最大值）。</returns>
        public int GetNextLargerZoom()
        {
            for (int i = 0; i < WebBrowserSizes.Length; i++)
            {
                if (WebBrowserSizes[i] > WebBrowserZoom)
                    return WebBrowserSizes[i];
            }
            return WebBrowserZoom;
        }

        /// <summary>
        /// 在 WebBrowserSizes 清單中找出比 WebBrowserZoom 小的最大縮放比例並回傳。
        /// 若已是最小縮放，回傳目前 WebBrowserZoom（不變）。
        /// </summary>
        /// <returns>下一個較小的縮放比例（百分比），或目前縮放（若已是最小值）。</returns>
        public int GetNextSmallerZoom()
        {
            for (int i = WebBrowserSizes.Length - 1; i >= 0; i--)
            {
                if (WebBrowserSizes[i] < WebBrowserZoom)
                    return WebBrowserSizes[i];
            }
            return WebBrowserZoom;
        }
    }
}

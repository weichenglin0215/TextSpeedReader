using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public class DisplayManager
    {
        // 系統字體列表
        public FontFamily[] FontFamilies { get; private set; }

        // 可選擇的字體大小列表
        public int[] FontSizes { get; } = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 32, 36, 48, 66, 72, 100, 125, 150, 200, 300, 400, 500 };

        // 網頁瀏覽器縮放比例
        public int WebBrowserZoom { get; set; } = 100;

        // 網頁瀏覽器可選擇的縮放大小
        public int[] WebBrowserSizes { get; } = { 10, 25, 33, 50, 66, 75, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500, 600 };

        public DisplayManager()
        {
            LoadSystemFonts();
        }

        // 獲取系統已安裝的字體
        private void LoadSystemFonts()
        {
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            FontFamilies = installedFontCollection.Families;
        }

        // 增加字體大小
        public float GetNextLargerFontSize(float currentSize)
        {
            for (int i = 0; i < FontSizes.Length; i++)
            {
                if (FontSizes[i] > currentSize)
                {
                    return FontSizes[i];
                }
            }
            return currentSize;
        }

        // 減少字體大小
        public float GetNextSmallerFontSize(float currentSize)
        {
            for (int i = FontSizes.Length - 1; i >= 0; i--)
            {
                if (FontSizes[i] < currentSize)
                {
                    return FontSizes[i];
                }
            }
            return currentSize;
        }

        // 增加網頁縮放
        public int GetNextLargerZoom()
        {
            for (int i = 0; i < WebBrowserSizes.Length; i++)
            {
                if (WebBrowserSizes[i] > WebBrowserZoom)
                {
                    return WebBrowserSizes[i];
                }
            }
            return WebBrowserZoom;
        }

        // 減少網頁縮放
        public int GetNextSmallerZoom()
        {
            for (int i = WebBrowserSizes.Length - 1; i >= 0; i--)
            {
                if (WebBrowserSizes[i] < WebBrowserZoom)
                {
                    return WebBrowserSizes[i];
                }
            }
            return WebBrowserZoom;
        }
    }
} 
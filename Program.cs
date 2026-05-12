using System;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 應用程式進入點，負責初始化環境並啟動主視窗。
    /// </summary>
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// 初始化視覺樣式、文字渲染相容性、JTextFileLib 單例，
        /// 然後啟動主表單 FormTextSpeedReader。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 啟用 Windows 視覺樣式（現代化外觀）
            Application.EnableVisualStyles();
            // 設定文字渲染與 GDI 相容，避免部分控制項顯示異常
            Application.SetCompatibleTextRenderingDefault(false);
            // 初始化 JTextFileLib 單例（檔案讀寫工具）
            JTextFileLib.Instance();
            // 啟動主視窗
            Application.Run(new FormTextSpeedReader());
        }
    }
}

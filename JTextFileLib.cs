using System;
using System.IO;
using System.Text;
using Ude;

namespace TextSpeedReader
{
    /// <summary>
    /// 文字檔案讀寫工具庫，採用單例模式（Singleton）。
    /// 提供自動偵測編碼的檔案讀取，以及將字串寫入檔案的功能。
    /// 使用 UDE（Universal Charset Detector）函式庫自動判斷檔案編碼。
    /// </summary>
    public class JTextFileLib
    {
        // 單例執行個體
        public static JTextFileLib? instance = null;

        /// <summary>
        /// 取得 JTextFileLib 的唯一執行個體（Lazy Initialization 單例）。
        /// </summary>
        public static JTextFileLib Instance()
        {
            if (instance == null)
                instance = new JTextFileLib();
            return instance;
        }

        // 常用的換行符號常數（供內部使用）
        private readonly char Char10LF = Convert.ToChar(10);   // LF  (\n)
        private readonly char Char13CR = Convert.ToChar(13);   // CR  (\r)
        private readonly string Str13CR = Convert.ToChar(13).ToString();          // "\r"
        private readonly string Str13_10CRLF = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); // "\r\n"

        /// <summary>
        /// 讀取文字檔案內容到字串。
        /// 會自動偵測檔案編碼（UTF-8、Big5、GBK 等），並選擇對應的解碼器。
        /// </summary>
        /// <param name="fileName">要讀取的檔案完整路徑。</param>
        /// <param name="textString">輸出參數：讀取到的文字內容。</param>
        /// <param name="isLF2CRLF">若為 true，會將純 LF (\n) 換行符號轉換為 CRLF (\r\n)。</param>
        /// <returns>讀取成功回傳 true；發生 IO 例外回傳 false。</returns>
        public bool ReadTxtFile(string fileName, ref string textString, bool isLF2CRLF)
        {
            try
            {
                // 先偵測編碼，再以正確編碼開啟 StreamReader，解決中文亂碼問題
                Encoding encoding = DetectEncoding(fileName);
                using (StreamReader str = new StreamReader(fileName, encoding))
                {
                    string tmpString = str.ReadToEnd();
                    // 若需要，將純 LF 換行符號轉換為 Windows 的 CRLF 格式
                    if (isLF2CRLF)
                        tmpString = ChangeLF2CRLF(tmpString);
                    textString = tmpString;
                }
                Console.WriteLine(fileName + " 檔案讀取完成");
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(fileName + " 檔案無法讀取: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 自動偵測文字檔案的字元編碼。
        /// 使用 UDE（Universal Charset Detector）分析檔案內容。
        /// 特殊處理：UDE 回報 windows-1252 時，實際上通常是 Big5（繁體中文）。
        /// 若無法判斷，預設回傳 Big5 編碼（適用繁體中文環境）。
        /// </summary>
        /// <param name="fileName">要偵測編碼的檔案完整路徑。</param>
        /// <returns>偵測到的 Encoding 物件；偵測失敗時回傳 Big5。</returns>
        public static Encoding DetectEncoding(string fileName)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                CharsetDetector detector = new CharsetDetector();
                detector.Feed(fs);
                detector.DataEnd();
                Console.WriteLine("detector.Charset: " + detector.Charset);

                if (detector.Charset != null)
                {
                    // UDE 在繁體中文 Big5 檔案上，有時會誤判為 windows-1252，
                    // 這裡強制修正為 Big5
                    if (detector.Charset == "windows-1252")
                        return Encoding.GetEncoding("Big5");
                    return Encoding.GetEncoding(detector.Charset);
                }
                else
                {
                    // 無法判斷時，預設使用 Big5（繁體中文 ANSI）
                    return Encoding.GetEncoding("Big5");
                }
            }
        }

        /// <summary>
        /// 將字串中的純 LF (\n) 換行符號轉換為 CRLF (\r\n)。
        /// 僅對尚未是 CRLF 格式的換行符號進行轉換，不會重複插入 \r。
        /// 主要用於從 Linux/Mac 格式的檔案讀入後，在 Windows RichTextBox 中正確顯示。
        /// </summary>
        /// <param name="inputString">包含純 LF 換行的字串。</param>
        /// <returns>將 LF 替換為 CRLF 後的字串。</returns>
        public string ChangeLF2CRLF(string inputString)
        {
            Console.WriteLine("開始 ChangeLF2CRLF()");
            int countPos = 0;
            int countNewPos;
            int countTime = 0;
            while (countPos < inputString.Length)
            {
                countNewPos = inputString.IndexOf(Char10LF, countPos);
                if (countNewPos == -1)
                    break;
                Console.WriteLine(countTime++.ToString() + " " + countNewPos);
                // 若前一個字元不是 \r，才插入 \r（避免 \r\n 被重複轉換）
                if (countNewPos > 0 && inputString.Substring(countNewPos - 1, 2) != Str13_10CRLF)
                    inputString = inputString.Insert(countNewPos, Str13CR);
                countPos = countNewPos + 1;
            }
            Console.WriteLine("完成 ChangeLF2CRLF()");
            return inputString;
        }

        /// <summary>
        /// 將字串內容寫入文字檔案。
        /// 僅支援覆蓋模式（isAppend=false）；附加模式（isAppend=true）會寫入同一路徑。
        /// </summary>
        /// <param name="fileName">要寫入的檔案完整路徑。</param>
        /// <param name="textString">要寫入的文字內容。</param>
        /// <param name="isAppend">false 表示覆蓋原有內容；true 表示附加到檔案末尾。</param>
        /// <returns>寫入成功回傳 true；發生 IO 例外回傳 false。</returns>
        public bool SaveTxtFile(string fileName, string textString, bool isAppend)
        {
            try
            {
                if (!isAppend)
                {
                    // 覆蓋模式：直接以 UTF-8 寫入（File.WriteAllText 預設 UTF-8）
                    File.WriteAllText(fileName, textString);
                }
                else
                {
                    // 附加模式：在檔案末尾新增一行內容
                    using (StreamWriter file = new StreamWriter(fileName, append: true))
                    {
                        file.WriteLine(textString);
                    }
                }
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(fileName + " 檔案無法寫入: " + e.Message);
                return false;
            }
        }
    }
}

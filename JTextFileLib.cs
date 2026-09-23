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
        /// 判斷順序：
        ///   1. BOM（UTF-8 / UTF-16 / UTF-32）——最可靠，優先採用。
        ///   2. 全檔是否為合法的 UTF-8 位元組序列——是則判定 UTF-8（無 BOM）。
        ///      這一步是必要的：UDE 對無 BOM 的 UTF-8 中文檔常誤判成 Big5，
        ///      一旦以 Big5 讀入就會亂碼，再存檔回去原本的 UTF-8 檔案就真的被毀掉了。
        ///   3. UDE（Universal Charset Detector）分析內容。
        ///      特殊處理：UDE 回報 windows-1252 時，實際上通常是 Big5（繁體中文）。
        ///   4. 仍無法判斷時，預設回傳 Big5 編碼（適用繁體中文環境）。
        /// </summary>
        /// <param name="fileName">要偵測編碼的檔案完整路徑。</param>
        /// <returns>偵測到的 Encoding 物件；偵測失敗時回傳 Big5。</returns>
        public static Encoding DetectEncoding(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            return DetectEncoding(bytes);
        }

        /// <summary>
        /// 由位元組內容偵測字元編碼（邏輯同 DetectEncoding(string)）。
        /// </summary>
        public static Encoding DetectEncoding(byte[] bytes)
        {
            // 1. BOM 優先判斷
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
                return new UTF8Encoding(true);
            if (bytes.Length >= 4 && bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00)
                return new UTF32Encoding(false, true);
            if (bytes.Length >= 4 && bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF)
                return new UTF32Encoding(true, true);
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
                return new UnicodeEncoding(false, true);
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
                return new UnicodeEncoding(true, true);

            // 2. 沒有 BOM，但整份內容都是合法的 UTF-8 → 直接判定 UTF-8
            if (IsValidUtf8(bytes))
                return new UTF8Encoding(false);

            // 3. 交給 UDE 判斷
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                CharsetDetector detector = new CharsetDetector();
                detector.Feed(ms);
                detector.DataEnd();
                Console.WriteLine("detector.Charset: " + detector.Charset);

                if (detector.Charset != null)
                {
                    // UDE 在繁體中文 Big5 檔案上，有時會誤判為 windows-1252，
                    // 這裡強制修正為 Big5
                    if (detector.Charset == "windows-1252")
                        return Encoding.GetEncoding("Big5");
                    try
                    {
                        return Encoding.GetEncoding(detector.Charset);
                    }
                    catch (ArgumentException)
                    {
                        // 取不到對應的 code page，退回 Big5
                        return Encoding.GetEncoding("Big5");
                    }
                }
                else
                {
                    // 4. 無法判斷時，預設使用 Big5（繁體中文 ANSI）
                    return Encoding.GetEncoding("Big5");
                }
            }
        }

        /// <summary>
        /// 檢查位元組序列是否為合法的 UTF-8（純 ASCII 也算合法）。
        /// </summary>
        private static bool IsValidUtf8(byte[] bytes)
        {
            try
            {
                // throwOnInvalidBytes: 遇到非法序列會丟例外，藉此判斷是否為合法 UTF-8
                new UTF8Encoding(false, true).GetString(bytes);
                return true;
            }
            catch (DecoderFallbackException)
            {
                return false;
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
            // 未指定編碼時沿用舊行為：UTF-8（不含 BOM）
            return SaveTxtFile(fileName, textString, isAppend, new UTF8Encoding(false));
        }

        /// <summary>
        /// 將字串內容以指定編碼寫入文字檔案。
        /// 注意：本方法不會改動 textString 中的換行符號，呼叫端必須自行
        /// 用 TextFileFormat.ConvertLineEndings() 決定要寫出的換行格式。
        /// </summary>
        /// <param name="fileName">要寫入的檔案完整路徑。</param>
        /// <param name="textString">要寫入的文字內容。</param>
        /// <param name="isAppend">false 表示覆蓋原有內容；true 表示附加到檔案末尾。</param>
        /// <param name="encoding">寫檔使用的編碼（是否輸出 BOM 由此 Encoding 實體決定）。</param>
        /// <returns>寫入成功回傳 true；發生 IO 例外回傳 false。</returns>
        public bool SaveTxtFile(string fileName, string textString, bool isAppend, Encoding encoding)
        {
            try
            {
                if (!isAppend)
                {
                    // 覆蓋模式：以指定編碼寫入，保留原檔的編碼與 BOM 狀態
                    File.WriteAllText(fileName, textString, encoding);
                }
                else
                {
                    // 附加模式：在檔案末尾新增一行內容
                    using (StreamWriter file = new StreamWriter(fileName, append: true, encoding))
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

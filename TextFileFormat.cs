using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 換行符號種類。
    /// </summary>
    public enum LineEndingKind
    {
        /// <summary>尚未偵測 / 無法判斷。</summary>
        Unknown,
        /// <summary>檔案只有一行，沒有任何換行符號。</summary>
        None,
        /// <summary>Windows：\r\n</summary>
        CRLF,
        /// <summary>UNIX / Linux / macOS(新)：\n</summary>
        LF,
        /// <summary>Mac(舊)：\r</summary>
        CR,
        /// <summary>同一檔案內混用多種換行符號。</summary>
        Mixed
    }

    /// <summary>
    /// 使用者在「存檔前格式確認」對話框中的選擇。
    /// </summary>
    public enum SaveFormatChoice
    {
        /// <summary>取消存檔。</summary>
        Cancel,
        /// <summary>轉換成 UTF-8 + Windows CRLF 後存檔。</summary>
        ConvertToUtf8CrLf,
        /// <summary>維持原本的編碼與換行格式存檔。</summary>
        KeepOriginal
    }

    /// <summary>
    /// 存檔前的格式檢查結果：已經處理好的內容、要使用的編碼，以及存檔後的格式。
    /// </summary>
    public sealed class SaveFormatResult
    {
        /// <summary>是否要繼續存檔（使用者按「取消」時為 false）。</summary>
        public bool Proceed { get; set; }
        /// <summary>已依目標換行格式轉換完成的文字內容。</summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>實際寫檔時要使用的 Encoding（已含是否輸出 BOM 的設定）。</summary>
        public Encoding Encoding { get; set; } = new UTF8Encoding(false);
        /// <summary>存檔完成後，這個檔案的格式（用於更新狀態列記錄）。</summary>
        public TextFileFormat Format { get; set; } = TextFileFormat.CreateUtf8CrLf();
        /// <summary>使用者是否選擇了「轉換成 UTF-8 + CRLF」。</summary>
        public bool Converted { get; set; }
    }

    /// <summary>
    /// 描述一個文字檔的「編碼」與「換行符號格式」，並提供偵測、轉換、
    /// 以及存檔前的格式確認對話框。
    /// </summary>
    public sealed class TextFileFormat
    {
        /// <summary>檔案的字元編碼。</summary>
        public Encoding Encoding { get; private set; }

        /// <summary>檔案開頭是否含有 BOM（位元組順序記號）。</summary>
        public bool HasBom { get; private set; }

        /// <summary>檔案使用的換行符號種類。</summary>
        public LineEndingKind LineEnding { get; set; }

        public TextFileFormat(Encoding encoding, bool hasBom, LineEndingKind lineEnding)
        {
            Encoding = encoding ?? new UTF8Encoding(false);
            HasBom = hasBom;
            LineEnding = lineEnding;
        }

        /// <summary>
        /// 產生本程式的標準格式：UTF-8（不含 BOM）+ Windows CRLF。
        /// 不加 BOM 的理由：JTextFileLib.DetectEncoding() 已經會先做全檔 UTF-8
        /// 合法性驗證，排在 UDE 之前，無 BOM 的中文檔也不會被誤判成 Big5，
        /// 所以 BOM 沒有必要；而 BOM 反而會干擾 Markdown、程式碼、shell script
        /// 等工具鏈。既有檔案存檔時則沿用原檔的 BOM 狀態，不會擅自加上或拿掉。
        /// </summary>
        public static TextFileFormat CreateUtf8CrLf()
        {
            return new TextFileFormat(new UTF8Encoding(false), false, LineEndingKind.CRLF);
        }

        /// <summary>編碼是否與 UTF-8 相容（UTF-8 本身，或純 ASCII——位元組完全相同）。</summary>
        public bool IsUtf8Compatible
        {
            get { return Encoding.CodePage == 65001 || Encoding.CodePage == 20127; }
        }

        /// <summary>換行格式是否為 Windows CRLF（單行檔案沒有換行符號，視為相容）。</summary>
        public bool IsWindowsCrLf
        {
            get { return LineEnding == LineEndingKind.CRLF || LineEnding == LineEndingKind.None; }
        }

        /// <summary>是否已經是「UTF-8 + Windows CRLF」的標準格式。</summary>
        public bool IsStandard
        {
            get { return IsUtf8Compatible && IsWindowsCrLf; }
        }

        /// <summary>供狀態列顯示的編碼名稱。</summary>
        public string EncodingDisplayName
        {
            get
            {
                switch (Encoding.CodePage)
                {
                    case 65001: return HasBom ? "UTF-8 BOM" : "UTF-8";
                    case 1200: return HasBom ? "UTF-16 LE BOM" : "UTF-16 LE";
                    case 1201: return HasBom ? "UTF-16 BE BOM" : "UTF-16 BE";
                    case 12000: return "UTF-32 LE";
                    case 12001: return "UTF-32 BE";
                    case 950: return "ANSI (Big5)";
                    case 936: return "ANSI (GBK)";
                    case 932: return "ANSI (Shift-JIS)";
                    case 949: return "ANSI (EUC-KR)";
                    case 1252: return "ANSI (Windows-1252)";
                    case 20127: return "ASCII";
                    default: return Encoding.WebName.ToUpperInvariant();
                }
            }
        }

        /// <summary>供狀態列顯示的換行符號名稱。</summary>
        public string LineEndingDisplayName
        {
            get { return GetLineEndingDisplayName(LineEnding); }
        }

        /// <summary>取得指定換行種類的顯示名稱。</summary>
        public static string GetLineEndingDisplayName(LineEndingKind kind)
        {
            switch (kind)
            {
                case LineEndingKind.CRLF: return "Windows (CRLF)";
                case LineEndingKind.LF: return "UNIX (LF)";
                case LineEndingKind.CR: return "Mac (CR)";
                case LineEndingKind.Mixed: return "混合換行";
                case LineEndingKind.None: return "無換行";
                default: return "－";
            }
        }

        /// <summary>
        /// 取得寫檔時要用的 Encoding 實體。
        /// Encoding.GetEncoding() 取得的 Unicode 編碼預設會輸出 BOM，
        /// 這裡依照 HasBom 重新建立，確保「原本沒有 BOM 就不要多加 BOM」。
        /// </summary>
        public Encoding GetWriteEncoding()
        {
            switch (Encoding.CodePage)
            {
                case 65001: return new UTF8Encoding(HasBom);
                case 1200: return new UnicodeEncoding(false, HasBom);
                case 1201: return new UnicodeEncoding(true, HasBom);
                case 12000: return new UTF32Encoding(false, HasBom);
                case 12001: return new UTF32Encoding(true, HasBom);
                default: return Encoding;
            }
        }

        /// <summary>
        /// 偵測檔案的編碼與換行格式。
        /// </summary>
        /// <param name="filePath">檔案完整路徑。</param>
        /// <param name="decodedText">
        /// 已經解碼完成、且「未經 LF→CRLF 轉換」的檔案內容。
        /// 傳入可省下一次讀檔；傳 null 則由本方法自行讀取。
        /// </param>
        public static TextFileFormat Detect(string filePath, string? decodedText)
        {
            Encoding encoding;
            bool hasBom = false;
            try
            {
                encoding = JTextFileLib.DetectEncoding(filePath);
                hasBom = HasByteOrderMark(filePath);
            }
            catch
            {
                encoding = new UTF8Encoding(false);
            }

            LineEndingKind lineEnding;
            try
            {
                string text = decodedText ?? File.ReadAllText(filePath, encoding);
                lineEnding = DetectLineEnding(text);
            }
            catch
            {
                lineEnding = LineEndingKind.Unknown;
            }

            return new TextFileFormat(encoding, hasBom, lineEnding);
        }

        /// <summary>檢查檔案開頭是否有 BOM。</summary>
        public static bool HasByteOrderMark(string filePath)
        {
            byte[] head = new byte[4];
            int read;
            using (FileStream fs = File.OpenRead(filePath))
            {
                read = fs.Read(head, 0, 4);
            }
            if (read >= 3 && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF) return true;   // UTF-8
            if (read >= 4 && head[0] == 0xFF && head[1] == 0xFE && head[2] == 0x00 && head[3] == 0x00) return true; // UTF-32 LE
            if (read >= 4 && head[0] == 0x00 && head[1] == 0x00 && head[2] == 0xFE && head[3] == 0xFF) return true; // UTF-32 BE
            if (read >= 2 && head[0] == 0xFF && head[1] == 0xFE) return true;                      // UTF-16 LE
            if (read >= 2 && head[0] == 0xFE && head[1] == 0xFF) return true;                      // UTF-16 BE
            return false;
        }

        /// <summary>
        /// 分析字串中使用的換行符號。同時出現兩種以上時回傳 Mixed。
        /// </summary>
        public static LineEndingKind DetectLineEnding(string text)
        {
            if (string.IsNullOrEmpty(text))
                return LineEndingKind.None;

            bool hasCrLf = false, hasLf = false, hasCr = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\r')
                {
                    if (i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        hasCrLf = true;
                        i++; // 跳過 \n，避免重複計算
                    }
                    else
                    {
                        hasCr = true;
                    }
                }
                else if (c == '\n')
                {
                    hasLf = true;
                }
            }

            int kinds = (hasCrLf ? 1 : 0) + (hasLf ? 1 : 0) + (hasCr ? 1 : 0);
            if (kinds == 0) return LineEndingKind.None;
            if (kinds > 1) return LineEndingKind.Mixed;
            if (hasCrLf) return LineEndingKind.CRLF;
            if (hasLf) return LineEndingKind.LF;
            return LineEndingKind.CR;
        }

        /// <summary>
        /// 把字串中的換行符號統一轉換成指定格式。
        /// 注意：RichTextBox.Text 取回的文字只會有 \n（控制項內部會把 \r\n 正規化掉），
        /// 所以寫檔前一定要用這個方法把換行符號補回來，否則 Windows CRLF 檔案會被存成 UNIX LF。
        /// </summary>
        public static string ConvertLineEndings(string text, LineEndingKind target)
        {
            if (string.IsNullOrEmpty(text))
                return text ?? string.Empty;

            // 先全部正規化成 \n，再展開成目標格式
            string normalized = text.Replace("\r\n", "\n").Replace("\r", "\n");

            switch (target)
            {
                case LineEndingKind.LF:
                    return normalized;
                case LineEndingKind.CR:
                    return normalized.Replace("\n", "\r");
                default:
                    // CRLF / None / Mixed / Unknown 一律輸出 Windows CRLF
                    // （Mixed 與 CR/CRLF 的逐行差異在載入 RichTextBox 時就已經遺失，無法還原）
                    return normalized.Replace("\n", "\r\n");
            }
        }

        /// <summary>把字串轉成 Windows CRLF 換行。</summary>
        public static string ToCrLf(string text)
        {
            return ConvertLineEndings(text, LineEndingKind.CRLF);
        }

        /// <summary>
        /// 存檔前的共用檢查：若目前格式不是「UTF-8 + Windows CRLF」就跳出提醒視窗，
        /// 讓使用者決定要轉換、維持原格式、還是取消存檔；
        /// 已經是標準格式時不會打擾使用者，直接回傳可存檔的結果。
        /// </summary>
        /// <param name="owner">對話框的擁有視窗。</param>
        /// <param name="fileNameForMessage">顯示在訊息中的檔名。</param>
        /// <param name="rawContent">要寫入的原始文字（通常來自 RichTextBox.Text，換行只有 \n）。</param>
        /// <param name="currentFormat">檔案目前的格式；null 表示新檔案，視為標準格式。</param>
        public static SaveFormatResult PrepareContentForSave(
            IWin32Window? owner,
            string fileNameForMessage,
            string rawContent,
            TextFileFormat? currentFormat)
        {
            TextFileFormat format = currentFormat ?? CreateUtf8CrLf();
            SaveFormatResult result = new SaveFormatResult();

            // 已經是 UTF-8 + CRLF：不打擾使用者，直接依原格式輸出
            if (format.IsStandard)
            {
                result.Proceed = true;
                result.Content = ConvertLineEndings(rawContent, format.LineEnding);
                result.Encoding = format.GetWriteEncoding();
                result.Format = new TextFileFormat(format.Encoding, format.HasBom,
                    DetectLineEnding(result.Content));
                return result;
            }

            string message =
                $"檔案「{fileNameForMessage}」目前的格式不是建議的「UTF-8 + Windows CRLF」：\r\n\r\n" +
                $"　　編碼：{format.EncodingDisplayName}\r\n" +
                $"　　換行：{format.LineEndingDisplayName}\r\n\r\n" +
                "是否要轉換成「UTF-8 (不含 BOM) + Windows CRLF」後再儲存？\r\n\r\n" +
                "　【是】　轉換成 UTF-8 + Windows CRLF 後儲存\r\n" +
                "　【否】　維持原本的編碼與換行格式儲存\r\n" +
                "　【取消】不要儲存";

            if (format.LineEnding == LineEndingKind.Mixed)
            {
                message += "\r\n\r\n（註：原檔混用多種換行符號，選「否」時會一律以 Windows CRLF 寫出。）";
            }

            DialogResult dr = owner != null
                ? MessageBox.Show(owner, message, "存檔格式確認",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
                : MessageBox.Show(message, "存檔格式確認",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (dr == DialogResult.Cancel)
            {
                result.Proceed = false;
                return result;
            }

            result.Proceed = true;
            if (dr == DialogResult.Yes)
            {
                TextFileFormat target = CreateUtf8CrLf();
                result.Converted = true;
                result.Content = ConvertLineEndings(rawContent, LineEndingKind.CRLF);
                result.Encoding = target.GetWriteEncoding();
                result.Format = new TextFileFormat(target.Encoding, target.HasBom,
                    DetectLineEnding(result.Content));
            }
            else
            {
                result.Content = ConvertLineEndings(rawContent, format.LineEnding);
                result.Encoding = format.GetWriteEncoding();
                result.Format = new TextFileFormat(format.Encoding, format.HasBom,
                    DetectLineEnding(result.Content));
            }
            return result;
        }
    }
}

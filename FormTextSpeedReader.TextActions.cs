using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSpeedReader
{
    public partial class FormTextSpeedReader
    {
        // 增加字體大小
        private void FontSizeAdd(object sender, EventArgs e)
        {
            if (richTextBoxText.Visible)
            {
                Font tmpFont = m_Font;
                float tmpFontSize = m_Font.Size;
                // 在預設字體大小列表中尋找下一個較大的尺寸
                for (int i = 0; i < m_FontSize.Length; i++)
                {
                    if (m_FontSize[i] > tmpFontSize)
                    {
                        tmpFontSize = m_FontSize[i];
                        break;
                    }
                }
                // 套用新字體大小
                Font newFont = new Font(tmpFont.FontFamily, tmpFontSize);
                richTextBoxText.Font = newFont;
                m_Font = newFont;
            }
            else if (webBrowser1.Visible)
            {
                //webbrowser 無法設定字體大小，僅能以縮放比例功能來達成
                // 在預設縮放比例列表中尋找下一個較大的比例
                for (int i = 0; i < m_WebBrowserSize.Length; i++)
                {
                    if (m_WebBrowserSize[i] > m_WebBrowserZoom)
                    {
                        m_WebBrowserZoom = m_WebBrowserSize[i];
                        break;
                    }
                }
                // 套用新縮放比例
                if (webBrowser1.Document?.Body != null)
                {
                    //webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
                    ApplyWebBrowserFontStyle(m_WebBrowserZoom.ToString());
                }

            }
        }

        // 減少字體大小按鈕點擊事件
        private void FontSizeReduce(object sender, EventArgs e)
        {
            if (richTextBoxText.Visible)
            {
                Font tmpFont = m_Font;
                float tmpFontSize = m_Font.Size;
                // 在預設字體大小列表中尋找下一個較小的尺寸
                for (int i = m_FontSize.Length - 1; i >= 0; i--)
                {
                    if (m_FontSize[i] < tmpFontSize)
                    {
                        tmpFontSize = m_FontSize[i];
                        break;
                    }
                }
                // 套用新字體大小
                Font newFont = new Font(tmpFont.FontFamily, tmpFontSize);
                richTextBoxText.Font = newFont;
                m_Font = newFont;
            }
            else if (webBrowser1.Visible)
            {
                //webbrowser 無法設定字體大小，僅能以縮放比例功能來達成
                // 在預設縮放比例列表中尋找下一個較小的比例
                for (int i = m_WebBrowserSize.Length - 1; i >= 0; i--)
                {
                    if (m_WebBrowserSize[i] < m_WebBrowserZoom)
                    {
                        m_WebBrowserZoom = m_WebBrowserSize[i];
                        break;
                    }
                }
                // 套用新縮放比例
                if (webBrowser1.Document?.Body != null)
                {
                    //webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
                    ApplyWebBrowserFontStyle(m_WebBrowserZoom.ToString());
                }
            }
        }
        // 變更文字字體
        private void ChangeFont(object sender, EventArgs e)
        {
            int count = m_FontFamilies.Length;
            // 尋找選擇的字體並套用
            object? selectedItem = toolStripComboBoxFonts.SelectedItem;
            if (selectedItem == null)
                return;

            for (int j = 0; j < count; ++j)
            {
                if (m_FontFamilies[j] != null && m_FontFamilies[j].Name == (string)selectedItem)
                {
                    Font newFont = new Font(m_FontFamilies[j], richTextBoxText.Font.Size, richTextBoxText.Font.Style);
                    richTextBoxText.Font = newFont;
                }
            }
        }



        // 自動移除目前文章中多餘的斷行
        private void AutoRemoveCR()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;
            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }
            if (string.IsNullOrEmpty(text))
                return;
            //如果選擇的字串最後一個字是斷行或分行符號就少選一個字元
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
                if (!processWholeDocument) richTextBoxText.SelectionLength -= 2;
                //MessageBox.Show("選取內容最後有斷行與新行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (!processWholeDocument && richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
                //MessageBox.Show("選取內容最後有斷行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (!processWholeDocument && richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
                //MessageBox.Show("選取內容最後有新行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            int textLength = text.Length;
            int currentPos = 0;
            StringBuilder result = new StringBuilder();
            while (currentPos < textLength)
            {
                int paragraphStart = currentPos;
                int paragraphEnd = currentPos;
                int doubleBreakStart = -1;
                // 搜尋當前段落結束（兩個連續斷行符號）
                while (paragraphEnd < textLength)
                {
                    if (text[paragraphEnd] == '\r' || text[paragraphEnd] == '\n')
                    {
                        int firstBreakEnd = paragraphEnd + 1;
                        if (paragraphEnd < textLength - 1 && text[paragraphEnd] == '\r' && text[paragraphEnd + 1] == '\n')
                            firstBreakEnd = paragraphEnd + 2;
                        if (firstBreakEnd < textLength && (text[firstBreakEnd] == '\r' || text[firstBreakEnd] == '\n'))
                        {
                            doubleBreakStart = paragraphEnd;
                            break;
                        }
                    }
                    paragraphEnd++;
                }
                // 沒找到則延伸到結尾
                if (doubleBreakStart == -1)
                    paragraphEnd = textLength;
                else
                    paragraphEnd = doubleBreakStart;
                if (paragraphEnd > paragraphStart)
                {
                    string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);
                    // 移除斷行合併
                    string mergedLine = "";
                    mergedLine = paragraph.Replace("\r", "").Replace("\n", "");
                    result.Append(mergedLine);
                }
                // 處理段落分隔符
                if (doubleBreakStart >= 0)
                {
                    // 包含分隔斷行符們
                    int sepEnd = doubleBreakStart;
                    while (sepEnd < textLength && (text[sepEnd] == '\r' || text[sepEnd] == '\n'))
                        sepEnd++;
                    result.Append(text.Substring(doubleBreakStart, sepEnd - doubleBreakStart));
                    currentPos = sepEnd;
                }
                else
                {
                    currentPos = paragraphEnd;
                }
            }
            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }



        // 自動移除目前文章中多餘的斷行，不包含該行最後一個字是句點或驚嘆號的行
        private void AutoRemoveCRWithoutDotAndExclamationMark()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;
            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }
            if (string.IsNullOrEmpty(text))
                return;
            //如果選擇的字串最後一個字是斷行或分行符號就少選一個字元
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
                if (!processWholeDocument) richTextBoxText.SelectionLength -= 2;
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (!processWholeDocument && richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (!processWholeDocument && richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
            }
            int textLength = text.Length;
            int currentPos = 0;
            StringBuilder result = new StringBuilder();
            while (currentPos < textLength)
            {
                int paragraphStart = currentPos;
                int paragraphEnd = currentPos;
                int doubleBreakStart = -1;
                // 搜尋當前段落結束（兩個連續斷行符號）
                while (paragraphEnd < textLength)
                {
                    if (text[paragraphEnd] == '\r' || text[paragraphEnd] == '\n')
                    {
                        int firstBreakEnd = paragraphEnd + 1;
                        if (paragraphEnd < textLength - 1 && text[paragraphEnd] == '\r' && text[paragraphEnd + 1] == '\n')
                            firstBreakEnd = paragraphEnd + 2;
                        if (firstBreakEnd < textLength && (text[firstBreakEnd] == '\r' || text[firstBreakEnd] == '\n'))
                        {
                            doubleBreakStart = paragraphEnd;
                            break;
                        }
                    }
                    paragraphEnd++;
                }
                // 沒找到則延伸到結尾
                if (doubleBreakStart == -1)
                    paragraphEnd = textLength;
                else
                    paragraphEnd = doubleBreakStart;
                if (paragraphEnd > paragraphStart)
                {
                    string paragraph = text.Substring(paragraphStart, paragraphEnd - paragraphStart);
                    // 處理段落內的斷行，移除斷行但保留以句點或驚嘆號結尾的行的斷行
                    StringBuilder mergedParagraph = new StringBuilder();
                    int lineStart = 0;
                    while (lineStart < paragraph.Length)
                    {
                        int lineBreakPos = paragraph.IndexOfAny(new char[] { '\r', '\n' }, lineStart);
                        if (lineBreakPos == -1) lineBreakPos = paragraph.Length;
                        string line = paragraph.Substring(lineStart, lineBreakPos - lineStart);
                        // 檢查該行最後一個字是否為句點或驚嘆號
                        char lastChar = line.Length > 0 ? line[line.Length - 1] : '\0';
                        bool shouldKeepLineBreak = (lastChar == '.' || lastChar == '。' || lastChar == '!' || lastChar == '！' || lastChar == '?' || lastChar == '？');
                        // 移除行內的斷行符號，但保留行內容
                        string lineWithoutBreaks = line.Replace("\r", "").Replace("\n", "");
                        mergedParagraph.Append(lineWithoutBreaks);
                        // 如果該行以句點或驚嘆號結尾，保留斷行符號
                        if (shouldKeepLineBreak && lineBreakPos < paragraph.Length)
                        {
                            // 保留斷行符號
                            if (lineBreakPos < paragraph.Length - 1 && paragraph[lineBreakPos] == '\r' && paragraph[lineBreakPos + 1] == '\n')
                            {
                                mergedParagraph.Append("\r\n");
                                lineStart = lineBreakPos + 2;
                            }
                            else if (paragraph[lineBreakPos] == '\r' || paragraph[lineBreakPos] == '\n')
                            {
                                mergedParagraph.Append(paragraph[lineBreakPos]);
                                lineStart = lineBreakPos + 1;
                            }
                            else
                            {
                                lineStart = lineBreakPos;
                            }
                        }
                        else
                        {
                            // 不保留斷行符號，繼續處理下一行
                            lineStart = lineBreakPos;
                            if (lineStart < paragraph.Length)
                            {
                                if (lineStart < paragraph.Length - 1 && paragraph[lineStart] == '\r' && paragraph[lineStart + 1] == '\n')
                                    lineStart += 2;
                                else if (paragraph[lineStart] == '\r' || paragraph[lineStart] == '\n')
                                    lineStart++;
                            }
                        }
                    }
                    result.Append(mergedParagraph.ToString());
                }
                // 處理段落分隔符
                if (doubleBreakStart >= 0)
                {
                    // 包含分隔斷行符們
                    int sepEnd = doubleBreakStart;
                    while (sepEnd < textLength && (text[sepEnd] == '\r' || text[sepEnd] == '\n'))
                        sepEnd++;
                    result.Append(text.Substring(doubleBreakStart, sepEnd - doubleBreakStart));
                    currentPos = sepEnd;
                }
                else
                {
                    currentPos = paragraphEnd;
                }
            }
            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }

        // 自動移除目前文章中多餘的斷行，包含句點與驚嘆號標點符號
        private void AutoRemoveCRWithDotAndExclamationMark()
        {
            string text = richTextBoxText.Text;
            int length = text.Length;
            int start = richTextBoxText.SelectionStart;
            int end = richTextBoxText.SelectionStart + richTextBoxText.SelectionLength;
            while (end < length)
            {
                int lineBreakPos = text.IndexOfAny(new char[] { '\r', '\n' }, end);
                if (lineBreakPos == -1) lineBreakPos = length;
                string line = text.Substring(end, lineBreakPos - end);
                char lastChar = line.Length > 0 ? line[line.Length - 1] : '\0';
                if (lastChar == '.' || lastChar == '。' || lastChar == '!' || lastChar == '！')
                {
                    end = lineBreakPos;
                }
                else
                {
                    end = lineBreakPos;
                }
                end = lineBreakPos;
                // 跳過 \r\n
                if (end < length - 1 && text[end] == '\r' && text[end + 1] == '\n') end += 2;
                else if (text[end] == '\r' || text[end] == '\n') end++;
            }
            richTextBoxText.Select(start, Math.Max(0, end - start));
        }



        // 移除行首和行尾的空白字元
        private void RemoveLeadingAndTrailingSpaces()
        {
            //若該行只有空白字元或TAB字元，請移除該行文字開頭的空白或TAB符號

            // 獲取選定文本
            string selectedText = richTextBoxText.SelectedText;
            if (string.IsNullOrEmpty(selectedText))
            {
                // 如果沒有選定文本，處理整個文檔
                selectedText = richTextBoxText.Text;
                if (string.IsNullOrEmpty(selectedText))
                    return;

                // 處理整個文檔
                ProcessRemoveLeadingSpaces(selectedText, true);
                selectedText = richTextBoxText.Text; //更新選取的文字
                ProcessRemoveEndingSpaces(selectedText, true); //處理整個文檔的行尾空白
            }
            else
            {
                // 處理選定範圍
                ProcessRemoveLeadingSpaces(selectedText, false);
                selectedText = richTextBoxText.SelectedText; //更新選取的文字
                ProcessRemoveEndingSpaces(selectedText, false); //處理選定範圍的行尾空白
            }
        }

        // (1) 移除行首連續的空白/TAB/全形空白；(2) 移除行尾連續的空白/TAB/全形空白
        private void ProcessRemoveLeadingSpaces(string text, bool processWholeDocument)
        {
            if (string.IsNullOrEmpty(text))
                return;

            StringBuilder result = new StringBuilder();
            bool hasChanges = false;
            int textLength = text.Length;
            int i = 0;

            while (i < textLength)
            {
                int lineStart = i;
                bool lineEndedWithBreak = false;

                while (i < textLength)
                {
                    char c = text[i];
                    // 包括半形空白, TAB, 全形空白(U+3000)
                    if (c == '\r' || c == '\n')
                    {
                        lineEndedWithBreak = true;
                        string line = text.Substring(lineStart, i - lineStart);
                        string processedLine = ProcessLineForLead(line, ref hasChanges);
                        result.Append(processedLine);

                        // 處理 \r\n 換行
                        if (c == '\r' && i + 1 < textLength && text[i + 1] == '\n')
                        {
                            result.Append("\r\n");
                            i += 2;
                        }
                        else
                        {
                            result.Append(c);
                            i++;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (i >= textLength && !lineEndedWithBreak && lineStart < textLength)
                {
                    string line = text.Substring(lineStart, textLength - lineStart);
                    string processedLine = ProcessLineForLead(line, ref hasChanges);
                    result.Append(processedLine);
                    break;
                }
            }
            if (hasChanges)
            {
                if (processWholeDocument)
                {
                    SuspendDrawing();
                    richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                    try
                    {
                        int originalSelectionStart = richTextBoxText.SelectionStart;
                        richTextBoxText.SelectAll();
                        richTextBoxText.SelectedText = result.ToString();
                        if (originalSelectionStart < richTextBoxText.Text.Length)
                            richTextBoxText.SelectionStart = originalSelectionStart;
                        else
                            richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                        richTextBoxText.ScrollToCaret();
                    }
                    finally
                    {
                        richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                        richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                        ResumeDrawing();
                        UpdateStatusLabel();
                    }
                }
                else
                {
                    int selStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectedText = result.ToString();
                    int newLength = result.Length;
                    richTextBoxText.Select(selStart, newLength);
                }
            }
        }

        private string ProcessLineForLead(string line, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            var trimmedLine = RemoveLeadingFullWhitespace(line);
            if (!object.ReferenceEquals(line, trimmedLine))
                hasChanges = true;
            return trimmedLine;
        }
        private string RemoveLeadingFullWhitespace(string line)
        {
            int i = 0;
            while (i < line.Length && (line[i] == ' ' || line[i] == '\t' || line[i] == '\u3000'))
            {
                i++;
            }
            return line.Substring(i);
        }

        private void ProcessRemoveEndingSpaces(string text, bool processWholeDocument)
        {
            if (string.IsNullOrEmpty(text))
                return;
            StringBuilder result = new StringBuilder();
            bool hasChanges = false;
            int textLength = text.Length;
            int i = 0;

            while (i < textLength)
            {
                int lineStart = i;
                bool lineEndedWithBreak = false;
                while (i < textLength)
                {
                    char c = text[i];
                    if (c == '\r' || c == '\n')
                    {
                        lineEndedWithBreak = true;
                        string line = text.Substring(lineStart, i - lineStart);
                        string processedLine = RemoveTrailingSpacesFull(line, ref hasChanges);
                        result.Append(processedLine);
                        if (c == '\r' && i + 1 < textLength && text[i + 1] == '\n')
                        {
                            result.Append("\r\n");
                            i += 2;
                        }
                        else
                        {
                            result.Append(c);
                            i++;
                        }
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (i >= textLength && !lineEndedWithBreak && lineStart < textLength)
                {
                    string line = text.Substring(lineStart, textLength - lineStart);
                    string processedLine = RemoveTrailingSpacesFull(line, ref hasChanges);
                    result.Append(processedLine);
                    break;
                }
            }
            if (hasChanges)
            {
                if (processWholeDocument)
                {
                    SuspendDrawing();
                    richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                    try
                    {
                        int originalSelectionStart = richTextBoxText.SelectionStart;
                        richTextBoxText.SelectAll();
                        richTextBoxText.SelectedText = result.ToString();
                        if (originalSelectionStart < richTextBoxText.Text.Length)
                            richTextBoxText.SelectionStart = originalSelectionStart;
                        else
                            richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                        richTextBoxText.ScrollToCaret();
                    }
                    finally
                    {
                        richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                        richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                        ResumeDrawing();
                        UpdateStatusLabel();
                    }
                }
                else
                {
                    int selStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectedText = result.ToString();
                    int newLength = result.Length;
                    richTextBoxText.Select(selStart, newLength);
                }
            }
        }
        private string RemoveTrailingSpacesFull(string line, ref bool hasChanges)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            int lastIndex = line.Length - 1;
            while (lastIndex >= 0 && (line[lastIndex] == ' ' || line[lastIndex] == '\t' || line[lastIndex] == '\u3000'))
            {
                lastIndex--;
            }
            if (lastIndex < line.Length - 1)
            {
                hasChanges = true;
                return line.Substring(0, lastIndex + 1);
            }
            return line;
        }

        private void KeepTwoCRBetweenLines()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;
            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }
            if (string.IsNullOrEmpty(text))
                return;

            // 識別換行符類型（優先使用 \r\n，然後是 \n，最後是 \r）
            string lineBreak = "\r\n";
            if (!text.Contains("\r\n"))
            {
                if (text.Contains("\n"))
                    lineBreak = "\n";
                else if (text.Contains("\r"))
                    lineBreak = "\r";
            }

            // 將所有換行符統一為標準格式以便處理
            string normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = normalizedText.Split('\n');

            StringBuilder result = new StringBuilder();

            // 收集所有非空行的索引
            List<int> nonEmptyLineIndices = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    nonEmptyLineIndices.Add(i);
                }
            }

            if (nonEmptyLineIndices.Count == 0)
            {
                // 如果沒有非空行，返回空字符串
                result.Append("");
            }
            else
            {
                // 處理每一對相鄰的非空行
                for (int idx = 0; idx < nonEmptyLineIndices.Count; idx++)
                {
                    int lineIndex = nonEmptyLineIndices[idx];

                    // 添加非空行內容
                    result.Append(lines[lineIndex]);

                    // 如果不是最後一個非空行，檢查與下一個非空行之間的空行數
                    if (idx < nonEmptyLineIndices.Count - 1)
                    {
                        int nextLineIndex = nonEmptyLineIndices[idx + 1];
                        int emptyLinesBetween = nextLineIndex - lineIndex - 1;

                        if (emptyLinesBetween == 0)
                        {
                            // 兩行之間沒有空白行，添加一個空白行
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                        else if (emptyLinesBetween == 1)
                        {
                            // 兩行之間已經有一個空白行，保留它
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                        else
                        {
                            // 兩行之間有多個空白行，只保留一個
                            result.Append(lineBreak);
                            result.Append(lineBreak);
                        }
                    }
                }
            }

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }

        private void EndingAddDot()
        {
            // 檢查每行結尾不是 "，。！？」… "符號(包括全形與半形字元)，請在行尾加上"。"(全形)

            // 獲取選定文本或整個文檔
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            // 定義不需要添加句號的結尾符號（包括全形與半形）
            char[] punctuationMarks = new char[]
            {
                ',', '，', '.', '。', '!', '！', '?', '？', '"', '"', '」', '…', ' ', '\u3000', '\t'
            };

            // 分割成行並處理每一行
            string[] lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 如果不是最後一行或不是空行，檢查行結尾
                if (!string.IsNullOrEmpty(line))
                {
                    // 檢查行結尾是否已經有不需要的符號
                    bool endsWithPunctuation = false;
                    if (line.Length > 0)
                    {
                        char lastChar = line[line.Length - 1];
                        endsWithPunctuation = punctuationMarks.Contains(lastChar);
                    }

                    // 如果沒有結尾符號，添加全形句號
                    if (!endsWithPunctuation)
                    {
                        line += "。";
                    }
                }

                // 添加行到結果
                result.Append(line);

                // 添加換行符（除了最後一行）
                if (i < lines.Length - 1)
                {
                    result.Append("\r\n");
                }
            }

            // 更新文本
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    // 處理整個文檔
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    richTextBoxText.SelectionStart = selectionStart;
                    richTextBoxText.SelectionLength = 0;
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 處理選定範圍
                richTextBoxText.SelectedText = result.ToString();
                richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.SelectionLength = result.Length;
            }

            // 滾動到游標位置
            richTextBoxText.ScrollToCaret();
        }
        private void WithoutCRBetweenLines()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;
            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }
            if (string.IsNullOrEmpty(text))
                return;

            // 識別換行符類型（優先使用 \r\n，然後是 \n，最後是 \r）
            string lineBreak = "\r\n";
            if (!text.Contains("\r\n"))
            {
                if (text.Contains("\n"))
                    lineBreak = "\n";
                else if (text.Contains("\r"))
                    lineBreak = "\r";
            }

            // 將所有換行符統一為標準格式以便處理
            string normalizedText = text.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] lines = normalizedText.Split('\n');

            StringBuilder result = new StringBuilder();

            // 收集所有非空行的索引
            List<int> nonEmptyLineIndices = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    nonEmptyLineIndices.Add(i);
                }
            }

            if (nonEmptyLineIndices.Count == 0)
            {
                // 如果沒有非空行，返回空字符串
                result.Append("");
            }
            else
            {
                // 處理每一對相鄰的非空行
                for (int idx = 0; idx < nonEmptyLineIndices.Count; idx++)
                {
                    int lineIndex = nonEmptyLineIndices[idx];

                    // 添加非空行內容
                    result.Append(lines[lineIndex]);

                    // 如果不是最後一個非空行，只添加一個換行符（沒有空白行）
                    if (idx < nonEmptyLineIndices.Count - 1)
                    {
                        result.Append(lineBreak);
                    }
                }
            }

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }
        private void AddSpaceAtBegining()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            // 取得要添加的空白數量
            int spaceCount = appSettings.AddSpaceChrCount;
            string spacePrefix = new string(' ', spaceCount);

            // 分割成行並處理每一行 (統一使用 \r\n 斷行)
            // 注意：Split如果不移除空元素，會保留原有的空行邏輯
            string[] lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                // 移除行首的連續空白字元(全形或半形)
                // 這裡複用已有的 RemoveLeadingFullWhitespace 方法
                string trimmedLine = RemoveLeadingFullWhitespace(line);

                if (trimmedLine.Length > 0)
                {
                    // 只有當行內有內容時才添加縮排
                    result.Append(spacePrefix);
                    result.Append(trimmedLine);
                }
                else
                {
                    // 空行直接保留空（不添加縮排空格，避免產生只有空白的行）
                }

                // 添加換行符（除了最後一行）
                if (i < lines.Length - 1)
                {
                    result.Append("\r\n");
                }
            }

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();

                    // 嘗試恢復游標位置
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;

                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                // 記錄選擇區舊長度
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();

                // 選取剛剛處理完的文字
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }

        private void SplitBeginingByJudgment()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            string judgment = appSettings.NewLineStartJudgment;
            if (string.IsNullOrEmpty(judgment))
            {
                MessageBox.Show("設定中的「新行開頭的判定字串」為空，請先至選項設定。", "提示");
                return;
            }

            // 在符合字串之前插入新行符號
            // 防止重複插入：如果前面已經是換行符，可以考慮不插，但簡單實作先直接插入
            // 使用 Replace 將 "Judgment" 替換為 "\r\nJudgment"
            // 注意：這可能會導致已經折行的變成兩行空行，但符合使用者需求描述
            string result = text.Replace(judgment, "\r\n" + judgment);

            // 如果沒有變更則不動作
            if (text == result) return;

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    // 為了避免整份文件重置導致捲動問題，盡量保持體驗
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result;

                    // 簡單恢復
                    if (selectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = selectionStart;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result;
                richTextBoxText.Select(selStart, result.Length);
            }
        }

        private void SplitEndByJudgment()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            string judgment = appSettings.NewLineEndJudgment;
            if (string.IsNullOrEmpty(judgment))
            {
                MessageBox.Show("設定中的「新行結尾的判定字串」為空，請先至選項設定。", "提示");
                return;
            }

            // 在符合字串之後插入新行符號
            // 使用 Replace 將 "Judgment" 替換為 "Judgment\r\n"
            string result = text.Replace(judgment, judgment + "\r\n");

            // 如果沒有變更則不動作
            if (text == result) return;

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result;

                    if (selectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = selectionStart;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result;
                richTextBoxText.Select(selStart, result.Length);
            }
        }

        private void MergeByJudgment()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            string startMark = appSettings.NewLineStartJudgment;
            string endMark = appSettings.NewLineEndJudgment;

            if (string.IsNullOrEmpty(startMark) || string.IsNullOrEmpty(endMark))
            {
                MessageBox.Show("設定中的「新行開頭」或「新行結尾」判定字串為空，請先至選項設定。", "提示");
                return;
            }

            StringBuilder result = new StringBuilder();
            int currentIndex = 0;
            int textLength = text.Length;

            while (currentIndex < textLength)
            {
                // 尋找下一個開始標記
                int startIndex = text.IndexOf(startMark, currentIndex);

                if (startIndex == -1)
                {
                    // 找不到開始標記，將剩餘文字加入並結束
                    result.Append(text.Substring(currentIndex));
                    break;
                }

                // 將目前位置到開始標記前的文字加入
                result.Append(text.Substring(currentIndex, startIndex - currentIndex));

                // 尋找對應的結束標記 (從開始標記之後開始找)
                int searchEndFrom = startIndex + startMark.Length;
                int endIndex = text.IndexOf(endMark, searchEndFrom);

                if (endIndex == -1)
                {
                    // 找不到結束標記，將剩餘文字加入並結束 (保留開始標記)
                    result.Append(text.Substring(startIndex));
                    break;
                }

                // 加入開始標記
                result.Append(startMark);

                // 取得判定字串之間的內容
                string content = text.Substring(searchEndFrom, endIndex - searchEndFrom);

                // 移除內容中的斷行符號 (合併成同一行)
                string mergedContent = content.Replace("\r", "").Replace("\n", "");
                result.Append(mergedContent);

                // 加入結束標記
                result.Append(endMark);

                // 更新目前位置到結束標記之後
                currentIndex = endIndex + endMark.Length;
            }

            // 如果沒有變更則不動作
            if (text == result.ToString()) return;

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();

                    if (selectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = selectionStart;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                richTextBoxText.Select(selStart, result.Length);
            }
        }
        private void SortLines()
        {
            // 1. 處理選取範圍：若未選取文字，則選取全文；若有選取，擴展至完整行
            if (richTextBoxText.SelectionLength == 0)
            {
                richTextBoxText.SelectAll();
            }
            else
            {
                int selectionStart = richTextBoxText.SelectionStart;
                int selectionLength = richTextBoxText.SelectionLength;
                string text = richTextBoxText.Text;

                // 往回找行首
                int newStart = selectionStart;
                while (newStart > 0)
                {
                    char c = text[newStart - 1];
                    if (c == '\r' || c == '\n') break;
                    newStart--;
                }

                // 往後找行尾
                int newEnd = selectionStart + selectionLength;
                while (newEnd < text.Length)
                {
                    char c = text[newEnd];
                    if (c == '\r' || c == '\n')
                    {
                        if (c == '\r' && newEnd + 1 < text.Length && text[newEnd + 1] == '\n')
                            newEnd += 2;
                        else
                            newEnd += 1;
                        break;
                    }
                    newEnd++;
                }
                richTextBoxText.Select(newStart, newEnd - newStart);
            }

            // 2. 跳出對話視窗
            using (Form sortDialog = new Form())
            {
                sortDialog.Text = "排序選項";
                sortDialog.Size = new Size(800, 160);
                sortDialog.StartPosition = FormStartPosition.CenterParent;
                sortDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                sortDialog.MaximizeBox = false;
                sortDialog.MinimizeBox = false;

                Label lbl = new Label();
                lbl.Text = "請選擇每一行的排序方式：";
                lbl.Location = new Point(20, 20);
                lbl.AutoSize = true;
                sortDialog.Controls.Add(lbl);

                int sortMode = 0; // 0:None, 1:Asc, 2:Desc, 3:AscPreserve, 4:DescPreserve

                int startX = 15;
                int btnY = 55;
                int btnHeight = 45;
                int gap = 16;

                // 取消
                Button btnCancel = new Button();
                btnCancel.Text = "取消";
                btnCancel.Location = new Point(startX, btnY);
                btnCancel.Size = new Size(80, btnHeight);
                btnCancel.DialogResult = DialogResult.Cancel;
                sortDialog.Controls.Add(btnCancel);
                sortDialog.CancelButton = btnCancel;
                startX += 80 + gap;

                // 正向
                Button btnAsc = new Button();
                btnAsc.Text = "正向排序";
                btnAsc.Location = new Point(startX, btnY);
                btnAsc.Size = new Size(100, btnHeight);
                btnAsc.Click += (s, e) => { sortMode = 1; sortDialog.DialogResult = DialogResult.OK; sortDialog.Close(); };
                sortDialog.Controls.Add(btnAsc);
                startX += btnAsc.Size.Width + gap;

                // 反向
                Button btnDesc = new Button();
                btnDesc.Text = "反向排序";
                btnDesc.Location = new Point(startX, btnY);
                btnDesc.Size = new Size(100, btnHeight);
                btnDesc.Click += (s, e) => { sortMode = 2; sortDialog.DialogResult = DialogResult.OK; sortDialog.Close(); };
                sortDialog.Controls.Add(btnDesc);
                startX += btnDesc.Size.Width + gap;

                // 正向(保留)
                Button btnAscP = new Button();
                btnAscP.Text = "正向排序(保留空白行)";
                btnAscP.Location = new Point(startX, btnY);
                btnAscP.Size = new Size(200, btnHeight);
                btnAscP.Click += (s, e) => { sortMode = 3; sortDialog.DialogResult = DialogResult.OK; sortDialog.Close(); };
                sortDialog.Controls.Add(btnAscP);
                startX += btnAscP.Size.Width + gap;

                // 反向(保留)
                Button btnDescP = new Button();
                btnDescP.Text = "反向排序(保留空白行)";
                btnDescP.Location = new Point(startX, btnY);
                btnDescP.Size = new Size(200, btnHeight);
                btnDescP.Click += (s, e) => { sortMode = 4; sortDialog.DialogResult = DialogResult.OK; sortDialog.Close(); };
                sortDialog.Controls.Add(btnDescP);

                sortDialog.ShowDialog(this);

                if (sortMode == 0) return;

                string selectedText = richTextBoxText.SelectedText;
                if (string.IsNullOrEmpty(selectedText)) return;

                bool endsWithNewLine = selectedText.EndsWith("\n") || selectedText.EndsWith("\r");

                List<string> lines = new List<string>();
                using (System.IO.StringReader sr = new System.IO.StringReader(selectedText))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                if (sortMode == 1 || sortMode == 2)
                {
                    lines.Sort(StringComparer.CurrentCulture);
                    if (sortMode == 2) lines.Reverse();
                }
                else
                {
                    // Preserve format logic
                    var blocks = new List<List<string>>();
                    List<string>? currentBlock = null;

                    foreach (var line in lines)
                    {
                        bool isEmpty = string.IsNullOrWhiteSpace(line);

                        if (!isEmpty)
                        {
                            currentBlock = new List<string>();
                            currentBlock.Add(line);
                            blocks.Add(currentBlock);
                        }
                        else
                        {
                            if (currentBlock == null)
                            {
                                currentBlock = new List<string>();
                                blocks.Add(currentBlock);
                            }
                            currentBlock.Add(line);
                        }
                    }

                    blocks.Sort((a, b) => {
                        string keyA = (a.Count > 0) ? a[0] : "";
                        string keyB = (b.Count > 0) ? b[0] : "";
                        return StringComparer.CurrentCulture.Compare(keyA, keyB);
                    });

                    if (sortMode == 4) blocks.Reverse();

                    lines.Clear();
                    foreach (var block in blocks)
                    {
                        lines.AddRange(block);
                    }
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lines.Count; i++)
                {
                    sb.Append(lines[i]);
                    if (i < lines.Count - 1)
                    {
                        sb.Append("\r\n");
                    }
                    else
                    {
                        if (endsWithNewLine)
                        {
                            sb.Append("\r\n");
                        }
                    }
                }

                int finalStart = richTextBoxText.SelectionStart;
                string newText = sb.ToString();
                richTextBoxText.SelectedText = newText;
                richTextBoxText.Select(finalStart, newText.Length);
            }
        }

        private void InsertBeginingEndByInsertText()
        {
            // 取得原始/選取內容和位置
            string text;
            bool processWholeDocument;
            int selectionStart = richTextBoxText.SelectionStart;
            int selectionLength = richTextBoxText.SelectionLength;

            if (selectionLength > 0)
            {
                text = richTextBoxText.SelectedText;
                processWholeDocument = false;
            }
            else
            {
                text = richTextBoxText.Text;
                processWholeDocument = true;
            }

            if (string.IsNullOrEmpty(text))
                return;

            // 取用設定中的插入字串
            string insertBegin = appSettings.InsertBeginingText ?? "";
            string insertEnd = appSettings.InsertEndText ?? "";

            // 按行處理，保留原有換行符
            List<string> lines = new List<string>();
            List<string> lineBreaks = new List<string>();

            int pos = 0;
            while (pos < text.Length)
            {
                int lineEnd = pos;
                string foundBreak = "";
                while (lineEnd < text.Length)
                {
                    if (text[lineEnd] == '\r')
                    {
                        if (lineEnd + 1 < text.Length && text[lineEnd + 1] == '\n')
                        {
                            foundBreak = "\r\n";
                            break;
                        }
                        else
                        {
                            foundBreak = "\r";
                            break;
                        }
                    }
                    else if (text[lineEnd] == '\n')
                    {
                        foundBreak = "\n";
                        break;
                    }
                    lineEnd++;
                }

                string line = text.Substring(pos, lineEnd - pos);
                lines.Add(line);
                lineBreaks.Add(foundBreak);

                if (string.IsNullOrEmpty(foundBreak))
                    pos = lineEnd;
                else
                    pos = lineEnd + foundBreak.Length;
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string br = lineBreaks[i];

                // 若該行只有空白或TAB，則不插入
                if (string.IsNullOrWhiteSpace(line))
                {
                    result.Append(line);
                    result.Append(br);
                    continue;
                }

                // 檢查行首是否已具有 insertBegin
                bool hasBegin = false;
                if (!string.IsNullOrEmpty(insertBegin) && line.Length >= insertBegin.Length)
                {
                    if (line.StartsWith(insertBegin)) hasBegin = true;
                }

                // 檢查行尾是否已具有 insertEnd
                bool hasEnd = false;
                if (!string.IsNullOrEmpty(insertEnd) && line.Length >= insertEnd.Length)
                {
                    if (line.EndsWith(insertEnd)) hasEnd = true;
                }

                if (!hasBegin)
                    line = insertBegin + line;
                if (!hasEnd)
                    line = line + insertEnd;

                result.Append(line);
                result.Append(br);
            }

            // 套用結果
            if (processWholeDocument)
            {
                SuspendDrawing();
                richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
                try
                {
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.SelectAll();
                    richTextBoxText.SelectedText = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
                }
                finally
                {
                    richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                    richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                    ResumeDrawing();
                    UpdateStatusLabel();
                }
            }
            else
            {
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                int newLength = result.Length;
                richTextBoxText.Select(selStart, newLength);
            }
        }

        private void InsertAnnotationAndSerialNumber()
        {
            string startMark = appSettings.AnnotationBegin;
            string endMark = appSettings.AnnotationEnd;

            // 1. 處理選取範圍：若未選取文字，則選取全文；若有選取，擴展至完整行
            bool processWholeDocument = (richTextBoxText.SelectionLength == 0);
            if (processWholeDocument)
            {
                richTextBoxText.SelectAll();
            }
            else
            {
                // 擴展選取範圍至完整行
                int selectionStart = richTextBoxText.SelectionStart;
                int selectionLength = richTextBoxText.SelectionLength;
                string text = richTextBoxText.Text;

                // 往回找行首
                int newStart = selectionStart;
                while (newStart > 0)
                {
                    char c = text[newStart - 1];
                    if (c == '\r' || c == '\n') break;
                    newStart--;
                }

                // 往後找行尾
                int newEnd = selectionStart + selectionLength;
                while (newEnd < text.Length)
                {
                    char c = text[newEnd];
                    if (c == '\r' || c == '\n')
                    {
                        if (c == '\r' && newEnd + 1 < text.Length && text[newEnd + 1] == '\n')
                            newEnd += 2;
                        else
                            newEnd += 1;
                        break;
                    }
                    newEnd++;
                }
                richTextBoxText.Select(newStart, newEnd - newStart);
            }

            int selStart = richTextBoxText.SelectionStart;
            int startLineIndex = richTextBoxText.GetLineFromCharIndex(selStart);
            string selectedText = richTextBoxText.SelectedText;

            if (string.IsNullOrEmpty(selectedText)) return;

            // 將選取的文字分割成行 (保留換行符)
            string[] parts = Regex.Split(selectedText, @"(\r\n|\r|\n)");
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < parts.Length; i += 2)
            {
                string line = parts[i];
                // 如果是最後一個部分且為空字串，且 selectedText 以換行符結尾，則不處理這一行
                if (i == parts.Length - 1 && string.IsNullOrEmpty(line) && i > 0)
                {
                    break;
                }

                int absoluteLineNumber = startLineIndex + (i / 2) + 1;

                if (line.StartsWith(startMark))
                {
                    // 已有註解開頭，判斷後續是否為數字
                    string afterStart = line.Substring(startMark.Length);
                    Match match = Regex.Match(afterStart, @"^(\d+)(.*)$");
                    if (match.Success)
                    {
                        // 規則 3：替換數字
                        string restOfLine = match.Groups[2].Value;
                        result.Append(startMark + absoluteLineNumber + restOfLine);
                    }
                    else
                    {
                        // 規則 4：插入編號
                        result.Append(startMark + absoluteLineNumber + afterStart);
                    }
                }
                else
                {
                    // 規則 2：添加註解開頭、編號與結尾
                    result.Append(startMark + absoluteLineNumber + endMark + line);
                }

                // 加上對應的換行符
                if (i + 1 < parts.Length)
                {
                    result.Append(parts[i + 1]);
                }
            }

            // 處理 Undo 功能與界面更新
            SuspendDrawing();
            richTextBoxText.TextChanged -= RichTextBoxText_TextChanged;
            richTextBoxText.SelectionChanged -= RichTextBoxText_SelectionChanged;
            try
            {
                richTextBoxText.SelectedText = result.ToString();
                // 重新選取處理後的範圍
                richTextBoxText.Select(selStart, result.Length);
                richTextBoxText.ScrollToCaret();
            }
            finally
            {
                richTextBoxText.TextChanged += RichTextBoxText_TextChanged;
                richTextBoxText.SelectionChanged += RichTextBoxText_SelectionChanged;
                ResumeDrawing();
                UpdateStatusLabel();
            }
        }

        private void AutoWordwrap()
        {
            richTextBoxText.WordWrap = !richTextBoxText.WordWrap;
            toolStripButtonAutoWordwrap.Checked = richTextBoxText.WordWrap;
            toolStripButtonAutoWordwrap.Text = toolStripButtonAutoWordwrap.Checked ? "✔自動換行" : "　自動換行";
        }
    }
}

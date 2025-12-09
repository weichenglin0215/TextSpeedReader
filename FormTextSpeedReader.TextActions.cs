using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
                Font tmpFont = richTextBoxText.Font;
                float tmpFontSize = richTextBoxText.Font.Size;
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
            }
            else if (webBrowser1.Visible)
            {
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
                    webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
                }
            }
        }

        // 減少字體大小按鈕點擊事件
        private void FontSizeReduce(object sender, EventArgs e)
        {
            if (richTextBoxText.Visible)
            {
                Font tmpFont = richTextBoxText.Font;
                float tmpFontSize = richTextBoxText.Font.Size;
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
            }
            else if (webBrowser1.Visible)
            {
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
                    webBrowser1.Document.Body.Style = "zoom: " + m_WebBrowserZoom.ToString() + "%";
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

        // 自動移除目前文章中多餘的斷行（按鈕）
        private void AutoRemoveCRButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCR();
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
                richTextBoxText.SelectionLength = richTextBoxText.SelectionLength - 2;
                //MessageBox.Show("選取內容最後有斷行與新行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
                //MessageBox.Show("選取內容最後有斷行符號，已自動去除最後的斷行符號再處理。", "提示");
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
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
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                richTextBoxText.ScrollToCaret();
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

        // 自動移除目前文章中多餘的斷行，不包含該行最後一個字是句點或驚嘆號的行（按鈕）
        private void AutoRemoveCRWithoutDotAndExclamationMarkButton_Click(object sender, EventArgs e)
        {
            AutoRemoveCRWithoutDotAndExclamationMark();
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
                richTextBoxText.SelectionLength = richTextBoxText.SelectionLength - 2;
            }
            else if (text.EndsWith("\n"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
            }
            else if (text.EndsWith("\r"))
            {
                text = text.Substring(0, text.Length - 1);
                if (richTextBoxText.SelectionLength > 0) richTextBoxText.SelectionLength--;
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
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                richTextBoxText.ScrollToCaret();
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

        // 移除行首和行尾的空白字元（按鈕）
        private void RemoveLeadSpace_Click(object sender, EventArgs e)
        {
            RemoveLeadingAndTrailingSpaces();
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
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.Text = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
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
                    int originalSelectionStart = richTextBoxText.SelectionStart;
                    richTextBoxText.Text = result.ToString();
                    if (originalSelectionStart < richTextBoxText.Text.Length)
                        richTextBoxText.SelectionStart = originalSelectionStart;
                    else
                        richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                    richTextBoxText.ScrollToCaret();
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
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                richTextBoxText.ScrollToCaret();
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
                // 處理整個文檔
                richTextBoxText.Text = result.ToString();
                richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.SelectionLength = 0;
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
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                richTextBoxText.ScrollToCaret();
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
                int originalSelectionStart = richTextBoxText.SelectionStart;
                richTextBoxText.Text = result.ToString();
                
                // 嘗試恢復游標位置
                if (originalSelectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = originalSelectionStart;
                else
                    richTextBoxText.SelectionStart = richTextBoxText.Text.Length;
                
                richTextBoxText.ScrollToCaret();
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
                // 為了避免整份文件重置導致捲動問題，盡量保持體驗
                richTextBoxText.Text = result;
                
                // 簡單恢復
                if (selectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.ScrollToCaret();
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
                richTextBoxText.Text = result;
                
                if (selectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.ScrollToCaret();
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
                richTextBoxText.Text = result.ToString();
                
                if (selectionStart < richTextBoxText.Text.Length)
                    richTextBoxText.SelectionStart = selectionStart;
                richTextBoxText.ScrollToCaret();
            }
            else
            {
                int selStart = richTextBoxText.SelectionStart;
                richTextBoxText.SelectedText = result.ToString();
                richTextBoxText.Select(selStart, result.Length);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TextSpeedReader
{
    /// <summary>
    /// 「搜尋檔案列表中的文字」彈窗 (Ctrl+Shift+F)。
    /// 模仿一般 IDE 的「在檔案中搜尋」功能：對 listViewFile 目前顯示的每個文字檔案，
    /// 搜尋符合輸入字串的所有位置，以 TreeView 階層列出：
    ///   主節點 = 檔案 (含命中數)
    ///   子節點 = 該檔案內每一筆命中所在行 (截斷顯示比對字附近的內容)
    /// 點擊子節點 → 開啟該檔案並捲動至對應行。
    /// </summary>
    public partial class FormSearchInFiles : Form
    {
        private readonly FormTextSpeedReader m_Owner;
        private List<string> m_FilePaths = new List<string>();

        private TextBox textBoxSearch = null!;
        private CheckBox checkBoxCaseSensitive = null!;
        private CheckBox checkBoxFullWidthEquiv = null!;
        private Button buttonSearch = null!;
        private TreeView treeViewResults = null!;
        private Label labelStatus = null!;

        public FormSearchInFiles(FormTextSpeedReader owner)
        {
            m_Owner = owner;
            // 必須先把 Size 設好，BuildUi 內的按鈕 Anchor=Top|Right 才能算出正確的右邊距，
            // 否則控制項會被推到視窗外面看不到 (例如「搜尋」按鈕)
            PositionWindow();
            BuildUi();
        }

        // 設定搜尋範圍 (每次呼叫起始視窗時，由 Owner 注入目前 listViewFile 的檔案清單)
        public void SetSearchScope(List<string> filePaths)
        {
            m_FilePaths = filePaths ?? new List<string>();
            labelStatus.Text = $"搜尋範圍：{m_FilePaths.Count} 個檔案";
        }

        // 建立介面控制項
        private void BuildUi()
        {
            this.Text = "搜尋檔案列表中的文字";
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;     // 永遠顯示在主視窗之前
            this.KeyPreview = true;
            // 全彈窗統一字體：12pt (其餘子控制項預設繼承)
            this.Font = new Font("Microsoft JhengHei UI", 12F);

            int padding = 8;
            int rowH = 34;
            int btnH = 32;     // 「搜尋」按鈕高度，略高於 TextBox 包住它，視覺對齊

            Label labelFind = new Label
            {
                Text = "比對字：",
                Location = new Point(padding, padding + 6),
                Width = 90,
                TextAlign = ContentAlignment.MiddleLeft
            };

            textBoxSearch = new TextBox
            {
                Location = new Point(padding + 100, padding + 4),
                Width = 380,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            textBoxSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DoSearch();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    e.SuppressKeyPress = true;
                }
            };

            buttonSearch = new Button
            {
                Text = "搜尋",
                Location = new Point(padding + 490, padding + 1),
                Width = 80,
                Height = btnH,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            buttonSearch.Click += (s, e) => DoSearch();

            checkBoxCaseSensitive = new CheckBox
            {
                Text = "大小寫相符",
                Location = new Point(padding, padding + rowH + 6),
                AutoSize = true
            };

            checkBoxFullWidthEquiv = new CheckBox
            {
                Text = "半形/全形視為相同",
                Checked = true,
                Location = new Point(padding + 160, padding + rowH + 6),
                AutoSize = true
            };

            labelStatus = new Label
            {
                Text = "搜尋範圍：0 個檔案",
                Location = new Point(padding, padding + rowH * 2 + 6),
                AutoSize = true,
                ForeColor = Color.DimGray
            };

            treeViewResults = new TreeView
            {
                Location = new Point(padding, padding + rowH * 3),
                Width = this.ClientSize.Width - padding * 2,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                HideSelection = false,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                // 字體繼承表單的 12pt；ItemHeight 預設 (0) 會自動依字體計算，避免項目擠在一起
                FullRowSelect = false,
                DrawMode = TreeViewDrawMode.OwnerDrawText
            };
            treeViewResults.DrawNode += TreeViewResults_DrawNode;
            treeViewResults.NodeMouseDoubleClick += (s, e) => OpenSelectedMatch(e.Node);
            treeViewResults.AfterSelect += (s, e) =>
            {
                // 子節點以單擊即開啟，符合 IDE 慣例
                if (e.Node != null && e.Node.Tag is MatchInfo) OpenSelectedMatch(e.Node);
            };

            this.Controls.Add(labelFind);
            this.Controls.Add(textBoxSearch);
            this.Controls.Add(buttonSearch);
            this.Controls.Add(checkBoxCaseSensitive);
            this.Controls.Add(checkBoxFullWidthEquiv);
            this.Controls.Add(labelStatus);
            this.Controls.Add(treeViewResults);

            // 配合 Anchor，先設定 ClientSize 一次以驅動排版
            ResizeTreeView();
            this.Resize += (s, e) => ResizeTreeView();
        }

        private void ResizeTreeView()
        {
            int padding = 8;
            int top = treeViewResults.Top;
            treeViewResults.Width = Math.Max(100, this.ClientSize.Width - padding * 2);
            treeViewResults.Height = Math.Max(100, this.ClientSize.Height - top - padding);
        }

        // 視窗位置：靠齊主螢幕左邊；寬 600；高 = 螢幕 80%
        private void PositionWindow()
        {
            Rectangle wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1200, 800);
            int w = 600;
            int h = (int)(wa.Height * 0.8);
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(w, h);
            this.Location = new Point(wa.Left, wa.Top + (wa.Height - h) / 2);
        }

        // 內部資料：紀錄一筆命中位置 (檔案路徑 + 行號 + 行內位置)
        private class MatchInfo
        {
            public string FullPath = "";
            public int LineIndex;       // 0-based
            public int MatchInLineStart; // 比對字在該行的字元起始 (對應原始 rawLine)
            public int MatchLength;      // 比對字長度
        }

        // 最後一次成功搜尋使用的設定 (供 OwnerDraw 凸顯字元時使用)
        private string m_LastNeedle = string.Empty;
        private bool m_LastCaseSensitive;
        private bool m_LastWidthEquiv;

        // 將「半形/全形視為相同」勾選後使用：把全形 ASCII (U+FF01~U+FF5E) 轉回半形，全形空白 → 半形空白
        private static string NormalizeWidth(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c >= 0xFF01 && c <= 0xFF5E) sb.Append((char)(c - 0xFEE0));
                else if (c == '　') sb.Append(' ');
                else sb.Append(c);
            }
            return sb.ToString();
        }

        // 執行搜尋並填充結果樹
        private void DoSearch()
        {
            System.Diagnostics.Debug.WriteLine($"[SearchInFiles] DoSearch start, scope={m_FilePaths.Count} files, needle='{textBoxSearch.Text}'");
            string needleRaw = textBoxSearch.Text;
            if (string.IsNullOrEmpty(needleRaw))
            {
                MessageBox.Show(this, "請輸入比對字。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool caseSensitive = checkBoxCaseSensitive.Checked;
            bool widthEquiv = checkBoxFullWidthEquiv.Checked;

            string needle = widthEquiv ? NormalizeWidth(needleRaw) : needleRaw;
            StringComparison cmp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            // 記住本次設定，供 OwnerDraw 凸顯字元 / 開啟檔案時精確選取
            m_LastNeedle = needleRaw;
            m_LastCaseSensitive = caseSensitive;
            m_LastWidthEquiv = widthEquiv;

            // 效能策略：
            //  (1) 單檔 > 5MB：彈窗詢問 (跳過所有/跳過此檔/讀取所有/讀取)
            //  (2) 單檔命中達 50 筆後彈窗詢問 (跳過後續/繼續下 100 筆)，達到後再次詢問
            const long LARGE_FILE_THRESHOLD = 5L * 1024 * 1024;
            const int INITIAL_MATCH_PROMPT = 50;
            const int CONTINUE_BATCH = 100;

            bool skipAllLarge = false;   // 使用者選「跳過所有大容量檔案」後生效
            bool readAllLarge = false;   // 使用者選「讀取所有大容量檔案」後生效
            int skippedTooLarge = 0;
            int unreadable = 0;

            buttonSearch.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            treeViewResults.BeginUpdate();
            treeViewResults.Nodes.Clear();

            int totalFiles = 0;
            int totalMatches = 0;

            try
            {
                foreach (string filePath in m_FilePaths)
                {
                    // ── 檔案大小決策 ──
                    long fileSize;
                    try { fileSize = new FileInfo(filePath).Length; }
                    catch { unreadable++; continue; }

                    if (fileSize > LARGE_FILE_THRESHOLD)
                    {
                        if (skipAllLarge) { skippedTooLarge++; continue; }
                        if (!readAllLarge)
                        {
                            LargeFileChoice choice = AskLargeFileChoice(filePath, fileSize);
                            if (choice == LargeFileChoice.SkipAll) { skipAllLarge = true; skippedTooLarge++; continue; }
                            if (choice == LargeFileChoice.SkipThis) { skippedTooLarge++; continue; }
                            if (choice == LargeFileChoice.ReadAll) { readAllLarge = true; }
                            // ReadThis 或 ReadAll → 繼續讀取
                        }
                    }

                    string content = m_Owner.ReadFileContentForSearch(filePath);
                    if (string.IsNullOrEmpty(content)) { unreadable++; continue; }

                    string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    TreeNode? fileNode = null;
                    int fileMatches = 0;
                    bool stopThisFile = false;
                    // 下一個彈窗閾值：第一次 50 筆，之後每 +100 再問一次
                    int nextPromptAt = INITIAL_MATCH_PROMPT;

                    for (int li = 0; li < lines.Length && !stopThisFile; li++)
                    {
                        string rawLine = lines[li];
                        string searchLine = widthEquiv ? NormalizeWidth(rawLine) : rawLine;

                        int from = 0;
                        while (from <= searchLine.Length - needle.Length)
                        {
                            int idx = searchLine.IndexOf(needle, from, cmp);
                            if (idx < 0) break;

                            if (fileNode == null)
                            {
                                fileNode = new TreeNode { Tag = filePath };
                                treeViewResults.Nodes.Add(fileNode);
                            }
                            fileMatches++;
                            totalMatches++;

                            string snippet = BuildSnippet(rawLine, idx, needle.Length);
                            TreeNode child = new TreeNode($"行 {li + 1,5}: {snippet}")
                            {
                                Tag = new MatchInfo
                                {
                                    FullPath = filePath,
                                    LineIndex = li,
                                    MatchInLineStart = idx,
                                    MatchLength = needle.Length
                                }
                            };
                            fileNode.Nodes.Add(child);

                            from = idx + Math.Max(1, needle.Length);

                            // 達到彈窗閾值 → 詢問是否繼續
                            if (fileMatches >= nextPromptAt)
                            {
                                if (AskContinueMatching(filePath, fileMatches))
                                {
                                    nextPromptAt = fileMatches + CONTINUE_BATCH;
                                }
                                else
                                {
                                    stopThisFile = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (fileNode != null)
                    {
                        string capTag = stopThisFile ? "，使用者中止" : "";
                        fileNode.Text = $"{Path.GetFileName(filePath)}  ({fileMatches} 筆命中{capTag})";
                        fileNode.ToolTipText = filePath;
                        totalFiles++;
                    }
                }

                foreach (TreeNode n in treeViewResults.Nodes) n.Expand();
            }
            finally
            {
                treeViewResults.EndUpdate();
                buttonSearch.Enabled = true;
                this.Cursor = Cursors.Default;
            }

            // 狀態列：摘要 + 警示資訊
            StringBuilder status = new StringBuilder();
            status.Append($"範圍：{m_FilePaths.Count} 個檔案 / 命中：{totalFiles} 個檔案、共 {totalMatches} 筆");
            if (skippedTooLarge > 0) status.Append($" / 跳過大容量檔案：{skippedTooLarge}");
            if (unreadable > 0) status.Append($" / 無法讀取：{unreadable}");
            labelStatus.Text = status.ToString();
        }

        // ── 大容量檔案決策 ──────────────────────────────
        private enum LargeFileChoice { SkipAll, SkipThis, ReadAll, ReadThis }

        // 跳出 4 選 1 彈窗詢問是否讀取超大檔案
        private LargeFileChoice AskLargeFileChoice(string filePath, long sizeBytes)
        {
            string sizeText = (sizeBytes / 1024.0 / 1024.0).ToString("F2") + " MB";
            string msg = $"「{Path.GetFileName(filePath)}」檔案容量 {sizeText}，超過 5 MB，是否讀取？";
            return ShowFourOptionDialog("大容量檔案", msg,
                "跳過所有大容量檔案", "跳過此檔案", "讀取所有大容量檔案", "讀取");
        }

        // 達到命中閾值時詢問是否繼續搜尋此檔案；true = 繼續下 100 筆；false = 跳過後續
        private bool AskContinueMatching(string filePath, int currentMatches)
        {
            string msg = $"「{Path.GetFileName(filePath)}」已搜尋 {currentMatches} 筆，是否繼續搜尋？";
            DialogResult dr = ShowTwoOptionDialog("命中筆數提示", msg,
                "繼續搜尋下 100 筆", "跳過後續搜尋");
            return dr == DialogResult.OK;
        }

        // 通用 4 按鈕對話框
        private LargeFileChoice ShowFourOptionDialog(string title, string message,
            string label1, string label2, string label3, string label4)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.TopMost = true;
                dlg.ClientSize = new Size(540, 130);
                dlg.Font = this.Font;

                Label lbl = new Label { Text = message, Location = new Point(15, 15), AutoSize = false, Size = new Size(510, 50) };
                dlg.Controls.Add(lbl);

                int bw = 125, by = 80, gap = 5;
                LargeFileChoice picked = LargeFileChoice.SkipThis;

                Button b1 = new Button { Text = label1, Size = new Size(bw, 36), Location = new Point(gap, by) };
                Button b2 = new Button { Text = label2, Size = new Size(bw, 36), Location = new Point(gap + (bw + gap), by) };
                Button b3 = new Button { Text = label3, Size = new Size(bw, 36), Location = new Point(gap + (bw + gap) * 2, by) };
                Button b4 = new Button { Text = label4, Size = new Size(bw, 36), Location = new Point(gap + (bw + gap) * 3, by) };
                b1.Click += (s, e) => { picked = LargeFileChoice.SkipAll; dlg.DialogResult = DialogResult.OK; };
                b2.Click += (s, e) => { picked = LargeFileChoice.SkipThis; dlg.DialogResult = DialogResult.OK; };
                b3.Click += (s, e) => { picked = LargeFileChoice.ReadAll; dlg.DialogResult = DialogResult.OK; };
                b4.Click += (s, e) => { picked = LargeFileChoice.ReadThis; dlg.DialogResult = DialogResult.OK; };
                dlg.Controls.Add(b1); dlg.Controls.Add(b2); dlg.Controls.Add(b3); dlg.Controls.Add(b4);
                dlg.AcceptButton = b4; // 預設按鈕：讀取此檔
                dlg.CancelButton = b2; // Esc：跳過此檔
                dlg.ShowDialog(this);
                return picked;
            }
        }

        // 通用 2 按鈕對話框 (回傳 OK = 第一個按鈕，Cancel = 第二個)
        private DialogResult ShowTwoOptionDialog(string title, string message, string okLabel, string cancelLabel)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = title;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.ShowInTaskbar = false;
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.TopMost = true;
                dlg.ClientSize = new Size(440, 130);
                dlg.Font = this.Font;

                Label lbl = new Label { Text = message, Location = new Point(15, 15), AutoSize = false, Size = new Size(410, 50) };
                dlg.Controls.Add(lbl);

                Button bOk = new Button { Text = okLabel, Size = new Size(200, 36), Location = new Point(15, 80), DialogResult = DialogResult.OK };
                Button bCancel = new Button { Text = cancelLabel, Size = new Size(200, 36), Location = new Point(225, 80), DialogResult = DialogResult.Cancel };
                dlg.Controls.Add(bOk); dlg.Controls.Add(bCancel);
                dlg.AcceptButton = bOk;
                dlg.CancelButton = bCancel;
                return dlg.ShowDialog(this);
            }
        }

        // 將命中所在行截斷顯示：取比對字前後約 30 字，超出部分以 ... 表示
        private static string BuildSnippet(string line, int matchStart, int matchLen)
        {
            const int ctx = 30;
            int dispStart = Math.Max(0, matchStart - ctx);
            int dispEnd = Math.Min(line.Length, matchStart + matchLen + ctx);
            string body = line.Substring(dispStart, dispEnd - dispStart);
            // 將內嵌的 Tab / 換行符替換為可見符號，避免破壞 TreeView 排版
            body = body.Replace("\t", " ");
            string prefix = dispStart > 0 ? "..." : "";
            string suffix = dispEnd < line.Length ? "..." : "";
            return prefix + body + suffix;
        }

        // 點擊子節點 → 通知主視窗開啟該檔案，並精確反白該行中的比對字
        private void OpenSelectedMatch(TreeNode? node)
        {
            if (node?.Tag is MatchInfo mi)
            {
                m_Owner.OpenFileAtLine(mi.FullPath, mi.LineIndex, mi.MatchInLineStart, mi.MatchLength);
            }
        }

        // ── OwnerDraw：將節點文字中所有比對字以黃底紅字粗體凸顯 ──
        private void TreeViewResults_DrawNode(object? sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node == null) { e.DrawDefault = true; return; }

            bool selected = (e.State & TreeNodeStates.Selected) != 0;
            Color foreColor = selected ? SystemColors.HighlightText : treeViewResults.ForeColor;
            Color backColor = selected ? SystemColors.Highlight : treeViewResults.BackColor;

            // 背景填滿 (包含整列以便看清楚選取狀態)
            using (SolidBrush bg = new SolidBrush(backColor))
                e.Graphics.FillRectangle(bg, e.Bounds);

            string text = e.Node.Text;
            Font baseFont = treeViewResults.Font;
            using Font boldFont = new Font(baseFont, FontStyle.Bold);

            // 比對字凸顯設定
            Color matchFore = selected ? Color.Yellow : Color.Red;
            Color matchBack = selected ? SystemColors.Highlight : Color.Yellow;

            const TextFormatFlags FLAGS = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine;
            Size measureBox = new Size(int.MaxValue, e.Bounds.Height);

            // 以較高的字體 (粗體) 為基準垂直置中，確保各行有足夠間距、不互相重疊
            int lineH = Math.Max(baseFont.Height, boldFont.Height);
            int y = e.Bounds.Y + Math.Max(0, (e.Bounds.Height - lineH) / 2);
            int x = e.Bounds.X;

            // 對檔案 (父) 節點以粗體一次畫完，不做字元凸顯
            bool isMatchChild = e.Node.Tag is MatchInfo;
            if (!isMatchChild || string.IsNullOrEmpty(m_LastNeedle))
            {
                Font f = isMatchChild ? baseFont : boldFont;
                TextRenderer.DrawText(e.Graphics, text, f, new Point(x, y), foreColor, backColor, FLAGS);
            }
            else
            {
                var ranges = FindMatchRanges(text, m_LastNeedle, m_LastCaseSensitive, m_LastWidthEquiv);
                int pos = 0;
                foreach (var (start, len) in ranges)
                {
                    if (start > pos)
                    {
                        string seg = text.Substring(pos, start - pos);
                        TextRenderer.DrawText(e.Graphics, seg, baseFont, new Point(x, y), foreColor, backColor, FLAGS);
                        x += TextRenderer.MeasureText(e.Graphics, seg, baseFont, measureBox, FLAGS).Width;
                    }
                    string mseg = text.Substring(start, len);
                    TextRenderer.DrawText(e.Graphics, mseg, boldFont, new Point(x, y), matchFore, matchBack, FLAGS);
                    x += TextRenderer.MeasureText(e.Graphics, mseg, boldFont, measureBox, FLAGS).Width;
                    pos = start + len;
                }
                if (pos < text.Length)
                {
                    string tail = text.Substring(pos);
                    TextRenderer.DrawText(e.Graphics, tail, baseFont, new Point(x, y), foreColor, backColor, FLAGS);
                }
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, foreColor, backColor);
        }

        // 在節點顯示文字中尋找比對字出現的所有區間 (考慮大小寫、半/全形設定)
        private static List<(int start, int len)> FindMatchRanges(
            string text, string needle, bool caseSensitive, bool widthEquiv)
        {
            var ranges = new List<(int, int)>();
            if (string.IsNullOrEmpty(needle) || string.IsNullOrEmpty(text)) return ranges;
            string hay = widthEquiv ? NormalizeWidth(text) : text;
            string nd = widthEquiv ? NormalizeWidth(needle) : needle;
            StringComparison cmp = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            int from = 0;
            while (from <= hay.Length - nd.Length)
            {
                int idx = hay.IndexOf(nd, from, cmp);
                if (idx < 0) break;
                ranges.Add((idx, nd.Length));
                from = idx + nd.Length;
            }
            return ranges;
        }
    }
}

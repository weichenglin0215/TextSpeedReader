using System.Windows.Forms;

namespace TextSpeedReader
{
    partial class FormTextSpeedReader
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ToolStripMenuItem toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTextSpeedReader));
            toolStripDropDownButtonArrange = new ToolStripDropDownButton();
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces = new ToolStripMenuItem();
            toolStripMenuItem_AutoRemoveCRButton = new ToolStripMenuItem();
            toolStripMenuItem_RemoveMoreThan120Char = new ToolStripMenuItem();
            splitContainerMain = new SplitContainer();
            splitContainerFolder = new SplitContainer();
            treeViewFolder = new TreeView();
            imageList1 = new ImageList(components);
            listViewFile = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            contextMenuStrip_ListViewFile = new ContextMenuStrip(components);
            toolStripMenuItem_ConvertSimple = new ToolStripMenuItem();
            toolStripMenuItem_ConvertTraditional = new ToolStripMenuItem();
            toolStripSeparator15 = new ToolStripSeparator();
            toolStripMenuItem_SearchFiles = new ToolStripMenuItem();
            toolStripMenuItem_DelFiles = new ToolStripMenuItem();
            toolStripSeparator14 = new ToolStripSeparator();
            toolStripMenuItem_RenameFile = new ToolStripMenuItem();
            listViewRecentFiles = new ListView();
            columnHeaderFileName = new ColumnHeader();
            columnHeaderCharCount = new ColumnHeader();
            richTextBoxText = new RichTextBox();
            contextMenuStrip_RichTextBox = new ContextMenuStrip(components);
            toolStripSeparator11 = new ToolStripSeparator();
            toolStripMenuItem_AutoSelectCR = new ToolStripMenuItem();
            toolStripMenuItem_AutoSelectWithPunctuation = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            toolStripMenuItem_RemoveCR = new ToolStripMenuItem();
            toolStripMenuItem_RemoveMoreThan120CharB = new ToolStripMenuItem();
            toolStripMenuItem_MergeNoneSpace = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            toolStripMenuItem_WithoutCRBetweenLines = new ToolStripMenuItem();
            toolStripMenuItem_KeepTwoCRBetweenLines = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            toolStripMenuItem_EditTextCovertSimplified = new ToolStripMenuItem();
            toolStripMenuItem_EditTextCovertTraditional = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            toolStripMenuItem_SelectedTextSaveAsNew = new ToolStripMenuItem();
            toolStripMenuItem_WholeTextSaveAsNew = new ToolStripMenuItem();
            toolStripSeparator13 = new ToolStripSeparator();
            webBrowser1 = new WebBrowser();
            navigationBar = new ToolStrip();
            ShowFolderButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripDropDownButtonSave = new ToolStripDropDownButton();
            toolStripMenuItem_ConvertToSimplified = new ToolStripMenuItem();
            toolStripMenuItem_FileConvertToSimplified = new ToolStripMenuItem();
            toolStripMenuItem_FileConvertToTraditional = new ToolStripMenuItem();
            toolStripMenuItem_CopyHtmlSaveFile = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripComboBoxFonts = new ToolStripComboBox();
            toolStripSeparator7 = new ToolStripSeparator();
            FontSizeAddButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            FontSizeReduceButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            QuitButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            RemoveLeadSpace = new ToolStripButton();
            AutoRemoveCRButton = new ToolStripButton();
            AutoRemoveCRWithoutDotAndExclamationMarkButton = new ToolStripButton();
            buttonConvertToSimplified = new ToolStripButton();
            toolStripButtonFileConvertToSimplified = new ToolStripButton();
            toolStripButtonCopyHtmlSaveFile = new ToolStripButton();
            FolderPathButton = new ToolStripButton();
            navBackButton = new ToolStripSplitButton();
            navForwardButton = new ToolStripSplitButton();
            navUpButton = new ToolStripButton();
            navAddressLabel = new ToolStripLabel();
            navFoldersButton = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabelNews = new ToolStripStatusLabel();
            toolStripStatusLabelFileName = new ToolStripStatusLabel();
            toolStripStatusLabelFixed = new ToolStripStatusLabel();
            toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerFolder).BeginInit();
            splitContainerFolder.Panel1.SuspendLayout();
            splitContainerFolder.Panel2.SuspendLayout();
            splitContainerFolder.SuspendLayout();
            contextMenuStrip_ListViewFile.SuspendLayout();
            contextMenuStrip_RichTextBox.SuspendLayout();
            navigationBar.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark
            // 
            toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark.Name = "toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark";
            toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark.Size = new Size(534, 30);
            toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark.Text = "自動移除多餘的斷行，跳過行尾句點或驚嘆號";
            toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark.Click += toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark_Click;
            // 
            // toolStripDropDownButtonArrange
            // 
            toolStripDropDownButtonArrange.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButtonArrange.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem_RemoveLeadingAndTrailingSpaces, toolStripMenuItem_AutoRemoveCRButton, toolStripMenuItem_AutoRemoveCRWithoutDotAndExclamationMark, toolStripMenuItem_RemoveMoreThan120Char });
            toolStripDropDownButtonArrange.Image = (Image)resources.GetObject("toolStripDropDownButtonArrange.Image");
            toolStripDropDownButtonArrange.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButtonArrange.Name = "toolStripDropDownButtonArrange";
            toolStripDropDownButtonArrange.Size = new Size(146, 29);
            toolStripDropDownButtonArrange.Text = "整理編排段落";
            // 
            // toolStripMenuItem_RemoveLeadingAndTrailingSpaces
            // 
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Name = "toolStripMenuItem_RemoveLeadingAndTrailingSpaces";
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Size = new Size(534, 30);
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Text = "移除行首行尾的空白字元";
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Click += toolStripMenuItem_RemoveLeadingAndTrailingSpaces_Click;
            // 
            // toolStripMenuItem_AutoRemoveCRButton
            // 
            toolStripMenuItem_AutoRemoveCRButton.Name = "toolStripMenuItem_AutoRemoveCRButton";
            toolStripMenuItem_AutoRemoveCRButton.Size = new Size(534, 30);
            toolStripMenuItem_AutoRemoveCRButton.Text = "自動移除多餘的斷行";
            toolStripMenuItem_AutoRemoveCRButton.Click += toolStripMenuItem_AutoRemoveCRButton_Click;
            // 
            // toolStripMenuItem_RemoveMoreThan120Char
            // 
            toolStripMenuItem_RemoveMoreThan120Char.Name = "toolStripMenuItem_RemoveMoreThan120Char";
            toolStripMenuItem_RemoveMoreThan120Char.Size = new Size(534, 30);
            toolStripMenuItem_RemoveMoreThan120Char.Text = "超過120個字尾是句點就自動分行，避免單行過長";
            toolStripMenuItem_RemoveMoreThan120Char.Click += toolStripMenuItem_RemoveMoreThan120Char_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerMain.Location = new Point(0, 32);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(splitContainerFolder);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(listViewRecentFiles);
            splitContainerMain.Panel2.Controls.Add(richTextBoxText);
            splitContainerMain.Panel2.Controls.Add(webBrowser1);
            splitContainerMain.Size = new Size(1300, 698);
            splitContainerMain.SplitterDistance = 500;
            splitContainerMain.SplitterWidth = 6;
            splitContainerMain.TabIndex = 4;
            // 
            // splitContainerFolder
            // 
            splitContainerFolder.Dock = DockStyle.Fill;
            splitContainerFolder.Location = new Point(0, 0);
            splitContainerFolder.Name = "splitContainerFolder";
            splitContainerFolder.Orientation = Orientation.Horizontal;
            // 
            // splitContainerFolder.Panel1
            // 
            splitContainerFolder.Panel1.Controls.Add(treeViewFolder);
            // 
            // splitContainerFolder.Panel2
            // 
            splitContainerFolder.Panel2.Controls.Add(listViewFile);
            splitContainerFolder.Size = new Size(500, 698);
            splitContainerFolder.SplitterDistance = 289;
            splitContainerFolder.SplitterWidth = 6;
            splitContainerFolder.TabIndex = 0;
            // 
            // treeViewFolder
            // 
            treeViewFolder.Dock = DockStyle.Fill;
            treeViewFolder.FullRowSelect = true;
            treeViewFolder.ImageIndex = 0;
            treeViewFolder.ImageList = imageList1;
            treeViewFolder.Location = new Point(0, 0);
            treeViewFolder.Name = "treeViewFolder";
            treeViewFolder.SelectedImageIndex = 0;
            treeViewFolder.Size = new Size(500, 289);
            treeViewFolder.TabIndex = 0;
            treeViewFolder.AfterSelect += treeViewFolder_AfterSelect;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "folder");
            imageList1.Images.SetKeyName(1, "file");
            // 
            // listViewFile
            // 
            listViewFile.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            listViewFile.ContextMenuStrip = contextMenuStrip_ListViewFile;
            listViewFile.Dock = DockStyle.Fill;
            listViewFile.GridLines = true;
            listViewFile.Location = new Point(0, 0);
            listViewFile.Name = "listViewFile";
            listViewFile.Size = new Size(500, 403);
            listViewFile.SmallImageList = imageList1;
            listViewFile.TabIndex = 0;
            listViewFile.UseCompatibleStateImageBehavior = false;
            listViewFile.View = View.Details;
            listViewFile.ColumnClick += listViewFile_ColumnClick;
            listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
            listViewFile.KeyDown += listViewFile_KeyDown;
            listViewFile.MouseClick += ListViewFile_MouseClick;
            listViewFile.MouseDown += ListViewFile_MouseDown;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Last Modified";
            columnHeader2.Width = 200;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Size";
            columnHeader3.TextAlign = HorizontalAlignment.Right;
            columnHeader3.Width = 90;
            // 
            // contextMenuStrip_ListViewFile
            // 
            contextMenuStrip_ListViewFile.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_ListViewFile.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_ConvertSimple, toolStripMenuItem_ConvertTraditional, toolStripSeparator15, toolStripMenuItem_SearchFiles, toolStripMenuItem_DelFiles, toolStripSeparator14, toolStripMenuItem_RenameFile });
            contextMenuStrip_ListViewFile.Name = "contextMenuStrip_ListViewFile";
            contextMenuStrip_ListViewFile.Size = new Size(385, 156);
            // 
            // toolStripMenuItem_ConvertSimple
            // 
            toolStripMenuItem_ConvertSimple.Name = "toolStripMenuItem_ConvertSimple";
            toolStripMenuItem_ConvertSimple.Size = new Size(384, 28);
            toolStripMenuItem_ConvertSimple.Text = "將選取檔案轉換成-簡體-並儲存新檔名";
            toolStripMenuItem_ConvertSimple.Click += toolStripButtonFileConvertToSimplified_Click;
            // 
            // toolStripMenuItem_ConvertTraditional
            // 
            toolStripMenuItem_ConvertTraditional.Name = "toolStripMenuItem_ConvertTraditional";
            toolStripMenuItem_ConvertTraditional.Size = new Size(384, 28);
            toolStripMenuItem_ConvertTraditional.Text = "將選取檔案轉換成-繁體-並儲存新檔名";
            toolStripMenuItem_ConvertTraditional.Click += toolStripMenuItem_ConvertTraditional_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new Size(381, 6);
            // 
            // toolStripMenuItem_SearchFiles
            // 
            toolStripMenuItem_SearchFiles.Name = "toolStripMenuItem_SearchFiles";
            toolStripMenuItem_SearchFiles.ShortcutKeys = Keys.Control | Keys.F;
            toolStripMenuItem_SearchFiles.Size = new Size(384, 28);
            toolStripMenuItem_SearchFiles.Text = "尋找檔案";
            toolStripMenuItem_SearchFiles.Click += toolStripMenuItem_SearchFiles_Click;
            // 
            // toolStripMenuItem_DelFiles
            // 
            toolStripMenuItem_DelFiles.Name = "toolStripMenuItem_DelFiles";
            toolStripMenuItem_DelFiles.ShortcutKeys = Keys.Delete;
            toolStripMenuItem_DelFiles.Size = new Size(384, 28);
            toolStripMenuItem_DelFiles.Text = "刪除檔案";
            toolStripMenuItem_DelFiles.Click += toolStripMenuItem_DelFiles_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new Size(381, 6);
            // 
            // toolStripMenuItem_RenameFile
            // 
            toolStripMenuItem_RenameFile.Name = "toolStripMenuItem_RenameFile";
            toolStripMenuItem_RenameFile.ShortcutKeys = Keys.F2;
            toolStripMenuItem_RenameFile.Size = new Size(384, 28);
            toolStripMenuItem_RenameFile.Text = "更名檔案";
            toolStripMenuItem_RenameFile.Click += toolStripMenuItem_RenameFile_Click;
            // 
            // listViewRecentFiles
            // 
            listViewRecentFiles.AutoArrange = false;
            listViewRecentFiles.Columns.AddRange(new ColumnHeader[] { columnHeaderFileName, columnHeaderCharCount });
            listViewRecentFiles.Location = new Point(116, 480);
            listViewRecentFiles.Name = "listViewRecentFiles";
            listViewRecentFiles.Size = new Size(489, 177);
            listViewRecentFiles.TabIndex = 1;
            listViewRecentFiles.UseCompatibleStateImageBehavior = false;
            listViewRecentFiles.View = View.Details;
            listViewRecentFiles.Visible = false;
            // 
            // columnHeaderFileName
            // 
            columnHeaderFileName.Text = "檔名";
            columnHeaderFileName.Width = 200;
            // 
            // columnHeaderCharCount
            // 
            columnHeaderCharCount.Text = "字數";
            columnHeaderCharCount.Width = 127;
            // 
            // richTextBoxText
            // 
            richTextBoxText.BackColor = SystemColors.WindowText;
            richTextBoxText.BorderStyle = BorderStyle.None;
            richTextBoxText.ContextMenuStrip = contextMenuStrip_RichTextBox;
            richTextBoxText.Dock = DockStyle.Fill;
            richTextBoxText.Font = new Font("標楷體", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 136);
            richTextBoxText.ForeColor = SystemColors.Window;
            richTextBoxText.Location = new Point(0, 0);
            richTextBoxText.Name = "richTextBoxText";
            richTextBoxText.Size = new Size(794, 698);
            richTextBoxText.TabIndex = 0;
            richTextBoxText.Text = "請先選取左上方目錄視窗，\n再點選左下方文字檔案 .txt 或 網頁檔案 .html\n\n新功能：選取.html文字轉存.txt\n\n左下角檔案區，按Del可刪除檔案，Ctrl+F 尋找檔案，F2 更換檔名。\n按右鍵可轉換繁簡體並儲存檔案。\n\n右方文字區可以編輯、尋找 Ctrl+F、取代Ctrl+H。\n右鍵有排版、分段落、轉換繁簡功能，分段轉存新檔。\n\n2025-11-16 Ver. 1.9.3.0";
            // 
            // contextMenuStrip_RichTextBox
            // 
            contextMenuStrip_RichTextBox.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_RichTextBox.Items.AddRange(new ToolStripItem[] { toolStripSeparator11, toolStripMenuItem_AutoSelectCR, toolStripMenuItem_AutoSelectWithPunctuation, toolStripSeparator12, toolStripMenuItem_RemoveCR, toolStripMenuItem_RemoveMoreThan120CharB, toolStripMenuItem_MergeNoneSpace, toolStripSeparator10, toolStripMenuItem_WithoutCRBetweenLines, toolStripMenuItem_KeepTwoCRBetweenLines, toolStripSeparator8, toolStripMenuItem_EditTextCovertSimplified, toolStripMenuItem_EditTextCovertTraditional, toolStripSeparator9, toolStripMenuItem_SelectedTextSaveAsNew, toolStripMenuItem_WholeTextSaveAsNew, toolStripSeparator13 });
            contextMenuStrip_RichTextBox.Name = "contextMenuStrip_RichTextBox";
            contextMenuStrip_RichTextBox.Size = new Size(535, 348);
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(531, 6);
            // 
            // toolStripMenuItem_AutoSelectCR
            // 
            toolStripMenuItem_AutoSelectCR.Name = "toolStripMenuItem_AutoSelectCR";
            toolStripMenuItem_AutoSelectCR.ShortcutKeys = Keys.Control | Keys.E;
            toolStripMenuItem_AutoSelectCR.Size = new Size(534, 28);
            toolStripMenuItem_AutoSelectCR.Text = "自動選取直到空白行";
            toolStripMenuItem_AutoSelectCR.Click += toolStripMenuItem_AutoSelectCR_Click;
            // 
            // toolStripMenuItem_AutoSelectWithPunctuation
            // 
            toolStripMenuItem_AutoSelectWithPunctuation.Name = "toolStripMenuItem_AutoSelectWithPunctuation";
            toolStripMenuItem_AutoSelectWithPunctuation.ShortcutKeys = Keys.Control | Keys.D;
            toolStripMenuItem_AutoSelectWithPunctuation.Size = new Size(534, 28);
            toolStripMenuItem_AutoSelectWithPunctuation.Text = "自動選取直到空白行或句點或驚嘆號";
            toolStripMenuItem_AutoSelectWithPunctuation.Click += toolStripMenuItem_AutoSelectWithPunctuation_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(531, 6);
            // 
            // toolStripMenuItem_RemoveCR
            // 
            toolStripMenuItem_RemoveCR.Name = "toolStripMenuItem_RemoveCR";
            toolStripMenuItem_RemoveCR.ShortcutKeys = Keys.Control | Keys.R;
            toolStripMenuItem_RemoveCR.Size = new Size(534, 28);
            toolStripMenuItem_RemoveCR.Text = "移除選取的文字斷行";
            toolStripMenuItem_RemoveCR.ToolTipText = "將所選文字合併成同一行";
            toolStripMenuItem_RemoveCR.Click += toolStripMenuItemRemoveLineBreaks_Click;
            // 
            // toolStripMenuItem_RemoveMoreThan120CharB
            // 
            toolStripMenuItem_RemoveMoreThan120CharB.Name = "toolStripMenuItem_RemoveMoreThan120CharB";
            toolStripMenuItem_RemoveMoreThan120CharB.ShortcutKeys = Keys.Control | Keys.T;
            toolStripMenuItem_RemoveMoreThan120CharB.Size = new Size(534, 28);
            toolStripMenuItem_RemoveMoreThan120CharB.Text = "超過120個字尾是句點就自動分行，避免單行過長";
            toolStripMenuItem_RemoveMoreThan120CharB.Click += toolStripMenuItem_RemoveMoreThan120CharB_Click;
            // 
            // toolStripMenuItem_MergeNoneSpace
            // 
            toolStripMenuItem_MergeNoneSpace.Enabled = false;
            toolStripMenuItem_MergeNoneSpace.Name = "toolStripMenuItem_MergeNoneSpace";
            toolStripMenuItem_MergeNoneSpace.ShortcutKeys = Keys.Control | Keys.M;
            toolStripMenuItem_MergeNoneSpace.Size = new Size(534, 28);
            toolStripMenuItem_MergeNoneSpace.Text = "若下一行文字之間無空格則合併";
            toolStripMenuItem_MergeNoneSpace.Click += toolStripMenuItem_MergeNoneSpace_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(531, 6);
            // 
            // toolStripMenuItem_WithoutCRBetweenLines
            // 
            toolStripMenuItem_WithoutCRBetweenLines.Name = "toolStripMenuItem_WithoutCRBetweenLines";
            toolStripMenuItem_WithoutCRBetweenLines.Size = new Size(534, 28);
            toolStripMenuItem_WithoutCRBetweenLines.Text = "段落之間消除任何空行";
            toolStripMenuItem_WithoutCRBetweenLines.Click += toolStripMenuItem_WithoutCRBetweenLines_Click;
            // 
            // toolStripMenuItem_KeepTwoCRBetweenLines
            // 
            toolStripMenuItem_KeepTwoCRBetweenLines.Name = "toolStripMenuItem_KeepTwoCRBetweenLines";
            toolStripMenuItem_KeepTwoCRBetweenLines.Size = new Size(534, 28);
            toolStripMenuItem_KeepTwoCRBetweenLines.Text = "段落之間保有一個空行";
            toolStripMenuItem_KeepTwoCRBetweenLines.Click += toolStripMenuItem_KeepTwoCRBetweenLines_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(531, 6);
            // 
            // toolStripMenuItem_EditTextCovertSimplified
            // 
            toolStripMenuItem_EditTextCovertSimplified.Name = "toolStripMenuItem_EditTextCovertSimplified";
            toolStripMenuItem_EditTextCovertSimplified.Size = new Size(534, 28);
            toolStripMenuItem_EditTextCovertSimplified.Text = "轉換成簡體字";
            toolStripMenuItem_EditTextCovertSimplified.Click += toolStripMenuItem_EditTextCovertSimplified_Click;
            // 
            // toolStripMenuItem_EditTextCovertTraditional
            // 
            toolStripMenuItem_EditTextCovertTraditional.Name = "toolStripMenuItem_EditTextCovertTraditional";
            toolStripMenuItem_EditTextCovertTraditional.Size = new Size(534, 28);
            toolStripMenuItem_EditTextCovertTraditional.Text = "轉換成繁體字";
            toolStripMenuItem_EditTextCovertTraditional.Click += toolStripMenuItem_EditTextCovertTraditional_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(531, 6);
            // 
            // toolStripMenuItem_SelectedTextSaveAsNew
            // 
            toolStripMenuItem_SelectedTextSaveAsNew.Name = "toolStripMenuItem_SelectedTextSaveAsNew";
            toolStripMenuItem_SelectedTextSaveAsNew.Size = new Size(534, 28);
            toolStripMenuItem_SelectedTextSaveAsNew.Text = "將選取文字另存新檔...";
            toolStripMenuItem_SelectedTextSaveAsNew.Click += toolStripMenuItem_SelectedTextSaveAsNew_Click;
            // 
            // toolStripMenuItem_WholeTextSaveAsNew
            // 
            toolStripMenuItem_WholeTextSaveAsNew.Name = "toolStripMenuItem_WholeTextSaveAsNew";
            toolStripMenuItem_WholeTextSaveAsNew.Size = new Size(534, 28);
            toolStripMenuItem_WholeTextSaveAsNew.Text = "以3000字為單位將整篇文字另存成多個新檔案...";
            toolStripMenuItem_WholeTextSaveAsNew.Click += toolStripMenuItem_WholeTextSaveAsNew_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new Size(531, 6);
            // 
            // webBrowser1
            // 
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Location = new Point(0, 0);
            webBrowser1.MinimumSize = new Size(20, 20);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.Size = new Size(794, 698);
            webBrowser1.TabIndex = 1;
            // 
            // navigationBar
            // 
            navigationBar.AutoSize = false;
            navigationBar.CanOverflow = false;
            navigationBar.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navigationBar.ImageScalingSize = new Size(24, 24);
            navigationBar.Items.AddRange(new ToolStripItem[] { ShowFolderButton, toolStripSeparator2, toolStripDropDownButtonArrange, toolStripSeparator6, toolStripDropDownButtonSave, toolStripSeparator1, toolStripComboBoxFonts, toolStripSeparator7, FontSizeAddButton, toolStripSeparator3, FontSizeReduceButton, toolStripSeparator4, QuitButton, toolStripSeparator5, RemoveLeadSpace, AutoRemoveCRButton, AutoRemoveCRWithoutDotAndExclamationMarkButton, buttonConvertToSimplified, toolStripButtonFileConvertToSimplified, toolStripButtonCopyHtmlSaveFile, FolderPathButton, navBackButton, navForwardButton, navUpButton, navAddressLabel, navFoldersButton });
            navigationBar.LayoutStyle = ToolStripLayoutStyle.Flow;
            navigationBar.Location = new Point(0, 0);
            navigationBar.Name = "navigationBar";
            navigationBar.Size = new Size(1304, 32);
            navigationBar.Stretch = true;
            navigationBar.TabIndex = 5;
            // 
            // ShowFolderButton
            // 
            ShowFolderButton.Checked = true;
            ShowFolderButton.CheckOnClick = true;
            ShowFolderButton.CheckState = CheckState.Checked;
            ShowFolderButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ShowFolderButton.Image = (Image)resources.GetObject("ShowFolderButton.Image");
            ShowFolderButton.ImageTransparentColor = Color.Magenta;
            ShowFolderButton.Name = "ShowFolderButton";
            ShowFolderButton.Size = new Size(116, 29);
            ShowFolderButton.Text = "顯示資料夾";
            ShowFolderButton.Click += ShowFolderButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 23);
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 23);
            // 
            // toolStripDropDownButtonSave
            // 
            toolStripDropDownButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButtonSave.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem_ConvertToSimplified, toolStripMenuItem_FileConvertToSimplified, toolStripMenuItem_FileConvertToTraditional, toolStripMenuItem_CopyHtmlSaveFile });
            toolStripDropDownButtonSave.Image = (Image)resources.GetObject("toolStripDropDownButtonSave.Image");
            toolStripDropDownButtonSave.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            toolStripDropDownButtonSave.Size = new Size(106, 29);
            toolStripDropDownButtonSave.Text = "檔案儲存";
            // 
            // toolStripMenuItem_ConvertToSimplified
            // 
            toolStripMenuItem_ConvertToSimplified.Name = "toolStripMenuItem_ConvertToSimplified";
            toolStripMenuItem_ConvertToSimplified.Size = new Size(591, 30);
            toolStripMenuItem_ConvertToSimplified.Text = "將目前TXT轉換成簡體並儲存新檔名";
            toolStripMenuItem_ConvertToSimplified.Click += toolStripMenuItem_ConvertToSimplified_Click;
            // 
            // toolStripMenuItem_FileConvertToSimplified
            // 
            toolStripMenuItem_FileConvertToSimplified.Name = "toolStripMenuItem_FileConvertToSimplified";
            toolStripMenuItem_FileConvertToSimplified.Size = new Size(591, 30);
            toolStripMenuItem_FileConvertToSimplified.Text = "將目錄清單中選取的TXT檔案轉換成-簡體-並儲存新檔名";
            toolStripMenuItem_FileConvertToSimplified.Click += toolStripMenuItem_FileConvertToSimplified_Click;
            // 
            // toolStripMenuItem_FileConvertToTraditional
            // 
            toolStripMenuItem_FileConvertToTraditional.Name = "toolStripMenuItem_FileConvertToTraditional";
            toolStripMenuItem_FileConvertToTraditional.Size = new Size(591, 30);
            toolStripMenuItem_FileConvertToTraditional.Text = "將目錄清單中選取的TXT檔案轉換成-繁體-並儲存新檔名";
            toolStripMenuItem_FileConvertToTraditional.Click += toolStripMenuItem_FileConvertToTraditional_Click;
            // 
            // toolStripMenuItem_CopyHtmlSaveFile
            // 
            toolStripMenuItem_CopyHtmlSaveFile.Name = "toolStripMenuItem_CopyHtmlSaveFile";
            toolStripMenuItem_CopyHtmlSaveFile.Size = new Size(591, 30);
            toolStripMenuItem_CopyHtmlSaveFile.Text = "複製HTML文字並儲存TXT檔案";
            toolStripMenuItem_CopyHtmlSaveFile.Click += toolStripMenuItem_CopyHtmlSaveFile_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 23);
            // 
            // toolStripComboBoxFonts
            // 
            toolStripComboBoxFonts.Alignment = ToolStripItemAlignment.Right;
            toolStripComboBoxFonts.DropDownHeight = 600;
            toolStripComboBoxFonts.DropDownWidth = 240;
            toolStripComboBoxFonts.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            toolStripComboBoxFonts.IntegralHeight = false;
            toolStripComboBoxFonts.Name = "toolStripComboBoxFonts";
            toolStripComboBoxFonts.Size = new Size(240, 33);
            toolStripComboBoxFonts.Text = "字型";
            toolStripComboBoxFonts.SelectedIndexChanged += ChangeFont;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 23);
            // 
            // FontSizeAddButton
            // 
            FontSizeAddButton.Alignment = ToolStripItemAlignment.Right;
            FontSizeAddButton.BackColor = SystemColors.Control;
            FontSizeAddButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FontSizeAddButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FontSizeAddButton.Image = (Image)resources.GetObject("FontSizeAddButton.Image");
            FontSizeAddButton.ImageTransparentColor = Color.Magenta;
            FontSizeAddButton.Name = "FontSizeAddButton";
            FontSizeAddButton.RightToLeft = RightToLeft.No;
            FontSizeAddButton.Size = new Size(29, 28);
            FontSizeAddButton.Text = "FontSizeAddButton";
            FontSizeAddButton.ToolTipText = "放大字體";
            FontSizeAddButton.Click += FontSizeAdd;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 23);
            // 
            // FontSizeReduceButton
            // 
            FontSizeReduceButton.Alignment = ToolStripItemAlignment.Right;
            FontSizeReduceButton.BackColor = SystemColors.Control;
            FontSizeReduceButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FontSizeReduceButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FontSizeReduceButton.Image = (Image)resources.GetObject("FontSizeReduceButton.Image");
            FontSizeReduceButton.ImageTransparentColor = Color.Magenta;
            FontSizeReduceButton.Name = "FontSizeReduceButton";
            FontSizeReduceButton.Size = new Size(29, 28);
            FontSizeReduceButton.Text = "FontSizeReduceButton";
            FontSizeReduceButton.ToolTipText = "縮小字型";
            FontSizeReduceButton.Click += FontSizeReduce;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 23);
            // 
            // QuitButton
            // 
            QuitButton.Alignment = ToolStripItemAlignment.Right;
            QuitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            QuitButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            QuitButton.Image = (Image)resources.GetObject("QuitButton.Image");
            QuitButton.ImageTransparentColor = Color.Magenta;
            QuitButton.Name = "QuitButton";
            QuitButton.Size = new Size(29, 28);
            QuitButton.Text = "Quit";
            QuitButton.Click += QuitButton_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 23);
            // 
            // RemoveLeadSpace
            // 
            RemoveLeadSpace.DisplayStyle = ToolStripItemDisplayStyle.Text;
            RemoveLeadSpace.ImageTransparentColor = Color.Magenta;
            RemoveLeadSpace.Name = "RemoveLeadSpace";
            RemoveLeadSpace.Size = new Size(236, 29);
            RemoveLeadSpace.Text = "移除行首行尾的空白字元";
            RemoveLeadSpace.ToolTipText = "移除行首的空白字元";
            RemoveLeadSpace.Visible = false;
            RemoveLeadSpace.Click += RemoveLeadSpace_Click;
            // 
            // AutoRemoveCRButton
            // 
            AutoRemoveCRButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AutoRemoveCRButton.ImageTransparentColor = Color.Magenta;
            AutoRemoveCRButton.Name = "AutoRemoveCRButton";
            AutoRemoveCRButton.Size = new Size(196, 29);
            AutoRemoveCRButton.Text = "自動移除多餘的斷行";
            AutoRemoveCRButton.ToolTipText = "自動移除沒必要的斷行";
            AutoRemoveCRButton.Visible = false;
            AutoRemoveCRButton.Click += AutoRemoveCRButton_Click;
            // 
            // AutoRemoveCRWithoutDotAndExclamationMarkButton
            // 
            AutoRemoveCRWithoutDotAndExclamationMarkButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Image = (Image)resources.GetObject("AutoRemoveCRWithoutDotAndExclamationMarkButton.Image");
            AutoRemoveCRWithoutDotAndExclamationMarkButton.ImageTransparentColor = Color.Magenta;
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Name = "AutoRemoveCRWithoutDotAndExclamationMarkButton";
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Size = new Size(416, 29);
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Text = "自動移除多餘的斷行，跳過行尾句點或驚嘆號";
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Visible = false;
            AutoRemoveCRWithoutDotAndExclamationMarkButton.Click += AutoRemoveCRWithoutDotAndExclamationMarkButton_Click;
            // 
            // buttonConvertToSimplified
            // 
            buttonConvertToSimplified.DisplayStyle = ToolStripItemDisplayStyle.Text;
            buttonConvertToSimplified.Image = (Image)resources.GetObject("buttonConvertToSimplified.Image");
            buttonConvertToSimplified.ImageTransparentColor = Color.Magenta;
            buttonConvertToSimplified.Name = "buttonConvertToSimplified";
            buttonConvertToSimplified.Size = new Size(331, 29);
            buttonConvertToSimplified.Text = "將目前TXT轉換成簡體並儲存新檔名";
            buttonConvertToSimplified.Visible = false;
            buttonConvertToSimplified.Click += buttonConvertToSimplified_Click;
            // 
            // toolStripButtonFileConvertToSimplified
            // 
            toolStripButtonFileConvertToSimplified.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonFileConvertToSimplified.Image = (Image)resources.GetObject("toolStripButtonFileConvertToSimplified.Image");
            toolStripButtonFileConvertToSimplified.ImageTransparentColor = Color.Magenta;
            toolStripButtonFileConvertToSimplified.Name = "toolStripButtonFileConvertToSimplified";
            toolStripButtonFileConvertToSimplified.Size = new Size(491, 29);
            toolStripButtonFileConvertToSimplified.Text = "將目錄清單中選取的TXT檔案轉換成簡體並儲存新檔名";
            toolStripButtonFileConvertToSimplified.Visible = false;
            toolStripButtonFileConvertToSimplified.Click += toolStripButtonFileConvertToSimplified_Click;
            // 
            // toolStripButtonCopyHtmlSaveFile
            // 
            toolStripButtonCopyHtmlSaveFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonCopyHtmlSaveFile.Image = (Image)resources.GetObject("toolStripButtonCopyHtmlSaveFile.Image");
            toolStripButtonCopyHtmlSaveFile.ImageTransparentColor = Color.Magenta;
            toolStripButtonCopyHtmlSaveFile.Name = "toolStripButtonCopyHtmlSaveFile";
            toolStripButtonCopyHtmlSaveFile.Size = new Size(286, 29);
            toolStripButtonCopyHtmlSaveFile.Text = "複製HTML文字並儲存TXT檔案";
            toolStripButtonCopyHtmlSaveFile.Visible = false;
            toolStripButtonCopyHtmlSaveFile.Click += toolStripButtonCopyHtmlSaveFile_Click;
            // 
            // FolderPathButton
            // 
            FolderPathButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FolderPathButton.Image = (Image)resources.GetObject("FolderPathButton.Image");
            FolderPathButton.ImageTransparentColor = Color.Magenta;
            FolderPathButton.Name = "FolderPathButton";
            FolderPathButton.Size = new Size(120, 29);
            FolderPathButton.Text = "開啟目錄";
            FolderPathButton.ToolTipText = "開啟目錄";
            FolderPathButton.Visible = false;
            FolderPathButton.Click += FolderPathButton_Click;
            // 
            // navBackButton
            // 
            navBackButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            navBackButton.Enabled = false;
            navBackButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navBackButton.Image = (Image)resources.GetObject("navBackButton.Image");
            navBackButton.ImageTransparentColor = Color.Magenta;
            navBackButton.Name = "navBackButton";
            navBackButton.Size = new Size(43, 28);
            navBackButton.Text = "Back";
            navBackButton.TextImageRelation = TextImageRelation.ImageAboveText;
            navBackButton.Visible = false;
            // 
            // navForwardButton
            // 
            navForwardButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            navForwardButton.Enabled = false;
            navForwardButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navForwardButton.Image = (Image)resources.GetObject("navForwardButton.Image");
            navForwardButton.ImageTransparentColor = Color.Magenta;
            navForwardButton.Name = "navForwardButton";
            navForwardButton.Size = new Size(43, 28);
            navForwardButton.Text = "Forward";
            navForwardButton.TextImageRelation = TextImageRelation.ImageAboveText;
            navForwardButton.Visible = false;
            // 
            // navUpButton
            // 
            navUpButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            navUpButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navUpButton.Image = (Image)resources.GetObject("navUpButton.Image");
            navUpButton.ImageTransparentColor = Color.Magenta;
            navUpButton.Name = "navUpButton";
            navUpButton.Size = new Size(29, 28);
            navUpButton.Text = "Up";
            navUpButton.TextImageRelation = TextImageRelation.ImageAboveText;
            navUpButton.Visible = false;
            // 
            // navAddressLabel
            // 
            navAddressLabel.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navAddressLabel.MergeIndex = 0;
            navAddressLabel.Name = "navAddressLabel";
            navAddressLabel.Overflow = ToolStripItemOverflow.Never;
            navAddressLabel.Size = new Size(88, 25);
            navAddressLabel.Text = "Address";
            navAddressLabel.Visible = false;
            // 
            // navFoldersButton
            // 
            navFoldersButton.Checked = true;
            navFoldersButton.CheckOnClick = true;
            navFoldersButton.CheckState = CheckState.Checked;
            navFoldersButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            navFoldersButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navFoldersButton.Image = (Image)resources.GetObject("navFoldersButton.Image");
            navFoldersButton.ImageTransparentColor = Color.Magenta;
            navFoldersButton.Name = "navFoldersButton";
            navFoldersButton.Size = new Size(29, 28);
            navFoldersButton.Text = "Folders";
            navFoldersButton.TextImageRelation = TextImageRelation.ImageAboveText;
            navFoldersButton.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelNews, toolStripStatusLabelFileName, toolStripStatusLabelFixed });
            statusStrip1.Location = new Point(0, 727);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1304, 25);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelNews
            // 
            toolStripStatusLabelNews.AutoSize = false;
            toolStripStatusLabelNews.Name = "toolStripStatusLabelNews";
            toolStripStatusLabelNews.Size = new Size(600, 19);
            toolStripStatusLabelNews.Text = "更新訊息";
            toolStripStatusLabelNews.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFileName
            // 
            toolStripStatusLabelFileName.AutoSize = false;
            toolStripStatusLabelFileName.Name = "toolStripStatusLabelFileName";
            toolStripStatusLabelFileName.Size = new Size(250, 19);
            toolStripStatusLabelFileName.Text = "檔名";
            toolStripStatusLabelFileName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFixed
            // 
            toolStripStatusLabelFixed.AutoSize = false;
            toolStripStatusLabelFixed.Name = "toolStripStatusLabelFixed";
            toolStripStatusLabelFixed.Size = new Size(350, 19);
            toolStripStatusLabelFixed.Text = "狀態訊息";
            toolStripStatusLabelFixed.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FormTextSpeedReader
            // 
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1304, 752);
            Controls.Add(statusStrip1);
            Controls.Add(navigationBar);
            Controls.Add(splitContainerMain);
            Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "FormTextSpeedReader";
            Text = "TextSpeedReader Ver. 1.9.3.0 ";
            FormClosing += FormTSRClosing;
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerFolder.Panel1.ResumeLayout(false);
            splitContainerFolder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerFolder).EndInit();
            splitContainerFolder.ResumeLayout(false);
            contextMenuStrip_ListViewFile.ResumeLayout(false);
            contextMenuStrip_RichTextBox.ResumeLayout(false);
            navigationBar.ResumeLayout(false);
            navigationBar.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }


        #endregion
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.SplitContainer splitContainerFolder;
        private System.Windows.Forms.RichTextBox richTextBoxText;
        private System.Windows.Forms.ToolStrip navigationBar;
        private System.Windows.Forms.ToolStripSplitButton navBackButton;
        private System.Windows.Forms.ToolStripSplitButton navForwardButton;
        private System.Windows.Forms.ToolStripButton navUpButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton navFoldersButton;
        private System.Windows.Forms.ToolStripLabel navAddressLabel;
        private System.Windows.Forms.TreeView treeViewFolder;
        private System.Windows.Forms.ListView listViewFile;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ToolStripButton QuitButton;
        private ToolStripButton FolderPathButton;
        private ToolStripSeparator toolStripSeparator2;
        private WebBrowser webBrowser1;
        private ToolStripButton FontSizeAddButton;
        private ToolStripButton FontSizeReduceButton;
        private ToolStripButton ShowFolderButton;
        private ListView listViewRecentFiles;
        private ColumnHeader columnHeaderFileName;
        private ColumnHeader columnHeaderCharCount;
        private ToolStripComboBox toolStripComboBoxFonts;
        private ColumnHeader columnHeader3;
        private ContextMenuStrip contextMenuStrip_RichTextBox;
        private ToolStripMenuItem toolStripMenuItem_RemoveCR;
        private ToolStripMenuItem toolStripMenuItem_AutoSelectCR;
        private ToolStripButton AutoRemoveCRButton;
        private ToolStripButton RemoveLeadSpace;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton buttonConvertToSimplified;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton toolStripButtonFileConvertToSimplified;
        private ContextMenuStrip contextMenuStrip_ListViewFile;
        private ToolStripMenuItem toolStripMenuItem_ConvertSimple;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton toolStripButtonCopyHtmlSaveFile;
        private StatusStrip statusStrip1;
        private ToolStripDropDownButton toolStripDropDownButtonArrange;
        private ToolStripMenuItem toolStripMenuItem_MergeNoneSpace;
        private ToolStripMenuItem toolStripMenuItem_AutoSelectWithPunctuation;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton AutoRemoveCRWithoutDotAndExclamationMarkButton;
        private ToolStripMenuItem toolStripMenuItem_RemoveLeadingAndTrailingSpaces;
        private ToolStripMenuItem toolStripMenuItem_AutoRemoveCRButton;
        private ToolStripDropDownButton toolStripDropDownButtonSave;
        private ToolStripMenuItem toolStripMenuItem_ConvertToSimplified;
        private ToolStripMenuItem toolStripMenuItem_FileConvertToSimplified;
        private ToolStripMenuItem toolStripMenuItem_CopyHtmlSaveFile;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem toolStripMenuItem_RemoveMoreThan120Char;
        private ToolStripMenuItem toolStripMenuItem_FileConvertToTraditional;
        private ToolStripMenuItem toolStripMenuItem_EditTextCovertSimplified;
        private ToolStripMenuItem toolStripMenuItem_EditTextCovertTraditional;
        private ToolStripMenuItem toolStripMenuItem_ConvertTraditional;
        private ToolStripStatusLabel toolStripStatusLabelNews;
        private ToolStripStatusLabel toolStripStatusLabelFixed;
        private ToolStripStatusLabel toolStripStatusLabelFileName;
        private ToolStripMenuItem toolStripMenuItem_KeepTwoCRBetweenLines;
        private ToolStripMenuItem toolStripMenuItem_WithoutCRBetweenLines;
        private ToolStripMenuItem toolStripMenuItem_DelFiles;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem toolStripMenuItem_SelectedTextSaveAsNew;
        private ToolStripMenuItem toolStripMenuItem_WholeTextSaveAsNew;
        private ToolStripMenuItem toolStripMenuItem_RemoveMoreThan120CharB;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem toolStripMenuItem_RenameFile;
        private ToolStripMenuItem toolStripMenuItem_SearchFiles;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripSeparator toolStripSeparator14;
    }
}


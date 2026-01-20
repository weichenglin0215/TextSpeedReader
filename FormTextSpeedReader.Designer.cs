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
            contextMenuStrip_treeViewFolder = new ContextMenuStrip(components);
            toolStripMenuItem_OpenFileManager = new ToolStripMenuItem();
            toolStripMenuItem_RenameDirectory = new ToolStripMenuItem();
            toolStripMenuItem_DeleteDirectory = new ToolStripMenuItem();
            toolStripMenuItem_ReCodeFolderName = new ToolStripMenuItem();
            toolStripMenuItem_ReCodeFullFoldersFilesName = new ToolStripMenuItem();
            toolStripMenuItem_FolderNameSim2Trad = new ToolStripMenuItem();
            imageList1 = new ImageList(components);
            listViewFile = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            contextMenuStrip_ListViewFile = new ContextMenuStrip(components);
            toolStripMenuItem_SaveTxtFile2 = new ToolStripMenuItem();
            toolStripMenuItem_SaveTxtAsNewFile2 = new ToolStripMenuItem();
            toolStripMenuItem_ConvertSimple = new ToolStripMenuItem();
            toolStripMenuItem_ConvertTraditional = new ToolStripMenuItem();
            toolStripSeparator15 = new ToolStripSeparator();
            toolStripMenuItem_SearchFiles = new ToolStripMenuItem();
            toolStripMenuItem_RenameFile = new ToolStripMenuItem();
            toolStripMenuItem_ReCodeFileName = new ToolStripMenuItem();
            toolStripMenuItem_FileNameSim2Trad = new ToolStripMenuItem();
            toolStripSeparator14 = new ToolStripSeparator();
            toolStripMenuItem_DelFiles = new ToolStripMenuItem();
            listViewRecentFiles = new ListView();
            columnHeaderFileName = new ColumnHeader();
            columnHeaderCharCount = new ColumnHeader();
            richTextBoxText = new RichTextBox();
            contextMenuStrip_RichTextBox = new ContextMenuStrip(components);
            toolStripSeparator11 = new ToolStripSeparator();
            toolStripMenuItem_AutoSelectCR = new ToolStripMenuItem();
            toolStripMenuItem_AutoSelectWithPunctuation = new ToolStripMenuItem();
            toolStripSeparator12 = new ToolStripSeparator();
            toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR = new ToolStripMenuItem();
            toolStripMenuItem_RemoveCR = new ToolStripMenuItem();
            toolStripMenuItem_AutoRemoveCRButton2 = new ToolStripMenuItem();
            toolStripSeparator19 = new ToolStripSeparator();
            toolStripMenuItem_AddSpaceAtBegining = new ToolStripMenuItem();
            toolStripMenuItem_RemoveMoreThan120CharB = new ToolStripMenuItem();
            toolStripMenuItem_EndingAddDot = new ToolStripMenuItem();
            toolStripMenuItem_MergeNoneSpace = new ToolStripMenuItem();
            toolStripSeparator20 = new ToolStripSeparator();
            toolStripMenuItem_SplitBeginingByJudgment = new ToolStripMenuItem();
            toolStripMenuItem_SplitEndByJudgment = new ToolStripMenuItem();
            toolStripMenuItem_MergeByJudgment = new ToolStripMenuItem();
            toolStripMenuItem_InsertBeginingEndByInsertText = new ToolStripMenuItem();
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
            toolStripMenuItem_SortLines = new ToolStripMenuItem();
            webBrowser1 = new WebBrowser();
            navigationBar = new ToolStrip();
            ShowFolderButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripDropDownButtonHistoryList = new ToolStripDropDownButton();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripDropDownButtonSave = new ToolStripDropDownButton();
            toolStripMenuItem_SaveTxtFile = new ToolStripMenuItem();
            toolStripMenuItem_SaveTxtAsNewFile = new ToolStripMenuItem();
            toolStripSeparator16 = new ToolStripSeparator();
            toolStripMenuItem_ConvertToSimplified = new ToolStripMenuItem();
            toolStripMenuItem_FileConvertToSimplified = new ToolStripMenuItem();
            toolStripMenuItem_FileConvertToTraditional = new ToolStripMenuItem();
            toolStripSeparator17 = new ToolStripSeparator();
            toolStripMenuItem_CopyHtmlSaveFileSimplified = new ToolStripMenuItem();
            toolStripMenuItem_CopyHtmlSaveFile = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButton_Option = new ToolStripButton();
            toolStripSeparator18 = new ToolStripSeparator();
            toolStripComboBoxFonts = new ToolStripComboBox();
            toolStripSeparator7 = new ToolStripSeparator();
            FontSizeAddButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            FontSizeReduceButton = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripButtonHTMLChangeFontChecker = new ToolStripButton();
            QuitButton = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            toolStripComboBoxHistoryList = new ToolStripComboBox();
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
            contextMenuStrip_treeViewFolder.SuspendLayout();
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
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Text = "＜Ｘ移除行首行尾的空白字元Ｘ＞";
            toolStripMenuItem_RemoveLeadingAndTrailingSpaces.Click += toolStripMenuItem_RemoveLeadingAndTrailingSpaces_Click;
            // 
            // toolStripMenuItem_AutoRemoveCRButton
            // 
            toolStripMenuItem_AutoRemoveCRButton.Name = "toolStripMenuItem_AutoRemoveCRButton";
            toolStripMenuItem_AutoRemoveCRButton.Size = new Size(534, 30);
            toolStripMenuItem_AutoRemoveCRButton.Text = "移除選取的文字斷行Ｘ》＼ｎ，保留空白行";
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
            splitContainerMain.Size = new Size(1580, 707);
            splitContainerMain.SplitterDistance = 497;
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
            splitContainerFolder.Size = new Size(497, 707);
            splitContainerFolder.SplitterDistance = 292;
            splitContainerFolder.SplitterWidth = 6;
            splitContainerFolder.TabIndex = 0;
            // 
            // treeViewFolder
            // 
            treeViewFolder.ContextMenuStrip = contextMenuStrip_treeViewFolder;
            treeViewFolder.Dock = DockStyle.Fill;
            treeViewFolder.FullRowSelect = true;
            treeViewFolder.ImageIndex = 0;
            treeViewFolder.ImageList = imageList1;
            treeViewFolder.Location = new Point(0, 0);
            treeViewFolder.Name = "treeViewFolder";
            treeViewFolder.SelectedImageIndex = 0;
            treeViewFolder.Size = new Size(497, 292);
            treeViewFolder.TabIndex = 0;
            treeViewFolder.AfterSelect += treeViewFolder_AfterSelect;
            // 
            // contextMenuStrip_treeViewFolder
            // 
            contextMenuStrip_treeViewFolder.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_treeViewFolder.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_OpenFileManager, toolStripMenuItem_RenameDirectory, toolStripMenuItem_DeleteDirectory, toolStripMenuItem_ReCodeFolderName, toolStripMenuItem_ReCodeFullFoldersFilesName, toolStripMenuItem_FolderNameSim2Trad });
            contextMenuStrip_treeViewFolder.Name = "contextMenuStrip_treeViewFolder";
            contextMenuStrip_treeViewFolder.Size = new Size(429, 172);
            // 
            // toolStripMenuItem_OpenFileManager
            // 
            toolStripMenuItem_OpenFileManager.Name = "toolStripMenuItem_OpenFileManager";
            toolStripMenuItem_OpenFileManager.Size = new Size(428, 28);
            toolStripMenuItem_OpenFileManager.Text = "開啟檔案總管...";
            toolStripMenuItem_OpenFileManager.Click += toolStripMenuItem_OpenFileManager_Click;
            // 
            // toolStripMenuItem_RenameDirectory
            // 
            toolStripMenuItem_RenameDirectory.Name = "toolStripMenuItem_RenameDirectory";
            toolStripMenuItem_RenameDirectory.Size = new Size(428, 28);
            toolStripMenuItem_RenameDirectory.Text = "更改目錄名稱...";
            toolStripMenuItem_RenameDirectory.Click += toolStripMenuItem_RenameDirectory_Click;
            // 
            // toolStripMenuItem_DeleteDirectory
            // 
            toolStripMenuItem_DeleteDirectory.BackColor = Color.Pink;
            toolStripMenuItem_DeleteDirectory.Name = "toolStripMenuItem_DeleteDirectory";
            toolStripMenuItem_DeleteDirectory.Size = new Size(428, 28);
            toolStripMenuItem_DeleteDirectory.Text = "刪除目錄...";
            toolStripMenuItem_DeleteDirectory.Click += toolStripMenuItem_DeleteDirectory_Click;
            // 
            // toolStripMenuItem_ReCodeFolderName
            // 
            toolStripMenuItem_ReCodeFolderName.Name = "toolStripMenuItem_ReCodeFolderName";
            toolStripMenuItem_ReCodeFolderName.Size = new Size(428, 28);
            toolStripMenuItem_ReCodeFolderName.Text = "單一目錄名稱編碼(亂碼)轉換...";
            toolStripMenuItem_ReCodeFolderName.Click += toolStripMenuItem_ReCodeFolderName_Click;
            // 
            // toolStripMenuItem_ReCodeFullFoldersFilesName
            // 
            toolStripMenuItem_ReCodeFullFoldersFilesName.Name = "toolStripMenuItem_ReCodeFullFoldersFilesName";
            toolStripMenuItem_ReCodeFullFoldersFilesName.Size = new Size(428, 28);
            toolStripMenuItem_ReCodeFullFoldersFilesName.Text = "全目錄、次目錄與檔案名稱編碼(亂碼)轉換...";
            toolStripMenuItem_ReCodeFullFoldersFilesName.Click += toolStripMenuItem_ReCodeFullFoldersFilesName_Click;
            // 
            // toolStripMenuItem_FolderNameSim2Trad
            // 
            toolStripMenuItem_FolderNameSim2Trad.Name = "toolStripMenuItem_FolderNameSim2Trad";
            toolStripMenuItem_FolderNameSim2Trad.Size = new Size(428, 28);
            toolStripMenuItem_FolderNameSim2Trad.Text = "目錄名稱ＳＣ簡體->ＴＣ繁體";
            toolStripMenuItem_FolderNameSim2Trad.Click += toolStripMenuItem_FolderNameSim2Trad_Click;
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
            listViewFile.Size = new Size(497, 409);
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
            contextMenuStrip_ListViewFile.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_SaveTxtFile2, toolStripMenuItem_SaveTxtAsNewFile2, toolStripMenuItem_ConvertSimple, toolStripMenuItem_ConvertTraditional, toolStripSeparator15, toolStripMenuItem_SearchFiles, toolStripMenuItem_RenameFile, toolStripMenuItem_ReCodeFileName, toolStripMenuItem_FileNameSim2Trad, toolStripSeparator14, toolStripMenuItem_DelFiles });
            contextMenuStrip_ListViewFile.Name = "contextMenuStrip_ListViewFile";
            contextMenuStrip_ListViewFile.Size = new Size(421, 288);
            // 
            // toolStripMenuItem_SaveTxtFile2
            // 
            toolStripMenuItem_SaveTxtFile2.Enabled = false;
            toolStripMenuItem_SaveTxtFile2.Name = "toolStripMenuItem_SaveTxtFile2";
            toolStripMenuItem_SaveTxtFile2.Size = new Size(420, 28);
            toolStripMenuItem_SaveTxtFile2.Text = "儲存TXT檔案";
            toolStripMenuItem_SaveTxtFile2.Visible = false;
            toolStripMenuItem_SaveTxtFile2.Click += toolStripMenuItem_SaveTxtFile2_Click;
            // 
            // toolStripMenuItem_SaveTxtAsNewFile2
            // 
            toolStripMenuItem_SaveTxtAsNewFile2.Enabled = false;
            toolStripMenuItem_SaveTxtAsNewFile2.Name = "toolStripMenuItem_SaveTxtAsNewFile2";
            toolStripMenuItem_SaveTxtAsNewFile2.Size = new Size(420, 28);
            toolStripMenuItem_SaveTxtAsNewFile2.Text = "另存TXT新檔...";
            toolStripMenuItem_SaveTxtAsNewFile2.Visible = false;
            toolStripMenuItem_SaveTxtAsNewFile2.Click += toolStripMenuItem_SaveTxtAsNewFile2_Click;
            // 
            // toolStripMenuItem_ConvertSimple
            // 
            toolStripMenuItem_ConvertSimple.Name = "toolStripMenuItem_ConvertSimple";
            toolStripMenuItem_ConvertSimple.Size = new Size(420, 28);
            toolStripMenuItem_ConvertSimple.Text = "將選取檔案轉換成-ＳＣ簡體-並儲存新檔名";
            toolStripMenuItem_ConvertSimple.Click += toolStripButtonFileConvertToSimplified_Click;
            // 
            // toolStripMenuItem_ConvertTraditional
            // 
            toolStripMenuItem_ConvertTraditional.Name = "toolStripMenuItem_ConvertTraditional";
            toolStripMenuItem_ConvertTraditional.Size = new Size(420, 28);
            toolStripMenuItem_ConvertTraditional.Text = "將選取檔案轉換成-ＴＣ繁體-並儲存新檔名";
            toolStripMenuItem_ConvertTraditional.Click += toolStripMenuItem_ConvertTraditional_Click;
            // 
            // toolStripSeparator15
            // 
            toolStripSeparator15.Name = "toolStripSeparator15";
            toolStripSeparator15.Size = new Size(417, 6);
            // 
            // toolStripMenuItem_SearchFiles
            // 
            toolStripMenuItem_SearchFiles.Name = "toolStripMenuItem_SearchFiles";
            toolStripMenuItem_SearchFiles.ShortcutKeys = Keys.Control | Keys.F;
            toolStripMenuItem_SearchFiles.Size = new Size(420, 28);
            toolStripMenuItem_SearchFiles.Text = "尋找檔案";
            toolStripMenuItem_SearchFiles.Click += toolStripMenuItem_SearchFiles_Click;
            // 
            // toolStripMenuItem_RenameFile
            // 
            toolStripMenuItem_RenameFile.Name = "toolStripMenuItem_RenameFile";
            toolStripMenuItem_RenameFile.ShortcutKeys = Keys.F2;
            toolStripMenuItem_RenameFile.Size = new Size(420, 28);
            toolStripMenuItem_RenameFile.Text = "更名檔案";
            toolStripMenuItem_RenameFile.Click += toolStripMenuItem_RenameFile_Click;
            // 
            // toolStripMenuItem_ReCodeFileName
            // 
            toolStripMenuItem_ReCodeFileName.Name = "toolStripMenuItem_ReCodeFileName";
            toolStripMenuItem_ReCodeFileName.Size = new Size(420, 28);
            toolStripMenuItem_ReCodeFileName.Text = "檔名編碼(亂碼)轉換...";
            toolStripMenuItem_ReCodeFileName.Click += toolStripMenuItem_ReCodeFileName_Click;
            // 
            // toolStripMenuItem_FileNameSim2Trad
            // 
            toolStripMenuItem_FileNameSim2Trad.Name = "toolStripMenuItem_FileNameSim2Trad";
            toolStripMenuItem_FileNameSim2Trad.Size = new Size(420, 28);
            toolStripMenuItem_FileNameSim2Trad.Text = "檔案名稱ＳＣ簡體->ＴＣ繁體";
            toolStripMenuItem_FileNameSim2Trad.Click += toolStripMenuItem_FileNameSim2Trad_Click;
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new Size(417, 6);
            // 
            // toolStripMenuItem_DelFiles
            // 
            toolStripMenuItem_DelFiles.BackColor = Color.Pink;
            toolStripMenuItem_DelFiles.Margin = new Padding(0, 10, 0, 10);
            toolStripMenuItem_DelFiles.Name = "toolStripMenuItem_DelFiles";
            toolStripMenuItem_DelFiles.ShortcutKeys = Keys.Delete;
            toolStripMenuItem_DelFiles.Size = new Size(420, 28);
            toolStripMenuItem_DelFiles.Text = "Ｘ刪除檔案...";
            toolStripMenuItem_DelFiles.Click += toolStripMenuItem_DelFiles_Click;
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
            richTextBoxText.Size = new Size(1077, 707);
            richTextBoxText.TabIndex = 0;
            richTextBoxText.Text = resources.GetString("richTextBoxText.Text");
            // 
            // contextMenuStrip_RichTextBox
            // 
            contextMenuStrip_RichTextBox.ImageScalingSize = new Size(20, 20);
            contextMenuStrip_RichTextBox.Items.AddRange(new ToolStripItem[] { toolStripSeparator11, toolStripMenuItem_AutoSelectCR, toolStripMenuItem_AutoSelectWithPunctuation, toolStripSeparator12, toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR, toolStripMenuItem_RemoveCR, toolStripMenuItem_AutoRemoveCRButton2, toolStripSeparator19, toolStripMenuItem_AddSpaceAtBegining, toolStripMenuItem_RemoveMoreThan120CharB, toolStripMenuItem_EndingAddDot, toolStripMenuItem_MergeNoneSpace, toolStripSeparator20, toolStripMenuItem_SplitBeginingByJudgment, toolStripMenuItem_SplitEndByJudgment, toolStripMenuItem_MergeByJudgment, toolStripMenuItem_InsertBeginingEndByInsertText, toolStripSeparator10, toolStripMenuItem_WithoutCRBetweenLines, toolStripMenuItem_KeepTwoCRBetweenLines, toolStripSeparator8, toolStripMenuItem_EditTextCovertSimplified, toolStripMenuItem_EditTextCovertTraditional, toolStripSeparator9, toolStripMenuItem_SelectedTextSaveAsNew, toolStripMenuItem_WholeTextSaveAsNew, toolStripSeparator13, toolStripMenuItem_SortLines });
            contextMenuStrip_RichTextBox.Name = "contextMenuStrip_RichTextBox";
            contextMenuStrip_RichTextBox.Size = new Size(597, 612);
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_AutoSelectCR
            // 
            toolStripMenuItem_AutoSelectCR.Name = "toolStripMenuItem_AutoSelectCR";
            toolStripMenuItem_AutoSelectCR.ShortcutKeys = Keys.Control | Keys.E;
            toolStripMenuItem_AutoSelectCR.Size = new Size(596, 28);
            toolStripMenuItem_AutoSelectCR.Text = "自動選取直到空白行";
            toolStripMenuItem_AutoSelectCR.Click += toolStripMenuItem_AutoSelectCR_Click;
            // 
            // toolStripMenuItem_AutoSelectWithPunctuation
            // 
            toolStripMenuItem_AutoSelectWithPunctuation.Name = "toolStripMenuItem_AutoSelectWithPunctuation";
            toolStripMenuItem_AutoSelectWithPunctuation.ShortcutKeys = Keys.Control | Keys.D;
            toolStripMenuItem_AutoSelectWithPunctuation.Size = new Size(596, 28);
            toolStripMenuItem_AutoSelectWithPunctuation.Text = "自動選取直到空白行或句點或驚嘆號";
            toolStripMenuItem_AutoSelectWithPunctuation.Click += toolStripMenuItem_AutoSelectWithPunctuation_Click;
            // 
            // toolStripSeparator12
            // 
            toolStripSeparator12.Name = "toolStripSeparator12";
            toolStripSeparator12.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR
            // 
            toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR.Name = "toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR";
            toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR.Size = new Size(596, 28);
            toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR.Text = "＜Ｘ移除行首行尾的空白字元Ｘ＞";
            toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR.Click += toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR_Click;
            // 
            // toolStripMenuItem_RemoveCR
            // 
            toolStripMenuItem_RemoveCR.Name = "toolStripMenuItem_RemoveCR";
            toolStripMenuItem_RemoveCR.ShortcutKeys = Keys.Control | Keys.R;
            toolStripMenuItem_RemoveCR.Size = new Size(596, 28);
            toolStripMenuItem_RemoveCR.Text = "移除選取的文字斷行Ｘ》＼ｎ，將所選文字合併成同一行";
            toolStripMenuItem_RemoveCR.ToolTipText = "將所選文字合併成同一行";
            toolStripMenuItem_RemoveCR.Click += toolStripMenuItemRemoveLineBreaks_Click;
            // 
            // toolStripMenuItem_AutoRemoveCRButton2
            // 
            toolStripMenuItem_AutoRemoveCRButton2.Name = "toolStripMenuItem_AutoRemoveCRButton2";
            toolStripMenuItem_AutoRemoveCRButton2.Size = new Size(596, 28);
            toolStripMenuItem_AutoRemoveCRButton2.Text = "移除選取的文字斷行Ｘ》＼ｎ，保留空白行";
            toolStripMenuItem_AutoRemoveCRButton2.Click += toolStripMenuItem_AutoRemoveCRButton2_Click;
            // 
            // toolStripSeparator19
            // 
            toolStripSeparator19.Name = "toolStripSeparator19";
            toolStripSeparator19.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_AddSpaceAtBegining
            // 
            toolStripMenuItem_AddSpaceAtBegining.Name = "toolStripMenuItem_AddSpaceAtBegining";
            toolStripMenuItem_AddSpaceAtBegining.Size = new Size(596, 28);
            toolStripMenuItem_AddSpaceAtBegining.Text = "每行開頭增加空白字元";
            toolStripMenuItem_AddSpaceAtBegining.Click += toolStripMenuItem_AddSpaceAtBegining_Click;
            // 
            // toolStripMenuItem_RemoveMoreThan120CharB
            // 
            toolStripMenuItem_RemoveMoreThan120CharB.Name = "toolStripMenuItem_RemoveMoreThan120CharB";
            toolStripMenuItem_RemoveMoreThan120CharB.ShortcutKeys = Keys.Control | Keys.T;
            toolStripMenuItem_RemoveMoreThan120CharB.Size = new Size(596, 28);
            toolStripMenuItem_RemoveMoreThan120CharB.Text = "超過120個字尾是句點就自動分行，避免單行過長";
            toolStripMenuItem_RemoveMoreThan120CharB.Click += toolStripMenuItem_RemoveMoreThan120CharB_Click;
            // 
            // toolStripMenuItem_EndingAddDot
            // 
            toolStripMenuItem_EndingAddDot.Name = "toolStripMenuItem_EndingAddDot";
            toolStripMenuItem_EndingAddDot.Size = new Size(596, 28);
            toolStripMenuItem_EndingAddDot.Text = "若每行結尾不是句點問號等等，就加上句點";
            toolStripMenuItem_EndingAddDot.Click += toolStripMenuItem_EndingAddDot_Click;
            // 
            // toolStripMenuItem_MergeNoneSpace
            // 
            toolStripMenuItem_MergeNoneSpace.Enabled = false;
            toolStripMenuItem_MergeNoneSpace.Name = "toolStripMenuItem_MergeNoneSpace";
            toolStripMenuItem_MergeNoneSpace.ShortcutKeys = Keys.Control | Keys.M;
            toolStripMenuItem_MergeNoneSpace.Size = new Size(596, 28);
            toolStripMenuItem_MergeNoneSpace.Text = "若下一行文字之間無空格則合併";
            toolStripMenuItem_MergeNoneSpace.Click += toolStripMenuItem_MergeNoneSpace_Click;
            // 
            // toolStripSeparator20
            // 
            toolStripSeparator20.Name = "toolStripSeparator20";
            toolStripSeparator20.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_SplitBeginingByJudgment
            // 
            toolStripMenuItem_SplitBeginingByJudgment.Name = "toolStripMenuItem_SplitBeginingByJudgment";
            toolStripMenuItem_SplitBeginingByJudgment.Size = new Size(596, 28);
            toolStripMenuItem_SplitBeginingByJudgment.Text = "以選項視窗的《《開頭判定文字來分割新行";
            toolStripMenuItem_SplitBeginingByJudgment.Click += toolStripMenuItem_SplitBeginingByJudgment_Click;
            // 
            // toolStripMenuItem_SplitEndByJudgment
            // 
            toolStripMenuItem_SplitEndByJudgment.Name = "toolStripMenuItem_SplitEndByJudgment";
            toolStripMenuItem_SplitEndByJudgment.Size = new Size(596, 28);
            toolStripMenuItem_SplitEndByJudgment.Text = "以選項視窗的結尾》》判定文字來分割新行";
            toolStripMenuItem_SplitEndByJudgment.Click += toolStripMenuItem_SplitEndByJudgment_Click;
            // 
            // toolStripMenuItem_MergeByJudgment
            // 
            toolStripMenuItem_MergeByJudgment.Name = "toolStripMenuItem_MergeByJudgment";
            toolStripMenuItem_MergeByJudgment.Size = new Size(596, 28);
            toolStripMenuItem_MergeByJudgment.Text = "以選項視窗的《《開頭至結尾》》判定字串，合併成同一行";
            toolStripMenuItem_MergeByJudgment.Click += toolStripMenuItem_MergeByJudgment_Click;
            // 
            // toolStripMenuItem_InsertBeginingEndByInsertText
            // 
            toolStripMenuItem_InsertBeginingEndByInsertText.Name = "toolStripMenuItem_InsertBeginingEndByInsertText";
            toolStripMenuItem_InsertBeginingEndByInsertText.Size = new Size(596, 28);
            toolStripMenuItem_InsertBeginingEndByInsertText.Text = "插入選項視窗的插入每行開頭與結尾文字";
            toolStripMenuItem_InsertBeginingEndByInsertText.Click += toolStripMenuItem_InsertBeginingEndByInsertText_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_WithoutCRBetweenLines
            // 
            toolStripMenuItem_WithoutCRBetweenLines.Name = "toolStripMenuItem_WithoutCRBetweenLines";
            toolStripMenuItem_WithoutCRBetweenLines.Size = new Size(596, 28);
            toolStripMenuItem_WithoutCRBetweenLines.Text = "【Ｘ】段落之間消除任何空行";
            toolStripMenuItem_WithoutCRBetweenLines.Click += toolStripMenuItem_WithoutCRBetweenLines_Click;
            // 
            // toolStripMenuItem_KeepTwoCRBetweenLines
            // 
            toolStripMenuItem_KeepTwoCRBetweenLines.Name = "toolStripMenuItem_KeepTwoCRBetweenLines";
            toolStripMenuItem_KeepTwoCRBetweenLines.Size = new Size(596, 28);
            toolStripMenuItem_KeepTwoCRBetweenLines.Text = "【　】段落之間保有一個空行";
            toolStripMenuItem_KeepTwoCRBetweenLines.Click += toolStripMenuItem_KeepTwoCRBetweenLines_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_EditTextCovertSimplified
            // 
            toolStripMenuItem_EditTextCovertSimplified.Name = "toolStripMenuItem_EditTextCovertSimplified";
            toolStripMenuItem_EditTextCovertSimplified.Size = new Size(596, 28);
            toolStripMenuItem_EditTextCovertSimplified.Text = "轉換成ＳＣ簡體字";
            toolStripMenuItem_EditTextCovertSimplified.Click += toolStripMenuItem_EditTextCovertSimplified_Click;
            // 
            // toolStripMenuItem_EditTextCovertTraditional
            // 
            toolStripMenuItem_EditTextCovertTraditional.Name = "toolStripMenuItem_EditTextCovertTraditional";
            toolStripMenuItem_EditTextCovertTraditional.Size = new Size(596, 28);
            toolStripMenuItem_EditTextCovertTraditional.Text = "轉換成ＴＣ繁體字";
            toolStripMenuItem_EditTextCovertTraditional.Click += toolStripMenuItem_EditTextCovertTraditional_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_SelectedTextSaveAsNew
            // 
            toolStripMenuItem_SelectedTextSaveAsNew.Name = "toolStripMenuItem_SelectedTextSaveAsNew";
            toolStripMenuItem_SelectedTextSaveAsNew.Size = new Size(596, 28);
            toolStripMenuItem_SelectedTextSaveAsNew.Text = "將選取文字另存新檔...";
            toolStripMenuItem_SelectedTextSaveAsNew.Click += toolStripMenuItem_SelectedTextSaveAsNew_Click;
            // 
            // toolStripMenuItem_WholeTextSaveAsNew
            // 
            toolStripMenuItem_WholeTextSaveAsNew.Name = "toolStripMenuItem_WholeTextSaveAsNew";
            toolStripMenuItem_WholeTextSaveAsNew.Size = new Size(596, 28);
            toolStripMenuItem_WholeTextSaveAsNew.Text = "以3000字為單位將整篇文字另存成多個新檔案...";
            toolStripMenuItem_WholeTextSaveAsNew.Click += toolStripMenuItem_WholeTextSaveAsNew_Click;
            // 
            // toolStripSeparator13
            // 
            toolStripSeparator13.Name = "toolStripSeparator13";
            toolStripSeparator13.Size = new Size(593, 6);
            // 
            // toolStripMenuItem_SortLines
            // 
            toolStripMenuItem_SortLines.Name = "toolStripMenuItem_SortLines";
            toolStripMenuItem_SortLines.Size = new Size(596, 28);
            toolStripMenuItem_SortLines.Text = "逐行排序...";
            toolStripMenuItem_SortLines.Click += toolStripMenuItem_SortLines_Click;
            // 
            // webBrowser1
            // 
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Location = new Point(0, 0);
            webBrowser1.MinimumSize = new Size(20, 20);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.Size = new Size(1077, 707);
            webBrowser1.TabIndex = 1;
            // 
            // navigationBar
            // 
            navigationBar.AutoSize = false;
            navigationBar.CanOverflow = false;
            navigationBar.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navigationBar.ImageScalingSize = new Size(24, 24);
            navigationBar.Items.AddRange(new ToolStripItem[] { ShowFolderButton, toolStripSeparator2, toolStripDropDownButtonHistoryList, toolStripDropDownButtonArrange, toolStripSeparator6, toolStripDropDownButtonSave, toolStripSeparator1, toolStripButton_Option, toolStripSeparator18, toolStripComboBoxFonts, toolStripSeparator7, FontSizeAddButton, toolStripSeparator3, FontSizeReduceButton, toolStripSeparator4, toolStripButtonHTMLChangeFontChecker, QuitButton, toolStripSeparator5, toolStripComboBoxHistoryList });
            navigationBar.LayoutStyle = ToolStripLayoutStyle.Flow;
            navigationBar.Location = new Point(0, 0);
            navigationBar.Name = "navigationBar";
            navigationBar.Size = new Size(1584, 32);
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
            // toolStripDropDownButtonHistoryList
            // 
            toolStripDropDownButtonHistoryList.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButtonHistoryList.Image = (Image)resources.GetObject("toolStripDropDownButtonHistoryList.Image");
            toolStripDropDownButtonHistoryList.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButtonHistoryList.Name = "toolStripDropDownButtonHistoryList";
            toolStripDropDownButtonHistoryList.Size = new Size(106, 29);
            toolStripDropDownButtonHistoryList.Text = "歷史清單";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 23);
            // 
            // toolStripDropDownButtonSave
            // 
            toolStripDropDownButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButtonSave.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItem_SaveTxtFile, toolStripMenuItem_SaveTxtAsNewFile, toolStripSeparator16, toolStripMenuItem_ConvertToSimplified, toolStripMenuItem_FileConvertToSimplified, toolStripMenuItem_FileConvertToTraditional, toolStripSeparator17, toolStripMenuItem_CopyHtmlSaveFileSimplified, toolStripMenuItem_CopyHtmlSaveFile });
            toolStripDropDownButtonSave.Image = (Image)resources.GetObject("toolStripDropDownButtonSave.Image");
            toolStripDropDownButtonSave.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButtonSave.Name = "toolStripDropDownButtonSave";
            toolStripDropDownButtonSave.Size = new Size(106, 29);
            toolStripDropDownButtonSave.Text = "檔案儲存";
            // 
            // toolStripMenuItem_SaveTxtFile
            // 
            toolStripMenuItem_SaveTxtFile.Name = "toolStripMenuItem_SaveTxtFile";
            toolStripMenuItem_SaveTxtFile.ShortcutKeys = Keys.Control | Keys.S;
            toolStripMenuItem_SaveTxtFile.Size = new Size(631, 30);
            toolStripMenuItem_SaveTxtFile.Text = "儲存TXT檔案";
            toolStripMenuItem_SaveTxtFile.Click += toolStripMenuItem_SaveTxtFile_Click;
            // 
            // toolStripMenuItem_SaveTxtAsNewFile
            // 
            toolStripMenuItem_SaveTxtAsNewFile.Name = "toolStripMenuItem_SaveTxtAsNewFile";
            toolStripMenuItem_SaveTxtAsNewFile.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            toolStripMenuItem_SaveTxtAsNewFile.Size = new Size(631, 30);
            toolStripMenuItem_SaveTxtAsNewFile.Text = "另存TXT新檔...";
            toolStripMenuItem_SaveTxtAsNewFile.Click += toolStripMenuItem_SaveTxtAsNewFile_Click;
            // 
            // toolStripSeparator16
            // 
            toolStripSeparator16.Name = "toolStripSeparator16";
            toolStripSeparator16.Size = new Size(628, 6);
            // 
            // toolStripMenuItem_ConvertToSimplified
            // 
            toolStripMenuItem_ConvertToSimplified.Name = "toolStripMenuItem_ConvertToSimplified";
            toolStripMenuItem_ConvertToSimplified.Size = new Size(631, 30);
            toolStripMenuItem_ConvertToSimplified.Text = "將目前TXT轉換成簡體並儲存新檔名";
            toolStripMenuItem_ConvertToSimplified.Click += toolStripMenuItem_ConvertToSimplified_Click;
            // 
            // toolStripMenuItem_FileConvertToSimplified
            // 
            toolStripMenuItem_FileConvertToSimplified.Name = "toolStripMenuItem_FileConvertToSimplified";
            toolStripMenuItem_FileConvertToSimplified.Size = new Size(631, 30);
            toolStripMenuItem_FileConvertToSimplified.Text = "將目錄清單中選取的TXT檔案轉換成-ＳＣ簡體-並儲存新檔名";
            toolStripMenuItem_FileConvertToSimplified.Click += toolStripMenuItem_FileConvertToSimplified_Click;
            // 
            // toolStripMenuItem_FileConvertToTraditional
            // 
            toolStripMenuItem_FileConvertToTraditional.Name = "toolStripMenuItem_FileConvertToTraditional";
            toolStripMenuItem_FileConvertToTraditional.Size = new Size(631, 30);
            toolStripMenuItem_FileConvertToTraditional.Text = "將目錄清單中選取的TXT檔案轉換成-ＴＣ繁體-並儲存新檔名";
            toolStripMenuItem_FileConvertToTraditional.Click += toolStripMenuItem_FileConvertToTraditional_Click;
            // 
            // toolStripSeparator17
            // 
            toolStripSeparator17.Name = "toolStripSeparator17";
            toolStripSeparator17.Size = new Size(628, 6);
            // 
            // toolStripMenuItem_CopyHtmlSaveFileSimplified
            // 
            toolStripMenuItem_CopyHtmlSaveFileSimplified.Name = "toolStripMenuItem_CopyHtmlSaveFileSimplified";
            toolStripMenuItem_CopyHtmlSaveFileSimplified.Size = new Size(631, 30);
            toolStripMenuItem_CopyHtmlSaveFileSimplified.Text = "複製HTML文字並儲存TXT檔案(ＳＣ簡體)";
            toolStripMenuItem_CopyHtmlSaveFileSimplified.Click += toolStripMenuItem_CopyHtmlSaveFileSimplified_Click;
            // 
            // toolStripMenuItem_CopyHtmlSaveFile
            // 
            toolStripMenuItem_CopyHtmlSaveFile.Name = "toolStripMenuItem_CopyHtmlSaveFile";
            toolStripMenuItem_CopyHtmlSaveFile.Size = new Size(631, 30);
            toolStripMenuItem_CopyHtmlSaveFile.Text = "複製HTML文字並儲存TXT檔案(ＴＣ繁體)";
            toolStripMenuItem_CopyHtmlSaveFile.Click += toolStripMenuItem_CopyHtmlSaveFile_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 23);
            // 
            // toolStripButton_Option
            // 
            toolStripButton_Option.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton_Option.Image = (Image)resources.GetObject("toolStripButton_Option.Image");
            toolStripButton_Option.ImageTransparentColor = Color.Magenta;
            toolStripButton_Option.Name = "toolStripButton_Option";
            toolStripButton_Option.Size = new Size(56, 29);
            toolStripButton_Option.Text = "選項";
            toolStripButton_Option.ToolTipText = "選項";
            toolStripButton_Option.Click += toolStripButton_Option_Click;
            // 
            // toolStripSeparator18
            // 
            toolStripSeparator18.Name = "toolStripSeparator18";
            toolStripSeparator18.Size = new Size(6, 23);
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
            // toolStripButtonHTMLChangeFontChecker
            // 
            toolStripButtonHTMLChangeFontChecker.Checked = true;
            toolStripButtonHTMLChangeFontChecker.CheckState = CheckState.Checked;
            toolStripButtonHTMLChangeFontChecker.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonHTMLChangeFontChecker.Image = (Image)resources.GetObject("toolStripButtonHTMLChangeFontChecker.Image");
            toolStripButtonHTMLChangeFontChecker.ImageTransparentColor = Color.Magenta;
            toolStripButtonHTMLChangeFontChecker.Name = "toolStripButtonHTMLChangeFontChecker";
            toolStripButtonHTMLChangeFontChecker.Size = new Size(212, 29);
            toolStripButtonHTMLChangeFontChecker.Text = "✔改變HTML字體底色";
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
            QuitButton.Visible = false;
            QuitButton.Click += QuitButton_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 23);
            // 
            // toolStripComboBoxHistoryList
            // 
            toolStripComboBoxHistoryList.Enabled = false;
            toolStripComboBoxHistoryList.Name = "toolStripComboBoxHistoryList";
            toolStripComboBoxHistoryList.Size = new Size(180, 31);
            toolStripComboBoxHistoryList.Text = "目錄與檔案歷史清單";
            toolStripComboBoxHistoryList.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabelNews, toolStripStatusLabelFileName, toolStripStatusLabelFixed });
            statusStrip1.Location = new Point(0, 732);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1584, 29);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelNews
            // 
            toolStripStatusLabelNews.AutoSize = false;
            toolStripStatusLabelNews.Name = "toolStripStatusLabelNews";
            toolStripStatusLabelNews.Size = new Size(600, 23);
            toolStripStatusLabelNews.Text = "更新訊息";
            toolStripStatusLabelNews.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFileName
            // 
            toolStripStatusLabelFileName.AutoSize = false;
            toolStripStatusLabelFileName.BackColor = SystemColors.ControlLight;
            toolStripStatusLabelFileName.Name = "toolStripStatusLabelFileName";
            toolStripStatusLabelFileName.Size = new Size(600, 23);
            toolStripStatusLabelFileName.Text = "檔名";
            toolStripStatusLabelFileName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabelFixed
            // 
            toolStripStatusLabelFixed.Name = "toolStripStatusLabelFixed";
            toolStripStatusLabelFixed.Size = new Size(82, 23);
            toolStripStatusLabelFixed.Text = "狀態訊息";
            toolStripStatusLabelFixed.TextAlign = ContentAlignment.MiddleRight;
            // 
            // FormTextSpeedReader
            // 
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 761);
            Controls.Add(statusStrip1);
            Controls.Add(navigationBar);
            Controls.Add(splitContainerMain);
            Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "FormTextSpeedReader";
            Text = "TextSpeedReader Ver. 2.4.0.0  (新增批次目錄名稱編碼(亂碼)轉換、檔名編碼(亂碼)轉換、檔名簡轉繁，逐行排序、關鍵字斷行、行首填空、保留字型大小、全新架構，單一執行檔)";
            FormClosing += FormTSRClosing;
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            splitContainerFolder.Panel1.ResumeLayout(false);
            splitContainerFolder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerFolder).EndInit();
            splitContainerFolder.ResumeLayout(false);
            contextMenuStrip_treeViewFolder.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TreeView treeViewFolder;
        private System.Windows.Forms.ListView listViewFile;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private ToolStripButton QuitButton;
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
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSeparator toolStripSeparator4;
        private ContextMenuStrip contextMenuStrip_ListViewFile;
        private ToolStripMenuItem toolStripMenuItem_ConvertSimple;
        private ToolStripSeparator toolStripSeparator5;
        private StatusStrip statusStrip1;
        private ToolStripDropDownButton toolStripDropDownButtonArrange;
        private ToolStripMenuItem toolStripMenuItem_MergeNoneSpace;
        private ToolStripMenuItem toolStripMenuItem_AutoSelectWithPunctuation;
        private ToolStripSeparator toolStripSeparator6;
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
        private ToolStripMenuItem toolStripMenuItem_SaveTxtFile;
        private ToolStripMenuItem toolStripMenuItem_SaveTxtAsNewFile;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripMenuItem toolStripMenuItem_RemoveLeadingAndTrailingSpacesRR;
        private ToolStripMenuItem toolStripMenuItem_EndingAddDot;
        private ToolStripButton toolStripButton_Option;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripMenuItem toolStripMenuItem_CopyHtmlSaveFileSimplified;
        private ContextMenuStrip contextMenuStrip_treeViewFolder;
        private ToolStripMenuItem toolStripMenuItem_RenameDirectory;
        private ToolStripMenuItem toolStripMenuItem_OpenFileManager;
        private ToolStripMenuItem toolStripMenuItem_AutoRemoveCRButton2;
        private ToolStripMenuItem toolStripMenuItem_SaveTxtFile2;
        private ToolStripMenuItem toolStripMenuItem_SaveTxtAsNewFile2;
        private ToolStripMenuItem toolStripMenuItem_AddSpaceAtBegining;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem toolStripMenuItem_SplitBeginingByJudgment;
        private ToolStripMenuItem toolStripMenuItem_SplitEndByJudgment;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem toolStripMenuItem_MergeByJudgment;
        private ToolStripMenuItem toolStripMenuItem_SortLines;
        private ToolStripComboBox toolStripComboBoxHistoryList;
        private ToolStripDropDownButton toolStripDropDownButtonHistoryList;
        private ToolStripButton toolStripButtonHTMLChangeFontChecker;
        private ToolStripMenuItem toolStripMenuItem_InsertBeginingEndByInsertText;
        private ToolStripMenuItem toolStripMenuItem_DeleteDirectory;
        private ToolStripMenuItem toolStripMenuItem_ReCodeFileName;
        private ToolStripMenuItem toolStripMenuItem_ReCodeFolderName;
        private ToolStripMenuItem toolStripMenuItem_FolderNameSim2Trad;
        private ToolStripMenuItem toolStripMenuItem_FileNameSim2Trad;
        private ToolStripMenuItem toolStripMenuItem_ReCodeFullFoldersFilesName;
    }
}


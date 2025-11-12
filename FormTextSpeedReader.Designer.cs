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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTextSpeedReader));
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
            listViewRecentFiles = new ListView();
            columnHeaderFileName = new ColumnHeader();
            columnHeaderCharCount = new ColumnHeader();
            richTextBoxText = new RichTextBox();
            contextMenuStrip_RichTextBox = new ContextMenuStrip(components);
            toolStripMenuItem_RemoveCR = new ToolStripMenuItem();
            toolStripMenuItem_AutoSelectCR = new ToolStripMenuItem();
            toolStripMenuItem_MergeNoneSpace = new ToolStripMenuItem();
            webBrowser1 = new WebBrowser();
            navigationBar = new ToolStrip();
            ShowFolderButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            RemoveLeadSpace = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            AutoRemoveCRButton = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            buttonConvertToSimplified = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripButtonFileConvertToSimplified = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            toolStripButtonCopyHtmlSaveFile = new ToolStripButton();
            FolderPathButton = new ToolStripButton();
            navBackButton = new ToolStripSplitButton();
            navForwardButton = new ToolStripSplitButton();
            navUpButton = new ToolStripButton();
            navAddressLabel = new ToolStripLabel();
            navFoldersButton = new ToolStripButton();
            toolStripComboBoxFonts = new ToolStripComboBox();
            FontSizeAddButton = new ToolStripButton();
            FontSizeReduceButton = new ToolStripButton();
            QuitButton = new ToolStripButton();
            toolStripSplitButton1 = new ToolStripSplitButton();
            statusStrip1 = new StatusStrip();
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
            SuspendLayout();
            // 
            // splitContainerMain
            // 
            splitContainerMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerMain.Location = new Point(0, 69);
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
            splitContainerMain.Size = new Size(1300, 658);
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
            splitContainerFolder.Size = new Size(500, 658);
            splitContainerFolder.SplitterDistance = 273;
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
            treeViewFolder.Size = new Size(500, 273);
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
            listViewFile.Size = new Size(500, 379);
            listViewFile.SmallImageList = imageList1;
            listViewFile.TabIndex = 0;
            listViewFile.UseCompatibleStateImageBehavior = false;
            listViewFile.View = View.Details;
            listViewFile.ColumnClick += listViewFile_ColumnClick;
            listViewFile.SelectedIndexChanged += ListViewFile_SelectedIndexChanged;
            listViewFile.KeyDown += listViewFile_KeyDown;
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
            contextMenuStrip_ListViewFile.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_ConvertSimple });
            contextMenuStrip_ListViewFile.Name = "contextMenuStrip_ListViewFile";
            contextMenuStrip_ListViewFile.Size = new Size(335, 28);
            // 
            // toolStripMenuItem_ConvertSimple
            // 
            toolStripMenuItem_ConvertSimple.Name = "toolStripMenuItem_ConvertSimple";
            toolStripMenuItem_ConvertSimple.Size = new Size(334, 24);
            toolStripMenuItem_ConvertSimple.Text = "將選取檔案轉換成簡體並儲存新檔名";
            toolStripMenuItem_ConvertSimple.Click += toolStripButtonFileConvertToSimplified_Click;
            // 
            // listViewRecentFiles
            // 
            listViewRecentFiles.AutoArrange = false;
            listViewRecentFiles.Columns.AddRange(new ColumnHeader[] { columnHeaderFileName, columnHeaderCharCount });
            listViewRecentFiles.Location = new Point(0, 277);
            listViewRecentFiles.Name = "listViewRecentFiles";
            listViewRecentFiles.Size = new Size(852, 262);
            listViewRecentFiles.TabIndex = 1;
            listViewRecentFiles.UseCompatibleStateImageBehavior = false;
            listViewRecentFiles.View = View.Details;
            listViewRecentFiles.Visible = false;
            // 
            // columnHeaderFileName
            // 
            columnHeaderFileName.Text = "檔名";
            columnHeaderFileName.Width = 720;
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
            richTextBoxText.ForeColor = SystemColors.Window;
            richTextBoxText.Location = new Point(0, 0);
            richTextBoxText.Name = "richTextBoxText";
            richTextBoxText.Size = new Size(794, 658);
            richTextBoxText.TabIndex = 0;
            richTextBoxText.Text = "";
            // 
            // contextMenuStrip_RichTextBox
            // 
            contextMenuStrip_RichTextBox.Items.AddRange(new ToolStripItem[] { toolStripMenuItem_RemoveCR, toolStripMenuItem_AutoSelectCR, toolStripMenuItem_MergeNoneSpace });
            contextMenuStrip_RichTextBox.Name = "contextMenuStrip_RichTextBox";
            contextMenuStrip_RichTextBox.Size = new Size(366, 98);
            // 
            // toolStripMenuItem_RemoveCR
            // 
            toolStripMenuItem_RemoveCR.Name = "toolStripMenuItem_RemoveCR";
            toolStripMenuItem_RemoveCR.ShortcutKeys = Keys.Control | Keys.R;
            toolStripMenuItem_RemoveCR.Size = new Size(365, 24);
            toolStripMenuItem_RemoveCR.Text = "移除選取的文字斷行";
            toolStripMenuItem_RemoveCR.ToolTipText = "將所選文字合併成同一行";
            toolStripMenuItem_RemoveCR.Click += toolStripMenuItemRemoveLineBreaks_Click;
            // 
            // toolStripMenuItem_AutoSelectCR
            // 
            toolStripMenuItem_AutoSelectCR.Name = "toolStripMenuItem_AutoSelectCR";
            toolStripMenuItem_AutoSelectCR.ShortcutKeys = Keys.Control | Keys.E;
            toolStripMenuItem_AutoSelectCR.Size = new Size(365, 24);
            toolStripMenuItem_AutoSelectCR.Text = "自動選取直到空白行";
            toolStripMenuItem_AutoSelectCR.Click += toolStripMenuItem_AutoSelectCR_Click;
            // 
            // toolStripMenuItem_MergeNoneSpace
            // 
            toolStripMenuItem_MergeNoneSpace.Enabled = false;
            toolStripMenuItem_MergeNoneSpace.Name = "toolStripMenuItem_MergeNoneSpace";
            toolStripMenuItem_MergeNoneSpace.ShortcutKeys = Keys.Control | Keys.M;
            toolStripMenuItem_MergeNoneSpace.Size = new Size(365, 24);
            toolStripMenuItem_MergeNoneSpace.Text = "若下一行文字之間無空格則合併";
            toolStripMenuItem_MergeNoneSpace.Click += toolStripMenuItem_MergeNoneSpace_Click;
            // 
            // webBrowser1
            // 
            webBrowser1.Dock = DockStyle.Fill;
            webBrowser1.Location = new Point(0, 0);
            webBrowser1.MinimumSize = new Size(20, 20);
            webBrowser1.Name = "webBrowser1";
            webBrowser1.Size = new Size(794, 658);
            webBrowser1.TabIndex = 1;
            // 
            // navigationBar
            // 
            navigationBar.CanOverflow = false;
            navigationBar.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            navigationBar.ImageScalingSize = new Size(24, 24);
            navigationBar.Items.AddRange(new ToolStripItem[] { ShowFolderButton, toolStripSeparator2, RemoveLeadSpace, toolStripSeparator1, AutoRemoveCRButton, toolStripSeparator3, buttonConvertToSimplified, toolStripSeparator4, toolStripButtonFileConvertToSimplified, toolStripSeparator5, toolStripButtonCopyHtmlSaveFile, FolderPathButton, navBackButton, navForwardButton, navUpButton, navAddressLabel, navFoldersButton, toolStripComboBoxFonts, FontSizeAddButton, FontSizeReduceButton, QuitButton, toolStripSplitButton1 });
            navigationBar.LayoutStyle = ToolStripLayoutStyle.Flow;
            navigationBar.Location = new Point(0, 0);
            navigationBar.Name = "navigationBar";
            navigationBar.Size = new Size(1304, 58);
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
            ShowFolderButton.Size = new Size(93, 24);
            ShowFolderButton.Text = "顯示資料夾";
            ShowFolderButton.Click += ShowFolderButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 23);
            // 
            // RemoveLeadSpace
            // 
            RemoveLeadSpace.DisplayStyle = ToolStripItemDisplayStyle.Text;
            RemoveLeadSpace.ImageTransparentColor = Color.Magenta;
            RemoveLeadSpace.Name = "RemoveLeadSpace";
            RemoveLeadSpace.Size = new Size(189, 24);
            RemoveLeadSpace.Text = "移除行首行尾的空白字元";
            RemoveLeadSpace.ToolTipText = "移除行首的空白字元";
            RemoveLeadSpace.Click += RemoveLeadSpace_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 23);
            // 
            // AutoRemoveCRButton
            // 
            AutoRemoveCRButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AutoRemoveCRButton.ImageTransparentColor = Color.Magenta;
            AutoRemoveCRButton.Name = "AutoRemoveCRButton";
            AutoRemoveCRButton.Size = new Size(157, 24);
            AutoRemoveCRButton.Text = "自動移除多餘的斷行";
            AutoRemoveCRButton.ToolTipText = "自動移除沒必要的斷行";
            AutoRemoveCRButton.Click += AutoRemoveCRButton_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 23);
            // 
            // buttonConvertToSimplified
            // 
            buttonConvertToSimplified.DisplayStyle = ToolStripItemDisplayStyle.Text;
            buttonConvertToSimplified.Image = (Image)resources.GetObject("buttonConvertToSimplified.Image");
            buttonConvertToSimplified.ImageTransparentColor = Color.Magenta;
            buttonConvertToSimplified.Name = "buttonConvertToSimplified";
            buttonConvertToSimplified.Size = new Size(265, 24);
            buttonConvertToSimplified.Text = "將目前TXT轉換成簡體並儲存新檔名";
            buttonConvertToSimplified.Click += buttonConvertToSimplified_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 23);
            // 
            // toolStripButtonFileConvertToSimplified
            // 
            toolStripButtonFileConvertToSimplified.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonFileConvertToSimplified.Image = (Image)resources.GetObject("toolStripButtonFileConvertToSimplified.Image");
            toolStripButtonFileConvertToSimplified.ImageTransparentColor = Color.Magenta;
            toolStripButtonFileConvertToSimplified.Name = "toolStripButtonFileConvertToSimplified";
            toolStripButtonFileConvertToSimplified.Size = new Size(393, 24);
            toolStripButtonFileConvertToSimplified.Text = "將目錄清單中選取的TXT檔案轉換成簡體並儲存新檔名";
            toolStripButtonFileConvertToSimplified.Click += toolStripButtonFileConvertToSimplified_Click;
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 23);
            // 
            // toolStripButtonCopyHtmlSaveFile
            // 
            toolStripButtonCopyHtmlSaveFile.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonCopyHtmlSaveFile.Image = (Image)resources.GetObject("toolStripButtonCopyHtmlSaveFile.Image");
            toolStripButtonCopyHtmlSaveFile.ImageTransparentColor = Color.Magenta;
            toolStripButtonCopyHtmlSaveFile.Name = "toolStripButtonCopyHtmlSaveFile";
            toolStripButtonCopyHtmlSaveFile.Size = new Size(229, 24);
            toolStripButtonCopyHtmlSaveFile.Text = "複製HTML文字並儲存TXT檔案";
            toolStripButtonCopyHtmlSaveFile.Click += toolStripButtonCopyHtmlSaveFile_Click;
            // 
            // FolderPathButton
            // 
            FolderPathButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FolderPathButton.Image = (Image)resources.GetObject("FolderPathButton.Image");
            FolderPathButton.ImageTransparentColor = Color.Magenta;
            FolderPathButton.Name = "FolderPathButton";
            FolderPathButton.Size = new Size(101, 28);
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
            navBackButton.Size = new Size(40, 28);
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
            navForwardButton.Size = new Size(40, 28);
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
            navUpButton.Size = new Size(28, 28);
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
            navAddressLabel.Size = new Size(69, 20);
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
            navFoldersButton.Size = new Size(28, 28);
            navFoldersButton.Text = "Folders";
            navFoldersButton.TextImageRelation = TextImageRelation.ImageAboveText;
            navFoldersButton.Visible = false;
            // 
            // toolStripComboBoxFonts
            // 
            toolStripComboBoxFonts.Alignment = ToolStripItemAlignment.Right;
            toolStripComboBoxFonts.DropDownHeight = 600;
            toolStripComboBoxFonts.DropDownWidth = 240;
            toolStripComboBoxFonts.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            toolStripComboBoxFonts.IntegralHeight = false;
            toolStripComboBoxFonts.Name = "toolStripComboBoxFonts";
            toolStripComboBoxFonts.Size = new Size(240, 28);
            toolStripComboBoxFonts.Text = "字型";
            toolStripComboBoxFonts.SelectedIndexChanged += ChangeFont;
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
            FontSizeAddButton.Size = new Size(28, 28);
            FontSizeAddButton.Text = "FontSizeAddButton";
            FontSizeAddButton.ToolTipText = "放大字體";
            FontSizeAddButton.Click += FontSizeAdd;
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
            FontSizeReduceButton.Size = new Size(28, 28);
            FontSizeReduceButton.Text = "FontSizeReduceButton";
            FontSizeReduceButton.ToolTipText = "縮小字型";
            FontSizeReduceButton.Click += FontSizeReduce;
            // 
            // QuitButton
            // 
            QuitButton.Alignment = ToolStripItemAlignment.Right;
            QuitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            QuitButton.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            QuitButton.Image = (Image)resources.GetObject("QuitButton.Image");
            QuitButton.ImageTransparentColor = Color.Magenta;
            QuitButton.Name = "QuitButton";
            QuitButton.Size = new Size(28, 28);
            QuitButton.Text = "Quit";
            QuitButton.Click += QuitButton_Click;
            // 
            // toolStripSplitButton1
            // 
            toolStripSplitButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripSplitButton1.Image = (Image)resources.GetObject("toolStripSplitButton1.Image");
            toolStripSplitButton1.ImageTransparentColor = Color.Magenta;
            toolStripSplitButton1.Name = "toolStripSplitButton1";
            toolStripSplitButton1.Size = new Size(184, 24);
            toolStripSplitButton1.Text = "toolStripSplitButton1";
            toolStripSplitButton1.Visible = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 730);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1304, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // FormTextSpeedReader
            // 
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1304, 752);
            Controls.Add(statusStrip1);
            Controls.Add(navigationBar);
            Controls.Add(splitContainerMain);
            Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "FormTextSpeedReader";
            Text = "TextSpeedReader";
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
        private ToolStripSplitButton toolStripSplitButton1;
        private ToolStripMenuItem toolStripMenuItem_MergeNoneSpace;
    }
}


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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTextSpeedReader));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerFolder = new System.Windows.Forms.SplitContainer();
            this.treeViewFolder = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listViewFile = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewRecentFiles = new System.Windows.Forms.ListView();
            this.columnHeaderFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCharCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.richTextBoxText = new System.Windows.Forms.RichTextBox();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.navigationBar = new System.Windows.Forms.ToolStrip();
            this.ShowFolderButton = new System.Windows.Forms.ToolStripButton();
            this.FolderPathButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.navBackButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navForwardButton = new System.Windows.Forms.ToolStripSplitButton();
            this.navUpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.navFoldersButton = new System.Windows.Forms.ToolStripButton();
            this.navAddressLabel = new System.Windows.Forms.ToolStripLabel();
            this.QuitButton = new System.Windows.Forms.ToolStripButton();
            this.FontSizeAddButton = new System.Windows.Forms.ToolStripButton();
            this.FontSizeReduceButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripComboBoxFonts = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFolder)).BeginInit();
            this.splitContainerFolder.Panel1.SuspendLayout();
            this.splitContainerFolder.Panel2.SuspendLayout();
            this.splitContainerFolder.SuspendLayout();
            this.navigationBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 33);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerFolder);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.listViewRecentFiles);
            this.splitContainerMain.Panel2.Controls.Add(this.richTextBoxText);
            this.splitContainerMain.Panel2.Controls.Add(this.webBrowser1);
            this.splitContainerMain.Size = new System.Drawing.Size(1300, 718);
            this.splitContainerMain.SplitterDistance = 400;
            this.splitContainerMain.SplitterWidth = 6;
            this.splitContainerMain.TabIndex = 4;
            // 
            // splitContainerFolder
            // 
            this.splitContainerFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFolder.Location = new System.Drawing.Point(0, 0);
            this.splitContainerFolder.Name = "splitContainerFolder";
            this.splitContainerFolder.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFolder.Panel1
            // 
            this.splitContainerFolder.Panel1.Controls.Add(this.treeViewFolder);
            // 
            // splitContainerFolder.Panel2
            // 
            this.splitContainerFolder.Panel2.Controls.Add(this.listViewFile);
            this.splitContainerFolder.Size = new System.Drawing.Size(400, 718);
            this.splitContainerFolder.SplitterDistance = 300;
            this.splitContainerFolder.SplitterWidth = 6;
            this.splitContainerFolder.TabIndex = 0;
            // 
            // treeViewFolder
            // 
            this.treeViewFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFolder.ImageIndex = 0;
            this.treeViewFolder.ImageList = this.imageList1;
            this.treeViewFolder.Location = new System.Drawing.Point(0, 0);
            this.treeViewFolder.Name = "treeViewFolder";
            this.treeViewFolder.SelectedImageIndex = 0;
            this.treeViewFolder.Size = new System.Drawing.Size(400, 300);
            this.treeViewFolder.TabIndex = 0;
            this.treeViewFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFolder_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder");
            this.imageList1.Images.SetKeyName(1, "file");
            // 
            // listViewFile
            // 
            this.listViewFile.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listViewFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFile.GridLines = true;
            this.listViewFile.HideSelection = false;
            this.listViewFile.Location = new System.Drawing.Point(0, 0);
            this.listViewFile.Name = "listViewFile";
            this.listViewFile.Size = new System.Drawing.Size(400, 412);
            this.listViewFile.SmallImageList = this.imageList1;
            this.listViewFile.TabIndex = 0;
            this.listViewFile.UseCompatibleStateImageBehavior = false;
            this.listViewFile.View = System.Windows.Forms.View.Details;
            this.listViewFile.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewFile_ColumnClick);
            this.listViewFile.SelectedIndexChanged += new System.EventHandler(this.ListViewFile_SelectedIndexChanged);
            this.listViewFile.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewFile_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Last Modified";
            this.columnHeader2.Width = 134;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Size";
            // 
            // listViewRecentFiles
            // 
            this.listViewRecentFiles.AutoArrange = false;
            this.listViewRecentFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderFileName,
            this.columnHeaderCharCount});
            this.listViewRecentFiles.HideSelection = false;
            this.listViewRecentFiles.Location = new System.Drawing.Point(0, 277);
            this.listViewRecentFiles.Name = "listViewRecentFiles";
            this.listViewRecentFiles.Size = new System.Drawing.Size(852, 262);
            this.listViewRecentFiles.TabIndex = 1;
            this.listViewRecentFiles.UseCompatibleStateImageBehavior = false;
            this.listViewRecentFiles.View = System.Windows.Forms.View.Details;
            this.listViewRecentFiles.Visible = false;
            // 
            // columnHeaderFileName
            // 
            this.columnHeaderFileName.Text = "檔名";
            this.columnHeaderFileName.Width = 720;
            // 
            // columnHeaderCharCount
            // 
            this.columnHeaderCharCount.Text = "字數";
            this.columnHeaderCharCount.Width = 127;
            // 
            // richTextBoxText
            // 
            this.richTextBoxText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxText.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxText.Name = "richTextBoxText";
            this.richTextBoxText.Size = new System.Drawing.Size(894, 718);
            this.richTextBoxText.TabIndex = 0;
            this.richTextBoxText.Text = "";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.Size = new System.Drawing.Size(894, 718);
            this.webBrowser1.TabIndex = 1;
            // 
            // navigationBar
            // 
            this.navigationBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.navigationBar.AutoSize = false;
            this.navigationBar.CanOverflow = false;
            this.navigationBar.Dock = System.Windows.Forms.DockStyle.None;
            this.navigationBar.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navigationBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.navigationBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.navigationBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowFolderButton,
            this.FolderPathButton,
            this.toolStripSeparator2,
            this.navBackButton,
            this.navForwardButton,
            this.navUpButton,
            this.toolStripSeparator1,
            this.navFoldersButton,
            this.navAddressLabel,
            this.QuitButton,
            this.FontSizeAddButton,
            this.FontSizeReduceButton,
            this.toolStripComboBoxFonts});
            this.navigationBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.navigationBar.Location = new System.Drawing.Point(0, 0);
            this.navigationBar.Name = "navigationBar";
            this.navigationBar.Size = new System.Drawing.Size(1300, 30);
            this.navigationBar.Stretch = true;
            this.navigationBar.TabIndex = 5;
            // 
            // ShowFolderButton
            // 
            this.ShowFolderButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ShowFolderButton.Image = ((System.Drawing.Image)(resources.GetObject("ShowFolderButton.Image")));
            this.ShowFolderButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowFolderButton.Name = "ShowFolderButton";
            this.ShowFolderButton.Size = new System.Drawing.Size(93, 27);
            this.ShowFolderButton.Text = "顯示資料夾";
            this.ShowFolderButton.Click += new System.EventHandler(this.ShowFolderButton_Click);
            // 
            // FolderPathButton
            // 
            this.FolderPathButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FolderPathButton.Image = ((System.Drawing.Image)(resources.GetObject("FolderPathButton.Image")));
            this.FolderPathButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FolderPathButton.Name = "FolderPathButton";
            this.FolderPathButton.Size = new System.Drawing.Size(101, 27);
            this.FolderPathButton.Text = "開啟目錄";
            this.FolderPathButton.ToolTipText = "開啟目錄";
            this.FolderPathButton.Visible = false;
            this.FolderPathButton.Click += new System.EventHandler(this.FolderPathButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.AutoSize = false;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(30, 30);
            this.toolStripSeparator2.Visible = false;
            // 
            // navBackButton
            // 
            this.navBackButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navBackButton.Enabled = false;
            this.navBackButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navBackButton.Image = ((System.Drawing.Image)(resources.GetObject("navBackButton.Image")));
            this.navBackButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navBackButton.Name = "navBackButton";
            this.navBackButton.Size = new System.Drawing.Size(40, 27);
            this.navBackButton.Text = "Back";
            this.navBackButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navBackButton.Visible = false;
            // 
            // navForwardButton
            // 
            this.navForwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navForwardButton.Enabled = false;
            this.navForwardButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("navForwardButton.Image")));
            this.navForwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navForwardButton.Name = "navForwardButton";
            this.navForwardButton.Size = new System.Drawing.Size(40, 27);
            this.navForwardButton.Text = "Forward";
            this.navForwardButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navForwardButton.Visible = false;
            // 
            // navUpButton
            // 
            this.navUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navUpButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navUpButton.Image = ((System.Drawing.Image)(resources.GetObject("navUpButton.Image")));
            this.navUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navUpButton.Name = "navUpButton";
            this.navUpButton.Size = new System.Drawing.Size(28, 27);
            this.navUpButton.Text = "Up";
            this.navUpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navUpButton.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AutoSize = false;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(30, 30);
            this.toolStripSeparator1.Visible = false;
            // 
            // navFoldersButton
            // 
            this.navFoldersButton.Checked = true;
            this.navFoldersButton.CheckOnClick = true;
            this.navFoldersButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.navFoldersButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.navFoldersButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navFoldersButton.Image = ((System.Drawing.Image)(resources.GetObject("navFoldersButton.Image")));
            this.navFoldersButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFoldersButton.Name = "navFoldersButton";
            this.navFoldersButton.Size = new System.Drawing.Size(28, 27);
            this.navFoldersButton.Text = "Folders";
            this.navFoldersButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navFoldersButton.Visible = false;
            // 
            // navAddressLabel
            // 
            this.navAddressLabel.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.navAddressLabel.MergeIndex = 0;
            this.navAddressLabel.Name = "navAddressLabel";
            this.navAddressLabel.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.navAddressLabel.Size = new System.Drawing.Size(69, 27);
            this.navAddressLabel.Text = "Address";
            this.navAddressLabel.Visible = false;
            // 
            // QuitButton
            // 
            this.QuitButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.QuitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.QuitButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.QuitButton.Image = ((System.Drawing.Image)(resources.GetObject("QuitButton.Image")));
            this.QuitButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.QuitButton.Name = "QuitButton";
            this.QuitButton.Size = new System.Drawing.Size(28, 27);
            this.QuitButton.Text = "Quit";
            this.QuitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // FontSizeAddButton
            // 
            this.FontSizeAddButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.FontSizeAddButton.BackColor = System.Drawing.SystemColors.Control;
            this.FontSizeAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FontSizeAddButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FontSizeAddButton.Image = ((System.Drawing.Image)(resources.GetObject("FontSizeAddButton.Image")));
            this.FontSizeAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontSizeAddButton.Name = "FontSizeAddButton";
            this.FontSizeAddButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FontSizeAddButton.Size = new System.Drawing.Size(28, 27);
            this.FontSizeAddButton.Text = "FontSizeAddButton";
            this.FontSizeAddButton.ToolTipText = "放大字體";
            this.FontSizeAddButton.Click += new System.EventHandler(this.FontSizeAdd);
            // 
            // FontSizeReduceButton
            // 
            this.FontSizeReduceButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.FontSizeReduceButton.BackColor = System.Drawing.SystemColors.Control;
            this.FontSizeReduceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FontSizeReduceButton.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FontSizeReduceButton.Image = ((System.Drawing.Image)(resources.GetObject("FontSizeReduceButton.Image")));
            this.FontSizeReduceButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontSizeReduceButton.Name = "FontSizeReduceButton";
            this.FontSizeReduceButton.Size = new System.Drawing.Size(28, 27);
            this.FontSizeReduceButton.Text = "FontSizeReduceButton";
            this.FontSizeReduceButton.ToolTipText = "縮小字型";
            this.FontSizeReduceButton.Click += new System.EventHandler(this.FontSizeReduce);
            // 
            // toolStripComboBoxFonts
            // 
            this.toolStripComboBoxFonts.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripComboBoxFonts.DropDownHeight = 600;
            this.toolStripComboBoxFonts.DropDownWidth = 240;
            this.toolStripComboBoxFonts.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.toolStripComboBoxFonts.IntegralHeight = false;
            this.toolStripComboBoxFonts.Name = "toolStripComboBoxFonts";
            this.toolStripComboBoxFonts.Size = new System.Drawing.Size(240, 30);
            this.toolStripComboBoxFonts.Text = "字型";
            this.toolStripComboBoxFonts.SelectedIndexChanged += new System.EventHandler(this.ChangeFont);
            // 
            // FormTextSpeedReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1304, 752);
            this.Controls.Add(this.navigationBar);
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("微軟正黑體", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FormTextSpeedReader";
            this.Text = "TextSpeedReader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTSRClosing);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerFolder.Panel1.ResumeLayout(false);
            this.splitContainerFolder.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFolder)).EndInit();
            this.splitContainerFolder.ResumeLayout(false);
            this.navigationBar.ResumeLayout(false);
            this.navigationBar.PerformLayout();
            this.ResumeLayout(false);

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
    }
}


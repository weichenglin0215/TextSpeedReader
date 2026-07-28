namespace TextSpeedReader
{
    partial class FormMarkdownEditor
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
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
            splitContainerMd = new SplitContainer();
            richTextBoxPreview = new RichTextBox();
            labelPreview = new Label();
            textBoxEditor = new TextBox();
            labelEditor = new Label();
            panelBottom = new Panel();
            labelFileName = new Label();
            buttonSave = new Button();
            buttonSaveAs = new Button();
            buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainerMd).BeginInit();
            splitContainerMd.Panel1.SuspendLayout();
            splitContainerMd.Panel2.SuspendLayout();
            splitContainerMd.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerMd
            // 
            splitContainerMd.Dock = DockStyle.Fill;
            splitContainerMd.Location = new Point(0, 0);
            splitContainerMd.Name = "splitContainerMd";
            // 
            // splitContainerMd.Panel1
            // 
            splitContainerMd.Panel1.Controls.Add(richTextBoxPreview);
            splitContainerMd.Panel1.Controls.Add(labelPreview);
            splitContainerMd.Panel1.Padding = new Padding(7);
            // 
            // splitContainerMd.Panel2
            // 
            splitContainerMd.Panel2.Controls.Add(textBoxEditor);
            splitContainerMd.Panel2.Controls.Add(labelEditor);
            splitContainerMd.Panel2.Padding = new Padding(7);
            splitContainerMd.Size = new Size(1091, 608);
            splitContainerMd.SplitterDistance = 545;
            splitContainerMd.SplitterWidth = 5;
            splitContainerMd.TabIndex = 0;
            // 
            // richTextBoxPreview
            // 
            richTextBoxPreview.BackColor = Color.White;
            richTextBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            richTextBoxPreview.Dock = DockStyle.Fill;
            richTextBoxPreview.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 136);
            richTextBoxPreview.Location = new Point(7, 31);
            richTextBoxPreview.Name = "richTextBoxPreview";
            richTextBoxPreview.ReadOnly = true;
            richTextBoxPreview.TabStop = false;
            richTextBoxPreview.Size = new Size(531, 570);
            richTextBoxPreview.TabIndex = 1;
            richTextBoxPreview.Text = "";
            // 
            // labelPreview
            // 
            labelPreview.Dock = DockStyle.Top;
            labelPreview.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            labelPreview.Location = new Point(7, 7);
            labelPreview.Name = "labelPreview";
            labelPreview.Size = new Size(531, 24);
            labelPreview.TabIndex = 0;
            labelPreview.Text = "Markdown 預覽";
            // 
            // textBoxEditor
            // 
            textBoxEditor.AcceptsReturn = true;
            textBoxEditor.AcceptsTab = true;
            textBoxEditor.BorderStyle = BorderStyle.FixedSingle;
            textBoxEditor.Cursor = Cursors.IBeam;
            textBoxEditor.Dock = DockStyle.Fill;
            textBoxEditor.Font = new Font("微軟正黑體", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxEditor.HideSelection = false;
            textBoxEditor.Location = new Point(7, 31);
            textBoxEditor.Multiline = true;
            textBoxEditor.Name = "textBoxEditor";
            textBoxEditor.ScrollBars = ScrollBars.Both;
            textBoxEditor.Size = new Size(527, 570);
            textBoxEditor.TabIndex = 1;
            textBoxEditor.TextChanged += textBoxEditor_TextChanged;
            // 
            // labelEditor
            // 
            labelEditor.Dock = DockStyle.Top;
            labelEditor.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            labelEditor.Location = new Point(7, 7);
            labelEditor.Name = "labelEditor";
            labelEditor.Size = new Size(527, 24);
            labelEditor.TabIndex = 0;
            labelEditor.Text = "Markdown 編輯";
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(labelFileName);
            panelBottom.Controls.Add(buttonSave);
            panelBottom.Controls.Add(buttonSaveAs);
            panelBottom.Controls.Add(buttonCancel);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 608);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1091, 56);
            panelBottom.TabIndex = 1;
            // 
            // labelFileName
            // 
            labelFileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelFileName.AutoEllipsis = true;
            labelFileName.Font = new Font("Microsoft JhengHei UI", 10F);
            labelFileName.Location = new Point(11, 17);
            labelFileName.Name = "labelFileName";
            labelFileName.Size = new Size(738, 24);
            labelFileName.TabIndex = 3;
            labelFileName.Text = "檔案：";
            // 
            // buttonSave
            // 
            buttonSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSave.Location = new Point(975, 10);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(104, 35);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "儲存(&S)";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonSaveAs
            // 
            buttonSaveAs.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSaveAs.Location = new Point(865, 10);
            buttonSaveAs.Name = "buttonSaveAs";
            buttonSaveAs.Size = new Size(104, 35);
            buttonSaveAs.TabIndex = 1;
            buttonSaveAs.Text = "另存新檔(&A)";
            buttonSaveAs.UseVisualStyleBackColor = true;
            buttonSaveAs.Click += buttonSaveAs_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Location = new Point(755, 10);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(104, 35);
            buttonCancel.TabIndex = 0;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // FormMarkdownEditor
            // 
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(1091, 664);
            Controls.Add(splitContainerMd);
            Controls.Add(panelBottom);
            Name = "FormMarkdownEditor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Markdown 檔案編輯";
            Load += FormMarkdownEditor_Load;
            splitContainerMd.Panel1.ResumeLayout(false);
            splitContainerMd.Panel2.ResumeLayout(false);
            splitContainerMd.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMd).EndInit();
            splitContainerMd.ResumeLayout(false);
            panelBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerMd;
        private RichTextBox richTextBoxPreview;
        private Label labelPreview;
        private TextBox textBoxEditor;
        private Label labelEditor;
        private Panel panelBottom;
        private Button buttonSave;
        private Button buttonSaveAs;
        private Button buttonCancel;
        private Label labelFileName;
    }
}

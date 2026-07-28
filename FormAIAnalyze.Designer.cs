namespace TextSpeedReader
{
    partial class FormAIAnalyze
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
            splitMain = new SplitContainer();
            labelModel = new Label();
            comboBoxModel = new ComboBox();
            buttonRefreshModels = new Button();
            labelParams = new Label();
            comboBoxParams = new ComboBox();
            labelPromptType = new Label();
            comboBoxPromptType = new ComboBox();
            buttonEditPrompt = new Button();
            labelUserInstruction = new Label();
            textBoxUserInstruction = new TextBox();
            labelLog = new Label();
            checkBoxThink = new CheckBox();
            textBoxLog = new TextBox();
            labelFileListTitle = new Label();
            buttonSelectAll = new Button();
            buttonClearSelection = new Button();
            buttonInvertSelection = new Button();
            labelFileCount = new Label();
            checkedListBoxFiles = new CheckedListBox();
            panelBottom = new Panel();
            labelStatus = new Label();
            buttonRun = new Button();
            buttonCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // splitMain
            // 
            splitMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitMain.Location = new Point(0, 0);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(labelModel);
            splitMain.Panel1.Controls.Add(comboBoxModel);
            splitMain.Panel1.Controls.Add(buttonRefreshModels);
            splitMain.Panel1.Controls.Add(labelParams);
            splitMain.Panel1.Controls.Add(comboBoxParams);
            splitMain.Panel1.Controls.Add(labelPromptType);
            splitMain.Panel1.Controls.Add(comboBoxPromptType);
            splitMain.Panel1.Controls.Add(buttonEditPrompt);
            splitMain.Panel1.Controls.Add(labelUserInstruction);
            splitMain.Panel1.Controls.Add(textBoxUserInstruction);
            splitMain.Panel1.Controls.Add(labelLog);
            splitMain.Panel1.Controls.Add(checkBoxThink);
            splitMain.Panel1.Controls.Add(textBoxLog);
            splitMain.Panel1MinSize = 480;
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(labelFileListTitle);
            splitMain.Panel2.Controls.Add(buttonSelectAll);
            splitMain.Panel2.Controls.Add(buttonClearSelection);
            splitMain.Panel2.Controls.Add(buttonInvertSelection);
            splitMain.Panel2.Controls.Add(labelFileCount);
            splitMain.Panel2.Controls.Add(checkedListBoxFiles);
            splitMain.Panel2MinSize = 320;
            splitMain.Size = new Size(1636, 616);
            splitMain.SplitterDistance = 1018;
            splitMain.SplitterWidth = 5;
            splitMain.TabIndex = 0;
            // 
            // labelModel
            // 
            labelModel.AutoSize = true;
            labelModel.Location = new Point(11, 16);
            labelModel.Name = "labelModel";
            labelModel.Size = new Size(77, 20);
            labelModel.TabIndex = 0;
            labelModel.Text = "AI 大模型";
            // 
            // comboBoxModel
            // 
            comboBoxModel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxModel.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxModel.Location = new Point(100, 13);
            comboBoxModel.Name = "comboBoxModel";
            comboBoxModel.Size = new Size(789, 28);
            comboBoxModel.TabIndex = 1;
            // 
            // buttonRefreshModels
            // 
            buttonRefreshModels.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonRefreshModels.Location = new Point(898, 11);
            buttonRefreshModels.Name = "buttonRefreshModels";
            buttonRefreshModels.Size = new Size(109, 30);
            buttonRefreshModels.TabIndex = 2;
            buttonRefreshModels.Text = "重新整理";
            buttonRefreshModels.UseVisualStyleBackColor = true;
            buttonRefreshModels.Click += buttonRefreshModels_Click;
            // 
            // labelParams
            // 
            labelParams.AutoSize = true;
            labelParams.Location = new Point(11, 53);
            labelParams.Name = "labelParams";
            labelParams.Size = new Size(61, 20);
            labelParams.TabIndex = 3;
            labelParams.Text = "AI 參數";
            // 
            // comboBoxParams
            // 
            comboBoxParams.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxParams.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxParams.Location = new Point(100, 50);
            comboBoxParams.Name = "comboBoxParams";
            comboBoxParams.Size = new Size(908, 28);
            comboBoxParams.TabIndex = 4;
            // 
            // labelPromptType
            // 
            labelPromptType.AutoSize = true;
            labelPromptType.Location = new Point(11, 90);
            labelPromptType.Name = "labelPromptType";
            labelPromptType.Size = new Size(73, 20);
            labelPromptType.TabIndex = 5;
            labelPromptType.Text = "文章類型";
            // 
            // comboBoxPromptType
            // 
            comboBoxPromptType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBoxPromptType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPromptType.Location = new Point(100, 88);
            comboBoxPromptType.Name = "comboBoxPromptType";
            comboBoxPromptType.Size = new Size(789, 28);
            comboBoxPromptType.TabIndex = 6;
            // 
            // buttonEditPrompt
            // 
            buttonEditPrompt.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonEditPrompt.Location = new Point(898, 86);
            buttonEditPrompt.Name = "buttonEditPrompt";
            buttonEditPrompt.Size = new Size(109, 30);
            buttonEditPrompt.TabIndex = 7;
            buttonEditPrompt.Text = "編輯";
            buttonEditPrompt.UseVisualStyleBackColor = true;
            buttonEditPrompt.Click += buttonEditPrompt_Click;
            // 
            // labelUserInstruction
            // 
            labelUserInstruction.AutoSize = true;
            labelUserInstruction.Location = new Point(11, 126);
            labelUserInstruction.Name = "labelUserInstruction";
            labelUserInstruction.Size = new Size(89, 20);
            labelUserInstruction.TabIndex = 8;
            labelUserInstruction.Text = "使用者指令";
            // 
            // textBoxUserInstruction
            // 
            textBoxUserInstruction.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxUserInstruction.Location = new Point(11, 150);
            textBoxUserInstruction.Multiline = true;
            textBoxUserInstruction.Name = "textBoxUserInstruction";
            textBoxUserInstruction.ScrollBars = ScrollBars.Vertical;
            textBoxUserInstruction.Size = new Size(997, 90);
            textBoxUserInstruction.TabIndex = 9;
            // 
            // labelLog
            // 
            labelLog.AutoSize = true;
            labelLog.Location = new Point(11, 250);
            labelLog.Name = "labelLog";
            labelLog.Size = new Size(120, 20);
            labelLog.TabIndex = 10;
            labelLog.Text = "執行紀錄 (LOG)";
            // 
            // checkBoxThink
            // 
            checkBoxThink.AutoSize = true;
            checkBoxThink.Location = new Point(160, 249);
            checkBoxThink.Name = "checkBoxThink";
            checkBoxThink.Size = new Size(346, 24);
            checkBoxThink.TabIndex = 12;
            checkBoxThink.Text = "深度思考模式 (開啟思考模式會增長分析時間)";
            checkBoxThink.UseVisualStyleBackColor = true;
            // 
            // textBoxLog
            // 
            textBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxLog.BackColor = Color.FromArgb(30, 30, 30);
            textBoxLog.Font = new Font("微軟正黑體", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxLog.ForeColor = Color.Gainsboro;
            textBoxLog.Location = new Point(11, 279);
            textBoxLog.Multiline = true;
            textBoxLog.Name = "textBoxLog";
            textBoxLog.ReadOnly = true;
            textBoxLog.ScrollBars = ScrollBars.Both;
            textBoxLog.Size = new Size(997, 327);
            textBoxLog.TabIndex = 11;
            // 
            // labelFileListTitle
            // 
            labelFileListTitle.AutoSize = true;
            labelFileListTitle.Location = new Point(11, 16);
            labelFileListTitle.Name = "labelFileListTitle";
            labelFileListTitle.Size = new Size(183, 20);
            labelFileListTitle.TabIndex = 0;
            labelFileListTitle.Text = "檔案列表 (勾選要分析的)";
            // 
            // buttonSelectAll
            // 
            buttonSelectAll.Location = new Point(11, 42);
            buttonSelectAll.Name = "buttonSelectAll";
            buttonSelectAll.Size = new Size(100, 30);
            buttonSelectAll.TabIndex = 1;
            buttonSelectAll.Text = "全選";
            buttonSelectAll.UseVisualStyleBackColor = true;
            buttonSelectAll.Click += buttonSelectAll_Click;
            // 
            // buttonClearSelection
            // 
            buttonClearSelection.Location = new Point(118, 42);
            buttonClearSelection.Name = "buttonClearSelection";
            buttonClearSelection.Size = new Size(100, 30);
            buttonClearSelection.TabIndex = 2;
            buttonClearSelection.Text = "清除選取";
            buttonClearSelection.UseVisualStyleBackColor = true;
            buttonClearSelection.Click += buttonClearSelection_Click;
            // 
            // buttonInvertSelection
            // 
            buttonInvertSelection.Location = new Point(225, 42);
            buttonInvertSelection.Name = "buttonInvertSelection";
            buttonInvertSelection.Size = new Size(100, 30);
            buttonInvertSelection.TabIndex = 3;
            buttonInvertSelection.Text = "反選";
            buttonInvertSelection.UseVisualStyleBackColor = true;
            buttonInvertSelection.Click += buttonInvertSelection_Click;
            // 
            // labelFileCount
            // 
            labelFileCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            labelFileCount.Location = new Point(302, 47);
            labelFileCount.Name = "labelFileCount";
            labelFileCount.Size = new Size(287, 21);
            labelFileCount.TabIndex = 4;
            labelFileCount.Text = "已勾選 0 / 0";
            labelFileCount.TextAlign = ContentAlignment.MiddleRight;
            // 
            // checkedListBoxFiles
            // 
            checkedListBoxFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBoxFiles.CheckOnClick = true;
            checkedListBoxFiles.FormattingEnabled = true;
            checkedListBoxFiles.HorizontalScrollbar = true;
            checkedListBoxFiles.IntegralHeight = false;
            checkedListBoxFiles.Location = new Point(11, 80);
            checkedListBoxFiles.Name = "checkedListBoxFiles";
            checkedListBoxFiles.Size = new Size(579, 526);
            checkedListBoxFiles.TabIndex = 5;
            checkedListBoxFiles.ItemCheck += checkedListBoxFiles_ItemCheck;
            // 
            // panelBottom
            // 
            panelBottom.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelBottom.Controls.Add(labelStatus);
            panelBottom.Controls.Add(buttonRun);
            panelBottom.Controls.Add(buttonCancel);
            panelBottom.Location = new Point(0, 620);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1636, 40);
            panelBottom.TabIndex = 1;
            // 
            // labelStatus
            // 
            labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            labelStatus.AutoEllipsis = true;
            labelStatus.Location = new Point(11, 8);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(1327, 23);
            labelStatus.TabIndex = 0;
            labelStatus.Text = "就緒";
            // 
            // buttonRun
            // 
            buttonRun.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonRun.Location = new Point(1489, 2);
            buttonRun.Name = "buttonRun";
            buttonRun.Size = new Size(136, 35);
            buttonRun.TabIndex = 2;
            buttonRun.Text = "執行AI分析(&R)";
            buttonRun.UseVisualStyleBackColor = true;
            buttonRun.Click += buttonRun_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Location = new Point(1344, 2);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(136, 35);
            buttonCancel.TabIndex = 1;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // FormAIAnalyze
            // 
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1636, 661);
            Controls.Add(splitMain);
            Controls.Add(panelBottom);
            MinimumSize = new Size(911, 492);
            Name = "FormAIAnalyze";
            StartPosition = FormStartPosition.CenterParent;
            Text = "AI 分析文章選項";
            Load += FormAIAnalyze_Load;
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel1.PerformLayout();
            splitMain.Panel2.ResumeLayout(false);
            splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            panelBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitMain;
        private Label labelModel;
        private ComboBox comboBoxModel;
        private Button buttonRefreshModels;
        private Label labelParams;
        private ComboBox comboBoxParams;
        private Label labelPromptType;
        private ComboBox comboBoxPromptType;
        private Button buttonEditPrompt;
        private Label labelUserInstruction;
        private TextBox textBoxUserInstruction;
        private Label labelLog;
        private CheckBox checkBoxThink;
        private TextBox textBoxLog;
        private Label labelFileListTitle;
        private Button buttonSelectAll;
        private Button buttonClearSelection;
        private Button buttonInvertSelection;
        private Label labelFileCount;
        private CheckedListBox checkedListBoxFiles;
        private Panel panelBottom;
        private Label labelStatus;
        private Button buttonRun;
        private Button buttonCancel;
    }
}

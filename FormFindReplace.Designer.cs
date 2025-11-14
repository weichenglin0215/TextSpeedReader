namespace TextSpeedReader
{
    partial class FormFindReplace
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
            this.labelFind = new System.Windows.Forms.Label();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.labelReplace = new System.Windows.Forms.Label();
            this.textBoxReplace = new System.Windows.Forms.TextBox();
            this.checkBoxMatchCase = new System.Windows.Forms.CheckBox();
            this.checkBoxWholeWord = new System.Windows.Forms.CheckBox();
            this.buttonFindNext = new System.Windows.Forms.Button();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelFind
            // 
            this.labelFind.AutoSize = true;
            this.labelFind.Location = new System.Drawing.Point(12, 15);
            this.labelFind.Name = "labelFind";
            this.labelFind.Size = new System.Drawing.Size(55, 15);
            this.labelFind.TabIndex = 0;
            this.labelFind.Text = "尋找(&F):";
            // 
            // textBoxFind
            // 
            this.textBoxFind.Location = new System.Drawing.Point(80, 12);
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(300, 23);
            this.textBoxFind.TabIndex = 1;
            this.textBoxFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFind_KeyDown);
            // 
            // labelReplace
            // 
            this.labelReplace.AutoSize = true;
            this.labelReplace.Location = new System.Drawing.Point(12, 45);
            this.labelReplace.Name = "labelReplace";
            this.labelReplace.Size = new System.Drawing.Size(55, 15);
            this.labelReplace.TabIndex = 2;
            this.labelReplace.Text = "取代(&R):";
            // 
            // textBoxReplace
            // 
            this.textBoxReplace.Location = new System.Drawing.Point(80, 42);
            this.textBoxReplace.Name = "textBoxReplace";
            this.textBoxReplace.Size = new System.Drawing.Size(300, 23);
            this.textBoxReplace.TabIndex = 3;
            this.textBoxReplace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxReplace_KeyDown);
            // 
            // checkBoxMatchCase
            // 
            this.checkBoxMatchCase.AutoSize = true;
            this.checkBoxMatchCase.Location = new System.Drawing.Point(80, 75);
            this.checkBoxMatchCase.Name = "checkBoxMatchCase";
            this.checkBoxMatchCase.Size = new System.Drawing.Size(75, 19);
            this.checkBoxMatchCase.TabIndex = 4;
            this.checkBoxMatchCase.Text = "大小寫(&C)";
            this.checkBoxMatchCase.UseVisualStyleBackColor = true;
            // 
            // checkBoxWholeWord
            // 
            this.checkBoxWholeWord.AutoSize = true;
            this.checkBoxWholeWord.Location = new System.Drawing.Point(170, 75);
            this.checkBoxWholeWord.Name = "checkBoxWholeWord";
            this.checkBoxWholeWord.Size = new System.Drawing.Size(75, 19);
            this.checkBoxWholeWord.TabIndex = 5;
            this.checkBoxWholeWord.Text = "全字(&W)";
            this.checkBoxWholeWord.UseVisualStyleBackColor = true;
            // 
            // buttonFindNext
            // 
            this.buttonFindNext.Location = new System.Drawing.Point(400, 10);
            this.buttonFindNext.Name = "buttonFindNext";
            this.buttonFindNext.Size = new System.Drawing.Size(100, 25);
            this.buttonFindNext.TabIndex = 6;
            this.buttonFindNext.Text = "尋找下一個(&F)";
            this.buttonFindNext.UseVisualStyleBackColor = true;
            this.buttonFindNext.Click += new System.EventHandler(this.buttonFindNext_Click);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Location = new System.Drawing.Point(400, 40);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(100, 25);
            this.buttonReplace.TabIndex = 7;
            this.buttonReplace.Text = "取代(&R)";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.Location = new System.Drawing.Point(400, 70);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.Size = new System.Drawing.Size(100, 25);
            this.buttonReplaceAll.TabIndex = 8;
            this.buttonReplaceAll.Text = "全部取代(&A)";
            this.buttonReplaceAll.UseVisualStyleBackColor = true;
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(400, 100);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 25);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "取消(&C)";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormFindReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 140);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonReplaceAll);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.buttonFindNext);
            this.Controls.Add(this.checkBoxWholeWord);
            this.Controls.Add(this.checkBoxMatchCase);
            this.Controls.Add(this.textBoxReplace);
            this.Controls.Add(this.labelReplace);
            this.Controls.Add(this.textBoxFind);
            this.Controls.Add(this.labelFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFindReplace";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "尋找";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        public System.Windows.Forms.Label labelFind;
        public System.Windows.Forms.TextBox textBoxFind;
        public System.Windows.Forms.Label labelReplace;
        public System.Windows.Forms.TextBox textBoxReplace;
        private System.Windows.Forms.CheckBox checkBoxMatchCase;
        private System.Windows.Forms.CheckBox checkBoxWholeWord;
        public System.Windows.Forms.Button buttonFindNext;
        public System.Windows.Forms.Button buttonReplace;
        public System.Windows.Forms.Button buttonReplaceAll;
        private System.Windows.Forms.Button buttonCancel;
    }
}


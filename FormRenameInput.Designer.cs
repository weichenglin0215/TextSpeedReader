namespace TextSpeedReader
{
    partial class FormRenameInput
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
            labelPrompt = new Label();
            textBoxFileName = new TextBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // labelPrompt
            // 
            labelPrompt.AutoSize = true;
            labelPrompt.Location = new Point(19, 23);
            labelPrompt.Margin = new Padding(5, 0, 5, 0);
            labelPrompt.Name = "labelPrompt";
            labelPrompt.Size = new Size(82, 23);
            labelPrompt.TabIndex = 0;
            labelPrompt.Text = "提示訊息";
            // 
            // textBoxFileName
            // 
            textBoxFileName.Location = new Point(19, 98);
            textBoxFileName.Margin = new Padding(5);
            textBoxFileName.Name = "textBoxFileName";
            textBoxFileName.Size = new Size(649, 30);
            textBoxFileName.TabIndex = 1;
            textBoxFileName.KeyDown += textBoxFileName_KeyDown;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(422, 143);
            buttonOK.Margin = new Padding(5);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(118, 46);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "確定(&O)";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(550, 143);
            buttonCancel.Margin = new Padding(5);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(118, 46);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // FormRenameInput
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(682, 203);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(textBoxFileName);
            Controls.Add(labelPrompt);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormRenameInput";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "更名檔案";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.TextBox textBoxFileName;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}


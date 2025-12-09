namespace TextSpeedReader
{
    partial class FormOptions
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
            checkBoxAutoOpenLastDirectory = new CheckBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            checkBoxKeepFontSize = new CheckBox();
            SuspendLayout();
            // 
            // checkBoxAutoOpenLastDirectory
            // 
            checkBoxAutoOpenLastDirectory.AutoSize = true;
            checkBoxAutoOpenLastDirectory.Checked = true;
            checkBoxAutoOpenLastDirectory.CheckState = CheckState.Checked;
            checkBoxAutoOpenLastDirectory.Location = new Point(29, 27);
            checkBoxAutoOpenLastDirectory.Margin = new Padding(4, 4, 4, 4);
            checkBoxAutoOpenLastDirectory.Name = "checkBoxAutoOpenLastDirectory";
            checkBoxAutoOpenLastDirectory.Size = new Size(268, 24);
            checkBoxAutoOpenLastDirectory.TabIndex = 0;
            checkBoxAutoOpenLastDirectory.Text = "啟動程式時自動開啟上次的目錄。";
            checkBoxAutoOpenLastDirectory.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(219, 107);
            buttonOK.Margin = new Padding(4, 4, 4, 4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(143, 40);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "確定(&O)";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(376, 107);
            buttonCancel.Margin = new Padding(4, 4, 4, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(143, 40);
            buttonCancel.TabIndex = 2;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // checkBoxKeepFontSize
            // 
            checkBoxKeepFontSize.AutoSize = true;
            checkBoxKeepFontSize.Checked = true;
            checkBoxKeepFontSize.CheckState = CheckState.Checked;
            checkBoxKeepFontSize.Location = new Point(29, 59);
            checkBoxKeepFontSize.Margin = new Padding(4);
            checkBoxKeepFontSize.Name = "checkBoxKeepFontSize";
            checkBoxKeepFontSize.Size = new Size(156, 24);
            checkBoxKeepFontSize.TabIndex = 3;
            checkBoxKeepFontSize.Text = "保留字型與大小。";
            checkBoxKeepFontSize.UseVisualStyleBackColor = true;
            // 
            // FormOptions
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(543, 160);
            Controls.Add(checkBoxKeepFontSize);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(checkBoxAutoOpenLastDirectory);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4, 4, 4, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormOptions";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "選項";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAutoOpenLastDirectory;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private CheckBox checkBoxKeepFontSize;
    }
}


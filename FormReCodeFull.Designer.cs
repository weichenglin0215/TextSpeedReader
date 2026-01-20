namespace TextSpeedReader
{
    partial class FormReCodeFull
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            labelOriginal = new Label();
            listBoxPreviews = new ListBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            labelWarning = new Label();
            checkBoxSim2Trad = new CheckBox();
            SuspendLayout();
            // 
            // labelOriginal
            // 
            labelOriginal.AutoSize = true;
            labelOriginal.Location = new Point(19, 19);
            labelOriginal.Name = "labelOriginal";
            labelOriginal.Size = new Size(117, 25);
            labelOriginal.TabIndex = 0;
            labelOriginal.Text = "根目錄名稱:";
            // 
            // listBoxPreviews
            // 
            listBoxPreviews.FormattingEnabled = true;
            listBoxPreviews.Location = new Point(19, 58);
            listBoxPreviews.Name = "listBoxPreviews";
            listBoxPreviews.Size = new Size(811, 404);
            listBoxPreviews.TabIndex = 1;
            listBoxPreviews.MouseDoubleClick += listBoxPreviews_MouseDoubleClick;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(583, 555);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(118, 46);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "確定(&O)";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(712, 555);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(118, 46);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // labelWarning
            // 
            labelWarning.Font = new Font("微軟正黑體", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            labelWarning.ForeColor = Color.Red;
            labelWarning.Location = new Point(19, 472);
            labelWarning.Name = "labelWarning";
            labelWarning.Size = new Size(811, 64);
            labelWarning.TabIndex = 4;
            labelWarning.Text = "確認執行後會將該目錄與次目錄與以下檔案名稱全部變更編碼，\r\n此動作無法復原，請務必確認後再執行！！！";
            // 
            // checkBoxSim2Trad
            // 
            checkBoxSim2Trad.AutoSize = true;
            checkBoxSim2Trad.Location = new Point(19, 565);
            checkBoxSim2Trad.Name = "checkBoxSim2Trad";
            checkBoxSim2Trad.Size = new Size(514, 29);
            checkBoxSim2Trad.TabIndex = 5;
            checkBoxSim2Trad.Text = "變更編碼之後，請將ＳＣ簡體檔名改成ＴＣ繁體檔名。";
            checkBoxSim2Trad.UseVisualStyleBackColor = true;
            // 
            // FormReCodeFull
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(842, 613);
            Controls.Add(checkBoxSim2Trad);
            Controls.Add(labelWarning);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(listBoxPreviews);
            Controls.Add(labelOriginal);
            Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormReCodeFull";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "批次目錄、次目錄與以下全部檔案名稱重新編碼，此動作無法復原。";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelOriginal;
        private System.Windows.Forms.ListBox listBoxPreviews;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.CheckBox checkBoxSim2Trad;
    }
}

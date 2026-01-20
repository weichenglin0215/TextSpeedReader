namespace TextSpeedReader
{
    partial class FormReCodeFileName
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
            SuspendLayout();
            // 
            // labelOriginal
            // 
            labelOriginal.AutoSize = true;
            labelOriginal.Location = new Point(19, 19);
            labelOriginal.Name = "labelOriginal";
            labelOriginal.Size = new Size(82, 23);
            labelOriginal.TabIndex = 0;
            labelOriginal.Text = "目前檔名:";
            // 
            // listBoxPreviews
            // 
            listBoxPreviews.FormattingEnabled = true;
            listBoxPreviews.ItemHeight = 23;
            listBoxPreviews.Location = new Point(19, 58);
            listBoxPreviews.Name = "listBoxPreviews";
            listBoxPreviews.Size = new Size(744, 349);
            listBoxPreviews.TabIndex = 1;
            listBoxPreviews.MouseDoubleClick += listBoxPreviews_MouseDoubleClick;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(516, 428);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(118, 46);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "確定(&O)";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(645, 428);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(118, 46);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "取消(&C)";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // FormReCodeFileName
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(11F, 23F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(782, 493);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(listBoxPreviews);
            Controls.Add(labelOriginal);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormReCodeFileName";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "自選編碼重新命名";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label labelOriginal;
        private System.Windows.Forms.ListBox listBoxPreviews;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}

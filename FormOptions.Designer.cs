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
            label1 = new Label();
            label2 = new Label();
            textBoxNewLineStartJudgment = new TextBox();
            textBoxNewLineEndJudgment = new TextBox();
            label3 = new Label();
            numericUpDown_AddSpaceChrCount = new NumericUpDown();
            buttonClearHistory = new Button();
            textBoxInsertEndText = new TextBox();
            textBoxInsertBeginingText = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            textBoxAnnotationBegin = new TextBox();
            textBoxAnnotationEnd = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numericUpDown_AddSpaceChrCount).BeginInit();
            SuspendLayout();
            // 
            // checkBoxAutoOpenLastDirectory
            // 
            checkBoxAutoOpenLastDirectory.AutoSize = true;
            checkBoxAutoOpenLastDirectory.Checked = true;
            checkBoxAutoOpenLastDirectory.CheckState = CheckState.Checked;
            checkBoxAutoOpenLastDirectory.Location = new Point(29, 27);
            checkBoxAutoOpenLastDirectory.Margin = new Padding(4);
            checkBoxAutoOpenLastDirectory.Name = "checkBoxAutoOpenLastDirectory";
            checkBoxAutoOpenLastDirectory.Size = new Size(268, 24);
            checkBoxAutoOpenLastDirectory.TabIndex = 0;
            checkBoxAutoOpenLastDirectory.Text = "啟動程式時自動開啟上次的目錄。";
            checkBoxAutoOpenLastDirectory.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            buttonOK.Location = new Point(230, 408);
            buttonOK.Margin = new Padding(4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(143, 40);
            buttonOK.TabIndex = 1;
            buttonOK.Text = "確定(&O)";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += buttonOK_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(387, 408);
            buttonCancel.Margin = new Padding(4);
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 97);
            label1.Name = "label1";
            label1.Size = new Size(153, 20);
            label1.TabIndex = 4;
            label1.Text = "新行開頭的判定字串";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(29, 135);
            label2.Name = "label2";
            label2.Size = new Size(153, 20);
            label2.TabIndex = 5;
            label2.Text = "新行結尾的判定字串";
            // 
            // textBoxNewLineStartJudgment
            // 
            textBoxNewLineStartJudgment.Location = new Point(206, 94);
            textBoxNewLineStartJudgment.Name = "textBoxNewLineStartJudgment";
            textBoxNewLineStartJudgment.Size = new Size(324, 28);
            textBoxNewLineStartJudgment.TabIndex = 6;
            textBoxNewLineStartJudgment.Text = "/*新咒語開始------*/";
            // 
            // textBoxNewLineEndJudgment
            // 
            textBoxNewLineEndJudgment.Location = new Point(206, 132);
            textBoxNewLineEndJudgment.Name = "textBoxNewLineEndJudgment";
            textBoxNewLineEndJudgment.Size = new Size(324, 28);
            textBoxNewLineEndJudgment.TabIndex = 7;
            textBoxNewLineEndJudgment.Text = "/*新咒語結束------*/";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(29, 178);
            label3.Name = "label3";
            label3.Size = new Size(185, 20);
            label3.TabIndex = 8;
            label3.Text = "每行行首增加空白字元數";
            // 
            // numericUpDown_AddSpaceChrCount
            // 
            numericUpDown_AddSpaceChrCount.Location = new Point(240, 176);
            numericUpDown_AddSpaceChrCount.Name = "numericUpDown_AddSpaceChrCount";
            numericUpDown_AddSpaceChrCount.Size = new Size(120, 28);
            numericUpDown_AddSpaceChrCount.TabIndex = 9;
            numericUpDown_AddSpaceChrCount.TextAlign = HorizontalAlignment.Center;
            numericUpDown_AddSpaceChrCount.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // buttonClearHistory
            // 
            buttonClearHistory.Location = new Point(29, 408);
            buttonClearHistory.Margin = new Padding(4);
            buttonClearHistory.Name = "buttonClearHistory";
            buttonClearHistory.Size = new Size(180, 40);
            buttonClearHistory.TabIndex = 10;
            buttonClearHistory.Text = "清除歷史紀錄";
            buttonClearHistory.UseVisualStyleBackColor = true;
            buttonClearHistory.Click += buttonClearHistory_Click;
            // 
            // textBoxInsertEndText
            // 
            textBoxInsertEndText.Location = new Point(206, 279);
            textBoxInsertEndText.Name = "textBoxInsertEndText";
            textBoxInsertEndText.Size = new Size(324, 28);
            textBoxInsertEndText.TabIndex = 14;
            textBoxInsertEndText.Text = ", ";
            // 
            // textBoxInsertBeginingText
            // 
            textBoxInsertBeginingText.Location = new Point(206, 241);
            textBoxInsertBeginingText.Name = "textBoxInsertBeginingText";
            textBoxInsertBeginingText.Size = new Size(324, 28);
            textBoxInsertBeginingText.TabIndex = 13;
            textBoxInsertBeginingText.Text = "\"";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(29, 282);
            label4.Name = "label4";
            label4.Size = new Size(137, 20);
            label4.TabIndex = 12;
            label4.Text = "插入每行結尾字串";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(29, 244);
            label5.Name = "label5";
            label5.Size = new Size(137, 20);
            label5.TabIndex = 11;
            label5.Text = "插入每行開頭字串";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(29, 350);
            label6.Name = "label6";
            label6.Size = new Size(73, 20);
            label6.TabIndex = 15;
            label6.Text = "註解開頭";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(300, 350);
            label7.Name = "label7";
            label7.Size = new Size(73, 20);
            label7.TabIndex = 16;
            label7.Text = "註解結尾";
            // 
            // textBoxAnnotationBegin
            // 
            textBoxAnnotationBegin.Location = new Point(108, 347);
            textBoxAnnotationBegin.Name = "textBoxAnnotationBegin";
            textBoxAnnotationBegin.Size = new Size(146, 28);
            textBoxAnnotationBegin.TabIndex = 17;
            textBoxAnnotationBegin.Text = "/*";
            // 
            // textBoxAnnotationEnd
            // 
            textBoxAnnotationEnd.Location = new Point(379, 347);
            textBoxAnnotationEnd.Name = "textBoxAnnotationEnd";
            textBoxAnnotationEnd.Size = new Size(151, 28);
            textBoxAnnotationEnd.TabIndex = 18;
            textBoxAnnotationEnd.Text = "*/";
            // 
            // FormOptions
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(10F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = buttonCancel;
            ClientSize = new Size(544, 461);
            Controls.Add(textBoxAnnotationEnd);
            Controls.Add(textBoxAnnotationBegin);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(textBoxInsertEndText);
            Controls.Add(textBoxInsertBeginingText);
            Controls.Add(label4);
            Controls.Add(label5);
            Controls.Add(numericUpDown_AddSpaceChrCount);
            Controls.Add(buttonClearHistory);
            Controls.Add(label3);
            Controls.Add(textBoxNewLineEndJudgment);
            Controls.Add(textBoxNewLineStartJudgment);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(checkBoxKeepFontSize);
            Controls.Add(buttonCancel);
            Controls.Add(buttonOK);
            Controls.Add(checkBoxAutoOpenLastDirectory);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormOptions";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "選項";
            ((System.ComponentModel.ISupportInitialize)numericUpDown_AddSpaceChrCount).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxAutoOpenLastDirectory;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private CheckBox checkBoxKeepFontSize;
        private Label label1;
        private Label label2;
        private TextBox textBoxNewLineStartJudgment;
        private TextBox textBoxNewLineEndJudgment;
        private Label label3;
        private NumericUpDown numericUpDown_AddSpaceChrCount;
        private Button buttonClearHistory;
        private TextBox textBoxAnnotationBegin;
        private TextBox textBoxInsertBeginingText;
        private Label label4;
        private Label label5;
        private TextBox textBoxInsertEndText;
        private Label label6;
        private Label label7;
        private TextBox textBoxAnnotationEnd;
    }
}


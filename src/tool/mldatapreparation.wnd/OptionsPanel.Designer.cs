namespace MLDataPreparation.Dll
{
    partial class OptionsPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            //this.btnLoadTesting.Click -= btnLoadTrainig_Click;
            //this.btnLoadTesting.Click -= btnLoadTesting_Click;
            //this.btnSetToGP.Click -= btnLoadTesting_Click;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.randomoizeDataSet = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.numCtrlNumForTest = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.presentigeRadio = new System.Windows.Forms.RadioButton();
            this.numberRadio = new System.Windows.Forms.RadioButton();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTest)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // randomoizeDataSet
            // 
            this.randomoizeDataSet.AutoSize = true;
            this.randomoizeDataSet.Location = new System.Drawing.Point(10, 35);
            this.randomoizeDataSet.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.randomoizeDataSet.Name = "randomoizeDataSet";
            this.randomoizeDataSet.Size = new System.Drawing.Size(246, 29);
            this.randomoizeDataSet.TabIndex = 48;
            this.randomoizeDataSet.Text = "Randomize data sets";
            this.randomoizeDataSet.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(286, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 25);
            this.label1.TabIndex = 49;
            this.label1.Text = "Select delimiter:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.button1.Location = new System.Drawing.Point(456, 87);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(182, 48);
            this.button1.TabIndex = 50;
            this.button1.Text = "Export...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(10, 71);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(259, 29);
            this.checkBox1.TabIndex = 51;
            this.checkBox1.Text = "Export to CNTK format";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.randomoizeDataSet);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.groupBox1.Location = new System.Drawing.Point(694, 23);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox1.Size = new System.Drawing.Size(680, 158);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export option";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "TAB",
            "COMMA",
            "SAPCE",
            "SEMICOLON",
            "COLON"});
            this.comboBox1.Location = new System.Drawing.Point(456, 27);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(182, 33);
            this.comboBox1.TabIndex = 52;
            // 
            // numCtrlNumForTest
            // 
            this.numCtrlNumForTest.Location = new System.Drawing.Point(148, 37);
            this.numCtrlNumForTest.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numCtrlNumForTest.Name = "numCtrlNumForTest";
            this.numCtrlNumForTest.Size = new System.Drawing.Size(156, 31);
            this.numCtrlNumForTest.TabIndex = 14;
            this.numCtrlNumForTest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "Select last :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // presentigeRadio
            // 
            this.presentigeRadio.AutoSize = true;
            this.presentigeRadio.Location = new System.Drawing.Point(312, 73);
            this.presentigeRadio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.presentigeRadio.Name = "presentigeRadio";
            this.presentigeRadio.Size = new System.Drawing.Size(251, 29);
            this.presentigeRadio.TabIndex = 16;
            this.presentigeRadio.Text = "% for testing. (0-50%)";
            this.presentigeRadio.UseVisualStyleBackColor = true;
            // 
            // numberRadio
            // 
            this.numberRadio.AutoSize = true;
            this.numberRadio.Checked = true;
            this.numberRadio.Location = new System.Drawing.Point(312, 33);
            this.numberRadio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numberRadio.Name = "numberRadio";
            this.numberRadio.Size = new System.Drawing.Size(282, 29);
            this.numberRadio.TabIndex = 17;
            this.numberRadio.TabStop = true;
            this.numberRadio.Text = "# for testing. (0-n/2 rows)";
            this.numberRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.numberRadio);
            this.groupBox5.Controls.Add(this.presentigeRadio);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.numCtrlNumForTest);
            this.groupBox5.ForeColor = System.Drawing.Color.Blue;
            this.groupBox5.Location = new System.Drawing.Point(32, 23);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.groupBox5.Size = new System.Drawing.Size(640, 158);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Data set for model validation";
            // 
            // OptionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "OptionsPanel";
            this.Size = new System.Drawing.Size(1380, 187);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTest)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.NumericUpDown numCtrlNumForTest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton presentigeRadio;
        private System.Windows.Forms.RadioButton numberRadio;
        private System.Windows.Forms.GroupBox groupBox5;
        public System.Windows.Forms.CheckBox randomoizeDataSet;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.ComboBox comboBox1;
    }
}

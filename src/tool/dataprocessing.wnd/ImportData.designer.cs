namespace DataProcessing.Wnd
{
    partial class ImportData
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportData));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numCtrlNumForTest = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btnImportData = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.firstRowHeaderCheck = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImportTS = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTest)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(16, 50);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(942, 31);
            this.textBox1.TabIndex = 0;
            // 
            // btnLoadData
            // 
            this.btnLoadData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadData.Location = new System.Drawing.Point(984, 44);
            this.btnLoadData.Margin = new System.Windows.Forms.Padding(4);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(116, 42);
            this.btnLoadData.TabIndex = 1;
            this.btnLoadData.Text = "File ...";
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(16, 119);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(360, 190);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Decimal separator for numeric ";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(50, 87);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(210, 29);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "comma (eg. 1,23)";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(50, 33);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(188, 29);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "point (eg. 1.23)";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numCtrlNumForTest);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.checkBox7);
            this.groupBox3.Controls.Add(this.checkBox6);
            this.groupBox3.Controls.Add(this.checkBox5);
            this.groupBox3.Controls.Add(this.checkBox4);
            this.groupBox3.Controls.Add(this.checkBox3);
            this.groupBox3.Controls.Add(this.checkBox2);
            this.groupBox3.Controls.Add(this.textBox2);
            this.groupBox3.Location = new System.Drawing.Point(402, 106);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(478, 204);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Column separator";
            // 
            // numCtrlNumForTest
            // 
            this.numCtrlNumForTest.Enabled = false;
            this.numCtrlNumForTest.Location = new System.Drawing.Point(324, 146);
            this.numCtrlNumForTest.Margin = new System.Windows.Forms.Padding(4);
            this.numCtrlNumForTest.Name = "numCtrlNumForTest";
            this.numCtrlNumForTest.Size = new System.Drawing.Size(128, 31);
            this.numCtrlNumForTest.TabIndex = 22;
            this.numCtrlNumForTest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(216, 150);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 25);
            this.label1.TabIndex = 21;
            this.label1.Text = "Time Leg:";
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(216, 100);
            this.checkBox7.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(158, 29);
            this.checkBox7.TabIndex = 20;
            this.checkBox7.Text = "Time Series";
            this.checkBox7.UseVisualStyleBackColor = true;
            this.checkBox7.CheckedChanged += new System.EventHandler(this.checkBox7_CheckedChanged);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(36, 162);
            this.checkBox6.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(81, 29);
            this.checkBox6.TabIndex = 18;
            this.checkBox6.Text = "Tab";
            this.checkBox6.UseVisualStyleBackColor = true;
            this.checkBox6.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(216, 48);
            this.checkBox5.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(93, 29);
            this.checkBox5.TabIndex = 17;
            this.checkBox5.Text = "other";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(36, 121);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(105, 29);
            this.checkBox4.TabIndex = 16;
            this.checkBox4.Text = "Space";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(36, 85);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(117, 29);
            this.checkBox3.TabIndex = 15;
            this.checkBox3.Text = "Comma";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(36, 46);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(144, 29);
            this.checkBox2.TabIndex = 14;
            this.checkBox2.Text = "Semicolon";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(324, 46);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.MaxLength = 1;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(78, 31);
            this.textBox2.TabIndex = 5;
            // 
            // btnImportData
            // 
            this.btnImportData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportData.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnImportData.Enabled = false;
            this.btnImportData.Location = new System.Drawing.Point(888, 160);
            this.btnImportData.Margin = new System.Windows.Forms.Padding(4);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(214, 42);
            this.btnImportData.TabIndex = 5;
            this.btnImportData.Text = "Import Data";
            this.btnImportData.UseVisualStyleBackColor = true;
            this.btnImportData.Click += new System.EventHandler(this.btnImportData_Click);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(16, 319);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(1082, 498);
            this.textBox3.TabIndex = 6;
            this.textBox3.WordWrap = false;
            // 
            // firstRowHeaderCheck
            // 
            this.firstRowHeaderCheck.AutoSize = true;
            this.firstRowHeaderCheck.Location = new System.Drawing.Point(904, 119);
            this.firstRowHeaderCheck.Margin = new System.Windows.Forms.Padding(4);
            this.firstRowHeaderCheck.Name = "firstRowHeaderCheck";
            this.firstRowHeaderCheck.Size = new System.Drawing.Size(210, 29);
            this.firstRowHeaderCheck.TabIndex = 7;
            this.firstRowHeaderCheck.Text = "First Row Header";
            this.firstRowHeaderCheck.UseVisualStyleBackColor = true;
            this.firstRowHeaderCheck.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(888, 267);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(214, 42);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnImportTS
            // 
            this.btnImportTS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportTS.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnImportTS.Enabled = false;
            this.btnImportTS.Location = new System.Drawing.Point(888, 213);
            this.btnImportTS.Margin = new System.Windows.Forms.Padding(4);
            this.btnImportTS.Name = "btnImportTS";
            this.btnImportTS.Size = new System.Drawing.Size(214, 42);
            this.btnImportTS.TabIndex = 15;
            this.btnImportTS.Text = "Import Time Series";
            this.btnImportTS.UseVisualStyleBackColor = true;
            this.btnImportTS.Click += new System.EventHandler(this.btnImportTS_Click);
            // 
            // ImportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1116, 835);
            this.Controls.Add(this.btnImportTS);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.firstRowHeaderCheck);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.btnImportData);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnLoadData);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ImportData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Experimental Data";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTest)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btnImportData;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.CheckBox firstRowHeaderCheck;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.Button btnImportTS;
        public System.Windows.Forms.NumericUpDown numCtrlNumForTest;
    }
}
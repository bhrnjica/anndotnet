namespace MLDataPreparation.Dll
{
    partial class DataPanel
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.numberRadio = new System.Windows.Forms.RadioButton();
            this.percentigeRadio = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.numCtrlNumForValidation = new System.Windows.Forms.NumericUpDown();
            this.listView1 = new System.Windows.Forms.ListView();
            this.randomoizeDataSet = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numberTestRadion = new System.Windows.Forms.RadioButton();
            this.percentigeRadioTest = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.numCtrlNumForTesting = new System.Windows.Forms.NumericUpDown();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForValidation)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTesting)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.numberRadio);
            this.groupBox5.Controls.Add(this.percentigeRadio);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.numCtrlNumForValidation);
            this.groupBox5.Location = new System.Drawing.Point(6, 895);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox5.Size = new System.Drawing.Size(643, 123);
            this.groupBox5.TabIndex = 26;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Validation data set";
            // 
            // numberRadio
            // 
            this.numberRadio.AutoSize = true;
            this.numberRadio.Checked = true;
            this.numberRadio.Location = new System.Drawing.Point(267, 34);
            this.numberRadio.Margin = new System.Windows.Forms.Padding(4);
            this.numberRadio.Name = "numberRadio";
            this.numberRadio.Size = new System.Drawing.Size(310, 29);
            this.numberRadio.TabIndex = 17;
            this.numberRadio.TabStop = true;
            this.numberRadio.Text = "# for validation. (0-n/2 rows)";
            this.numberRadio.UseVisualStyleBackColor = true;
            this.numberRadio.CheckedChanged += new System.EventHandler(this.numberRadio_CheckedChanged);
            // 
            // percentigeRadio
            // 
            this.percentigeRadio.AutoSize = true;
            this.percentigeRadio.Location = new System.Drawing.Point(267, 74);
            this.percentigeRadio.Margin = new System.Windows.Forms.Padding(4);
            this.percentigeRadio.Name = "percentigeRadio";
            this.percentigeRadio.Size = new System.Drawing.Size(279, 29);
            this.percentigeRadio.TabIndex = 16;
            this.percentigeRadio.Text = "% for validation. (0-50%)";
            this.percentigeRadio.UseVisualStyleBackColor = true;
            this.percentigeRadio.CheckedChanged += new System.EventHandler(this.percentigeRadio_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 25);
            this.label2.TabIndex = 15;
            this.label2.Text = "Select last :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numCtrlNumForValidation
            // 
            this.numCtrlNumForValidation.Location = new System.Drawing.Point(142, 37);
            this.numCtrlNumForValidation.Margin = new System.Windows.Forms.Padding(4);
            this.numCtrlNumForValidation.Name = "numCtrlNumForValidation";
            this.numCtrlNumForValidation.Size = new System.Drawing.Size(117, 31);
            this.numCtrlNumForValidation.TabIndex = 14;
            this.numCtrlNumForValidation.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(6);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1374, 827);
            this.listView1.TabIndex = 25;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            this.listView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDown);
            // 
            // randomoizeDataSet
            // 
            this.randomoizeDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.randomoizeDataSet.AutoSize = true;
            this.randomoizeDataSet.Location = new System.Drawing.Point(6, 839);
            this.randomoizeDataSet.Margin = new System.Windows.Forms.Padding(6);
            this.randomoizeDataSet.Name = "randomoizeDataSet";
            this.randomoizeDataSet.Size = new System.Drawing.Size(328, 29);
            this.randomoizeDataSet.TabIndex = 18;
            this.randomoizeDataSet.Text = "Randomize data set then split";
            this.randomoizeDataSet.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.numberTestRadion);
            this.groupBox1.Controls.Add(this.percentigeRadioTest);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numCtrlNumForTesting);
            this.groupBox1.Location = new System.Drawing.Point(661, 895);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(6);
            this.groupBox1.Size = new System.Drawing.Size(624, 123);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test data set";
            // 
            // numberTestRadion
            // 
            this.numberTestRadion.AutoSize = true;
            this.numberTestRadion.Checked = true;
            this.numberTestRadion.Enabled = false;
            this.numberTestRadion.Location = new System.Drawing.Point(271, 29);
            this.numberTestRadion.Margin = new System.Windows.Forms.Padding(4);
            this.numberTestRadion.Name = "numberTestRadion";
            this.numberTestRadion.Size = new System.Drawing.Size(310, 29);
            this.numberTestRadion.TabIndex = 17;
            this.numberTestRadion.TabStop = true;
            this.numberTestRadion.Text = "# for validation. (0-n/2 rows)";
            this.numberTestRadion.UseVisualStyleBackColor = true;
            // 
            // percentigeRadioTest
            // 
            this.percentigeRadioTest.AutoSize = true;
            this.percentigeRadioTest.Enabled = false;
            this.percentigeRadioTest.Location = new System.Drawing.Point(271, 69);
            this.percentigeRadioTest.Margin = new System.Windows.Forms.Padding(4);
            this.percentigeRadioTest.Name = "percentigeRadioTest";
            this.percentigeRadioTest.Size = new System.Drawing.Size(279, 29);
            this.percentigeRadioTest.TabIndex = 16;
            this.percentigeRadioTest.Text = "% for validation. (0-50%)";
            this.percentigeRadioTest.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 25);
            this.label1.TabIndex = 15;
            this.label1.Text = "Select last :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numCtrlNumForTesting
            // 
            this.numCtrlNumForTesting.Location = new System.Drawing.Point(142, 31);
            this.numCtrlNumForTesting.Margin = new System.Windows.Forms.Padding(4);
            this.numCtrlNumForTesting.Name = "numCtrlNumForTesting";
            this.numCtrlNumForTesting.Size = new System.Drawing.Size(117, 31);
            this.numCtrlNumForTesting.TabIndex = 14;
            this.numCtrlNumForTesting.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DataPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.randomoizeDataSet);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "DataPanel";
            this.Size = new System.Drawing.Size(1380, 1040);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForValidation)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCtrlNumForTesting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton numberRadio;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown numCtrlNumForValidation;
        public System.Windows.Forms.RadioButton percentigeRadio;
        public System.Windows.Forms.CheckBox randomoizeDataSet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton numberTestRadion;
        public System.Windows.Forms.RadioButton percentigeRadioTest;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.NumericUpDown numCtrlNumForTesting;
    }
}

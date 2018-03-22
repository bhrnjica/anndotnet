namespace ANNdotNet.Wnd.Dialogs
{
    partial class MModelEvaluation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MModelEvaluation));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txPSS = new System.Windows.Forms.TextBox();
            this.txHSS = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.txMaRecall = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txAAccuracy = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txMaPrecision = new System.Windows.Forms.TextBox();
            this.txMiRecall = new System.Windows.Forms.TextBox();
            this.txMiPrecision = new System.Windows.Forms.TextBox();
            this.txOAccuracy = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1182, 598);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txPSS);
            this.panel1.Controls.Add(this.txHSS);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.txMaRecall);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txAAccuracy);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txMaPrecision);
            this.panel1.Controls.Add(this.txMiRecall);
            this.panel1.Controls.Add(this.txMiPrecision);
            this.panel1.Controls.Add(this.txOAccuracy);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Location = new System.Drawing.Point(3, 463);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(956, 132);
            this.panel1.TabIndex = 2;
            // 
            // txPSS
            // 
            this.txPSS.Location = new System.Drawing.Point(770, 92);
            this.txPSS.Name = "txPSS";
            this.txPSS.ReadOnly = true;
            this.txPSS.Size = new System.Drawing.Size(103, 26);
            this.txPSS.TabIndex = 40;
            // 
            // txHSS
            // 
            this.txHSS.Location = new System.Drawing.Point(472, 95);
            this.txHSS.Name = "txHSS";
            this.txHSS.ReadOnly = true;
            this.txHSS.Size = new System.Drawing.Size(103, 26);
            this.txHSS.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(627, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 20);
            this.label4.TabIndex = 38;
            this.label4.Text = "Peirce Skill Score:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(326, 95);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(141, 20);
            this.label15.TabIndex = 37;
            this.label15.Text = "Heidke Skill Score:";
            // 
            // txMaRecall
            // 
            this.txMaRecall.Location = new System.Drawing.Point(770, 60);
            this.txMaRecall.Name = "txMaRecall";
            this.txMaRecall.ReadOnly = true;
            this.txMaRecall.Size = new System.Drawing.Size(103, 26);
            this.txMaRecall.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(603, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 20);
            this.label3.TabIndex = 32;
            this.label3.Text = "Macro-average recall:";
            // 
            // txAAccuracy
            // 
            this.txAAccuracy.Location = new System.Drawing.Point(154, 60);
            this.txAAccuracy.Name = "txAAccuracy";
            this.txAAccuracy.ReadOnly = true;
            this.txAAccuracy.Size = new System.Drawing.Size(103, 26);
            this.txAAccuracy.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 20);
            this.label2.TabIndex = 30;
            this.label2.Text = "Average accuracy:";
            // 
            // txMaPrecision
            // 
            this.txMaPrecision.Location = new System.Drawing.Point(472, 63);
            this.txMaPrecision.Name = "txMaPrecision";
            this.txMaPrecision.ReadOnly = true;
            this.txMaPrecision.Size = new System.Drawing.Size(103, 26);
            this.txMaPrecision.TabIndex = 29;
            // 
            // txMiRecall
            // 
            this.txMiRecall.Location = new System.Drawing.Point(770, 25);
            this.txMiRecall.Name = "txMiRecall";
            this.txMiRecall.ReadOnly = true;
            this.txMiRecall.Size = new System.Drawing.Size(103, 26);
            this.txMiRecall.TabIndex = 27;
            // 
            // txMiPrecision
            // 
            this.txMiPrecision.Location = new System.Drawing.Point(472, 25);
            this.txMiPrecision.Name = "txMiPrecision";
            this.txMiPrecision.ReadOnly = true;
            this.txMiPrecision.Size = new System.Drawing.Size(103, 26);
            this.txMiPrecision.TabIndex = 28;
            // 
            // txOAccuracy
            // 
            this.txOAccuracy.Location = new System.Drawing.Point(154, 25);
            this.txOAccuracy.Name = "txOAccuracy";
            this.txOAccuracy.ReadOnly = true;
            this.txOAccuracy.Size = new System.Drawing.Size(103, 26);
            this.txOAccuracy.TabIndex = 26;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(609, 28);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(154, 20);
            this.label12.TabIndex = 23;
            this.label12.Text = "Micro-average recall:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(128, 20);
            this.label13.TabIndex = 22;
            this.label13.Text = "Overall accuracy:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(286, 63);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(187, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "Macro-average Precision:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(286, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(180, 20);
            this.label11.TabIndex = 24;
            this.label11.Text = "Micro-average precision:";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.HoverSelection = true;
            this.listView1.Location = new System.Drawing.Point(3, 91);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1176, 366);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(257, 37);
            this.label1.TabIndex = 4;
            this.label1.Text = "Confusion Matrix";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1176, 45);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboBox1);
            this.panel3.Controls.Add(this.label16);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1176, 45);
            this.panel3.TabIndex = 4;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Training Data Set",
            "Testing Data Set"});
            this.comboBox1.Location = new System.Drawing.Point(140, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(284, 28);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(8, 9);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(126, 20);
            this.label16.TabIndex = 0;
            this.label16.Text = "Select Data Set:";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(1053, 632);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(126, 38);
            this.button2.TabIndex = 2;
            this.button2.Text = "Close";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // MModelEvaluation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1194, 680);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MModelEvaluation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Binary GP Model Evaluation";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txMaPrecision;
        private System.Windows.Forms.TextBox txMiRecall;
        private System.Windows.Forms.TextBox txMiPrecision;
        private System.Windows.Forms.TextBox txOAccuracy;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txAAccuracy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txMaRecall;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txPSS;
        private System.Windows.Forms.TextBox txHSS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label15;
    }
}
namespace ANNdotNet.Wnd.Dll.Panels
{
    partial class RunPanel
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.zedFitness = new ZedGraph.ZedGraphControl();
            this.zedModel = new ZedGraph.ZedGraphControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbEvalFunction = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbLossFunction = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cActivationO = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cActivationH = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupNetwork = new System.Windows.Forms.GroupBox();
            this.netSeditdrop = new System.Windows.Forms.TextBox();
            this.checkDropRate = new System.Windows.Forms.CheckBox();
            this.checkStabilisation = new System.Windows.Forms.CheckBox();
            this.netSedit1 = new System.Windows.Forms.TextBox();
            this.netSlabel1 = new System.Windows.Forms.Label();
            this.netSlabel2 = new System.Windows.Forms.Label();
            this.netSEdit3 = new System.Windows.Forms.TextBox();
            this.netSEdit2 = new System.Windows.Forms.TextBox();
            this.netSlabel3 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.cbNetworkSettings = new System.Windows.Forms.ComboBox();
            this.groupTraining = new System.Windows.Forms.GroupBox();
            this.cbLearnerType = new System.Windows.Forms.ComboBox();
            this.txtL2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtL1 = new System.Windows.Forms.TextBox();
            this.txMomentum = new System.Windows.Forms.TextBox();
            this.textLearningRate = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.groupIteration = new System.Windows.Forms.GroupBox();
            this.ebMinibatchSize = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.m_cbIterationType = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.m_eb_iterations = new System.Windows.Forms.TextBox();
            this.currentIteration = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupNetwork.SuspendLayout();
            this.groupTraining.SuspendLayout();
            this.groupIteration.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 725F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(406, 538);
            this.tableLayoutPanel1.TabIndex = 19;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 267F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(402, 534);
            this.tableLayoutPanel2.TabIndex = 20;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.zedFitness, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.zedModel, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(269, 2);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.13021F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.86979F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(131, 530);
            this.tableLayoutPanel3.TabIndex = 21;
            // 
            // zedFitness
            // 
            this.zedFitness.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedFitness.Location = new System.Drawing.Point(4, 4);
            this.zedFitness.Margin = new System.Windows.Forms.Padding(4);
            this.zedFitness.Name = "zedFitness";
            this.zedFitness.ScrollGrace = 0D;
            this.zedFitness.ScrollMaxX = 0D;
            this.zedFitness.ScrollMaxY = 0D;
            this.zedFitness.ScrollMaxY2 = 0D;
            this.zedFitness.ScrollMinX = 0D;
            this.zedFitness.ScrollMinY = 0D;
            this.zedFitness.ScrollMinY2 = 0D;
            this.zedFitness.Size = new System.Drawing.Size(123, 257);
            this.zedFitness.TabIndex = 0;
            this.zedFitness.UseExtendedPrintDialog = true;
            // 
            // zedModel
            // 
            this.zedModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedModel.Location = new System.Drawing.Point(4, 269);
            this.zedModel.Margin = new System.Windows.Forms.Padding(4);
            this.zedModel.Name = "zedModel";
            this.zedModel.ScrollGrace = 0D;
            this.zedModel.ScrollMaxX = 0D;
            this.zedModel.ScrollMaxY = 0D;
            this.zedModel.ScrollMaxY2 = 0D;
            this.zedModel.ScrollMinX = 0D;
            this.zedModel.ScrollMinY = 0D;
            this.zedModel.ScrollMinY2 = 0D;
            this.zedModel.Size = new System.Drawing.Size(123, 257);
            this.zedModel.TabIndex = 18;
            this.zedModel.UseExtendedPrintDialog = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupNetwork);
            this.panel1.Controls.Add(this.groupTraining);
            this.panel1.Controls.Add(this.groupIteration);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 528);
            this.panel1.TabIndex = 22;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbEvalFunction);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbLossFunction);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(2, 449);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 76);
            this.groupBox2.TabIndex = 44;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Training functions";
            // 
            // cbEvalFunction
            // 
            this.cbEvalFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEvalFunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbEvalFunction.FormattingEnabled = true;
            this.cbEvalFunction.Items.AddRange(new object[] {
            "BinaryCrossEntropy",
            "CrossEntropyWithSoftmax",
            "ClassificationError",
            "SquaredErr"});
            this.cbEvalFunction.Location = new System.Drawing.Point(74, 47);
            this.cbEvalFunction.Name = "cbEvalFunction";
            this.cbEvalFunction.Size = new System.Drawing.Size(165, 21);
            this.cbEvalFunction.TabIndex = 41;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "Eval F:";
            // 
            // cbLossFunction
            // 
            this.cbLossFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLossFunction.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbLossFunction.FormattingEnabled = true;
            this.cbLossFunction.Items.AddRange(new object[] {
            "BinaryCrossEntropy",
            "CrossEntropyWithSoftmax",
            "ClassificationError",
            "SquaredError"});
            this.cbLossFunction.Location = new System.Drawing.Point(74, 20);
            this.cbLossFunction.Name = "cbLossFunction";
            this.cbLossFunction.Size = new System.Drawing.Size(165, 21);
            this.cbLossFunction.TabIndex = 39;
            this.cbLossFunction.Tag = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Loss F:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cActivationO);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cActivationH);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 367);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 76);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Activation function";
            // 
            // cActivationO
            // 
            this.cActivationO.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cActivationO.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cActivationO.FormattingEnabled = true;
            this.cActivationO.Items.AddRange(new object[] {
            "None",
            "ReLU",
            "Softmax",
            "Tanh"});
            this.cActivationO.Location = new System.Drawing.Point(74, 47);
            this.cActivationO.Name = "cActivationO";
            this.cActivationO.Size = new System.Drawing.Size(165, 21);
            this.cActivationO.TabIndex = 41;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "Output L:";
            // 
            // cActivationH
            // 
            this.cActivationH.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cActivationH.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cActivationH.FormattingEnabled = true;
            this.cActivationH.Items.AddRange(new object[] {
            "None",
            "ReLU",
            "Softmax",
            "Tanh"});
            this.cActivationH.Location = new System.Drawing.Point(74, 20);
            this.cActivationH.Name = "cActivationH";
            this.cActivationH.Size = new System.Drawing.Size(165, 21);
            this.cActivationH.TabIndex = 39;
            this.cActivationH.Tag = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Hidden L:";
            // 
            // groupNetwork
            // 
            this.groupNetwork.Controls.Add(this.netSeditdrop);
            this.groupNetwork.Controls.Add(this.checkDropRate);
            this.groupNetwork.Controls.Add(this.checkStabilisation);
            this.groupNetwork.Controls.Add(this.netSedit1);
            this.groupNetwork.Controls.Add(this.netSlabel1);
            this.groupNetwork.Controls.Add(this.netSlabel2);
            this.groupNetwork.Controls.Add(this.netSEdit3);
            this.groupNetwork.Controls.Add(this.netSEdit2);
            this.groupNetwork.Controls.Add(this.netSlabel3);
            this.groupNetwork.Controls.Add(this.label22);
            this.groupNetwork.Controls.Add(this.cbNetworkSettings);
            this.groupNetwork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupNetwork.Location = new System.Drawing.Point(3, 229);
            this.groupNetwork.Name = "groupNetwork";
            this.groupNetwork.Size = new System.Drawing.Size(256, 132);
            this.groupNetwork.TabIndex = 19;
            this.groupNetwork.TabStop = false;
            this.groupNetwork.Text = "Network Settings";
            // 
            // netSeditdrop
            // 
            this.netSeditdrop.Location = new System.Drawing.Point(194, 99);
            this.netSeditdrop.Name = "netSeditdrop";
            this.netSeditdrop.Size = new System.Drawing.Size(44, 20);
            this.netSeditdrop.TabIndex = 34;
            this.netSeditdrop.Text = "20";
            // 
            // checkDropRate
            // 
            this.checkDropRate.AutoSize = true;
            this.checkDropRate.Location = new System.Drawing.Point(101, 102);
            this.checkDropRate.Name = "checkDropRate";
            this.checkDropRate.Size = new System.Drawing.Size(90, 17);
            this.checkDropRate.TabIndex = 33;
            this.checkDropRate.Text = "Drop rate (%):";
            this.checkDropRate.UseVisualStyleBackColor = true;
            // 
            // checkStabilisation
            // 
            this.checkStabilisation.AutoSize = true;
            this.checkStabilisation.Location = new System.Drawing.Point(13, 102);
            this.checkStabilisation.Name = "checkStabilisation";
            this.checkStabilisation.Size = new System.Drawing.Size(82, 17);
            this.checkStabilisation.TabIndex = 32;
            this.checkStabilisation.Text = "Stabilisation";
            this.checkStabilisation.UseVisualStyleBackColor = true;
            // 
            // netSedit1
            // 
            this.netSedit1.Location = new System.Drawing.Point(79, 47);
            this.netSedit1.Name = "netSedit1";
            this.netSedit1.Size = new System.Drawing.Size(44, 20);
            this.netSedit1.TabIndex = 24;
            this.netSedit1.Text = "20";
            // 
            // netSlabel1
            // 
            this.netSlabel1.AutoSize = true;
            this.netSlabel1.Location = new System.Drawing.Point(7, 50);
            this.netSlabel1.Name = "netSlabel1";
            this.netSlabel1.Size = new System.Drawing.Size(50, 13);
            this.netSlabel1.TabIndex = 28;
            this.netSlabel1.Text = "Neurons:";
            // 
            // netSlabel2
            // 
            this.netSlabel2.AutoSize = true;
            this.netSlabel2.Location = new System.Drawing.Point(136, 50);
            this.netSlabel2.Name = "netSlabel2";
            this.netSlabel2.Size = new System.Drawing.Size(52, 13);
            this.netSlabel2.TabIndex = 29;
            this.netSlabel2.Text = "H Layers:";
            // 
            // netSEdit3
            // 
            this.netSEdit3.Location = new System.Drawing.Point(194, 73);
            this.netSEdit3.Name = "netSEdit3";
            this.netSEdit3.Size = new System.Drawing.Size(44, 20);
            this.netSEdit3.TabIndex = 26;
            this.netSEdit3.Text = "20";
            // 
            // netSEdit2
            // 
            this.netSEdit2.Location = new System.Drawing.Point(194, 47);
            this.netSEdit2.Name = "netSEdit2";
            this.netSEdit2.Size = new System.Drawing.Size(44, 20);
            this.netSEdit2.TabIndex = 25;
            this.netSEdit2.Text = "20";
            // 
            // netSlabel3
            // 
            this.netSlabel3.AutoSize = true;
            this.netSlabel3.Location = new System.Drawing.Point(125, 76);
            this.netSlabel3.Name = "netSlabel3";
            this.netSlabel3.Size = new System.Drawing.Size(63, 13);
            this.netSlabel3.TabIndex = 30;
            this.netSlabel3.Text = "Embedding:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(13, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(34, 13);
            this.label22.TabIndex = 23;
            this.label22.Text = "Type:";
            // 
            // cbNetworkSettings
            // 
            this.cbNetworkSettings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNetworkSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbNetworkSettings.FormattingEnabled = true;
            this.cbNetworkSettings.Items.AddRange(new object[] {
            "Simple Feed Forward NN",
            "Depp Feed Forward NN",
            "LSTM Recurrent NN",
            "Sequence LSTM Recurrent NN"});
            this.cbNetworkSettings.Location = new System.Drawing.Point(53, 19);
            this.cbNetworkSettings.Name = "cbNetworkSettings";
            this.cbNetworkSettings.Size = new System.Drawing.Size(188, 21);
            this.cbNetworkSettings.TabIndex = 22;
            this.cbNetworkSettings.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // groupTraining
            // 
            this.groupTraining.Controls.Add(this.cbLearnerType);
            this.groupTraining.Controls.Add(this.txtL2);
            this.groupTraining.Controls.Add(this.label12);
            this.groupTraining.Controls.Add(this.label13);
            this.groupTraining.Controls.Add(this.label19);
            this.groupTraining.Controls.Add(this.label14);
            this.groupTraining.Controls.Add(this.txtL1);
            this.groupTraining.Controls.Add(this.txMomentum);
            this.groupTraining.Controls.Add(this.textLearningRate);
            this.groupTraining.Controls.Add(this.label18);
            this.groupTraining.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupTraining.Location = new System.Drawing.Point(2, 121);
            this.groupTraining.Name = "groupTraining";
            this.groupTraining.Size = new System.Drawing.Size(256, 102);
            this.groupTraining.TabIndex = 18;
            this.groupTraining.TabStop = false;
            this.groupTraining.Text = "Training Parameters";
            // 
            // cbLearnerType
            // 
            this.cbLearnerType.FormattingEnabled = true;
            this.cbLearnerType.Items.AddRange(new object[] {
            "SGDLearner",
            "MomentumSGDLearner"});
            this.cbLearnerType.Location = new System.Drawing.Point(74, 19);
            this.cbLearnerType.Name = "cbLearnerType";
            this.cbLearnerType.Size = new System.Drawing.Size(168, 21);
            this.cbLearnerType.TabIndex = 29;
            this.cbLearnerType.SelectedIndexChanged += new System.EventHandler(this.cbLearnerType_SelectedIndexChanged);
            // 
            // txtL2
            // 
            this.txtL2.Location = new System.Drawing.Point(198, 67);
            this.txtL2.Margin = new System.Windows.Forms.Padding(2);
            this.txtL2.Name = "txtL2";
            this.txtL2.Size = new System.Drawing.Size(44, 20);
            this.txtL2.TabIndex = 25;
            this.txtL2.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 22);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 13);
            this.label12.TabIndex = 30;
            this.label12.Text = "Learner:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(33, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(33, 13);
            this.label13.TabIndex = 33;
            this.label13.Text = "Rate:";
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(134, 70);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(59, 14);
            this.label19.TabIndex = 26;
            this.label19.Text = "L2 Regul.:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 73);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 34;
            this.label14.Text = "Momentum:";
            // 
            // txtL1
            // 
            this.txtL1.Location = new System.Drawing.Point(197, 46);
            this.txtL1.Margin = new System.Windows.Forms.Padding(2);
            this.txtL1.Name = "txtL1";
            this.txtL1.Size = new System.Drawing.Size(45, 20);
            this.txtL1.TabIndex = 27;
            this.txtL1.Text = "0";
            // 
            // txMomentum
            // 
            this.txMomentum.Location = new System.Drawing.Point(74, 70);
            this.txMomentum.Name = "txMomentum";
            this.txMomentum.Size = new System.Drawing.Size(44, 20);
            this.txMomentum.TabIndex = 32;
            this.txMomentum.Text = "5";
            // 
            // textLearningRate
            // 
            this.textLearningRate.Location = new System.Drawing.Point(74, 46);
            this.textLearningRate.Name = "textLearningRate";
            this.textLearningRate.Size = new System.Drawing.Size(44, 20);
            this.textLearningRate.TabIndex = 31;
            this.textLearningRate.Text = "0.001";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(138, 47);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 14);
            this.label18.TabIndex = 28;
            this.label18.Text = "L1 regul.:";
            // 
            // groupIteration
            // 
            this.groupIteration.Controls.Add(this.ebMinibatchSize);
            this.groupIteration.Controls.Add(this.label33);
            this.groupIteration.Controls.Add(this.m_cbIterationType);
            this.groupIteration.Controls.Add(this.label21);
            this.groupIteration.Controls.Add(this.m_eb_iterations);
            this.groupIteration.Controls.Add(this.currentIteration);
            this.groupIteration.Controls.Add(this.label20);
            this.groupIteration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupIteration.Location = new System.Drawing.Point(3, 3);
            this.groupIteration.Name = "groupIteration";
            this.groupIteration.Size = new System.Drawing.Size(255, 112);
            this.groupIteration.TabIndex = 17;
            this.groupIteration.TabStop = false;
            this.groupIteration.Text = "Iteration";
            // 
            // ebMinibatchSize
            // 
            this.ebMinibatchSize.Location = new System.Drawing.Point(150, 84);
            this.ebMinibatchSize.Name = "ebMinibatchSize";
            this.ebMinibatchSize.Size = new System.Drawing.Size(91, 20);
            this.ebMinibatchSize.TabIndex = 25;
            this.ebMinibatchSize.Text = "64";
            // 
            // label33
            // 
            this.label33.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label33.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label33.Location = new System.Drawing.Point(14, 86);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(130, 18);
            this.label33.TabIndex = 24;
            this.label33.Text = "Minibatch size:";
            // 
            // m_cbIterationType
            // 
            this.m_cbIterationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbIterationType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_cbIterationType.FormattingEnabled = true;
            this.m_cbIterationType.Items.AddRange(new object[] {
            "Generation number",
            "Fitness >="});
            this.m_cbIterationType.Location = new System.Drawing.Point(13, 33);
            this.m_cbIterationType.Name = "m_cbIterationType";
            this.m_cbIterationType.Size = new System.Drawing.Size(135, 21);
            this.m_cbIterationType.TabIndex = 19;
            // 
            // label21
            // 
            this.label21.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label21.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label21.Location = new System.Drawing.Point(14, 16);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(76, 18);
            this.label21.TabIndex = 18;
            this.label21.Text = "Envolve until:";
            // 
            // m_eb_iterations
            // 
            this.m_eb_iterations.Location = new System.Drawing.Point(150, 34);
            this.m_eb_iterations.Name = "m_eb_iterations";
            this.m_eb_iterations.Size = new System.Drawing.Size(91, 20);
            this.m_eb_iterations.TabIndex = 17;
            this.m_eb_iterations.Text = "100";
            // 
            // currentIteration
            // 
            this.currentIteration.Location = new System.Drawing.Point(150, 60);
            this.currentIteration.Name = "currentIteration";
            this.currentIteration.Size = new System.Drawing.Size(91, 20);
            this.currentIteration.TabIndex = 1;
            this.currentIteration.Text = "0";
            // 
            // label20
            // 
            this.label20.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(14, 63);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(101, 18);
            this.label20.TabIndex = 0;
            this.label20.Text = "Iteration:";
            // 
            // RunPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(217)))), ((int)(((byte)(239)))));
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RunPanel";
            this.Size = new System.Drawing.Size(406, 538);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupNetwork.ResumeLayout(false);
            this.groupNetwork.PerformLayout();
            this.groupTraining.ResumeLayout(false);
            this.groupTraining.PerformLayout();
            this.groupIteration.ResumeLayout(false);
            this.groupIteration.PerformLayout();
            this.ResumeLayout(false);

        }


        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        protected ZedGraph.ZedGraphControl zedFitness;
        private ZedGraph.ZedGraphControl zedModel;
        private System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.GroupBox groupNetwork;
        protected System.Windows.Forms.GroupBox groupTraining;
        protected System.Windows.Forms.GroupBox groupIteration;
        protected System.Windows.Forms.TextBox ebMinibatchSize;
        protected System.Windows.Forms.Label label33;
        protected System.Windows.Forms.ComboBox m_cbIterationType;
        protected System.Windows.Forms.Label label21;
        protected System.Windows.Forms.TextBox m_eb_iterations;
        protected System.Windows.Forms.TextBox currentIteration;
        protected System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox cbLearnerType;
        private System.Windows.Forms.TextBox txtL2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtL1;
        private System.Windows.Forms.TextBox txMomentum;
        private System.Windows.Forms.TextBox textLearningRate;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ComboBox cbNetworkSettings;
        private System.Windows.Forms.TextBox netSedit1;
        private System.Windows.Forms.Label netSlabel1;
        private System.Windows.Forms.Label netSlabel2;
        private System.Windows.Forms.TextBox netSEdit3;
        private System.Windows.Forms.TextBox netSEdit2;
        private System.Windows.Forms.Label netSlabel3;
        private System.Windows.Forms.TextBox netSeditdrop;
        private System.Windows.Forms.CheckBox checkDropRate;
        private System.Windows.Forms.CheckBox checkStabilisation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cActivationO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cActivationH;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbEvalFunction;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbLossFunction;
        private System.Windows.Forms.Label label4;
    }
}

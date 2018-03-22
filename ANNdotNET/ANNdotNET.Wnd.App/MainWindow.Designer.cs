namespace ANNdotNet.Wnd.App
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Start Page");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ribbon1 = new System.Windows.Forms.Ribbon();
            this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
            this.ribbonPanel1 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton2 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton3 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton12 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton14 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton15 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton4 = new System.Windows.Forms.RibbonButton();
            this.ribbonButtonList1 = new System.Windows.Forms.RibbonButtonList();
            this.ribbonItemGroup1 = new System.Windows.Forms.RibbonItemGroup();
            this.ribbonDescriptionMenuItem1 = new System.Windows.Forms.RibbonDescriptionMenuItem();
            this.ribbonPanel5 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton13 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton17 = new System.Windows.Forms.RibbonButton();
            this.ribbonSeparator1 = new System.Windows.Forms.RibbonSeparator();
            this.ribbonButton16 = new System.Windows.Forms.RibbonButton();
            this.ribbonPanel3 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton7 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton8 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton9 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton18 = new System.Windows.Forms.RibbonButton();
            this.ribbonPanel2 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton5 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton6 = new System.Windows.Forms.RibbonButton();
            this.ribbonPanel4 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton10 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton11 = new System.Windows.Forms.RibbonButton();
            this.ribbonButton1 = new System.Windows.Forms.RibbonButton();
            this.ribbonDescriptionMenuItem2 = new System.Windows.Forms.RibbonDescriptionMenuItem();
            this.ribbonDescriptionMenuItem3 = new System.Windows.Forms.RibbonDescriptionMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 102);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(935, 565);
            this.splitContainer1.SplitterDistance = 170;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 3;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(0);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageKey = "start.png";
            treeNode1.Name = "Node0";
            treeNode1.Tag = "startpage";
            treeNode1.Text = "Start Page";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(170, 565);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView1.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "start.png");
            this.imageList1.Images.SetKeyName(1, "experiment.png");
            this.imageList1.Images.SetKeyName(2, "model.png");
            this.imageList1.Images.SetKeyName(3, "runningmodel.png");
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage7);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(762, 565);
            this.tabControl2.TabIndex = 2;
            this.tabControl2.Visible = false;
            // 
            // tabPage5
            // 
            this.tabPage5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(754, 539);
            this.tabPage5.TabIndex = 2;
            this.tabPage5.Text = "Training";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(754, 539);
            this.tabPage7.TabIndex = 4;
            this.tabPage7.Text = "Prediction";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(762, 565);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(754, 539);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(754, 539);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Info";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 669);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(935, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(42, 17);
            this.toolStripStatusLabel1.Text = "Ready.";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(878, 17);
            this.toolStripStatusLabel2.Spring = true;
            this.toolStripStatusLabel2.Text = "No application message!";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ribbon1
            // 
            this.ribbon1.BackColor = System.Drawing.SystemColors.Control;
            this.ribbon1.CaptionBarVisible = false;
            this.ribbon1.CausesValidation = false;
            this.ribbon1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbon1.ForeColor = System.Drawing.Color.DarkViolet;
            this.ribbon1.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.Margin = new System.Windows.Forms.Padding(2);
            this.ribbon1.Minimized = false;
            this.ribbon1.Name = "ribbon1";
            // 
            // 
            // 
            this.ribbon1.OrbDropDown.BorderRoundness = 8;
            this.ribbon1.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.OrbDropDown.Name = "";
            this.ribbon1.OrbDropDown.Size = new System.Drawing.Size(527, 447);
            this.ribbon1.OrbDropDown.TabIndex = 0;
            this.ribbon1.OrbStyle = System.Windows.Forms.RibbonOrbStyle.Office_2013;
            this.ribbon1.OrbVisible = false;
            // 
            // 
            // 
            this.ribbon1.QuickAccessToolbar.Text = "";
            this.ribbon1.RibbonTabFont = new System.Drawing.Font("Trebuchet MS", 9F);
            this.ribbon1.Size = new System.Drawing.Size(935, 102);
            this.ribbon1.TabIndex = 0;
            this.ribbon1.Tabs.Add(this.ribbonTab1);
            this.ribbon1.TabsMargin = new System.Windows.Forms.Padding(5, 2, 20, 0);
            this.ribbon1.TabSpacing = 4;
            this.ribbon1.Text = "ribbon1";
            this.ribbon1.ThemeColor = System.Windows.Forms.RibbonTheme.Blue;
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Name = "ribbonTab1";
            this.ribbonTab1.Panels.Add(this.ribbonPanel1);
            this.ribbonTab1.Panels.Add(this.ribbonPanel5);
            this.ribbonTab1.Panels.Add(this.ribbonPanel3);
            this.ribbonTab1.Panels.Add(this.ribbonPanel2);
            this.ribbonTab1.Panels.Add(this.ribbonPanel4);
            this.ribbonTab1.Text = "ANNdotNET v1.0 alpha";
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.ButtonMoreEnabled = false;
            this.ribbonPanel1.ButtonMoreVisible = false;
            this.ribbonPanel1.Items.Add(this.ribbonButton2);
            this.ribbonPanel1.Items.Add(this.ribbonButton3);
            this.ribbonPanel1.Items.Add(this.ribbonButton12);
            this.ribbonPanel1.Items.Add(this.ribbonButton4);
            this.ribbonPanel1.Name = "ribbonPanel1";
            this.ribbonPanel1.Text = "Standard";
            // 
            // ribbonButton2
            // 
            this.ribbonButton2.AltKey = "N";
            this.ribbonButton2.Image = global::ANNdotNet.Wnd.App.Properties.Resources.newgp16;
            this.ribbonButton2.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.newgp16;
            this.ribbonButton2.Name = "ribbonButton2";
            this.ribbonButton2.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton2.SmallImage")));
            this.ribbonButton2.Text = "New";
            this.ribbonButton2.ToolTip = "Creates new ANNdotNET project.";
            this.ribbonButton2.ToolTipTitle = "New ANNdotNET project (Alt+N)";
            this.ribbonButton2.Click += new System.EventHandler(this.New_Click);
            // 
            // ribbonButton3
            // 
            this.ribbonButton3.AltKey = "O";
            this.ribbonButton3.Enabled = false;
            this.ribbonButton3.Image = global::ANNdotNet.Wnd.App.Properties.Resources.opengp16;
            this.ribbonButton3.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.opengp16;
            this.ribbonButton3.Name = "ribbonButton3";
            this.ribbonButton3.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton3.SmallImage")));
            this.ribbonButton3.Text = "Open";
            this.ribbonButton3.ToolTip = "Use this command to open already saved *.gpa file types.";
            this.ribbonButton3.ToolTipTitle = "Open existing ANNdotNET project (Alt+O)";
            this.ribbonButton3.Click += new System.EventHandler(this.Open_Click);
            // 
            // ribbonButton12
            // 
            this.ribbonButton12.DropDownItems.Add(this.ribbonButton14);
            this.ribbonButton12.DropDownItems.Add(this.ribbonButton15);
            this.ribbonButton12.Enabled = false;
            this.ribbonButton12.Image = global::ANNdotNet.Wnd.App.Properties.Resources.savegp16;
            this.ribbonButton12.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.savegp16;
            this.ribbonButton12.Name = "ribbonButton12";
            this.ribbonButton12.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton12.SmallImage")));
            this.ribbonButton12.Style = System.Windows.Forms.RibbonButtonStyle.DropDown;
            this.ribbonButton12.Text = "Save";
            // 
            // ribbonButton14
            // 
            this.ribbonButton14.AltKey = "S";
            this.ribbonButton14.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.ribbonButton14.Image = global::ANNdotNet.Wnd.App.Properties.Resources.savegp24;
            this.ribbonButton14.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.savegp24;
            this.ribbonButton14.Name = "ribbonButton14";
            this.ribbonButton14.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton14.SmallImage")));
            this.ribbonButton14.Text = "Save";
            this.ribbonButton14.ToolTip = "In case of new project, file save dialog will appear.";
            this.ribbonButton14.ToolTipTitle = "Save current changes for selected ANNdotNET project (Alt +S)";
            this.ribbonButton14.Click += new System.EventHandler(this.Save_Click);
            // 
            // ribbonButton15
            // 
            this.ribbonButton15.AltKey = "A";
            this.ribbonButton15.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.ribbonButton15.Image = global::ANNdotNet.Wnd.App.Properties.Resources.savegp24;
            this.ribbonButton15.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.savegp24;
            this.ribbonButton15.Name = "ribbonButton15";
            this.ribbonButton15.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton15.SmallImage")));
            this.ribbonButton15.Text = "Save As";
            this.ribbonButton15.ToolTip = "Use this command for saving current project in to different file.";
            this.ribbonButton15.ToolTipTitle = "Save selected ANNdotNET proejct to new file on disk (Alt+A)";
            this.ribbonButton15.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // ribbonButton4
            // 
            this.ribbonButton4.AltKey = "C";
            this.ribbonButton4.DropDownItems.Add(this.ribbonButtonList1);
            this.ribbonButton4.DropDownItems.Add(this.ribbonItemGroup1);
            this.ribbonButton4.DropDownItems.Add(this.ribbonDescriptionMenuItem1);
            this.ribbonButton4.Image = global::ANNdotNet.Wnd.App.Properties.Resources.closegp16;
            this.ribbonButton4.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.closegp16;
            this.ribbonButton4.Name = "ribbonButton4";
            this.ribbonButton4.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton4.SmallImage")));
            this.ribbonButton4.Text = "Close";
            this.ribbonButton4.ToolTip = "Use this command to close selected project.";
            this.ribbonButton4.ToolTipTitle = "Close currently selected ANNdotNET project (Alt+C)";
            this.ribbonButton4.Click += new System.EventHandler(this.Close_Click);
            // 
            // ribbonButtonList1
            // 
            this.ribbonButtonList1.ButtonsSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
            this.ribbonButtonList1.FlowToBottom = false;
            this.ribbonButtonList1.ItemsSizeInDropwDownMode = new System.Drawing.Size(7, 5);
            this.ribbonButtonList1.Name = "ribbonButtonList1";
            this.ribbonButtonList1.Text = "ribbonButtonList1";
            // 
            // ribbonItemGroup1
            // 
            this.ribbonItemGroup1.Name = "ribbonItemGroup1";
            this.ribbonItemGroup1.Text = "ribbonItemGroup1";
            // 
            // ribbonDescriptionMenuItem1
            // 
            this.ribbonDescriptionMenuItem1.DescriptionBounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.ribbonDescriptionMenuItem1.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.ribbonDescriptionMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem1.Image")));
            this.ribbonDescriptionMenuItem1.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem1.LargeImage")));
            this.ribbonDescriptionMenuItem1.Name = "ribbonDescriptionMenuItem1";
            this.ribbonDescriptionMenuItem1.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem1.SmallImage")));
            this.ribbonDescriptionMenuItem1.Text = "ribbonDescriptionMenuItem1";
            // 
            // ribbonPanel5
            // 
            this.ribbonPanel5.ButtonMoreEnabled = false;
            this.ribbonPanel5.ButtonMoreVisible = false;
            this.ribbonPanel5.Items.Add(this.ribbonButton13);
            this.ribbonPanel5.Items.Add(this.ribbonButton17);
            this.ribbonPanel5.Items.Add(this.ribbonSeparator1);
            this.ribbonPanel5.Items.Add(this.ribbonButton16);
            this.ribbonPanel5.Name = "ribbonPanel5";
            this.ribbonPanel5.Text = "Model Preparation";
            // 
            // ribbonButton13
            // 
            this.ribbonButton13.Image = global::ANNdotNet.Wnd.App.Properties.Resources.loaddata32;
            this.ribbonButton13.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.loaddata32;
            this.ribbonButton13.MaxSizeMode = System.Windows.Forms.RibbonElementSizeMode.Large;
            this.ribbonButton13.Name = "ribbonButton13";
            this.ribbonButton13.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton13.SmallImage")));
            this.ribbonButton13.Text = "Data";
            this.ribbonButton13.ToolTip = "Load data from CSV file and define the training and testing data.";
            this.ribbonButton13.ToolTipTitle = "Import textual data";
            this.ribbonButton13.Click += new System.EventHandler(this.LoadDataSet_Click);
            // 
            // ribbonButton17
            // 
            this.ribbonButton17.Enabled = false;
            this.ribbonButton17.Image = global::ANNdotNet.Wnd.App.Properties.Resources.loaddatai32;
            this.ribbonButton17.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.loaddatai32;
            this.ribbonButton17.Name = "ribbonButton17";
            this.ribbonButton17.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton17.SmallImage")));
            this.ribbonButton17.Text = "Images";
            this.ribbonButton17.ToolTip = "Find images location inorder to create training and testing data set";
            this.ribbonButton17.ToolTipTitle = "Import Images ";
            // 
            // ribbonSeparator1
            // 
            this.ribbonSeparator1.Name = "ribbonSeparator1";
            this.ribbonSeparator1.Text = "";
            // 
            // ribbonButton16
            // 
            this.ribbonButton16.Image = global::ANNdotNet.Wnd.App.Properties.Resources.newmodel32;
            this.ribbonButton16.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.newmodel32;
            this.ribbonButton16.Name = "ribbonButton16";
            this.ribbonButton16.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton16.SmallImage")));
            this.ribbonButton16.Tag = "";
            this.ribbonButton16.Text = "Create Model";
            this.ribbonButton16.ToolTip = "Once the data is defined, use this option to create new model.";
            this.ribbonButton16.ToolTipTitle = "Create New ANN Model";
            this.ribbonButton16.Click += new System.EventHandler(this.NewModel_Click);
            // 
            // ribbonPanel3
            // 
            this.ribbonPanel3.ButtonMoreEnabled = false;
            this.ribbonPanel3.ButtonMoreVisible = false;
            this.ribbonPanel3.Items.Add(this.ribbonButton7);
            this.ribbonPanel3.Items.Add(this.ribbonButton8);
            this.ribbonPanel3.Items.Add(this.ribbonButton9);
            this.ribbonPanel3.Items.Add(this.ribbonButton18);
            this.ribbonPanel3.Name = "ribbonPanel3";
            this.ribbonPanel3.Text = "Model";
            // 
            // ribbonButton7
            // 
            this.ribbonButton7.AltKey = "E";
            this.ribbonButton7.Image = global::ANNdotNet.Wnd.App.Properties.Resources.excel24;
            this.ribbonButton7.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.excel24;
            this.ribbonButton7.Name = "ribbonButton7";
            this.ribbonButton7.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton7.SmallImage")));
            this.ribbonButton7.Text = "Export";
            this.ribbonButton7.ToolTip = "Use this command to export selected model to Excel for further analysis.";
            this.ribbonButton7.ToolTipTitle = "Export selected model to Excel (Alt+E)";
            this.ribbonButton7.Click += new System.EventHandler(this.ExpExcel_Click);
            // 
            // ribbonButton8
            // 
            this.ribbonButton8.AltKey = "O";
            this.ribbonButton8.Image = global::ANNdotNet.Wnd.App.Properties.Resources.onnx32;
            this.ribbonButton8.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.onnx32;
            this.ribbonButton8.Name = "ribbonButton8";
            this.ribbonButton8.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton8.SmallImage")));
            this.ribbonButton8.Text = "ONNX";
            this.ribbonButton8.ToolTip = "Use this command to export selected ANN model to ONNX format.";
            this.ribbonButton8.ToolTipTitle = "Export selected model in to ONNX format (Alt+O)";
            this.ribbonButton8.Click += new System.EventHandler(this.ExpONNX_Click);
            // 
            // ribbonButton9
            // 
            this.ribbonButton9.AltKey = "CNTK";
            this.ribbonButton9.Image = global::ANNdotNet.Wnd.App.Properties.Resources.cntk_logo32;
            this.ribbonButton9.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.cntk_logo32;
            this.ribbonButton9.Name = "ribbonButton9";
            this.ribbonButton9.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton9.SmallImage")));
            this.ribbonButton9.Text = "CNTK";
            this.ribbonButton9.ToolTip = "Use this command to export selected ANN model to native CNTK format.";
            this.ribbonButton9.ToolTipTitle = "Export selected model in to CNTK format. (Alt+K)";
            this.ribbonButton9.Click += new System.EventHandler(this.CNTK_Click);
            // 
            // ribbonButton18
            // 
            this.ribbonButton18.Image = global::ANNdotNet.Wnd.App.Properties.Resources.eval32;
            this.ribbonButton18.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.eval32;
            this.ribbonButton18.Name = "ribbonButton18";
            this.ribbonButton18.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton18.SmallImage")));
            this.ribbonButton18.Text = "Evaluate";
            this.ribbonButton18.Click += new System.EventHandler(this.evalModel_Click);
            // 
            // ribbonPanel2
            // 
            this.ribbonPanel2.ButtonMoreEnabled = false;
            this.ribbonPanel2.ButtonMoreVisible = false;
            this.ribbonPanel2.Items.Add(this.ribbonButton5);
            this.ribbonPanel2.Items.Add(this.ribbonButton6);
            this.ribbonPanel2.Name = "ribbonPanel2";
            this.ribbonPanel2.Text = "Training";
            // 
            // ribbonButton5
            // 
            this.ribbonButton5.AltKey = "P";
            this.ribbonButton5.Image = global::ANNdotNet.Wnd.App.Properties.Resources.runmodel16;
            this.ribbonButton5.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.runmodel16;
            this.ribbonButton5.Name = "ribbonButton5";
            this.ribbonButton5.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton5.SmallImage")));
            this.ribbonButton5.Text = "Start";
            this.ribbonButton5.ToolTip = "After all ANN parameters has been set, use this command to start GP search proces" +
    "s. In running mode, press this button one more time in order to stop running pro" +
    "cess";
            this.ribbonButton5.ToolTipTitle = "Start ANN process of searching and building model (Alt+P)";
            this.ribbonButton5.Click += new System.EventHandler(this.Run_Click);
            // 
            // ribbonButton6
            // 
            this.ribbonButton6.AltKey = "T";
            this.ribbonButton6.Image = global::ANNdotNet.Wnd.App.Properties.Resources.stopmodel16;
            this.ribbonButton6.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.stopmodel16;
            this.ribbonButton6.Name = "ribbonButton6";
            this.ribbonButton6.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton6.SmallImage")));
            this.ribbonButton6.Text = "Stop";
            this.ribbonButton6.ToolTip = "Use this command to stop GP running process.";
            this.ribbonButton6.ToolTipTitle = "Stops the ANN running process (Alt +T)";
            this.ribbonButton6.Click += new System.EventHandler(this.Stop_Click);
            // 
            // ribbonPanel4
            // 
            this.ribbonPanel4.ButtonMoreEnabled = false;
            this.ribbonPanel4.ButtonMoreVisible = false;
            this.ribbonPanel4.Items.Add(this.ribbonButton10);
            this.ribbonPanel4.Items.Add(this.ribbonButton11);
            this.ribbonPanel4.Name = "ribbonPanel4";
            this.ribbonPanel4.Text = "Help";
            // 
            // ribbonButton10
            // 
            this.ribbonButton10.AltKey = "A";
            this.ribbonButton10.Image = global::ANNdotNet.Wnd.App.Properties.Resources.about16;
            this.ribbonButton10.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.about16;
            this.ribbonButton10.Name = "ribbonButton10";
            this.ribbonButton10.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton10.SmallImage")));
            this.ribbonButton10.Text = "About";
            this.ribbonButton10.ToolTip = "Use this command to see ANNdotNET owner and license details.";
            this.ribbonButton10.ToolTipTitle = "About ANNdotNET (Alt+A)";
            this.ribbonButton10.Click += new System.EventHandler(this.About_Click);
            // 
            // ribbonButton11
            // 
            this.ribbonButton11.AltKey = "X";
            this.ribbonButton11.Image = global::ANNdotNet.Wnd.App.Properties.Resources.exit16;
            this.ribbonButton11.LargeImage = global::ANNdotNet.Wnd.App.Properties.Resources.exit16;
            this.ribbonButton11.Name = "ribbonButton11";
            this.ribbonButton11.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton11.SmallImage")));
            this.ribbonButton11.Text = "Exit";
            this.ribbonButton11.ToolTip = "While GP process is running, it is not recommended to exit the application.";
            this.ribbonButton11.ToolTipTitle = "Exit ANNdotNET (Alt+X)";
            this.ribbonButton11.Click += new System.EventHandler(this.Exit_Click);
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.Image")));
            this.ribbonButton1.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.LargeImage")));
            this.ribbonButton1.MaxSizeMode = System.Windows.Forms.RibbonElementSizeMode.Compact;
            this.ribbonButton1.Name = "ribbonButton1";
            this.ribbonButton1.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.SmallImage")));
            this.ribbonButton1.Text = "ribbonButton1";
            // 
            // ribbonDescriptionMenuItem2
            // 
            this.ribbonDescriptionMenuItem2.DescriptionBounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.ribbonDescriptionMenuItem2.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.ribbonDescriptionMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem2.Image")));
            this.ribbonDescriptionMenuItem2.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem2.LargeImage")));
            this.ribbonDescriptionMenuItem2.Name = "ribbonDescriptionMenuItem2";
            this.ribbonDescriptionMenuItem2.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem2.SmallImage")));
            this.ribbonDescriptionMenuItem2.Text = "ribbonDescriptionMenuItem2";
            // 
            // ribbonDescriptionMenuItem3
            // 
            this.ribbonDescriptionMenuItem3.DescriptionBounds = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.ribbonDescriptionMenuItem3.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Left;
            this.ribbonDescriptionMenuItem3.Image = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem3.Image")));
            this.ribbonDescriptionMenuItem3.LargeImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem3.LargeImage")));
            this.ribbonDescriptionMenuItem3.Name = "ribbonDescriptionMenuItem3";
            this.ribbonDescriptionMenuItem3.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonDescriptionMenuItem3.SmallImage")));
            this.ribbonDescriptionMenuItem3.Text = "ribbonDescriptionMenuItem3";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(935, 691);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ribbon1);
            this.ForeColor = System.Drawing.Color.DarkViolet;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(900, 730);
            this.Name = "MainWindow";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Ribbon ribbon1;
        private System.Windows.Forms.RibbonTab ribbonTab1;
        private System.Windows.Forms.RibbonPanel ribbonPanel1;
        private System.Windows.Forms.RibbonPanel ribbonPanel2;
        private System.Windows.Forms.RibbonPanel ribbonPanel3;
        private System.Windows.Forms.RibbonPanel ribbonPanel4;
        private System.Windows.Forms.RibbonButton ribbonButton2;
        private System.Windows.Forms.RibbonButton ribbonButton3;
        private System.Windows.Forms.RibbonButton ribbonButton4;
        private System.Windows.Forms.RibbonButton ribbonButton5;
        private System.Windows.Forms.RibbonButton ribbonButton6;
        private System.Windows.Forms.RibbonButton ribbonButton7;
        private System.Windows.Forms.RibbonButton ribbonButton8;
        private System.Windows.Forms.RibbonButton ribbonButton9;
        private System.Windows.Forms.RibbonButton ribbonButton10;
        private System.Windows.Forms.RibbonButton ribbonButton11;
        private System.Windows.Forms.RibbonButton ribbonButton1;
        private System.Windows.Forms.RibbonButtonList ribbonButtonList1;
        private System.Windows.Forms.RibbonItemGroup ribbonItemGroup1;
        private System.Windows.Forms.RibbonButton ribbonButton12;
        private System.Windows.Forms.RibbonDescriptionMenuItem ribbonDescriptionMenuItem1;
        private System.Windows.Forms.RibbonDescriptionMenuItem ribbonDescriptionMenuItem2;
        private System.Windows.Forms.RibbonDescriptionMenuItem ribbonDescriptionMenuItem3;
        private System.Windows.Forms.RibbonButton ribbonButton14;
        private System.Windows.Forms.RibbonButton ribbonButton15;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;

        
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;

        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage7;
        private System.Windows.Forms.RibbonPanel ribbonPanel5;
        private System.Windows.Forms.RibbonButton ribbonButton13;
        private System.Windows.Forms.RibbonButton ribbonButton16;
        private System.Windows.Forms.RibbonButton ribbonButton17;
        private System.Windows.Forms.RibbonButton ribbonButton18;
        private System.Windows.Forms.RibbonSeparator ribbonSeparator1;
    }
}


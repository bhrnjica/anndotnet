
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANNdotNet.Wnd.Dll.Panels;
using MLDataPreparation.Dll;
using ANNdotNet.Wnd.Dll.Controllers;
using System.Threading;
using ANNdotNET.Net.Lib.Entities;
using ANNdotNET.Net.Lib.Controllers;
using ANNdotNET.Net.Lib;
using DataProcessing.MLBasicTypes;
using DataProcessing.MLData;
using System.IO;

namespace ANNdotNet.Wnd.App
{
    public partial class MainWindow : Form
    { 
        private AppController Controller { get; set; }
        private StartPanel startPanel1;
        private ExperimentPanel expPanel1;
        private RunPanel runPanel1;
        private TestPanel testPanel1;
        private InfoPanel infoPanel1;
        private CancellationTokenSource m_TokenSource;

        public MainWindow()
        {
            Controller = new AppController();
            Controller.ReportException = this.ReportException;
            InitializeComponent();
           

            this.Icon = Extensions.LoadIconFromName("ANNdotNet.Wnd.Dll.Images.anndotnet.ico");

            #region additional initialization
            this.startPanel1 = new ANNdotNet.Wnd.Dll.Panels.StartPanel();
            this.expPanel1 = new ExperimentPanel();
            this.runPanel1 = new RunPanel();
            this.testPanel1 = new TestPanel();
            this.infoPanel1 = new InfoPanel();

            // 
            // startPanel1
            // 
            this.startPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.startPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startPanel1.Location = new System.Drawing.Point(0, 0);
            this.startPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.startPanel1.Name = "startPanel1";
            this.startPanel1.New = null;
            this.startPanel1.Open = null;
            this.startPanel1.Size = new System.Drawing.Size(762, 395);
            this.startPanel1.TabIndex = 0;

            // 
            // expPanel1
            // 
            this.expPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.expPanel1.Location = new System.Drawing.Point(3, 3);
            this.expPanel1.Name = "expPanel1";
            this.expPanel1.Size = new System.Drawing.Size(463, 222);
            this.expPanel1.ShowOptionPanel();
            this.expPanel1.TabIndex = 0;
            // 
            // runPanel1
            // 
            this.infoPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoPanel1.Location = new System.Drawing.Point(3, 3);
            this.infoPanel1.Name = "runPanel1";
            this.infoPanel1.Size = new System.Drawing.Size(463, 222);
            this.infoPanel1.TabIndex = 1;
            
            // 
            // runPanel1
            // 
            this.runPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.runPanel1.Location = new System.Drawing.Point(3, 3);
            this.runPanel1.Name = "runPanel1";
            this.runPanel1.Size = new System.Drawing.Size(463, 222);
            this.runPanel1.TabIndex = 0;
           
            // 
            // testPanel1
            // 
            this.testPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testPanel1.Location = new System.Drawing.Point(3, 3);
            this.testPanel1.Name = "testPanel1";
            this.testPanel1.Size = new System.Drawing.Size(463, 222);
            this.testPanel1.TabIndex = 0;

            //
            this.splitContainer1.Panel2.Controls.Add(this.startPanel1);
            this.tabPage5.Controls.Add(runPanel1);
            this.tabPage7.Controls.Add(testPanel1);
            this.tabPage1.Controls.Add(expPanel1);
            this.tabPage2.Controls.Add(infoPanel1);
            
            #endregion
            treeView1.LabelEdit = true;
            this.Load += MainWindow_Load;
            this.FormClosing += MainWindow_FormClosing;
            this.ribbonTab1.Text = AboutANNdotNET.AssemblyTitle;

            SetStopMode("");
            //setup expanel
            expPanel1.LockEncoding = true;
            expPanel1.DefaultBEncoding = DataProcessing.MLBasicTypes.CategoryEncoding.OneHot;

        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ribbonButton5.Checked)
            {
                if (MessageBox.Show("Model is running? By closing window you will lose all unsaved data.", "ANNdotNET", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            else if (MessageBox.Show("Are you sure you want to Exit ANNdotNET?", "ANNdotNET", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

            treeView1.ExpandAll();
            treeView1.Select();
            startPanel1.Open = Open;
            //
            this.ribbonTab1.Text = "ANNdotNET v1.0 alpha";
        }


        #region Handling View
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            //get previously selected node to save state
            var n = treeView1.SelectedNode;
            if (n == null)
                return;
            //
            SaveView(n.Tag.ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //splitContainer1.Panel2
            if (e.Node.Text == "Start Page")
            {
                startPanel1.Visible = true;
                tabControl1.Visible = false;
                tabControl2.Visible = false;

            }
            else if (e.Node.Level == 0)
            {
                startPanel1.Visible = false;
                tabControl1.Visible = true;
                tabControl2.Visible = false;
            }
            else if (e.Node.Level == 1)
            {
                startPanel1.Visible = false;
                tabControl1.Visible = false;
                tabControl2.Visible = true;
            }
            else
            {
                startPanel1.Visible = true;
                tabControl1.Visible = false;
                tabControl2.Visible = false;
            }

            ShowView(e.Node.Tag.ToString());
        }

        private void ShowView(string guid)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                var p = Controller.GetView(guid);
                this.Text = "Start Page";
                Controller.ActiveView = p;

                if (p == null)
                {
                    //disable model commands
                    ribbonPanel3.Enabled = false;
                    ribbonPanel2.Enabled = false;
                    ribbonPanel5.Enabled = false;
                    return;
                }
                    
                //
                if(p is ProjectController)
                {
                   var pController = (ProjectController)p;
                   this.Text = pController.Project.Name;

                    if (pController.Project.DataSet!=null)
                     expPanel1.SetDataSet(pController.Project.DataSet);

                    //disable model commands
                    ribbonPanel3.Enabled = false;
                    ribbonPanel2.Enabled = false;
                    ribbonPanel5.Enabled = true;
                    //enable project commands

                }
                else if(p is ANNModel)
                {

                    var pModel = (ANNModel)p;
                    runPanel1.Reset(pModel);
                    if (pModel.ModelData == null)
                    {
                        runPanel1.ActivatePanel(ActiveModelData.GetDefaults(), pModel);
                        
                    }
                    else
                    {
                        runPanel1.ActivatePanel(pModel.ModelData, pModel);
                    }

                    testPanel1.ActivatePanel(pModel);
                    this.Text = pModel.Name;
                    //enable model commands
                    ribbonPanel3.Enabled = true;
                    ribbonPanel2.Enabled = true;
                    ribbonPanel5.Enabled = false;
                }



            }
            catch (Exception ex)
            {
                ReportException(ex);
               // throw;
            }
            finally
            {
                //back normal cursor
                Cursor.Current = Cursors.Default;
            }

        }

        

        private void SaveView(string guid)
        {
            try
            {
                var p = Controller.GetView(guid);

                if (p == null)
                    return;

                else if (p is ProjectController)
                {
                    Cursor.Current = Cursors.WaitCursor;


                    var pController = p as ProjectController;

                    //after the data is retrieved panel should be reset
                    if(pController.Project.DataSet != null)
                        pController.Project.DataSet= expPanel1.GetDataSet();

                    expPanel1.ResetExperimentalPanel();

                }
                else if (p is ANNModel)
                {
                    var model = p as ANNModel;

                    //save only termination criteria from run panel
                    model.ModelData = runPanel1.GetParameters();
                    runPanel1.ResetChart();
                    //test panel
                    testPanel1.ResetChart();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //back normal cursor
                Cursor.Current = Cursors.Default;
            }

        }

        /// <summary>
        /// Central method for handling and showing all application exception
        /// </summary>
        /// <param name="ex"></param>
        void ReportException(Exception ex)
        {
            MessageBox.Show(ex.Message);

        }
        #endregion


        #region New, Open Save and Close Project
        public void Open(string filePath)
        {
            try
            {
                //OpenFileDialog dlg = new OpenFileDialog();
                //dlg.Filter = "CNTK Model File |*.model| All files (*.*)|*.*";
                //if (dlg.ShowDialog() == DialogResult.OK)
                //{
                //    //savedModelChechState = dlg.FileName;
                //    //var str = File.ReadAllLines(dlg.FileName + ".dnn");
                //    //textBox6.Text = str[0];
                //    //textBox5.Text = str[1];
                //    //textBox7.Text = str[2];
                //    //textBox8.Text = str[3];
                //    //textBox9.Text = str[4];
                //    //m_model = Function.Load(dlg.FileName, DeviceDescriptor.CPUDevice);
                //}
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
        }
        public void New()
        {
            if(treeView1.Nodes.Count > 1)
            {
                MessageBox.Show("Multiple project creation is not supported! CUrrent project can be closed by selecting close command.");
                return;
            }

            TreeNode tn = createTreeNode(Guid.NewGuid().ToString(), $"Project{treeView1.Nodes.Count}", 1);
            treeView1.Nodes.Add(tn);

            //create project in backed
            Controller.NewProject(tn.Tag.ToString(), tn.Text);

            //then select project
            treeView1.SelectedNode = tn;
        }

        private void Save(object exp, string filePath)
        {
            try
            {
                ////save the trained model
                //if (m_Trainer != null)
                //{
                //    SaveFileDialog dlg = new SaveFileDialog();
                //    dlg.Filter = "CNTK Model File | *.model | All files(*.*) | *.* ";
                //    if (dlg.ShowDialog() == DialogResult.OK)
                //    {
                //        string inputDim = textBox6.Text;
                //        string outputDim = textBox5.Text;
                //        string embedDim = textBox7.Text;
                //        string hidDim = textBox8.Text;
                //        string cellDim = textBox9.Text;


                //        m_Trainer.SaveCheckpoint(dlg.FileName);
                //        File.WriteAllLines(dlg.FileName + ".dnn", new string[] { inputDim, outputDim, embedDim, hidDim, cellDim });
                //        var lns = File.ReadAllLines(dlg.FileName + ".dnn");
                //    }
                //}
            }
            catch (Exception ex)
            {
               ReportException(ex);
            }
        }
        
        private void CloseProject()
        {
            try
            {
                //
                var project = getActiveProject();
                if (project == null)
                    return;

                //close project
                if (Controller.CloseProject(project))
                {
                    TreeNode fNode = null;
                    foreach (TreeNode nn1 in treeView1.Nodes)
                    {
                        if (nn1.Tag.Equals(project.GetGuid()))
                        {
                            fNode = nn1;
                            break;
                        }
                        else
                            fNode = fromID(project.GetGuid(), nn1);
                    }

                    //
                    treeView1.Nodes.Remove(fNode);

                    //destroj the project
                    project = null;
                }
            }
            catch (Exception ex)
            {

                ReportException(ex);
            }
        }

        private void Run()
        {
            try
            {
                //
                if (m_TokenSource != null && !m_TokenSource.Token.IsCancellationRequested)
                {
                    m_TokenSource.Cancel();
                    return;
                }

                m_TokenSource = new CancellationTokenSource();
                var setData = runPanel1.GetParameters();
                runPanel1.ResetData();
                Controller.Run(setData, reportProgres,m_TokenSource.Token);
            }
            catch (Exception ex)
            {
                ReportException(ex);

            }
            
        }

        void reportProgres(int i, float loss, float eval, (List<List<float>>, List<List<float>>, List<List<float>>) trainD, 
                                                          (List<List<float>>, List<List<float>>, List<List<float>>) testD)
        {
            runPanel1.ReportProgress(i, loss, eval, trainD.Item1, trainD.Item2);
            testPanel1.ReportProgress(i, loss, eval, testD.Item1, testD.Item2);
        }

        private void Stop()
        {
            m_TokenSource.Cancel();
        }

       

        #endregion

        #region Model Creation and Updating
        void CreateModel(bool isRandomizedData)
        {
            if (Controller.ActiveView == null)
                return;
            ProjectController p = null;
            if (Controller.ActiveView is ProjectController)
            {
                p = (ProjectController)Controller.ActiveView;
            }
            else if (Controller.ActiveView is ANNModel)
            {
                p = (ProjectController)((ANNModel)Controller.ActiveView).Parent.Controller;
            }

            CreateModel(p, isRandomizedData);
        }
        
        public void CreateModel(ProjectController project, bool randomizeData)
        {
            try
            {
                TreeNode fNode = null;
                foreach (TreeNode nn1 in treeView1.Nodes)
                {
                    if (nn1.Tag.Equals(project.GetGuid()))
                    {
                        fNode = nn1;
                        break;
                    }
                    else
                        fNode = fromID(project.GetGuid(), nn1);
                }

                //
                string guid = Guid.NewGuid().ToString();
                var modelName = $"Model{fNode.Nodes.Count}";
                //create model

                //prepare data
                // create full dataset with all columns and rows
                var fulldata = expPanel1.GetDataSet();
                project.Project.DataSet = fulldata;

                var countOutputCol = fulldata.MetaData.Where(x => x.Param.Equals(ParameterType.Output.Description(),
                        StringComparison.InvariantCultureIgnoreCase) && !x.IsIngored).Count();

                if (countOutputCol != 1)
                {
                    MessageBox.Show("Error: Only one output column must be defined, model creation cannot be proceeded.");
                    return;
                }

                //
                string fileName = System.IO.Path.GetTempPath() + guid + "cntk";
                var exp = ExportData.WriteFiles(fileName, (true, expPanel1.randomoizeDataSet.Checked, '\t',
                    (int)expPanel1.numCtrlNumForTest.Value, expPanel1.presentigeRadio.Checked), fulldata);

                //
                var inputCount = exp.GetEncodedColumnInputCount();
                var outCol = exp.GetColumnsFromOutput().First();
                var outputCount = outCol.GetEncodedColumCount();

                var trainRowCount = (uint)exp.GetRowCount(false);
                var testRowCount = (uint)exp.GetRowCount(true);
                List<string> classes = null;
                string label = exp.GetColumnsFromOutput(false).First().Name;

                if (inputCount > 1)
                    classes = exp.GetColumnsFromOutput(false).First().Statistics.Categories;

                project.CreateModel(guid, modelName, fileName, inputCount, outputCount, trainRowCount, testRowCount, classes, label, randomizeData);
                


                //expand tree item and select it
                var tn = createTreeNode(guid, modelName, 2);
                fNode.Nodes.Add(tn);
                tn.Expand();
                treeView1.SelectedNode = tn;

            }
            catch (Exception ex)
            {

                ReportException(ex);
            }
        }
        private void DeleteModel(TreeNode sn)
        {
            //var m = getActiveModel() as dynamic;
            //if(m!=null && m.Model.Guid == sn.Tag.ToString())
            //{
            //    var result = MessageBox.Show($"Are you sure you want to delete {m.Model.Name} model?","ANNdotNET" ,MessageBoxButtons.YesNo);
            //    if (result == DialogResult.No)
            //        return;

            //    var project = m.Parent;
            //    project.Models.Remove(m);
            //    //
            //    var p= project.Project.Models.Where(x=>x.Guid== m.Model.Guid).FirstOrDefault();
            //    if(p!=null)
            //    {
            //        project.Project.Models.Remove(p);
            //    }
            //    project.Project.IsDirty = true;
            //    treeView1.Nodes.Remove(sn);
            //}
        }
        private void RenameTreeItem(TreeNode sn, string newName)
        {
            //if(sn.Level==0)
            //{
            //    var p = getActiveProject();
            //    if (p != null && p.Project.Guid == sn.Tag.ToString())
            //    {
            //        p.Project.Name = newName;
            //        p.Project.IsDirty = true;
                   
            //    }
            //}
            //else if(sn.Level == 1)
            //{
            //    var m = getActiveModel();
            //    if (m != null && m.Model.Guid == sn.Tag.ToString())
            //    {
            //        m.Model.Name = newName;
            //        m.Model.IsDiry = true;
            //    }
            //}
            //sn.Text = newName;

        }
        void UpdateModel(bool randomizeData)
        {
            if (MessageBox.Show("Once the model is updated, previous solution will be discarded.", "ANNdotNET", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            //DialogResult res = DialogResult.OK;
            //var modelIndex = 0;
            //var project = getActiveProject();
            //if(project==null)
            //{
            //    MessageBox.Show("Project is not selected.");
            //    return;
            //}
            //if(project.Models.Count==0)
            //{
            //    MessageBox.Show("Select new project.");
            //    return;
            //}
            //else if (project.Models.Count > 1)
            //{
            //    PromptForName dlg = new PromptForName();
            //    dlg.comboBox1.Items.AddRange(project.Models.Select(x=>x.Model.Name).ToArray());
            //    res = dlg.ShowDialog();
            //    modelIndex = dlg.comboBox1.SelectedIndex;
            //}

            //if(res== DialogResult.OK)
            //{
            //    var model = project.Project.Models[modelIndex];

            //    var dataset = project.Project.DataSet.GetDataSet(randomizeData);
            //    var exp = new Data.Experiment(dataset);


            //    model.DataSet = dataset;
            //    model.ExpData = exp;
                
            //    model.ResetSolution();
                
            //}
        }

        /// <summary>
        /// Evaluate model
        /// </summary>
        private void evaluateModel()
        {
            if (Controller.ActiveView == null)
                return;
            ANNModel model = null;
            if (Controller.ActiveView is ProjectController)
            {
                return;
            }
            else// if (Controller.ActiveView is ANNModel)
            {
                model = (ANNModel)Controller.ActiveView;
                
            }

            Controller.EvaluateModel(model);
        }
        #endregion

        #region Helper
        //inline methods for 
        private TreeNode fromID(string itemId, TreeNode rootNode)
        {
            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Tag.Equals(itemId))
                    return node;
                TreeNode next = fromID(itemId, node);
                if (next != null)
                    return next;
            }
            return null;
        }
        private TreeNode getTreeItem(string projectName)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Text.Equals(projectName))
                    return node;
            }
            return null;
        }
        private ProjectController getActiveProject()
        {

            var n = treeView1.SelectedNode;
            if (n == null)
            {
                MessageBox.Show("Select project before action.");
                return null;
            }

            ProjectController exp = null;

            var activeView = Controller.GetView(n.Tag.ToString());
            if (activeView is ANNModel ee)
                exp = (ProjectController)ee.Parent.Controller;
            else if (activeView is ProjectController)
                exp = (ProjectController)activeView;

            return exp;
        }
        private object getActiveModel()
        {
            var n = treeView1.SelectedNode;
            if (n == null)
            {
                MessageBox.Show("Select model before action.");
                return null;
            }

            ANNModel exp = null;

            var activeView = Controller.GetView(n.Tag.ToString());

            if (activeView is ANNModel ee)
                return ee;
            else
            {
                MessageBox.Show("Select model before action.");
                return null;
            }
        }

        private TreeNode createTreeNode(string guid, string name, int image)
        {
            var n = new TreeNode();
            n.Text = name;
            n.Tag = guid;
            n.SelectedImageIndex = image;
            n.ImageIndex = image;
            return n;
        }
        private string PromptToOpenFile(string fileDescription = "ANNdotNET standard file", string extension = "*.gpa")
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();

            if (string.IsNullOrEmpty(extension))
                dlg.Filter = "Plain text files (*.csv;*.txt;*.dat)|*.csv;*.txt;*.dat |All files (*.*)|*.*";
            else
                dlg.Filter = string.Format("{1} ({0})|{0}|All files (*.*)|*.*", extension, fileDescription);
            //
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }
        private string PromptToSaveFile(string fileDescription = "ANNdotNET standard file", string extension = "*.gpa")
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();

            if (string.IsNullOrEmpty(extension))
                dlg.Filter = "Plain text files (*.csv;*.txt;*.dat)|*.csv;*.txt;*.dat |All files (*.*)|*.*";
            else
                dlg.Filter = string.Format("{1} ({0})|{0}|All files (*.*)|*.*", extension, fileDescription);

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return dlg.FileName;
            else
                return null;


        }
        public void SetStopMode(string guid)
        {
            //setColorForGUI(false);
            //var model = Controller.GetView(guid);
            //if (model != null && model is ModelController)
            //{
            //    ((ModelController)model).IsRunnig=false;
            //    if(((ModelController)model).CancelationSource!=null)
            //        ((ModelController)model).CancelationSource.Cancel();
            //}
               
            //setImageIndexToRunningModel(guid, 2);
            //ribbonButton5.Checked = false;

            //this.toolStripStatusLabel1.Text = "Ready";
            //this.toolStripStatusLabel2.Text = "No application message.";
            //AppStatus = "Ready";
            //StatusMessage = "No application message.";
        }

        private void setColorForGUI(bool isRunning)
        {
           
            var color1 = Color.BlueViolet;
            var color2 = Color.White;

            if (isRunning)
            {
                color1 = Color.Green;
                color2 = Color.White;
            }
               
            ribbon1.BackColor = color1;
            statusStrip1.BackColor = color1;
            startPanel1.ForeColor = Color.White;
            this.ForeColor = color2;
        }

        public void SetRunMode(string guid)
        {
            //AppStatus = "Running...";
            this.toolStripStatusLabel1.Text = "Running...";
            this.toolStripStatusLabel2.Text = "ANNdotNET search process has been started! ";
            //StatusMessage = "ANNdotNET search process has been started! ";

            ribbonButton5.Checked = !ribbonButton5.Checked;
            if (ribbonButton5.Checked == false)
                Stop();

            setImageIndexToRunningModel(guid, 3);

            setColorForGUI(true);
        }

        private void setImageIndexToRunningModel(string guid, int v)
        {
            TreeNode fNode = null;
            foreach (TreeNode nn1 in treeView1.Nodes)
            {
                if (nn1.Tag.Equals(guid))
                {
                    fNode = nn1;
                    break;
                }
                else
                    fNode = fromID(guid, nn1);
                if (fNode != null)
                    break;
            }
            if (fNode != null)
            {
                this.Invoke(
                   new Action(() =>
                   {
                       fNode.ImageIndex = v;
                       fNode.SelectedImageIndex = v;
                   }
                   ));
                
            }
        }
        #endregion

        #region command
        private void NewModel_Click(object sender, EventArgs e)
        {
            var pc = Controller.ActiveView as ProjectController;
            expPanel1.CreateModel = (x1)=> CreateModel(x1);
            expPanel1.CreateNewModel();
        }
        private void LoadDataSet_Click(object sender, EventArgs e)
        {

            expPanel1.LoadData();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            var str = PromptToOpenFile();
            if (!string.IsNullOrEmpty(str))
                Open(str);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            //var exp = getActiveProject();
            //if (exp == null)
            //    return;

            //string filePath = null;
            //if (string.IsNullOrEmpty(exp.Project.FilePath) || exp.Project.FilePath == "[New GP Project]")
            //    filePath = PromptToSaveFile();
            //else
            //    filePath = exp.Project.FilePath;

            //if (string.IsNullOrEmpty(filePath))
            //    return;

            //Save(exp, filePath);
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            var exp = getActiveProject();
            if (exp == null)
                return;

            string filePath = PromptToSaveFile();
            if (string.IsNullOrEmpty(filePath))
                return;

            Save(exp, filePath);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            CloseProject();
        }

        private void New_Click(object sender, EventArgs e)
        {

            New();
        }
        private void Run_Click(object sender, EventArgs e)
        {

            Run();
        }

        private void Stop_Click(object sender, EventArgs e)
        {

            Stop();

        }
        private void ExpExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // 
                var model = getActiveModel() as ANNModel;
                if (model == null)
                    return;

                var filepath = PromptToSaveFile("Microsoft Excel files", " *.xlsx");
                if (!string.IsNullOrEmpty(filepath))
                    model.ExportToE(filepath);
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }
        }

        private void ExpONNX_Click(object sender, EventArgs e)
        {
            try
            {
                //not implemented
                return;
                var model = getActiveModel() as ANNModel;
                if (model == null)
                    return;

                var filepath = PromptToSaveFile("ONNX files", " *.nb");
                if (!string.IsNullOrEmpty(filepath))
                    model.ExportONNX(filepath);
            }
            catch (Exception ex)
            {
                if (ex != null)
                 ReportException(ex);
            }
        }

        private void CNTK_Click(object sender, EventArgs e)
        {
            try
            {
                //not implemented
                var model = getActiveModel() as ANNModel;
                if (model == null)
                    return;

                var filepath = PromptToSaveFile("CNTK files", "*.cntk");
                if (!string.IsNullOrEmpty(filepath))
                    model.ExportCNTK(filepath);
            }
            catch (Exception ex)
            {
                if (ex != null)
                  ReportException(ex);
            }
        }
        private void evalModel_Click(object sender, EventArgs e)
        {
            try
            {
                var model = getActiveModel();
                if (model == null)
                    return;

                evaluateModel();
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }
        }

    
        private void About_Click(object sender, EventArgs e)
        {
            var dlg = new AboutANNdotNET();
            dlg.ShowDialog();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                var sn = treeView1.SelectedNode;

                if (sn != null && sn.Level == 1 && !sn.IsEditing)
                {
                    DeleteModel(sn);
                }

            }

        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.CancelEdit || e.Node.Text == e.Label || e.Label==null)
                return;
            RenameTreeItem(e.Node, e.Label);
        }




        #endregion

        
    }
}

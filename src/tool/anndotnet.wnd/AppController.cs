//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using anndotnet.wnd.commands;
using anndotnet.wnd.Models;
using anndotnet.wnd.Mvvm;
using ANNdotNET.Lib;
using MLDataPreparation.Dll;
using NNetwork.Core.Common;

namespace anndotnet.wnd
{
    /// <summary>
    /// Controller implementation for Main application window
    /// </summary>
    public class AppController : ObservableObject
    {
        CancellationTokenSource m_TokenSource;
        public Action<Exception> ReportException { get; internal set; }

        public AppController()
        {
            //app model creation
            m_appModel = new AppModel();

            //register commands
            registerCommands();

            //load start page
            var start = newStartPage();

            //select start page
            start.IsSelected = true;
        }


        #region Properties
        /// <summary>
        /// indicator shows if the Run button pressed
        /// </summary>
        private bool m_IsRunChecked;
        public bool IsRunChecked
        {
            get
            {
                return m_IsRunChecked;
            }
            set
            {
                if (m_IsRunChecked != value)
                {
                    m_IsRunChecked = value;
                    RaisePropertyChangedEvent("IsRunChecked");
                    RaisePropertyChangedEvent("IsTrainRunning");

                }
            }
        }

        /// <summary>
        /// indicator shows if the application is in training process
        /// </summary>
        public bool IsTrainRunning
        {
            get
            {
                return !IsRunChecked;
            }
           
        }

        /// <summary>
        /// Main application model
        /// </summary>
        private AppModel m_appModel;
        public AppModel AppModel
        {
            get
            {
                return m_appModel;
            }
            set
            {
                if (m_appModel != value)
                {
                    m_appModel = value;
                    RaisePropertyChangedEvent("AppModel");
                }
            }
        }

        /// <summary>
        /// Currently selected ModelView
        /// </summary>
        private BaseModel m_activeViewModel;
        public BaseModel ActiveViewModel
        {
            get
            {
                return m_activeViewModel;
            }
            set
            {
                if (m_activeViewModel != value)
                {
                    m_activeViewModel = value;
                    RaisePropertyChangedEvent("ActiveViewModel");
                }
            }
        }

        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        string m_StatusMessage = "No application message.";
        public string StatusMessage
        {
            get
            {
                return $"Last status message: {m_StatusMessage}";
            }
            set
            {
                m_StatusMessage = value;
               RaisePropertyChangedEvent("StatusMessage");
            }
        }
      
        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        string m_AppStatus = " Ready!";
        public string AppStatus
        {
            get
            {
                return m_AppStatus;
            }
            set
            {
                m_AppStatus = value;
                RaisePropertyChangedEvent("AppStatus");
            }
        }
        #endregion

        #region Helpers

        /// <summary>
        /// creates new project. The command creates new project file 
        /// and folder where other files will be created. No other  file location should exists. 
        /// </summary>
        /// <returns>currently created experiment</returns>
        public ANNProjectController NewProject()
        {
            try
            {
                var filePath = promptToSaveFile();
                if (string.IsNullOrEmpty(filePath))
                    return null;
                var prj = new ANNProjectController(ActiveModelChanged);
                Project.NewProjectFile(Path.GetFileNameWithoutExtension(filePath), filePath);
                prj.Initproject(filePath);
                return prj;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public void OpenProject(string filePath)
        {
            try
            {
                if (AppModel.Project.Count > 1)
                    throw new Exception("Close previous project, before open a new one.");
                var existing = new ANNProjectController(ActiveModelChanged);
                existing.Initproject(filePath);
                m_appModel.Project.Add(existing);
                existing.IsSelected = true;
                existing.IsExpanded = true;
            }
            catch (Exception ex)
            {
                ReportException(ex);
                //throw;
            }
           
        }
        /// <summary>
        /// creates new project
        /// </summary>
        /// <param name="name"></param>
        /// <returns>currently created experiment</returns>
        private BaseModel newStartPage()
        {
            var exp = new StartModel(ActiveModelChanged);
            m_appModel.Project.Add(exp);

            return exp;
        }

        /// <summary>
        /// Handling active View of the application
        /// </summary>
        /// <param name="active"></param>
        /// <param name="isSelected"></param>
        void ActiveModelChanged(BaseModel active, bool isSelected)
        {
            if (isSelected)
            {
                //
                ActiveViewModel = active;
            }
            else
            {
                //deactivation of previous model
                ActiveViewModel.IsEditing = false;
            }

        }

        /// <summary>
        /// Helper method for color changing when the application
        /// changes the states of training process
        /// </summary>
        /// <param name="isRunning"></param>
        public void SetRunnigColor(bool isRunning)
        {
            if (isRunning)
            {
               // Application.Current.Resources["ANNdotNET.CustomColorBrush"] = new SolidColorBrush(Colors.Green);
                Application.Current.Resources["RibbonThemeColorBrush"] = new SolidColorBrush(Colors.Green);
                Application.Current.Resources["ANNdotNET.HighlightColor"] = Colors.Green;
                Application.Current.Resources["ANNdotNET.InactiveForeground"] = Colors.Green;

                AppStatus = "Running...";
                StatusMessage = "ANNdotNET training process has been started! ";
            }

            else
            {
                //Application.Current.Resources["ANNdotNET.CustomColorBrush"] = new SolidColorBrush(Colors.BlueViolet);
                Application.Current.Resources["RibbonThemeColorBrush"] = new SolidColorBrush(Colors.BlueViolet);
                Application.Current.Resources["ANNdotNET.HighlightColor"] = Colors.BlueViolet;
                Application.Current.Resources["ANNdotNET.InactiveForeground"] = Colors.Blue;

                AppStatus = "Ready";
                StatusMessage = "No application message.";
            }

        }

        internal void DeleteModel(MLConfigController model)
        {
            try
            {
               var prj = AppModel.Project[1] as ANNProjectController;
                prj.Models.Remove(model);
                model.DeleteModel();

            }
            catch (Exception ex)
            {

                if (ReportException != null)
                    ReportException(ex);
            }
           
        }

        public void TrainingCompleated(TrainResult result)
        {
            try
            {
                if (!(ActiveViewModel is MLConfigController))
                    return;
                if (m_TokenSource != null)
                    m_TokenSource.Dispose();
                //
                m_TokenSource = null;

                //un check run button
                var mainWnd = App.Current.MainWindow as MainWindow;
                mainWnd.StopButtonClick();

                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                new Action(

                    () =>
                    {
                        //
                        IsRunChecked = false;
                        SetRunnigColor(false);
                        StatusMessage = $"Training process {result.ProcessState} at {result.Iteration} iterations!";
                    }

                ));
                

            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }
        }
        #endregion

        #region Commands

        private void registerCommands()
        {
            var binding = new CommandBinding(AppCommands.AboutCommand, onAbout, onCanExecAbout);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.CloseCommand, onClose, onCanExecClose);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.ExitCommand, onExit, onCanExecExit);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.ExportCommand, onExport, onCanExecExport);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.SaveAsCommand, onSaveAs, onCanExecSaveAs);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);


            binding = new CommandBinding(AppCommands.SaveCommand, onSave, onCanExecSave);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.OpenCommand, onOpen, onCanExecOpen);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.NewCommand, onNew, onCanExecNew);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.RunCommand, onRun, onCanExecRun);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.StopCommand, onStop, onCanExecStop);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.AddLayerCommand, onAddLayer, onCanExecAddLayer);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.RemoveLayerCommand, onRemoveLayer, onCanExecRemoveLayer);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.LoadDataCommand, onLoadData, onCanExecLoadData);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);

            binding = new CommandBinding(AppCommands.CreateModelCommand, onCreateModel, onCanExecCreateModel);

            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);


            //tree item click
            binding = new CommandBinding(AppCommands.TreeItemClickCommand, onTreeItemClicked, onCanTreeItemClicked);
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), binding);
        }

        private void onCreateModel(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var prj = ActiveViewModel as ANNProjectController;
                if (prj != null)
                {
                    var m = new MLConfigController(ActiveModelChanged);
                    prj.CreateMLConfig(m);
                }

                return;
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
            
        }

        private void onCanExecCreateModel(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            var prj = ActiveViewModel as ANNProjectController;
            if (prj == null)
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = !(prj.DataSet==null || prj.DataSet.Data==null);

        }

        private void onLoadData(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //prepare control to load data
                var cntCtrl = e.Parameter as DependencyObject;
                var ctrl = FindChild<WindowsFormsHost>(cntCtrl, "hostWF");
                var expCtrl = (DataPanel)ctrl.Child;
                if (expCtrl == null)
                    return;

                expCtrl.LoadData();
                //setup project 
                //
                var prj = ActiveViewModel as ANNProjectController;
                if (prj == null)
                    throw new Exception("Active project is null.");
                prj.DataSet = expCtrl.GetDataSet();
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
            
           
        }

        private void onCanExecLoadData(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;

            var cntCtrl = e.Parameter as DependencyObject;
            if (cntCtrl == null)
            {
                e.CanExecute = false;
                return;
            }
              
            var ctrl = FindChild<WindowsFormsHost>(cntCtrl, "hostWF");

            if (ctrl == null)
            {
                e.CanExecute = false;
                return;
            }

           // var expCtrl = (DataPanel)ctrl.Child;
            if (ctrl.Child is DataPanel)
                e.CanExecute = false;

            e.CanExecute = true;
        }

        private void onCanExecAddLayer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onAddLayer(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                //
                var model = ActiveViewModel as MLConfigController;
                var layer = ((ComboBoxItem)e.Parameter).Content as string;
                //check if network configuration is Custom,
                if (model.Network.Where(x => x.Type == LayerType.Custom).Count() > 0)
                {
                    MessageBox.Show("Custom configuration is not allow to be edited.", "ANNdotNET");
                    return;
                }

                //
                if (e.Parameter == null)
                {
                    MessageBox.Show("Please select Layer from combo box first!", "ANNdotNET");
                    return;
                }

                

                //create layer
                var itm = new NNLayer();
                itm.UseActivation = true;
                itm.Activation = Activation.None;
                if (layer == "Normalization Layer")
                    itm.Type = LayerType.Normalization;
                else if (layer == "Dense Layer")
                    itm.Type = LayerType.Dense;
                else if (layer == "LSTM Layer")
                {
                    itm.Activation = Activation.TanH;
                    itm.Type = LayerType.LSTM;
                }

                else if (layer == "Embedding Layer")
                {
                    itm.Type = LayerType.Embedding;
                    itm.UseActivation = false;
                }

                else if (layer == "Drop Layer")
                    itm.Type = LayerType.Drop;
                else
                    throw new Exception("Unsupported Layer!");
                itm.Name = layer;
                //normalization layer must be on first position
                if (itm.Type == LayerType.Normalization)
                {
                    if(model.Network.Where(x=>x.Type== LayerType.Normalization).Count() == 0)
                        model.Network.Insert(0,itm);
                    else
                    {
                        MessageBox.Show("Only one normalization layer is allowed.");
                    }
                }
                else
                    model.Network.Add(itm);
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
        }

        private void onCanExecRemoveLayer(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onRemoveLayer(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (e.Parameter == null)
                {
                    MessageBox.Show("Please select Layer from combo box first!");
                    return;
                }
                var model = ActiveViewModel as MLConfigController;
                //check if network configuation is Custom,
                if (model.Network.Where(x => x.Type == LayerType.Custom).Count() > 0)
                {
                    MessageBox.Show("Custom configuration is not allow to be deleted.");
                    return;
                }

                var layer = ((NNLayer)e.Parameter);
                if(MessageBox.Show($"Are you sure you want to remove {layer.Name}?","ANNdotNET", 
                    MessageBoxButton.YesNo,MessageBoxImage.Warning)== MessageBoxResult.Yes)
                model.Network.Remove(layer);
            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
        }

        /// <summary>
        /// DoubleClick Event to enable DoubleClick command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onCanTreeItemClicked(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        /// <summary>
        /// DoubleClick Event to enable editing tree item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onTreeItemClicked(object sender, ExecutedRoutedEventArgs e)
        {
            //var model = e.Parameter as BaseModel;
            //if (model.IsSelected && !(model is StartModel))
            //    model.IsEditing = true;
            AppStatus = "Ready";
            StatusMessage = "No application message.";
        }

        private void onCanExecAbout(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onAbout(object sender, ExecutedRoutedEventArgs e)
        {
            AboutANNdotNET dlg = new AboutANNdotNET();
            dlg.Show();
            //MessageBox.Show("ANNdotNET vNext, Bahrudin Hrnjica, 2006-2018");
        }

        private void onCanExecClose(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = ActiveViewModel is ANNProjectController;
        }

        public void onClose(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var exp = ActiveViewModel as ANNProjectController;
                if (ActiveViewModel is ANNProjectController)
                {
                    var prj = (ANNProjectController)ActiveViewModel;
             
                    //save data
                    prj.Save();
                    //then close 
                    AppModel.Project.Remove(prj);
                    prj = null;
                }
                else if (ActiveViewModel is MLConfigController)
                {
                    var model = ActiveViewModel as MLConfigController;
                    model.SaveModel();
                }


            }

            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }
        }

        private void onCanExecExit(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onExit(object sender, ExecutedRoutedEventArgs e)
        {
            onClose(sender, e);
            App.Current.MainWindow.Close();           
        }

        private void onCanExecRun(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = (ActiveViewModel is MLConfigController && !IsRunChecked);
        }

        private void onRun(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var model = ActiveViewModel as MLConfigController;
                if (model != null)
                {
                    //
                    if (m_TokenSource != null && !m_TokenSource.Token.IsCancellationRequested)
                    {
                        m_TokenSource.Cancel();
                        return;
                    }
                    else if(m_TokenSource !=null && m_TokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    SetRunnigColor(true);
                    //uncheck run button
                    var mainWnd = App.Current.MainWindow as MainWindow;
                    mainWnd.RunButtonClick();

                    m_TokenSource = new CancellationTokenSource();
                    model.RunTraining(m_TokenSource.Token);
                }

            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }

        }
        
        private void onCanExecStop(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onStop(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (!(ActiveViewModel is MLConfigController))
                    return;
                if (m_TokenSource != null)
                    m_TokenSource.Cancel();
            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }


        }
        private void onCanExecExport(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ActiveViewModel is MLConfigController;
            e.Handled = true;
        }

        private void onExport(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                
                // 
                var model = ActiveViewModel as MLConfigController;
                if (model == null)
                    return;
                //save
                model.SaveModel();

                if (e.Parameter.ToString() == "Excel")
                {
                    var filepath = promptToSaveFile("Microsoft Excel files", " *.xlsx");
                    if (!string.IsNullOrEmpty(filepath))
                        model.ExportToExcel(filepath);
                }
                if (e.Parameter.ToString() == "CSV")
                {
                    var filepath = promptToSaveFile("comma separated values files", " *.csv");
                    if (!string.IsNullOrEmpty(filepath))
                        model.ExportToCSV(filepath);
                }
                else if (e.Parameter.ToString() == "CNTK")
                {
                    var filepath = promptToSaveFile("Microsoft CNTK files", " *.cntk");
                    if (!string.IsNullOrEmpty(filepath))
                        model.ExportToCNTK(filepath);
                }
                else if (e.Parameter.ToString() == "ONNX")
                {
                    var filepath = promptToSaveFile("ONNX files", " *.onnx");
                    if (!string.IsNullOrEmpty(filepath))
                        model.ExportToONNX(filepath);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }

        }

        private void onCanExecSaveAs(object sender, CanExecuteRoutedEventArgs e)
        {
            onCanExecSave(sender, e);
        }

        private void onSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string filePath = promptToSaveFile();
                if (string.IsNullOrEmpty(filePath))
                    return;

                var p = ActiveViewModel as ANNProjectController;

                p.Settings = new ProjectSettings();
                //
                var fi = new FileInfo(filePath);
                p.Settings.ProjectFolder = fi.Directory.FullName;
                p.Settings.ProjectFile = fi.Name;
                Project.NewProjectFile(Path.GetFileNameWithoutExtension(p.Settings.ProjectFile),filePath);
                p.Save();

            }
            catch (Exception ex)
            {
                ReportException(ex);
            }
        }

        private void onCanExecSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = !(ActiveViewModel is StartModel);
        }

        private void onSave(object sender, ExecutedRoutedEventArgs e)
        {

            try
            {
                
                if(ActiveViewModel is ANNProjectController)
                {
                    var p = ActiveViewModel as ANNProjectController;
                    p.Save();
                }
                else if (ActiveViewModel is MLConfigController)
                {
                    var m = ActiveViewModel as MLConfigController;
                    m.SaveModel();
                }

            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }
        }

        private void onCanExecOpen(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onOpen(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                string filePath = PromptToOpenFile();
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (AppModel.Project.Count == 2)
                    {
                        AppModel.Project.RemoveAt(1);
                        throw new Exception("Close existing project!");
                    }

                    OpenProject(filePath);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }
        }

       

        private void onCanExecNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.Handled = true;
            e.CanExecute = true;
        }

        private void onNew(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (AppModel.Project.Count > 1)
                    throw new Exception("Existing project must be closed!");

                var prj = NewProject();
                if (prj == null)
                    return;
                AppModel.Project.Add(prj);
                if (prj != null)
                {
                    prj.IsExpanded = true;
                    prj.IsSelected = true;
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                    ReportException(ex);
            }

        }

        #endregion
    }
}

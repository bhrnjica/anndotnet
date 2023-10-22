//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
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
using Anndotnet.App.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading;

namespace Anndotnet.App.ViewModel
{
    /// <summary>
    /// Controller implementation for Main application window
    /// </summary>
    public partial class AppViewModel : ObservableObject
    {
        CancellationTokenSource?  m_TokenSource;
        public Action<Exception>? ReportException { get; internal set; }

        public AppViewModel()
        {
            //app mlConfig creation
            _appModel = new AppModel();

            //register commands
           // registerCommands();

            //load start page
            //var start = newStartPage();

            //select start page
           // start.IsSelected = true;
        }


        #region Properties
        /// <summary>
        /// Indicator shows if the Run button pressed
        /// </summary>
        [ObservableProperty]
        private bool _isRunChecked;
        

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
        /// Main application mlConfig
        /// </summary>
        [ObservableProperty]
        private AppModel _appModel;


        /// <summary>
        /// Currently selected ModelView
        /// </summary>
        //[ObservableProperty]
        //private BaseModel _activeViewModel;
       

        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        [ObservableProperty]
        private string _statusMessage = "No application message.";


        /// <summary>
        /// Status message appear on Status bar
        /// </summary>
        [ObservableProperty]
        string _appStatus = " Ready!";
        
        #endregion

        #region Helpers

        /// <summary>
        /// creates new project. The command creates new project file 
        /// and folder where other files will be created. No other  file location should exists. 
        /// </summary>
        /// <returns>currently created experiment</returns>
        //public async Task<ANNProjectController> NewProject(ProjectType pType)
        //{
        //    try
        //    {
        //        var filePath = promptToSaveFile();
        //        if (string.IsNullOrEmpty(filePath))
        //            return null;
        //        var prj = new ANNProjectController(ActiveModelChanged);
        //        Project.NewProjectFile(Path.GetFileNameWithoutExtension(filePath), filePath, pType);
        //        await prj.Initproject(filePath, pType);
        //        return prj;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}
        //public async void OpenProject(string filePath)
        //{
        //    try
        //    {
        //        //restore cursor 
        //        MainWindow.SetCursor(true);

        //        if (AppModel.Project.Count > 1)
        //            throw new Exception("Close previous project, before open a new one.");
        //        //
        //        var existing = new ANNProjectController(ActiveModelChanged);
        //        await existing.Initproject(filePath);

        //        //add project to appModel
        //        m_appModel.Project.Add(existing);
        //        existing.IsSelected = true;
        //        existing.IsExpanded = true;
        //        //restore cursor 
        //        MainWindow.SetCursor(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        //restore cursor 
        //        MainWindow.SetCursor(false);
        //        ReportException(ex);
        //        //throw;
        //    }

        //}
        /// <summary>
        /// creates new project
        /// </summary>
        /// <param name="name"></param>
        /// <returns>currently created experiment</returns>
        //private BaseModel newStartPage()
        //{
        //    var exp = new StartModel(ActiveModelChanged);
        //    m_appModel.Project.Add(exp);

        //    return exp;
        //}

        /// <summary>
        /// Handling active View of the application
        /// </summary>
        /// <param name="active"></param>
        /// <param name="isSelected"></param>
        //void ActiveModelChanged(BaseModel active, bool isSelected)
        //{
        //    if (isSelected)
        //    {
        //        //
        //        ActiveViewModel = active;
        //    }
        //    else
        //    {
        //        //deactivation of previous mlConfig
        //        ActiveViewModel.IsEditing = false;
        //    }

        //}

        /// <summary>
        /// Helper method for color changing when the application
        /// changes the states of training process
        /// </summary>
        /// <param name="isRunning"></param>
        //public void SetRunnigColor(bool isRunning)
        //{
        //    if (isRunning)
        //    {
        //        // Application.Current.Resources["ANNdotNET.CustomColorBrush"] = new SolidColorBrush(Colors.Green);
        //        Application.Current.Resources["RibbonThemeColorBrush"] = new SolidColorBrush(Colors.Green);
        //        Application.Current.Resources["ANNdotNET.HighlightColor"] = Colors.Green;
        //        Application.Current.Resources["ANNdotNET.InactiveForeground"] = Colors.Green;
        //        if(IsRunChecked)
        //        {
        //            AppStatus = "Running...";
        //            StatusMessage = "ANNdotNET training process has been started! ";
        //        }
               
        //    }

        //    else
        //    {
        //        //Application.Current.Resources["ANNdotNET.CustomColorBrush"] = new SolidColorBrush(Colors.BlueViolet);
        //        Application.Current.Resources["RibbonThemeColorBrush"] = new SolidColorBrush(Colors.BlueViolet);
        //        Application.Current.Resources["ANNdotNET.HighlightColor"] = Colors.BlueViolet;
        //        Application.Current.Resources["ANNdotNET.InactiveForeground"] = Colors.Blue;

               
        //        AppStatus = "Ready";
        //        StatusMessage = "No application message.";
        //    }

        //}

        //internal void DeleteModel(MLConfigController mlconfigController)
        //{
        //    try
        //    {
        //        var prj = AppModel.Project[1] as ANNProjectController;
        //        prj.Models.Remove(mlconfigController);
        //        mlconfigController.Delete();

        //    }
        //    catch (Exception ex)
        //    {

        //        if (ReportException != null)
        //            ReportException(ex);
        //    }

        //}

        //public void TrainingCompleated(TrainResult result)
        //{
        //    try
        //    {
        //        if (!(ActiveViewModel is MLConfigController))
        //            return;
        //        if (m_TokenSource != null)
        //            m_TokenSource.Dispose();
        //        //
        //        m_TokenSource = null;

        //        //un check run button
        //        var mainWnd = App.Current.MainWindow as MainWindow;
        //        mainWnd.StopButtonClick();
        //        //
        //        Application.Current.Dispatcher.BeginInvoke(
        //            DispatcherPriority.Background,
        //        new Action(

        //            () =>
        //            {
        //                if (AppCommands.EvaluateModelCommand != null)
        //                    AppCommands.EvaluateModelCommand.Execute(null, null);
        //                //
        //                IsRunChecked = false;
        //                SetRunnigColor(false);
        //                StatusMessage = $"Training process {result.ProcessState} at {result.Iteration} iterations!";
        //            }

        //        ));


        //    }
        //    catch (Exception ex)
        //    {
        //        if (ReportException != null)
        //            ReportException(ex);
        //    }
        //}
        //public void ModelEvaluationAction(bool isCompleted)
        //{
        //    try
        //    {
        //        if (!(ActiveViewModel is MLConfigController))
        //            return;


        //        Application.Current.Dispatcher.BeginInvoke(
        //            DispatcherPriority.Background,
        //        new Action(

        //            () =>
        //            {
        //                if (isCompleted)
        //                {
        //                    //
        //                    SetRunnigColor(false);
        //                    StatusMessage = $"Evaluation process completed!";
        //                }
        //                else
        //                {
        //                    //
        //                    SetRunnigColor(true);
        //                    StatusMessage = $"Evaluation process has been started. Please wait....";
        //                }

        //            }

        //        ));


        //    }
        //    catch (Exception ex)
        //    {
        //        if (ReportException != null)
        //            ReportException(ex);
        //    }
        //}
        //public void OpenProjectProgressAction(int currentValue, int totalValue)
        //{
        //    try
        //    {
        //        if (!(ActiveViewModel is MLConfigController))
        //            return;

        //        bool isCompleted = currentValue == totalValue;
        //        if (isCompleted)
        //        {
        //            //
        //            SetRunnigColor(false);
        //            StatusMessage = $"Open annProject completed!";
        //        }
        //        else
        //        {
        //            //
        //            SetRunnigColor(true);
        //            StatusMessage = $"Open annProject {currentValue} of {totalValue}. Please wait....";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (ReportException != null)
        //            ReportException(ex);
        //    }
        //}
        #endregion

  
    }
}

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
using anndotnet.wnd.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace anndotnet.wnd.Pages
{
    /// <summary>
    /// Interaction logic for ExperimentPage.xaml
    /// </summary>
    public partial class ProjectPage
    {
        /// <summary>
        /// 
        /// </summary>
        public ProjectPage()
        {
            InitializeComponent();
            this.Loaded += ExperimentPage_Loaded;
            this.DataContextChanged += ExperimentPage_DataContextChanged;
           
        }
        

        /// <summary>
        /// When ExpData tree items is changed this method should be called because of the DataCOntext is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExperimentPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //For old experiment page save the state
                if (e.OldValue != null)
                {

                    var model = e.OldValue as ANNProjectController;
                    model.DataSet = project.GetDataSet();
                }
                //for project show previously stored state
                if (e.NewValue != null)
                {

                    var prjCont = e.NewValue as ANNProjectController;
                    if (prjCont != null)
                    {
                        project.ResetExperimentalPanel();
                        if (prjCont.DataSet != null)
                            project.SetDataSet(prjCont.DataSet);
                        //hide raw dataset when the project is no raw data set
                        if (prjCont.Type != ProjectType.Default)
                        {
                            rawDataTab.Visibility = Visibility.Collapsed;
                            prjCont.SelectedPage = 1;
                        }
                           

                        prjCont.LoadRichText(this.richText);
                    }

                }
            }
            catch (System.Exception ex)
            {

                Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(

                    () =>
                    {
                        var appCnt = App.Current.MainWindow.DataContext as AppController;
                        appCnt.ReportException(ex);
                    }

                ));
            }
            
        }

        private void ExperimentPage_Loaded(object sender, RoutedEventArgs e)
        {
            //experiment.CreateModel = CreateNewModel;
            //experiment.UpdateModel = UpdateModel;
        }
        /// <summary>
        /// Event handler for hyper-link clicked in the rich text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var hyperlink = (Hyperlink)sender;
            Process.Start(hyperlink.NavigateUri.ToString());
        }
    }
}

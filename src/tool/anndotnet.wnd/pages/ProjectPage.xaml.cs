//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                        //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using anndotnet.wnd.Models;
using ANNdotNET.Core;
using DataProcessing.Core;
using DataProcessing.Wnd;
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
                    var prjCont = e.OldValue as ANNProjectController;
                    //set wait cursor 
                    MainWindow.SetCursor(true);
                    
                    //hide raw dataset when the project is no raw data set
                    if (prjCont.Settings.ProjectType == ProjectType.Default)
                    {
                        prjCont.DataSet = this.project.GetDataSet();
                        //project.Dispose();
                    }
                    else if (prjCont.Settings.ProjectType == ProjectType.ImageClassification)
                    {
                        //
                        prjCont.DataSet = iclassificator.GetDataSet();
                        //project.Dispose();
                    }
                    else
                    {
                        
                    }

                    //
                    richText = null;
                }
                //for project show previously stored state
                if (e.NewValue != null)
                {

                    var prjCont = e.NewValue as ANNProjectController;
                    if (prjCont != null)
                    {
                        
                        //hide raw dataset when the project is no raw data set
                        if (prjCont.Settings.ProjectType == ProjectType.Default)
                        {
                            project.ResetExperimentalPanel();
                            if (prjCont.DataSet != null)
                                project.SetDataSet(prjCont.DataSet);

                            rawDataTab.Visibility = Visibility.Visible;
                            imgDataTab.Visibility = Visibility.Collapsed;
                            prjCont.SelectedPage = 0;
                        }
                        else if (prjCont.Settings.ProjectType == ProjectType.ImageClassification)
                        {
                            var imgClassMode = iclassificator.LoadDataSet(prjCont.DataSet);
                            rawDataTab.Visibility = Visibility.Collapsed;
                            imgDataTab.Visibility = Visibility.Visible;
                            iclassificator.DataContext = imgClassMode;
                            prjCont.SelectedPage = 1;
                        }
                        else
                        {
                            rawDataTab.Visibility = Visibility.Collapsed;
                            imgDataTab.Visibility = Visibility.Collapsed;
                            prjCont.SelectedPage = 2;
                        }


                        prjCont.LoadRichText(this.richText);
                    }

                    //restore cursor 
                    MainWindow.SetCursor(false);
                }
            }
            catch (System.Exception ex)
            {
                //restore cursor 
                MainWindow.SetCursor(false);

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
       
        private ImageClassificatorModel loadDataToModel(ANNDataSet dataSet)
        {
            var model = new ImageClassificatorModel();
            if (dataSet != null && dataSet.Data.Count > 0)
            {
                //
                var row = dataSet.Data.First();
                model.Channels = int.Parse(row[3]);
                model.Height = int.Parse(row[4]);
                model.Width = int.Parse(row[5]);
                foreach (var r in dataSet.Data)
                {
                    var itm = new ImageLabelItem();
                    itm.Label = r[0];
                    itm.Folder = r[1];
                    itm.Query = r[2];
                    model.Labels.Add(itm);
                }
                return model;
            }
            else
                return null;

            
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

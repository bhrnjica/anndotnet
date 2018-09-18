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
using System.Windows;
using System.Windows.Threading;

namespace anndotnet.wnd.Pages
{
    /// <summary>
    /// Interaction logic for ModelPage.xaml
    /// </summary>
    public partial class TrainingPage
    {
        public TrainingPage()
        {
            InitializeComponent();

            this.DataContextChanged += ModelPage_DataContextChanged;
        }

        /// <summary>
        /// This method is called when DataContex of the ModelPage is changed, or when the different model
        /// in different project is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //for the old DataContex we should save the state
                if (e.OldValue != null)
                {
                    MLConfigController model = e.OldValue as MLConfigController;
                    if (model != null && !model.Deleted)
                        model.SaveModel();
                
                }
                //for new model we should show previously stored state
                if (e.NewValue != null)
                {

                        MLConfigController model = e.NewValue as MLConfigController;
                        evaluation.DataContext = null;
                        evaluation.DataContext = model;
                        model.InitModel();
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
    }
}

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
using System.Windows;
using System.Windows.Threading;

namespace anndotnet.wnd.Pages
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage
    {
        
        public Action<Exception> ReportException { get; internal set; }
        public StartPage()
        {
            InitializeComponent();
            this.Loaded += StartPage_Loaded;
            this.DataContextChanged += StartPage_DataContextChanged;
        }

        private void StartPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //for the old DataContex we should save the state
                if (e.OldValue != null)
                {
                    //set wait cursor 
                    MainWindow.SetCursor(true);

                    start.Dispose();
                }
                //for new model we should show previously stored state
                if (e.NewValue != null)
                {

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

        private void StartPage_Loaded(object sender, RoutedEventArgs e)
        {

            //connecting New and Open action from StartPage to main New and Open commands
            //start.New = newExperiment;
            //start.Open = openExperiment;
        }

       
    }
}

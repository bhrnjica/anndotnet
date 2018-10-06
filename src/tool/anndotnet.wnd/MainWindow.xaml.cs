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
using Fluent;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace anndotnet.wnd
{
    /// <summary>
    /// Code-Behind implementation of man window
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        System.Windows.Controls.ContextMenu m_CMenu = new System.Windows.Controls.ContextMenu();
        
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing; ;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                var dc = DataContext as AppController;

                if (dc != null)
                { 
                    dc.SetRunnigColor(false);
                    dc.ReportException = ReportException;
                }
                var mi = new System.Windows.Controls.MenuItem();

                m_CMenu.Items.Add(new System.Windows.Controls.MenuItem() { Header="Rename", Command= commands.AppCommands.RenameConfigCommand  });
              //  m_CMenu.Items.Add(new System.Windows.Controls.MenuItem() { Header = "Duplicate", Command = commands.AppCommands.DuplicateConfigCommand });
                m_CMenu.Items.Add(new System.Windows.Controls.MenuItem() { Header = "Delete", Command = commands.AppCommands.DeleteConfigCommand });

            }
        }

        public void onRenameTreeItem(object sender, ExecutedRoutedEventArgs e)
        {
            var appCont = this.DataContext as AppController;
            renameModel(appCont.ActiveViewModel as MLConfigController);
        }

        public void onDuplicateTreeItem(object sender, ExecutedRoutedEventArgs e)
        {
            
        }

        public void onDeleteTreeItem(object sender, ExecutedRoutedEventArgs e)
        {
            var appCont = this.DataContext as AppController;
            deleteModel(appCont.ActiveViewModel as MLConfigController);
        }


        public void StopButtonClick()
        {
            runBtn.IsChecked = false;
            stopBtn.IsEnabled = false;
            treeCtrl.IsEnabled = true;

        }
        public void RunButtonClick()
        {
            stopBtn.IsEnabled =  true;
            treeCtrl.IsEnabled = false;

        }

        static Cursor MainDefault=null;
        static Cursor Wait = null;
        public static void SetCursor(bool isWait)
        {
            var appContr = Application.Current.MainWindow.DataContext as AppController;
            if (isWait)
            {
                
                appContr.StatusMessage = "The application is processing data...";
                appContr.SetRunnigColor(true);
                var prev = App.Current.MainWindow.Cursor;
                if (prev == null)
                    prev = System.Windows.Input.Cursors.Arrow;
                if (Wait == null)
                    Wait = System.Windows.Input.Cursors.Wait;

                App.Current.MainWindow.Cursor = Wait;
                MainDefault = prev;
            }
            else
            {
                var prev = App.Current.MainWindow.Cursor;
                if (MainDefault == null)
                    MainDefault = System.Windows.Input.Cursors.Arrow;


                App.Current.MainWindow.Cursor = MainDefault;
                Wait = prev;
                appContr.SetRunnigColor(false);
            }

        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if (isModelExist())
                {
                    var appCont = this.DataContext as AppController;

                    if (appCont.IsRunChecked)
                    {
                        if (MessageBox.Show("Training process is running. Closing Application you will lost recent changes.", "ANNdotNET", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else if (appCont.AppModel.Project.Count < 2)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to Exit ANNdotNET?", "ANNdotNET", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            e.Cancel = true;
                        }
                        else
                        {
                            appCont.onClose(null, null);
                        }


                    }

                }
            }
            catch (Exception ex)
            {
                SetCursor(false);
                ReportException(ex);
                //throw;
            }            
        }

        /// <summary>
        /// Central method for handling and showing all application exceptions
        /// </summary>
        /// <param name="ex"></param>
        void ReportException(Exception ex)
        {
            Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Background,
               new Action(
           ()=>MessageBox.Show(ex.Message, "ANNdotNET v1.0")
            ));

        }

        private bool isModelExist()
        {
            var appCont = this.DataContext as AppController;
            if (appCont != null)
            {
                var appModel = appCont.AppModel;
                if (appModel.Project.Count > 1)
                {

                    return true;
                }
                return true;
            }
            return false;
        }

        #region TreeView Editable Helper
        private void ContentPresenter_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cnt = sender as System.Windows.Controls.ContentPresenter;
            if (cnt != null && cnt.Content is MLConfigController)
            {
                var mlconfig = cnt.Content as MLConfigController;
                if (mlconfig.IsSelected)
                {
                    var tx = e.Source as TextBlock;
                    tx.ContextMenu = m_CMenu;
                    tx.ContextMenu.IsOpen = true;
                }
            }
        }
        private void editableTextBoxHeader_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            var dx = tb.DataContext as Models.BaseModel;
            if (dx != null)
                dx.IsEditing = false;
        }

        private void editableTextBoxHeader_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb.IsVisible)
            {
                tb.Focus();
                tb.SelectAll();
                oldText = tb.Text;      // back up - for possible canceling
            }
        }
        string oldText;
        private void editableTextBoxHeader_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = sender as System.Windows.Controls.TextBox;
                var exp = tb.DataContext as Models.ANNProjectController;
                var mod = tb.DataContext as Models.MLConfigController;
                if (exp != null)
                {
                    exp.Name = tb.Text;
                    exp.IsEditing = false;
                }
                else if (mod != null)
                {
                    mod.Name = tb.Text;
                    mod.IsEditing = false;
                }


            }
            if (e.Key == Key.Escape)
            {
                var tb = sender as System.Windows.Controls.TextBox;
                var dx = tb.DataContext as Models.BaseModel;
                tb.Text = oldText;
                 
                dx.IsEditing = false;
            }
        }

        private void treeCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TreeView;
            var dx = tb.SelectedValue as Models.BaseModel;
            if (e.Key == Key.F2 && dx is MLConfigController)
                renameModel(dx as MLConfigController);
            else if (e.Key == Key.Delete)
            {
                if (tb.SelectedValue is Models.ANNProjectController)
                {
                    MessageBox.Show("Project is not allow to be deleted. Try close button.");
                }
                else if (tb.SelectedValue is Models.StartModel)
                {
                    MessageBox.Show("Start Page is not allow to be deleted.");
                }
                else if (tb.SelectedValue is Models.MLConfigController)
                {
                    var model = (Models.MLConfigController)tb.SelectedValue;

                    deleteModel(model);

                }
            }
        }

        private void deleteModel(MLConfigController model)
        {
            if (MessageBox.Show($"Are you sure you want to delete '{model.Name}' model and all related files?", "ANNdotNET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var cont = DataContext as AppController;
                if (cont == null)
                    return;
                cont.DeleteModel(model);
            }
        }

        private void renameModel(MLConfigController model)
        {
            model.IsEditing = true;
        }
        private void duplicateModel(MLConfigController model)
        {
            
        }

        private void treeCtrl_LostFocus(object sender, RoutedEventArgs e)
        {
            //var tb = sender as System.Windows.Controls.TreeView;
            //var dx = tb.SelectedValue as MLConfig.BaseModel;
            //dx.IsEditing = false;
            //e.Handled = true;
        }
        #endregion

    }
}

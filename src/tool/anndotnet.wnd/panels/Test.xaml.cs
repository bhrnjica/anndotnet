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
using anndotnet.wnd.Models;
using ANNdotNET.Core;
using ANNdotNET.Lib;
using DataProcessing.Wnd;
using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace anndotnet.wnd.Panels
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : UserControl
    {
        ObservableCollection<ObservableCollection<string>> m_TestData;
        public Test()
        {
            InitializeComponent();
            DataContextChanged += Test_DataContextChanged;

            
        }

        private void Test_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {        
                if (DataContext!=null)
                {
                    if (dataGrid != null)
                    {
                        dataGrid.Columns.Clear();
                        //dataGrid.Items.Clear();
                        dataGrid.Items.Refresh();
                        listResult.Items.Clear();
                    }
                    var dc = DataContext as anndotnet.wnd.Models.MLConfigController;
                    if(dc != null)
                    {
                        var testData = ((MLConfigController)DataContext).TestData;
                        if (testData == null || testData.Count == 0)
                        {
                            return;
                        }
                       
                        int index = 0;
                        //firs numeric column shoud be positioned first
                        foreach(var c in testData.Where(x => x.Kind != DataKind.Label && x.Type== MLDataType.Numeric))
                        {
                            //
                            if (c.Type == MLDataType.Numeric)
                            {
                                var dgc = new DataGridTextColumn();
                                dgc.Header = c.Name;
                                
                                dataGrid.Columns.Add(dgc);

                                Binding b = new Binding($"[{index}]");
                                dgc.Binding = b; 
                            }
                            index++;
                        }
                        foreach (var c in testData.Where(x => x.Kind != DataKind.Label && x.Type == MLDataType.Category))
                        {
                            if (c.Type == MLDataType.Category)
                            {
                                var dgc = new DataGridComboBoxColumn();
                                if (c.Classes != null)
                                    dgc.ItemsSource = c.Classes;

                                dgc.Header = c.Name;

                                dataGrid.Columns.Add(dgc);

                                Binding b = new Binding($"[{index}]");
                                dgc.TextBinding = b;
                            }
                          
                            index++;
                        }

                        //get output row and set result column to label name
                        var name = testData.Where(x => x.Kind == DataKind.Label).FirstOrDefault().Name;
                        var gv = listResult.View as GridView;
                        gv.Columns.FirstOrDefault().Header = name;

                        if (m_TestData == null)
                            m_TestData = new ObservableCollection<ObservableCollection<string>>();
                        else
                            m_TestData.Clear();
                        dataGrid.ItemsSource = m_TestData;
                        
                        
                    }
                }

            }
            catch (Exception ex)
            {

                var ac = App.Current.MainWindow.DataContext as AppController;
                if (ac != null)
                    ac.ReportException(ex);
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                var rowIndex = e.Row.GetIndex();
                var colIndex = e.Column.DisplayIndex;
                if(rowIndex >= m_TestData.Count)
                {
                    e.Cancel = true;
                    return;
                }
                if (!(m_TestData.Count > 0 &&  colIndex < m_TestData[0].Count))
                {
                    e.Cancel = true;
                    return;
                }
                var cmb = e.EditingElement as ComboBox;
                if (cmb != null)
                {
                    var value = cmb.Text;
                    m_TestData[rowIndex][colIndex] = value;
                }
                else if (e.EditingElement is TextBox)
                {
                    var value = ((TextBox)e.EditingElement).Text;
                    m_TestData[rowIndex][colIndex] = value;
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                var appCnt = App.Current.MainWindow.DataContext as AppController;
                if(appCnt !=null)
                    appCnt.ReportException(ex);
            }
           

            
        }

        /// <summary>
        /// add row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var testData = ((MLConfigController)DataContext).TestData;

                var row = new ObservableCollection<string>();
                foreach (var c in testData.Where(x => x.Kind != DataKind.Label))
                    row.Add(" ");

                //datagri
                m_TestData.Add(row);
            }
            catch (Exception ex)
            {

                var appCnt = App.Current.MainWindow.DataContext as AppController;
                if (appCnt != null)
                    appCnt.ReportException(ex);
            }
            
        }

        /// <summary>
        /// Delete row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex < 0)
                {
                    MessageBox.Show("Row is not selected!", "ANNdotNET");
                    return;
                }
                if (MessageBox.Show("Are you sure you want to delete the selected row!", "ANNdotNET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    m_TestData.RemoveAt(dataGrid.SelectedIndex);
            }
            catch (Exception ex)
            {

                var appCnt = App.Current.MainWindow.DataContext as AppController;
                if (appCnt != null)
                    appCnt.ReportException(ex);
            }
            
        }


        string[][] strData;
        /// <summary>
        /// Load data from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTestData_BtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //prepare control to load data
                ImportData dlg = new ImportData();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (dlg.Data == null)
                        return;
                    strData = dlg.Data;
                    // strHeader = dlg.Header;
                    if (strData.Length <= 0)
                    {
                        MessageBox.Show("Test files is empty!");
                        return;
                    }

                    if (strData.First().Length != dataGrid.Columns.Count)
                    {
                        MessageBox.Show("Test file cannot be evaluate. The columns number is different than model expects!\n" +
                            "Column number and order must be the same as data grid columns.");
                        return;
                    }

                    m_TestData.Clear();

                    foreach (var row in strData)
                    {
                        var r = new ObservableCollection<string>(row);
                        m_TestData.Add(r);
                    }
                }
            }
            catch (Exception)
            {
                if (dataGrid.SelectedIndex < 0)
                {
                    MessageBox.Show("Row is not selected!", "ANNdotNET");
                    return;
                }
                if (MessageBox.Show("Are you sure you want to delete the selected row!", "ANNdotNET", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    m_TestData.RemoveAt(dataGrid.SelectedIndex);
            }
            
        }

        private void EvaluateModel_BtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = (MLConfigController)DataContext;
                var testMetaData = model.TestData;
                var columns = dataGrid.Columns;
                //check if the trained model exists
                if (string.IsNullOrEmpty(model.TrainingParameters.LastBestModel) || string.IsNullOrEmpty(model.TrainingParameters.LastBestModel.Trim(' ')))
                {
                    throw new Exception("There is no model to test. Model must be trained first.");
                }

                //Load ML model configuration file
                var modelPath = Project.GetMLConfigPath(model.Settings, model.Name);
                var dicMParameters = Project.LoadMLConfig(modelPath);
                var trainedModelRelativePath = Project.GetParameterValue(dicMParameters["training"], "TrainedModel");
                //add full path of model folder since model file doesn't contains any apsolute path
                dicMParameters.Add("root", Project.GetMLConfigFolder(modelPath));
                var strModelToEvaluatePath = $"{dicMParameters["root"]}\\{trainedModelRelativePath}";
                ProcessDevice pd = ProcessDevice.Default;
                listResult.Items.Clear();
                //parse each row and transform it to ML ready data in order to predict the result
                for (int i = 0; i < m_TestData.Count; i++)
                {
                    var vector = new List<float>();
                    for (int j = 0; j < m_TestData[0].Count; j++)
                    {
                        //check if the value valid
                        if (string.IsNullOrEmpty(m_TestData[i][j]))
                            throw new Exception($"Value cannot be empty.");

                        var value = m_TestData[i][j];
                        
                        if (columns[j] is DataGridComboBoxColumn)
                        {
                            var combo = columns[j] as DataGridComboBoxColumn;

                            foreach (var itm in combo.ItemsSource)
                            {
                                var c = itm.ToString();
                                if (m_TestData[i][j].Equals(c, StringComparison.InvariantCultureIgnoreCase))
                                    vector.Add(1);
                                else
                                    vector.Add(0);
                            }
                        }
                        else
                        {
                            if (m_TestData[i][j].Contains(","))
                                throw new Exception("Decimal separator should be point.");
                            var vald = double.Parse(m_TestData[i][j], CultureInfo.InvariantCulture);
                            vector.Add((float)vald);
                        }
                    }
                    //
                    var val = Project.Predict(strModelToEvaluatePath, vector.ToArray(), pd);
                    var labelCol = testMetaData.Where(x => x.Kind == DataKind.Label).FirstOrDefault();
                    if (labelCol.Type == MLDataType.Category)
                    {
                        var ind = int.Parse(val.ToString());
                        listResult.Items.Add(labelCol.Classes[ind]);
                    }
                    else
                        listResult.Items.Add(  val);
                }
            }
            catch (Exception ex)
            {
                var appCnt = App.Current.MainWindow.DataContext as AppController;
                if (appCnt != null)
                    appCnt.ReportException(ex);
            }
            
        }
    }
}

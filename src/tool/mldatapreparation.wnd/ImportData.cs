//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool on .NET Platform                                 //
// Copyright 2006-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See license section of  https://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac,Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using DataProcessing.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MLDataPreparation.Dll
{
    /// <summary>
    /// WInForm User controls to perfume import Raw Dataset into ANNdotNET 
    /// </summary>
    public partial class ImportData : Form
    {
        private string originData = "";
        private string[] originLines;
        public ImportData()
        {
            InitializeComponent();

           
        }

        //Import file into dataset parser
        private void btnLoadData_Click(object sender, EventArgs e)
        {
            try
            {
                var strFile = GetFileFromOpenDialog("", "");
                if (strFile == null)
                    return;
                this.Cursor = Cursors.WaitCursor;
                textBox1.Text = strFile;
                originLines = File.ReadAllLines(strFile).Where(l => !l.StartsWith("@") && !l.StartsWith("#") && !l.StartsWith("!")).ToArray();
                var data = string.Join(Environment.NewLine, originLines.Take(1000));
                originData = data;
                textBox3.Text = data;
                ProcesData();

                if (!string.IsNullOrEmpty(data))
                    btnImportData.Enabled = true;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
            
        }

        public static string GetFileFromOpenDialog(string fileDescription = "All files ", string extension = "*.*")
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //if (string.IsNullOrEmpty(extension))
            //    dlg.Filter = "Plain text files (*.csv;*.txt;*.dat,*.*;)|*.csv;*.txt;*.dat |All files (*.*)|*.*";
            //else
            dlg.Filter = string.Format("All files (*.*)|*.*");
            //
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            else
                return null;
        }

        private void ProcesData()
        {
            if (string.IsNullOrEmpty(originData))
                return;
            var data = string.Join(Environment.NewLine, originLines.Take(1000));
            if (string.IsNullOrEmpty(data))
                return;

            if (checkBox2.Checked)
                data = data.Replace(";", "\t|\t");
            if (checkBox3.Checked)
                data = data.Replace(",", "\t|\t");
            if (checkBox4.Checked)
                data = data.Replace(" ", "\t|\t");
            if (checkBox6.Checked)
                data = data.Replace("\t", "\t|\t");
            if (checkBox5.Checked)
            {
                if (!string.IsNullOrEmpty(textBox2.Text))
                    data = data.Replace(textBox2.Text[0], '|');
            }

            //if header is present separate data with horizontal line
            if (firstRowHeaderCheck.Checked)
            {
                var index = data.IndexOf(Environment.NewLine);
                var index2 = data.IndexOf(Environment.NewLine, index + 1);
                int counter=0;
                while(counter<index2-index)
                {
                    data=data.Insert(index,"-");
                    counter++;
                }
                data = data.Insert(index, Environment.NewLine);
            }
            

            textBox3.Text = data;
        }

        // import dataset into data frame
        private void btnImportData_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("No file is selected!");
                    return;
                }


                var colDelimiter = GetColumDelimiter();
                //define the row
                //string[] rows = originData.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
                this.Cursor = Cursors.WaitCursor;
                var result = ANNDataSet.prepareData(originLines, colDelimiter, firstRowHeaderCheck.Checked, radioButton1.Checked);

                Header = result.header;
                Data = result.data;
            }
            catch (Exception ex)
            {

                reportException(ex);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
            
        }


        //import time series and convert to data frame 
        private void btnImportTS_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("No file is selected!");
                    return;
                }


                var colDelimiter = GetColumDelimiter();

                if (numCtrlNumForTest.Value < 1 && numCtrlNumForTest.Value > originData.Length)
                {
                    MessageBox.Show("Invalid number of time lag. Please specify the time lag between 1 and row number.");
                    return;
                }

                if (string.IsNullOrEmpty(originData))
                    return;

                //
                //transform the time series into data frame
                var result = ANNDataSet.prepareTimeSeriesData(originLines, (int)numCtrlNumForTest.Value, colDelimiter, firstRowHeaderCheck.Checked);
                Header = result.header;
                Data = result.data;
            }
            catch (Exception ex)
            {

                reportException(ex);
            }
        }


        private void reportException(Exception ex)
        {
            MessageBox.Show(ex.Message, "Data Processing");
        }

        public string[] Header { get; set; }
        public string[][] Data { get; set; }
        
        private char[] GetColumDelimiter()
        {
            var col = new List<char>();

            if (checkBox2.Checked)
                col.Add(';');
            if (checkBox3.Checked)
                col.Add(',');
            if (checkBox4.Checked)
                col.Add(' ');
            if (checkBox6.Checked)
                col.Add('\t');
            if (checkBox5.Checked)
                col.Add(textBox2.Text[0]);

            return col.ToArray();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var ch = sender as CheckBox;
            if (ch.Name == "checkBox5")
            {
                if (ch.Checked)
                    textBox2.Enabled = true;
                else
                {
                    textBox2.Text = "";
                    textBox2.Enabled = false;
                }
            }
            ProcesData();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox7.Checked)
            {
                btnImportData.Enabled = false;
                btnImportTS.Enabled = true;
                label1.Enabled = true;
                numCtrlNumForTest.Enabled = true;
            }
            else
            {
                btnImportData.Enabled = true;
                btnImportTS.Enabled = false;
                label1.Enabled = false;
                numCtrlNumForTest.Enabled = false;
            }

        }


        ///// <summary>
        ///// Transforms the string of time series into data frame string 
        ///// </summary>
        ///// <param name="tdata"></param>
        ///// <param name="lagTime"></param>
        ///// <returns></returns>
        //private (string[] header, string[][] data) prepareTimeSeriesData(string[] tdata, int lagTime, char[] delimiters ,bool isHeader)
        //{
            
        //    //split data on for feature and label datasets
        //    var header = new List<string>();
        //    var data = new List<string[]>();
            
        //    //define header if specified
        //    if(isHeader)
        //    {
        //        //
        //        var cols = tdata[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        //        for (int j=0; j < cols.Length; j++)
        //        {
        //            if(j+1 < cols.Length)
        //            {
        //                //add regular header
        //                header.Add(cols[j]);
        //            }
        //            else
        //            {
        //                //add lagged features header
        //                for (int i = 0; i < lagTime; i++)
        //                    header.Add($"{cols[j]}-{lagTime-i}");

        //                //add last column header
        //                header.Add($"{cols[j]}");
        //            }
                   
        //        }         
        //    }
        //    //
        //    int l = isHeader ? 1 : 0;
        //    var lagValues = new Queue<string>(); 
        //    for (; l < tdata.Length; l++)
        //    {
                
        //        var col = tdata[l].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

        //        //fill lagged features
        //        if (lagValues.Count > lagTime)
        //            lagValues.Dequeue();
        //        lagValues.Enqueue(col.Last());

        //        //until lagged features are not defined don't add data to data-frame
        //        var row = new List<string>();
        //        for (int j = 0; j < col.Length && lagValues.Count > lagTime; j++)
        //        {
                    
        //            if (j + 1 < col.Length)
        //            {
        //                //add regular header
        //                row.Add(col[j]);
        //            }
        //            else
        //            {
        //                //add lagged features
        //                for (int f = 0; f < lagTime; f++)
        //                {
        //                    row.Add(lagValues.ElementAt(f));
        //                }
        //                //add label
        //                row.Add(col[j]);
        //            }

        //        }

        //        if (lagValues.Count > lagTime)
        //            data.Add(row.ToArray());

        //    }

        //    //
        //    return (header.ToArray(), data.ToArray());

        //}
    }
}

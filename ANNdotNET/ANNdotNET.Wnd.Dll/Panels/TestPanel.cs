//////////////////////////////////////////////////////////////////////////////////////////
// GPdotNET - Genetic Programming Tool                                                  //
// Copyright 2006-2017 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the GNU Library General Public License (LGPL)       //
// See license section of  https://github.com/bhrnjica/gpdotnet/blob/master/license.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac,Bosnia and Herzegovina                                                         //
// http://bhrnjica.wordpress.com                                                        //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Entities;
using ZedGraph;


namespace ANNdotNet.Wnd.Dll.Panels
{
   /// <summary>
   /// Class implements calculation of best solution against Test data
   /// </summary>
    public partial class TestPanel : UserControl
    {
        #region CTor and Fields
        LineItem actualData;
        LineItem predictedData;

        /// <summary>
        /// CTOR
        /// </summary>
        public TestPanel()
        {
            InitializeComponent();
           


        }
        
        double max = 0;
       
        List<string> Classes;
        private string YAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            //zedModel.GraphPane.XAxis.Scale.Min = 0;
            zedModel.GraphPane.YAxis.Scale.Min = -0.1;
            zedModel.GraphPane.YAxis.Scale.Max = max + 0.2;
            zedModel.GraphPane.YAxis.MajorGrid.DashOff = 0;
            zedModel.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedModel.GraphPane.YAxis.Scale.MajorStep = 1;
            zedModel.GraphPane.YAxis.Scale.MinorStep = 1;
            zedModel.GraphPane.XAxis.Scale.MajorStep = 1;
            zedModel.GraphPane.XAxis.Scale.MinorStepAuto = false;

            if (Classes == null || val >= Classes.Count|| val < 0)
                return String.Format("{0}", (int)val);
            else
                return String.Format("{0}", Classes[(int)val]);
        }


        /// <summary>
        /// Uodate GP Model chart when data or GP model is changed
        /// </summary>
        /// <param name="y">output value</param>
        /// <param name="gpModel"> indicator is it about GPMOdel or Data Point</param>
        public void UpdateChartDataAsync(LineItem points)
        {
            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.Invoke(
                    new Action(() =>
                    {
                        updateChartData(points);
                    }
                    ));
            }
            else
            {
                updateChartData(points);

            }
        }
        private void updateChartData(LineItem points)
        {
            //firs chech if curvelist already exist
            var list = this.zedModel.GraphPane.CurveList.Find((x) => x.Label.Text == points.Label.Text);

            if (list == null)
            {
                this.zedModel.GraphPane.CurveList.Add(points);
                this.zedModel.GraphPane.AxisChange(this.CreateGraphics());
            }

            zedModel.RestoreScale(zedModel.GraphPane);
        }

        #endregion

        #region Private Methods

        
        #endregion

        #region Public Methods
       
        public void updateChartData(List<List<float>> actual, List<List<float>> predicted)
        {
            actualData.Clear();
            predictedData.Clear();
            //
            for (int i = 0; i < actual.Count; i++)
            {
                float act = 0;
                float pred = 0;
                //category output
                if (actual[i].Count > 1)
                {
                    act = actual[i].IndexOf(actual[i].Max());
                    pred = predicted[i].IndexOf(predicted[i].Max());
                }
                else
                {
                    act = actual[i].First();
                    pred = predicted[i].First();
                }


                actualData.AddPoint(new PointPair(i, act));
                predictedData.AddPoint(new PointPair(i, pred));
            }

            zedModel.RestoreScale(zedModel.GraphPane);
        }

        public void ResetChart()
        {
            max = 0;
            Classes = null;
            zedModel.GraphPane.YAxis.ScaleFormatEvent -= YAxis_ScaleFormatEvent;
           
        }

        
        
        public void ReportProgress(int i, float loss, float eval, List<List<float>> actual, List<List<float>> predicted)
        {
            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.Invoke(
                    new Action(() =>
                    {
                        ReportProgressSync(i, loss, eval, actual, predicted);

                    }
                    ));
            }
            else
            {
                ReportProgressSync(i, loss, eval, actual, predicted);

            }
        }
        public void ReportProgressSync(int i, float loss, float eval, List<List<float>> actual, List<List<float>> predicted)
        {

            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.Invoke(
                    new Action(() =>
                    {
                        updateChartData(actual, predicted);
                    }
                    ));
            }
            else
            {
                updateChartData(actual, predicted);
            }

        }


        public void Reset(ANNModel model)
        {
            ResetChart();
            PrepareGraphs(model);
            

        }

        /// <summary>
        /// Initi props for charts
        /// </summary>
        public void PrepareGraphs(ANNModel model)
        {
            //clear prev. state
            zedModel.GraphPane.CurveList.Clear();

            // 
            // 
            predictedData = new LineItem("Predicted", null, null, Color.Blue, ZedGraph.SymbolType.None, 1);
            predictedData.Symbol.Fill = new Fill(Color.Blue);
            predictedData.Symbol.Size = 1;
            //
            actualData = new LineItem("Actual", null, null, Color.Red, ZedGraph.SymbolType.None, 1);
            actualData.Symbol.Fill = new Fill(Color.Red);
            actualData.Symbol.Size = 1;
            // 
            if (model.OutputDim == 1)
            {
                predictedData = new LineItem("Predicted", null, null, Color.Blue, ZedGraph.SymbolType.None, 1);
                predictedData.Symbol.Fill = new Fill(Color.Blue);
                predictedData.Symbol.Size = 1;
                //
                actualData = new LineItem("Actual", null, null, Color.Red, ZedGraph.SymbolType.None, 1);
                actualData.Symbol.Fill = new Fill(Color.Red);
                actualData.Symbol.Size = 1;

                zedModel.GraphPane.YAxis.Scale.MaxAuto = true;
                zedModel.GraphPane.YAxis.Scale.MinAuto = true;

                zedModel.GraphPane.YAxis.MajorGrid.IsVisible = false;
                zedModel.GraphPane.XAxis.MajorGrid.IsVisible = false;

                zedModel.GraphPane.YAxis.Scale.MajorStepAuto = true;
                zedModel.GraphPane.YAxis.Scale.MinorStepAuto = true;

                zedModel.GraphPane.XAxis.Scale.MajorStep = 1;
                zedModel.GraphPane.XAxis.Scale.MinorStep = 0;


            }
            else
            {
                Classes = model.Classes;
                max = model.Classes.Count-1;

                predictedData = new LineItem("Predicted", null, null, Color.Red, ZedGraph.SymbolType.Circle, 0);
                predictedData.Symbol.Fill = new Fill(Color.Red);
                predictedData.Symbol.Size = 5;


                actualData = new LineItem("Actual", null, null, Color.Blue, ZedGraph.SymbolType.Diamond, 0);
                actualData.Symbol.Border = new Border(Color.Blue, 0.5f);
                actualData.Symbol.Fill = new Fill(Color.Blue);
                actualData.Symbol.Size = 4;

                zedModel.GraphPane.YAxis.ScaleFormatEvent += YAxis_ScaleFormatEvent;

                //zedModel.GraphPane.XAxis.Scale.Min = 0;
                zedModel.GraphPane.YAxis.Scale.Min = -0.1;
                zedModel.GraphPane.YAxis.Scale.Max = max + 0.2;
                zedModel.GraphPane.YAxis.MajorGrid.DashOff = 0;
                zedModel.GraphPane.YAxis.MajorGrid.IsVisible = true;
                zedModel.GraphPane.XAxis.MajorGrid.DashOff = 0;
                zedModel.GraphPane.XAxis.MajorGrid.IsVisible = false;
                zedModel.GraphPane.YAxis.Scale.MajorStep = 1;
                zedModel.GraphPane.YAxis.Scale.MinorStep = 1;
                zedModel.GraphPane.XAxis.Scale.MajorStep = 1;
                zedModel.GraphPane.XAxis.Scale.MinorStep = 1;

            }

            zedModel.GraphPane.Title.Text = "ANN Iteration Simulation";
            zedModel.GraphPane.XAxis.Title.Text = "Samples";
            zedModel.GraphPane.YAxis.Title.Text = model.Label;
            zedModel.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            zedModel.GraphPane.CurveList.Add(actualData);
            zedModel.GraphPane.CurveList.Add(predictedData);
        }

        /// <summary>
        /// Deserilization of run condition
        /// </summary>
        /// <param name="p"></param>
        public void ActivatePanel(ANNModel model)
        {
            
            //prepare graphs
            PrepareGraphs(model);

            //update model
            updateChartData(model.ActualV, model.PredictedV);
            this.zedModel.RestoreScale(zedModel.GraphPane);
        }

        #endregion


    }
}

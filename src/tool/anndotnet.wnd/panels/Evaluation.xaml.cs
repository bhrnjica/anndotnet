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
using anndotnet.wnd.panels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ZedGraph;

namespace anndotnet.wnd.Panels
{
    /// <summary>
    /// Interaction logic for Evaluation.xaml
    /// </summary>
    public partial class Evaluation : UserControl
    {
        public int ModelOutputDim = 2;
        public LineItem actualTraining = null;
        public LineItem predictedTraining = null;
        public LineItem actualValidation = null;
        public LineItem predictedValidation = null;
        public Evaluation()
        {
            InitializeComponent();
            this.DataContextChanged += Evaluation_DataContextChanged;

            //prepage graph
            prepareGraphPanel(trainingGraph);
            prepareGraphPanel(validationGraph);
        }
        object emptyContent(string strTitle)
        {
            return  new Rectangle() { StrokeThickness = 1, Stroke = Brushes.Black };
           // validationContent.Content = new Rectangle() { StrokeThickness = 1, Stroke = Brushes.Black };
        }
        private void Evaluation_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //clear prev. state
                trainingGraph.GraphPane.CurveList.Clear();
                validationGraph.GraphPane.CurveList.Clear();
                //force to update the control
                this.trainingGraph.RestoreScale(trainingGraph.GraphPane);
                this.validationGraph.RestoreScale(validationGraph.GraphPane);


                //hide items
                var itm = trainingItems.Items[0] as Regression;
                itm.Visibility = Visibility.Collapsed;
                var itm2 = trainingItems.Items[1] as BinaryClassification;
                itm2.Visibility = Visibility.Collapsed;
                var itm3 = trainingItems.Items[2] as Multiclass;
                itm3.Visibility = Visibility.Collapsed;

                //hide items
                var itm11 = validatingItems.Items[0] as Regression;
                itm11.Visibility = Visibility.Collapsed;
                var itm21 = validatingItems.Items[1] as BinaryClassification;
                itm21.Visibility = Visibility.Collapsed;
                var itm31 = validatingItems.Items[2] as Multiclass;
                itm31.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {

                var ac = App.Current.MainWindow.DataContext as AppController;
                if (ac != null)
                    ac.ReportException(ex);
            }
            

        }

        public void PrepareGraphs(ModelEvaluation modelEval)
        {
            //clear prev. state
            trainingGraph.GraphPane.CurveList.Clear();
            validationGraph.GraphPane.CurveList.Clear();
                        

            //prepare series
            prepareSeriesGraph(modelEval, trainingGraph, ref actualTraining, ref predictedTraining);
            prepareSeriesGraph(modelEval, validationGraph, ref actualValidation, ref predictedValidation);

            //add series after initialization
            trainingGraph.GraphPane.CurveList.Add(actualTraining);
            trainingGraph.GraphPane.CurveList.Add(predictedTraining);
            validationGraph.GraphPane.CurveList.Add(actualValidation);
            validationGraph.GraphPane.CurveList.Add(predictedValidation);
            
        }

        private void prepareGraphPanel(ZedGraphControl zedGraph)
        {
            ///chart for training/predicted data
            zedGraph.GraphPane.Title.Text = "Training/Predicted evaluation";
            zedGraph.GraphPane.XAxis.Title.Text = "iterations";
            zedGraph.GraphPane.YAxis.Title.Text =  "label";
            zedGraph.GraphPane.Legend.IsVisible = false;
            zedGraph.GraphPane.Title.IsVisible = false;

        }

        private void prepareSeriesGraph(ModelEvaluation modelEval, ZedGraphControl zedGraph, ref LineItem actualSeries, ref LineItem predictedSeries)
        {
            // 
            if (modelEval.ModelOutputDim == 1)
            {
                predictedSeries = new LineItem("Predicted", null, null, System.Drawing.Color.Blue, ZedGraph.SymbolType.None, 1);
                predictedSeries.Symbol.Fill = new Fill(System.Drawing.Color.Blue);
                predictedSeries.Symbol.Size = 1;
                //
                actualSeries = new LineItem("Actual", null, null, System.Drawing.Color.Orange, ZedGraph.SymbolType.None, 1);
                actualSeries.Symbol.Fill = new Fill(System.Drawing.Color.Orange);
                actualSeries.Symbol.Size = 1;

                zedGraph.GraphPane.YAxis.Scale.MaxAuto = true;
                zedGraph.GraphPane.YAxis.Scale.MinAuto = true;

                zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = false;
                zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = false;

                zedGraph.GraphPane.YAxis.Scale.MajorStepAuto = true;
                zedGraph.GraphPane.YAxis.Scale.MinorStepAuto = true;

                zedGraph.GraphPane.XAxis.Scale.MajorStep = 1;
                zedGraph.GraphPane.XAxis.Scale.MinorStep = 0;


            }
            else
            {
                Classes = modelEval.Classes;
                if (modelEval.Classes == null || modelEval.Classes.Count == 0)
                    max = 0;
                else
                    max = modelEval.Classes.Count - 1;

                predictedSeries = new LineItem("Predicted", null, null, System.Drawing.Color.Orange, ZedGraph.SymbolType.Circle, 0);
                predictedSeries.Symbol.Fill = new Fill(System.Drawing.Color.Orange);
                predictedSeries.Symbol.Size = 5;


                actualSeries = new LineItem("Actual", null, null, System.Drawing.Color.Blue, ZedGraph.SymbolType.Diamond, 0);
                actualSeries.Symbol.Border = new ZedGraph.Border(System.Drawing.Color.Blue, 0.5f);
                actualSeries.Symbol.Fill = new Fill(System.Drawing.Color.Blue);
                actualSeries.Symbol.Size = 4;

                zedGraph.GraphPane.YAxis.ScaleFormatEvent += YAxis_ScaleFormatEvent;

                //zedModel.GraphPane.XAxis.Scale.Min = 0;
                zedGraph.GraphPane.YAxis.Scale.Min = -0.1;
                zedGraph.GraphPane.YAxis.Scale.Max = max + 0.2;
                zedGraph.GraphPane.YAxis.MajorGrid.DashOff = 0;
                zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
                zedGraph.GraphPane.XAxis.MajorGrid.DashOff = 0;
                zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = false;
                zedGraph.GraphPane.YAxis.Scale.MajorStep = 1;
                zedGraph.GraphPane.YAxis.Scale.MinorStep = 1;
                zedGraph.GraphPane.XAxis.Scale.MajorStep = 1;
                zedGraph.GraphPane.XAxis.Scale.MinorStep = 1;

            }
        }

        double max = 0;
        List<string> Classes;
        private string YAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            //zedModel.GraphPane.XAxis.Scale.Min = 0;
            pane.YAxis.Scale.Min = -0.1;
            pane.YAxis.Scale.Max = max + 0.2;
            pane.YAxis.MajorGrid.DashOff = 0;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.DashOff = 0;
            pane.XAxis.MajorGrid.IsVisible = false;
            pane.YAxis.Scale.MajorStep = 1;
            pane.YAxis.Scale.MinorStep = 0;

            if (Classes == null || val >= Classes.Count)
                return String.Format("{0}", (int)val);
            else
                return String.Format("{0}", Classes[(int)val]);
        }

        /// <summary>
        /// Refresh evaluation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MLConfigController pCont = this.DataContext as MLConfigController;
                var appCnt = anndotnet.wnd.App.Current.MainWindow.DataContext as AppController;
                appCnt.ModelEvaluationAction(false);

                //send model evaluation in the background
                var modeEval = await Task<ModelEvaluation>.Run(()=> pCont.EvaluateModel());

                appCnt.ModelEvaluationAction(true);

                PrepareGraphs(modeEval);

                for (int i=0; i<modeEval.TrainingValue.Count; i++)
                {
                    actualTraining.AddPoint(modeEval.TrainingValue[i]);
                    predictedTraining.AddPoint(modeEval.ModelValueTraining[i]);
                }
                for (int i = 0; i < modeEval.ValidationValue.Count; i++)
                {
                    actualValidation.AddPoint(modeEval.ValidationValue[i]);
                    predictedValidation.AddPoint(modeEval.ModelValueValidation[i]);
                }

            
                //Refresh the charts
                trainingGraph.RestoreScale(trainingGraph.GraphPane);
                validationGraph.RestoreScale(validationGraph.GraphPane);

                //regression
                if (modeEval.ModelOutputDim==1)
                {
                    var itm = trainingItems.Items[0] as Regression;
                    itm.Visibility = Visibility.Visible;
                    var itm2 = trainingItems.Items[1] as BinaryClassification;
                    itm2.Visibility = Visibility.Collapsed;
                    var itm3 = trainingItems.Items[2] as Multiclass;
                    itm3.Visibility = Visibility.Collapsed;
                    itm.DataContext = modeEval.TrainPerformance;
                    //validation set
                    var itm11 = validatingItems.Items[0] as Regression;
                    itm11.Visibility = Visibility.Visible;
                    var itm21 = validatingItems.Items[1] as BinaryClassification;
                    itm21.Visibility = Visibility.Collapsed;
                    var itm31 = validatingItems.Items[2] as Multiclass;
                    itm31.Visibility = Visibility.Collapsed;
                    itm11.DataContext = modeEval.ValidationPerformance;
                }//binary classification
                else if(modeEval.ModelOutputDim==2)
                {
                    var itm = trainingItems.Items[0] as Regression;
                    itm.Visibility = Visibility.Collapsed;                  
                    var itm2 = trainingItems.Items[1] as BinaryClassification;
                    itm2.Visibility = Visibility.Visible;
                    var itm3 = trainingItems.Items[2] as Multiclass;
                    itm3.Visibility = Visibility.Collapsed;
                    itm2.DataContext = modeEval.TrainPerformance;

                    var itm11 = validatingItems.Items[0] as Regression;
                    itm11.Visibility = Visibility.Collapsed;
                    var itm21 = validatingItems.Items[1] as BinaryClassification;
                    itm21.Visibility = Visibility.Visible;
                    var itm31 = validatingItems.Items[2] as Multiclass;
                    itm31.Visibility = Visibility.Collapsed;
                    itm21.DataContext = modeEval.ValidationPerformance;

                }//multi-class classification
                else if (modeEval.ModelOutputDim > 2)
                {
                    var itm = trainingItems.Items[0] as Regression;
                    itm.Visibility = Visibility.Collapsed;
                    var itm2 = trainingItems.Items[1] as BinaryClassification;
                    itm2.Visibility = Visibility.Collapsed;
                    var itm3 = trainingItems.Items[2] as Multiclass;
                    itm3.Visibility = Visibility.Visible;
                    itm3.DataContext = modeEval.TrainPerformance;

                    var itm11 = validatingItems.Items[0] as Regression;
                    itm11.Visibility = Visibility.Collapsed;
                    var itm21 = validatingItems.Items[1] as BinaryClassification;
                    itm21.Visibility = Visibility.Collapsed;
                    var itm31 = validatingItems.Items[2] as Multiclass;
                    itm31.Visibility = Visibility.Visible;
                    itm31.DataContext = modeEval.ValidationPerformance;
                }

            }
            catch (Exception ex)
            {

                AppController appCont = App.Current.MainWindow.DataContext as AppController;
                appCont.ReportException(ex);
            }
           

        }
    }
}

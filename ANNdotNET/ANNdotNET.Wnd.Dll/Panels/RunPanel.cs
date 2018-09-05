//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool                                                  //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                       //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ZedGraph;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using ANNdotNET.Net.Lib;
using ANNdotNET.Net.Lib.Entities;

namespace ANNdotNet.Wnd.Dll.Panels
{
    /// <summary>
    /// Class implements simulation of Running GP
    /// </summary>
    public partial class RunPanel : UserControl//, IGPPanel
    {
        #region Ctor and Fields
        LineItem lossFunction;
        LineItem evalFunciton;
        LineItem actualData;
        LineItem predictedData;

        public RunPanel()
        {
            InitializeComponent();
            

        }

        public void SetParameters(ActiveModelData data)
        {
            //
            m_cbIterationType.SelectedIndex = data.IterType;
            m_eb_iterations.Text = data.IterValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            ebMinibatchSize.Text = data.MinibatchSize.ToString();
            currentIteration.Text = "0";
            //
            cbLearnerType.SelectedIndex = (int)data.LearnerType;
            textLearningRate.Text = data.LearningRate.ToString(System.Globalization.CultureInfo.InvariantCulture);
            txMomentum.Text= data.Momentum.ToString(System.Globalization.CultureInfo.InvariantCulture);
            txtL1.Text= data.L1Regularizer.ToString(System.Globalization.CultureInfo.InvariantCulture);
            txtL2.Text = data.L2Regularizer.ToString(System.Globalization.CultureInfo.InvariantCulture);

            //
            cbNetworkSettings.SelectedIndex = data.NetworkType;
            netSedit1.Text = data.Neurons.ToString();
            netSEdit2.Text = data.HLayers.ToString();
            netSEdit3.Text = data.Embeding.ToString();
            netSeditdrop.Text = data.DropRate.ToString();
            checkStabilisation.Checked = data.UseStabilisation;
            checkDropRate.Checked = data.UseDropRate;

            //
            cActivationH.SelectedIndex = (int)data.ActivationHidden;
            cActivationO.SelectedIndex = (int)data.ActivationOutput;

            cbLossFunction.SelectedIndex = (int)data.LossFunction;
            cbEvalFunction.SelectedIndex = (int)data.EvaluationFunction;

        }
        public ActiveModelData GetParameters()
        {
            ActiveModelData data = new ActiveModelData();
            //
            data.IterType= m_cbIterationType.SelectedIndex;
            data.IterValue = GetValue(m_eb_iterations.Text);
            data.MinibatchSize = (uint)GetValue(ebMinibatchSize.Text);

            //
            data.LearnerType = (LearnerType)cbLearnerType.SelectedIndex;
            data.LearningRate = GetValue(textLearningRate.Text); 
            data.Momentum = GetValue(txMomentum.Text); 
            data.L1Regularizer = GetValue(txtL1.Text); 
            data.L2Regularizer = GetValue(txtL2.Text); 

            //
            data.NetworkType = cbNetworkSettings.SelectedIndex;
            data.Neurons = (int)GetValue(netSedit1.Text); 
            data.HLayers = (int)GetValue(netSEdit2.Text); 
            data.Embeding = (int)GetValue(netSEdit3.Text);  
            data.DropRate = GetValue(netSeditdrop.Text); 
            data.UseStabilisation = checkStabilisation.Checked;
            data.UseDropRate = checkDropRate.Checked;

            //
            data.ActivationHidden = (Activation)cActivationH.SelectedIndex;
            data.ActivationOutput = (Activation)cActivationO.SelectedIndex;

            //
            data.LossFunction = (LossFunctions)cbEvalFunction.SelectedIndex;
            data.EvaluationFunction =  (LossFunctions)cbEvalFunction.SelectedIndex;

            return data;

        }

        private float GetValue(string text)
        {
            var str = text.Replace(",", ".");
            float value;
            bool b = float.TryParse(text, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value);
            return value;
        }

        public void Reset(ANNModel model)
        {
            ResetChart();
            PrepareGraphs(model);
            SetParameters(ActiveModelData.GetDefaults());

        }
        #endregion

        #region Protected and Private methods

        /// <summary>
        /// Initi props for charts
        /// </summary>
        public void PrepareGraphs(ANNModel model)
        {
            //clear prev. state
            zedFitness.GraphPane.CurveList.Clear();
            zedModel.GraphPane.CurveList.Clear();

            // 
            lossFunction = new LineItem("Loss Value", null, null, Color.Blue, ZedGraph.SymbolType.None, 1);
            lossFunction.Symbol.Fill = new Fill(Color.Blue);
            lossFunction.Symbol.Size = 1;
            //
            evalFunciton = new LineItem("Model Evaluation", null, null, Color.Red, ZedGraph.SymbolType.None, 1);
            evalFunciton.Symbol.Fill = new Fill(Color.Red);
            evalFunciton.Symbol.Size = 1;


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

            ///Fitness simulation chart
            zedFitness.GraphPane.Title.Text = "Loss/Eval simulations";
            zedFitness.GraphPane.XAxis.Title.Text = "Iterations";
            zedFitness.GraphPane.YAxis.Title.Text = "Value";
            zedFitness.GraphPane.CurveList.Add(lossFunction);
            zedFitness.GraphPane.CurveList.Add(evalFunciton);

            zedModel.GraphPane.Title.Text = "ANN Iteration Simulation";
            zedModel.GraphPane.XAxis.Title.Text = "Samples";
            zedModel.GraphPane.YAxis.Title.Text = model.Label;
            zedModel.GraphPane.YAxis.MajorGrid.IsZeroLine = false;
            zedModel.GraphPane.CurveList.Add(actualData);
            zedModel.GraphPane.CurveList.Add(predictedData);
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Simple FFNN
            if (cbNetworkSettings.SelectedIndex == 0)
            {
                //neurons
                netSlabel1.Visible = true;
                netSedit1.Visible = true;

                //layers
                netSlabel2.Visible = false;
                netSlabel1.Text = "Neurons:";
                netSEdit2.Visible = false;

                //cells
                netSlabel3.Visible = false;
                netSEdit3.Visible = false;

                //check boxes
                checkStabilisation.Visible = false;
                checkDropRate.Visible = false;
                netSeditdrop.Visible = false;

            }
            //Deep FFNN
            else if (cbNetworkSettings.SelectedIndex == 1)
            {
                //neurons
                netSlabel1.Visible = true;
                netSlabel1.Text = "Neurons:";
                netSedit1.Visible = true;

                //layers
                netSlabel2.Visible = true;
                netSEdit2.Visible = true;

                //cells
                netSlabel3.Visible = false;
                netSEdit3.Visible = false;

                //check boxes
                checkStabilisation.Visible = false;
                checkDropRate.Visible = false;
                netSeditdrop.Visible = false;
            }
            //LSTM RNN
            else if (cbNetworkSettings.SelectedIndex == 2)
            {
                //neurons
                netSlabel1.Visible = true;
                netSlabel1.Text = "LSTM Cells:";
                netSedit1.Visible = true;

                //layers
                netSlabel2.Visible = true;
                netSEdit2.Visible = true;

                //embeding
                netSlabel3.Visible = false;
                netSEdit3.Visible = false;

                //check boxes
                checkStabilisation.Visible = true;
                checkDropRate.Visible = true;
                netSeditdrop.Visible = true;
            }
            //Sequence LSTM RNN
            else if (cbNetworkSettings.SelectedIndex == 3)
            {
                //neurons / Cells
                netSlabel1.Visible = true;
                netSlabel1.Text = "LSTM Cells:";
                netSedit1.Visible = true;

                //h layers / 
                netSlabel2.Visible = true;
                // netSEdit2.Text = "H Layers:";
                netSEdit2.Visible = true;

                //embedding
                netSlabel3.Visible = true;
                netSEdit3.Visible = true;


                //check boxes
                checkStabilisation.Visible = true;
                checkDropRate.Visible = true;
                netSeditdrop.Visible = true;
            }
        }

        private void updateChartFitness(int i, float loss, float eval, bool refresh = true)
        {
            //
           
            lossFunction.AddPoint(new PointPair(i, loss));
            evalFunciton.AddPoint(new PointPair(i, eval));

            if(refresh)
                zedFitness.RestoreScale(zedFitness.GraphPane);
        }

        List<string> Classes;
        private string YAxis_ScaleFormatEvent(GraphPane pane, Axis axis, double val, int index)
        {
            //zedModel.GraphPane.XAxis.Scale.Min = 0;
            zedModel.GraphPane.YAxis.Scale.Min = -0.1;
            zedModel.GraphPane.YAxis.Scale.Max = max + 0.2;
            zedModel.GraphPane.YAxis.MajorGrid.DashOff = 0;
            zedModel.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedModel.GraphPane.XAxis.MajorGrid.DashOff = 0;
            zedModel.GraphPane.XAxis.MajorGrid.IsVisible = false;
            zedModel.GraphPane.YAxis.Scale.MajorStep = 1;
            zedModel.GraphPane.YAxis.Scale.MinorStep = 0;

            if (Classes == null || val >= Classes.Count)
                return String.Format("{0}", (int)val);
            else
                return String.Format("{0}", Classes[(int)val]);
        }

        public void ResetData()
        {
            lossFunction.Clear();
            evalFunciton.Clear();
            actualData.Clear();
            predictedData.Clear();

        }
        #endregion

        #region Public Methods

        public void ReportProgress(int i, float loss, float eval, List<List<float>> actual, List<List<float>> predicted)
        {
            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.Invoke(
                    new Action(() =>
                    {
                        ReportProgressSync(i,loss,eval, actual, predicted);
                        
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
            currentIteration.Text = i.ToString(CultureInfo.InvariantCulture);
            UpdateChartFitnessAsync(i, loss, eval, actual, predicted);
            
        }

        public void UpdateChartFitnessAsync(int i, float loss, float eval, List<List<float>> actual, List<List<float>> predicted)
        {
            if (this.InvokeRequired)
            {
                // Execute the same method, but this time on the GUI thread
                this.Invoke(
                    new Action(() =>
                    {
                        updateChartFitness(i, loss, eval);
                        updateChartData(actual, predicted);
                    }
                    ));
            }
            else
            {
                updateChartFitness(i, loss, eval);
                updateChartData(actual, predicted);
            }
        }
       
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
        double max = 0;
        /// <summary>
        /// Deserilization of run condition
        /// </summary>
        /// <param name="p"></param>
        public void ActivatePanel(ActiveModelData data, ANNModel model)
        {
            SetParameters(data);

            //clear previous data if exist
            lossFunction.Clear();
            evalFunciton.Clear();
            actualData.Clear();
            predictedData.Clear();

            //prepare graphs
            PrepareGraphs(model);

            //update loss and eval
            for (int i=0; i < model.LossData.Count; i++)
            {
                updateChartFitness(i, model.LossData[i], model.EvalData[i], false);
            }
            
            //update mdoel
            updateChartData(model.ActualT, model.PredictedT);
            zedFitness.RestoreScale(zedFitness.GraphPane);
            this.zedModel.RestoreScale(zedModel.GraphPane);
        }
       
        public void ResetChart()
        {
            max = 0;
            Classes = null;
            zedModel.GraphPane.YAxis.ScaleFormatEvent -= YAxis_ScaleFormatEvent;
        }



        #endregion

        /// <summary>
        /// Available learners in CNTK 
        /// SGDLearner
        /// MomentumSGDLearner
        /// RMSPropLearner
        /// FSAdaGradLearner
        /// AdamLearner
        /// AdaGradLearner
        /// AdaDeltaLearner
        /// Only two are suported in ANNdotNET
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLearnerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbLearnerType.SelectedIndex==0)
            {
                label14.Visible = false;
                txMomentum.Visible = false;
            }
            else
            {
                label14.Visible = true;
                txMomentum.Visible = true;
            }
        }
    }

}

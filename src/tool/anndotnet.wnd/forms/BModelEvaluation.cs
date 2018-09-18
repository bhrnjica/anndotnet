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
using GPdotNet.MathStuff;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using ANNdotNet.Wnd;
namespace ANNdotNet.Wnd.Dialogs
{
    public partial class BModelEvaluation : Form
    {
        double m_AUC;
        double m_AUCtest;
        List<ConfusionMatrix> m_CMCollection;
        List<ConfusionMatrix> m_CMCollectiontest;
        string[] m_Classes;
        double[] m_yobs = null;
        double[] m_ypre = null;
        //

        double[] m_yobst = null;
        double[] m_ypret = null;

        double[] m_x = null;
        double[] m_y = null;
        double[] m_x1 = null;
        double[] m_y1 = null;

        public BModelEvaluation()
        {
            InitializeComponent();
            //this.Icon = Extensions.LoadIconFromName("GPdotNet.Wnd.Dll.Images.gpdotnet.ico");
            Load += BModelEvaluation_Load;
        }

        private void BModelEvaluation_Load(object sender, EventArgs e)
        {
            
            //
            calculateROCCurve(true);
            if (m_yobst != null)
                calculateROCCurve(false);

            comboBox1.SelectedIndex = 0;
            trThreshold.Value = 50;

            //show ROC for training
            displayData(m_x, m_y);

        }
        public void loadClasses(string[] classes)
        {
            m_Classes = classes;
        }
        public  void loadData(double[] y1, double[] ytr, double[] y2, double[] yts)
        {
            comboBox1.Items.Add("Training Data Set");
            m_yobs = y1;
            m_ypre = ytr;

            if(y2 != null)
                comboBox1.Items.Add("Testing Data Set"); 
            m_yobst = y2;
            m_ypret = yts;
            
        }

       
        private void calculateROCCurve(bool trainingData = true)
        {
            if(trainingData)
            {
                m_CMCollection = new List<ConfusionMatrix>();

                //add extra point 
                var o = convertToIntAray(m_yobs);
                var p = calculateOutput(m_ypre, -1.0);
                var c = new ConfusionMatrix(o, p, 2);
                m_CMCollection.Add(c);

                //Calculate Confusion matrix based on threshold value
                for (int i = trThreshold.Minimum; i <= trThreshold.Maximum; i++)
                {
                    var thresholdValue = ((double)i / (double)trThreshold.Maximum);
                    var observation = convertToIntAray(m_yobs);
                    var prediction = calculateOutput(m_ypre, thresholdValue);
                    var cm = new ConfusionMatrix(observation, prediction, 2);
                    m_CMCollection.Add(cm);
                }



                (double[] x, double[] y) = calculateROC(trainingData);
                m_y = y;
                m_x = x;
                m_AUC = calculateAUC(true);
                
                //draw graph
                //displayData(x, y);
            }
               
            else
            {
                m_CMCollectiontest = new List<ConfusionMatrix>();

                //add extra point 
                var o = convertToIntAray(m_yobst);
                var p = calculateOutput(m_ypret, -1.0);
                var c = new ConfusionMatrix(o, p, 2);
                m_CMCollectiontest.Add(c);

                //Calculate Confusion matrix based on threshold value
                for (int i = trThreshold.Minimum; i <= trThreshold.Maximum; i++)
                {
                    var thresholdValue = ((double)i / (double)trThreshold.Maximum);
                    var observation = convertToIntAray(m_yobst);
                    var prediction = calculateOutput(m_ypret, thresholdValue);
                    var cm = new ConfusionMatrix(observation, prediction, 2);
                    m_CMCollectiontest.Add(cm);
                }

                (double[] x, double[] y) = calculateROC(trainingData);

                m_AUCtest = calculateAUC(false);
                m_y1 = y;
                m_x1 = x;
                //draw graph
                //displayData(x, y);

            }

        }

        private double calculateAUC(bool trainingData)
        {
            var coll = trainingData ? m_CMCollection : m_CMCollectiontest;
            double area = 0.0;
            double h = 0.0;
            //
            for (int i = 0; i < coll.Count - 1; i++)
            {
                var cm0 = coll[i];
                var cm1 = coll[i+1];
                var fpr0 = 1.0 - ConfusionMatrix.Specificity(cm0.Matrix,0);
                var tpr0 = ConfusionMatrix.Recall(cm0.Matrix,1);//Recall
                var fpr1 = 1.0 - ConfusionMatrix.Specificity(cm1.Matrix, 0);
                var tpr1 = ConfusionMatrix.Recall(cm1.Matrix,1);//Recall

                //calc average y
                h = (tpr0 + tpr1)/2.0;

                var temp = h * Math.Abs(fpr1 - fpr0);

                area += temp;
            }


            if (area < 0.5)
                area = 1.0 - area;

            return area;
        }

        private (double[] x, double[] y) calculateROC(bool trainingData)
        {

            var coll = trainingData ? m_CMCollection : m_CMCollectiontest;
            double[] x = new double[coll.Count];
            double[] y = new double[coll.Count];
            for (int i = 0; i < coll.Count; i++)
            {
               var cm = coll[i];
                x[i] = 1 - ConfusionMatrix.Specificity(cm.Matrix,0);
                y[i] = ConfusionMatrix.Recall(cm.Matrix,1);
            }

            return (x, y);
        }

        private int[] convertToIntAray(double[] y)
        {
            int[] retVal = new int[y.Length];
            for (int i = 0; i < y.Length; i++)
            {
                    retVal[i] = (int)y[i];
            }
            return retVal;
        }

        private int[] calculateOutput(double[] y, double thresholdValue)
        {
            int[] retVal = new int[y.Length];
            for(int i = 0 ; i < y.Length; i++)
            {
                if (y[i] > thresholdValue)
                    retVal[i] = 1;
                else
                    retVal[i] = 0;
            }
            return retVal;
        }


        private void setConfusionMatrix(int threshold)
        {
            if (comboBox1.SelectedIndex > 0 && m_CMCollectiontest == null)
                return;
            threshold++;
            //
            int positiveValueIndex=1;
            int negativeValueIndex = 0;

            var cm = comboBox1.SelectedIndex > 0 ? m_CMCollectiontest[threshold]: m_CMCollection[threshold];
            var numObserv = comboBox1.SelectedIndex > 0 ? m_yobst.Length : m_yobs.Length;
            //
            txtError.Text = ConfusionMatrix.Error(cm.Matrix).ToString("F5");
            txtAUC.Text = comboBox1.SelectedIndex > 0 ? m_AUCtest.ToString("F3"): m_AUC.ToString("F3");

            //confusion matrix for current threshold
            txAccuracy.Text = ConfusionMatrix.OAC(cm.Matrix).ToString("F5");
            txPrecision.Text = ConfusionMatrix.Precision(cm.Matrix,1).ToString("F5");
            txRecall.Text = ConfusionMatrix.Recall(cm.Matrix, 1).ToString("F5");
            txScore.Text = ConfusionMatrix.Fscore(cm.Matrix,1).ToString("F5");

            txHSS.Text = ConfusionMatrix.HSS(cm.Matrix, numObserv).ToString("F5");
            txPSS.Text = ConfusionMatrix.PSS(cm.Matrix, numObserv).ToString("F5");

            //lab
            txPositiveLabel.Text = m_Classes[1];// "TRUE";
            txNegativeLable.Text = m_Classes[0];// "FALSE";

            //false and true positive negative
            txFN.Text = cm.Matrix[positiveValueIndex][negativeValueIndex].ToString();//cm.FalseNegatives 
            txFP.Text = cm.Matrix[negativeValueIndex][positiveValueIndex].ToString();//cm.FalsePositives 
            txTP.Text = cm.Matrix[positiveValueIndex][positiveValueIndex].ToString();//cm.TruePositives 
            txTN.Text = cm.Matrix[negativeValueIndex][negativeValueIndex].ToString();//cm.TrueNegatives 
        }




        
        private void displayData(double[] x, double[] y)
        {
            this.zedModel.GraphPane.CurveList.Clear();
            this.zedModel.GraphPane.CurveList.Clear();
            this.zedModel.GraphPane.GraphObjList.Clear();

            //this.zedModel.GraphPane.Border.IsVisible = false;
            //this.zedModel.GraphPane.Legend.IsVisible = false;
            //this.zedModel.GraphPane.Title.IsVisible = false;
            this.zedModel.GraphPane.XAxis.MinorGrid.IsVisible = false;
            this.zedModel.GraphPane.YAxis.MinorGrid.IsVisible = false;

            zedModel.GraphPane.XAxis.Scale.Min = -0.01;
            zedModel.GraphPane.XAxis.Scale.Max = 1;
            zedModel.GraphPane.YAxis.Scale.Min = 0;
            zedModel.GraphPane.YAxis.Scale.Max = 1.01;

            zedModel.GraphPane.Title.Text = "Area under the ROC curve";
            zedModel.GraphPane.XAxis.Title.Text = "False Positive Rate";
            zedModel.GraphPane.YAxis.Title.Text = "True Positive Rate";

            var curve = zedModel.GraphPane.AddCurve("ROC Curve", x, y, System.Drawing.Color.Blue, ZedGraph.SymbolType.None);
            curve.Symbol.Border = new ZedGraph.Border(System.Drawing.Color.Black, 0.1f);
            curve.Line.Width = 3;

            var random = zedModel.GraphPane.AddCurve("Random", x, x, System.Drawing.Color.Red, ZedGraph.SymbolType.None);
            random.Symbol.Border = new ZedGraph.Border(System.Drawing.Color.Red, 0.1f);
            random.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            random.Line.Width = 1;



            zedModel.GraphPane.AxisChange(zedModel.CreateGraphics());
            zedModel.Invalidate();   
        }


        /// <summary>
        /// change the value of threshold
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBar1_Scroll(object sender, EventArgs e)
        {


            labThresholdValue.Text = (trThreshold.Value/100.0).ToString("F2");
            //
            setConfusionMatrix(trThreshold.Value);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > 0 && m_y1 != null)
            {
                displayData(m_x1, m_y1);
                setConfusionMatrix(trThreshold.Value);
            }
             
            else
            {
                displayData(m_x, m_y);
                setConfusionMatrix(trThreshold.Value);
            }
           

        }
    }
}

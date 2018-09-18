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
using ANNdotNet.Wnd.Dialogs;
using GPdotNet.MathStuff;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace anndotnet.wnd.panels
{
    /// <summary>
    /// Interaction logic for Regression.xaml
    /// </summary>
    public partial class BinaryClassification : UserControl
    {
        public BinaryClassification()
        {
            InitializeComponent();
            this.DataContextChanged += BinaryClassification_DataContextChanged;
        }

        private void BinaryClassification_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(this.DataContext is Models.ModelPerformance))
                return;

            var modelPerf = (Models.ModelPerformance)this.DataContext;
            var perfData = ((Models.ModelPerformance)this.DataContext).PerformanceData;
            if (perfData == null)
                return;
            //
            var observation = perfData["obs_train"].Select(x => (int)(double)x).ToArray();
            var prediction = perfData["prd_train"].Select(x => (int)Math.Round((double)x)).ToArray();
            var cm = new ConfusionMatrix(observation, prediction, 2);
            //
            modelPerf.TP = cm.Matrix[1][1];
            modelPerf.FP = cm.Matrix[0][1];
            modelPerf.FN = cm.Matrix[1][0];
            modelPerf.TN = cm.Matrix[0][0];

            modelPerf.Acc = (float)Math.Round(ConfusionMatrix.OAC(cm.Matrix),3);
            modelPerf.Precision = (float)Math.Round(ConfusionMatrix.Precision(cm.Matrix, 1), 3);
            modelPerf.Recall = (float)Math.Round(ConfusionMatrix.Recall(cm.Matrix, 1), 3);
            modelPerf.F1Score = (float)Math.Round(ConfusionMatrix.Fscore(cm.Matrix, 1), 3);

            modelPerf.HSS = (float)Math.Round(ConfusionMatrix.HSS(cm.Matrix, observation.Length), 3);
            modelPerf.PSS = (float)Math.Round(ConfusionMatrix.PSS(cm.Matrix, observation.Length), 3);
        }

        private void ROCCurve_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is Models.ModelPerformance))
                return;

            var modelPerf = (Models.ModelPerformance)this.DataContext;
            var ret = ((Models.ModelPerformance)this.DataContext).PerformanceData;

            BModelEvaluation dlg = new BModelEvaluation();
            dlg.Text = "Model performance for selected data set";
            var cl = ret["Classes"].Select(x => x.ToString()).ToArray();
            dlg.loadClasses(cl);
            dlg.loadData(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray(),null,null);

            dlg.ShowDialog();
        }

        private void CMatric_Click(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is Models.ModelPerformance))
                return;

            var modelPerf = (Models.ModelPerformance)this.DataContext;
            var ret = ((Models.ModelPerformance)this.DataContext).PerformanceData;

            MModelEvaluation dlg = new MModelEvaluation();
            dlg.Text = "Confusion matrix for training data set";
            var cl = ret["Classes"].Select(x => x.ToString()).ToArray();
            dlg.loadClasses(cl);
            dlg.loadData(ret["obs_train"].Select(x => (double)x).ToArray(), ret["prd_train"].Select(x => (double)x).ToArray(),
                  null,null);

            dlg.ShowDialog();
        }
    }
}

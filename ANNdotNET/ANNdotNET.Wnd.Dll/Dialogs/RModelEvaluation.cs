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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GPdotNet.MathStuff;
using ANNdotNet.Wnd;
namespace ANNdotNet.Wnd.Dialogs
{
    public partial class RModelEvaluation : Form
    {
        Dictionary<string, Dictionary<string, double>> m_evals;
        public RModelEvaluation()
        {
            InitializeComponent();
            this.Icon = Extensions.LoadIconFromName("GPdotNet.Wnd.Dll.Images.gpdotnet.ico");
            comboBox1.SelectedIndex = 0;
            this.Load += RModelEvaluation_Load;
        }

        private void RModelEvaluation_Load(object sender, EventArgs e)
        {
            fillFields();
        }

        private void fillFields()
        {
            if (m_evals != null)
            {
                var ind = comboBox1.SelectedIndex;
                var dic = m_evals.ElementAt(ind).Value;
                //
                txSE.Text = dic["Square Error"].ToString("N3");
                txRMSE.Text = dic["Root Mean Square Error"].ToString("N3");
                txNSA.Text = dic["Nash-Sutcliffe Efficiency"].ToString("N3");
                txPBIAS.Text = dic["Percent bias"].ToString("N3");
                txR.Text = dic["Correlation coeff."].ToString("N3");
                txR2.Text = dic["Determination coeff."].ToString("N3");
               
            }
        }

        public void Evaluate(double[] y1,double[] ytr, double[] y2, double [] yts )
        {
           
            var key1 = "Model";
            var key2 = "Prediction";
            var dic = new Dictionary<string, Dictionary<string, double>>();
            var p1 = new Dictionary<string, double>();
            var p2 = new Dictionary<string, double>();

            //Square residual
            p1.Add("Square Error", y1.SE(ytr));
            p2.Add("Square Error", yts != null ? y2.SE(yts) : 0);

            //RMSE 
            p1.Add("Root Mean Square Error", y1.RMSE(ytr));
            p2.Add("Root Mean Square Error", yts != null ? y2.RMSE(yts) : 0);


            //Nash-Sutcliffe efficiency 
            p1.Add("Nash-Sutcliffe Efficiency", y1.NSE(ytr));
            p2.Add("Nash-Sutcliffe Efficiency", yts != null ? y2.NSE(yts) : 0);

            //Percent bias (PBIAS)
            p1.Add("Percent bias", y1.PBIAS(ytr));
            p2.Add("Percent bias", yts != null ? y2.PBIAS(yts) : 0);


            //correlation coefficient
            p1.Add("Correlation coeff.", y1.R(ytr));
            p2.Add("Correlation coeff.", yts != null ? y2.R(yts) : 0);

            //determination coefficient
            p1.Add("Determination coeff.", y1.R2(ytr));
            p2.Add("Determination coeff.", yts != null ? y2.R2(yts) : 0);

            //
            dic.Add(key1, p1);
            dic.Add(key2, p2);

            m_evals= dic;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillFields();
        }
    }
}

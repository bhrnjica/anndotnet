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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANNdotNet.Wnd.Dll;
namespace ANNdotNet.Wnd.Dll.Panels
{
    public partial class StartPanel : UserControl
    {
        public Action<string> Open { get; set; }
        public Action New { get; set; }

        public StartPanel()
        {
            InitializeComponent();
            this.Load += StartPanel_Load;
        }

        private void StartPanel_Load(object sender, EventArgs e)
        {
            pbLink1.Image = Extensions.LoadImageFromName("ANNdotNet.Wnd.Dll.Images.hyperlink_32.png");
            pbLink2.Image = Extensions.LoadImageFromName("ANNdotNet.Wnd.Dll.Images.hyperlink_32.png");

            pbLink3.Image = Extensions.LoadImageFromName("ANNdotNet.Wnd.Dll.Images.hyperlink_32.png");
            pbLogoHor.Image = Extensions.LoadImageFromName("ANNdotNet.Wnd.Dll.Images.ANNLogo_350x134pix.png");
        }
        #region Predefined  Samples

        #region Links
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTTP_Portal(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://github.com/bhrnjica/gpdotnet");
            //txtStatusMessage.Text = "Ready!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTTP_BLOG(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://bhrnjica.wordpress.com/gpdotnet");
            //txtStatusMessage.Text = "Ready!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTTP_UserGuide(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string strPath = Application.StartupPath + "\\Resources_Files\\GPdotNET_User_Guide.pdf";
                System.Diagnostics.Process.Start(strPath);
                // txtStatusMessage.Text = "Ready!"; ex
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        #endregion

        #region Regression and Approximation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REG_simple_fun_mod(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\simple_reg.gpa";

            Open(strPath);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REG_concrete_slum_test(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\concrete_slump_test.gpa";

            Open(strPath);
            //var fName = Path.GetFileName(_filePath);
            //this.Text = string.Format("{0} - {1}", _appName, fName);
            //txtStatusMessage.Text = "Ready!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REG_WaterParameters(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\water_quality.gpa";

            Open(strPath);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REG_surface_roughness(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\surface_roughness.gpa";

            Open(strPath);
        }

        #endregion

        #region TimeSeries
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TSMSFT_Forcasting(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources_Files\\montly_msft_2003_2012.gpa";

            //Open(strPath);
            //var fName = Path.GetFileName(_filePath);
            //this.Text = string.Format("{0} - {1}", _appName, fName);
            //txtStatusMessage.Text = "Ready!";
        }
        #endregion

        #region Classification problems BCC and MCC

        private void BCC_SimpleCase(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\simple_binary.gpa";

            Open(strPath);
        }
        private void BCC_Titanic(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\titanic.gpa";

            Open(strPath);
        }

        private void BCC_Credit_Approval(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\credit_approval.gpa";

            Open(strPath);

        }

        private void HeartDataSet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\hr.gpa";
            Open(strPath);
        }

        private void mcc_simple_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\simple_mcc.gpa";

            Open(strPath);
        }
        private void MCC_Sample1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\glassident.gpa";

            Open(strPath);

        }
        private void MCC_Sample2(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\iris.gpa";
            Open(strPath);

        }


        private void ImgSegmentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\segment_model.gpa";

            Open(strPath);
        }



        #endregion

        #endregion

    }
}

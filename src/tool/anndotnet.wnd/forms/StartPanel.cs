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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace anndotnet.wnd.Panels
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
            System.Diagnostics.Process.Start("http://github.com/bhrnjica/anndotnet");
            //txtStatusMessage.Text = "Ready!";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HTTP_BLOG(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://bhrnjica.wordpress.com/anndotnet");
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
                System.Diagnostics.Process.Start("https://github.com/bhrnjica/anndotnet/wiki");
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
        private void REG_concrete_slump(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Concrete\\ConcreteSlumpProject.ann";

            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void REG_BikeSharing(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Bike\\BikeSharingProject.ann";

            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
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
            string strPath = Application.StartupPath + "\\Resources\\AirQ\\AirQuality.ann";

            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }
        #endregion

        #region Classification problems BCC and MCC

        private void MCC_BreastCancer(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\BreastC\\BreastCancerProject.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }
        private void BCC_Titanic(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Titanic\\TitanicProject.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }

        private void BCC_Mushroom(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Mushroom\\MushroomProject.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }

        private void BCC_Credit_Approval(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\credit_approval.gpa";

            Open(strPath);

        }

       

        private void mcc_irisflower(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Iris\\IrisProject.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }
        private void mcc_glass_idettification(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\Glass\\GlassIdentificationProject.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);

        }
        private void MCC_MNISTExample(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string strPath = Application.StartupPath + "\\Resources\\MNIST\\MNIST-Project.ann";
            AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
            appCOnt.OpenProject(strPath);
        }

        #endregion

        #endregion


    }
}

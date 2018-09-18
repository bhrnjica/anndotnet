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
using System.Windows;

namespace anndotnet.wnd.Pages
{
    /// <summary>
    /// Interaction logic for ExperimentPage.xaml
    /// </summary>
    public partial class ProjectPage
    {
        /// <summary>
        /// 
        /// </summary>
        public ProjectPage()
        {
            InitializeComponent();
            this.Loaded += ExperimentPage_Loaded;
            this.DataContextChanged += ExperimentPage_DataContextChanged;
           
        }
        

        /// <summary>
        /// When ExpData tree items is changed this method should be called because of the DataCOntext is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExperimentPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //For old experiment page save the state
            if(e.OldValue!=null)
            {
                
                var model = e.OldValue as ANNProjectController;
                model.DataSet = project.GetDataSet();
            }
            //for project show previously stored state
            if(e.NewValue !=null)
            {
                
                var prjCont = e.NewValue as ANNProjectController;
                if(prjCont != null)
                {
                    project.ResetExperimentalPanel();
                    if (prjCont.DataSet != null)
                        project.SetDataSet(prjCont.DataSet);
                }
               
            } 
        }

        private void ExperimentPage_Loaded(object sender, RoutedEventArgs e)
        {
            //experiment.CreateModel = CreateNewModel;
            //experiment.UpdateModel = UpdateModel;
        }


    }
}

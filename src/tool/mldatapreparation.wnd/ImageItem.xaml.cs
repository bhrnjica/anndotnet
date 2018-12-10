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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataProcessing.Wnd
{


    
    /// <summary>
    /// Interaction logic for ItemItem.xaml
    /// </summary>
    public partial class ImageItem : UserControl
    {
        static string m_lastVisitedFolder;
        public ImageItem()
        {
            InitializeComponent();
           // this.DataContextChanged += LayerItem_DataContextChanged;
        }

       
        private void folderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = m_lastVisitedFolder;
            if (dialog.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                var lbl = this.DataContext as ImageLabelItem;
                //txtCdim.Text = dialog.SelectedPath;
                if(lbl!=null)
                {
                    lbl.Folder = dialog.SelectedPath;
                    txtCdim.Text = dialog.SelectedPath;
                }
                    
                m_lastVisitedFolder = dialog.SelectedPath;
            }
               
        }

    }
}

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
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace anndotnet.wnd.panels
{
    /// <summary>
    /// Interaction logic for LayerItem.xaml
    /// </summary>
    public partial class LayerItem : UserControl
    {
        public LayerItem()
        {
            InitializeComponent();
            this.DataContextChanged += LayerItem_DataContextChanged;
        }

        private void LayerItem_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
           if(this.DataContext != null)
            {
                var nn = this.DataContext as NNetwork.Core.Common.NNLayer;
                if(nn.Name.StartsWith("Normalization"))
                {
                    //visibility
                    lblHdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtHdim.Visibility = System.Windows.Visibility.Collapsed;
                    lblAct.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAct.Visibility = System.Windows.Visibility.Collapsed;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtCdim.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Collapsed;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (nn.Name.StartsWith("Drop"))
                {
                    //visibility
                    lblHdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtHdim.Visibility = System.Windows.Visibility.Collapsed;
                    lblAct.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAct.Visibility = System.Windows.Visibility.Collapsed;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtCdim.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Collapsed;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Visible;
                    txtDrop.Visibility = System.Windows.Visibility.Visible;
                    lblPerc.Visibility = System.Windows.Visibility.Visible;
                }
                else if (nn.Name.StartsWith("Embedding") || nn.Name.StartsWith("NALU"))
                {
                    //visibility
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAct.Visibility = System.Windows.Visibility.Collapsed;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtCdim.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Collapsed;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (nn.Name.StartsWith("Dense"))
                {
                    //first row
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Visible;
                    cmbAct.Visibility = System.Windows.Visibility.Visible;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Collapsed;
                    txtCdim.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Collapsed;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (nn.Name.StartsWith("LSTM") || nn.Name.StartsWith("GRU"))
                {
                    lblCdim.Content = "Cell dimension: ";
                    isChecked2.Content = "Peepholes";
                    //first row
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Visible;
                    cmbAct.Visibility = System.Windows.Visibility.Visible;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Visible;
                    txtCdim.Visibility = System.Windows.Visibility.Visible;
                    isChecked1.Visibility = System.Windows.Visibility.Visible;
                    isChecked2.Visibility = System.Windows.Visibility.Visible;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }
                else if (nn.Name.StartsWith("CudaStacked"))
                {
                    lblHdim.Content = "Layer dimension: ";
                    lblCdim.Content = "Layer count: ";
                    isChecked2.Content = "Is bidirectional";
                    //first row
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAct.Visibility = System.Windows.Visibility.Collapsed;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Visible;
                    txtCdim.Visibility = System.Windows.Visibility.Visible;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Visible;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    lblCdim.Content = "Cell dimension: ";
                    isChecked2.Content = "Peepholes";
                    //first row
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Visible;
                    cmbAct.Visibility = System.Windows.Visibility.Visible;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Visible;
                    txtCdim.Visibility = System.Windows.Visibility.Visible;
                    isChecked1.Visibility = System.Windows.Visibility.Visible;
                    isChecked2.Visibility = System.Windows.Visibility.Visible;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                }

            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }
    }
}

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
                    //third row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Visible;
                    lblDesc.Content = "Description: The layer normalizes the values of input data by z-score calculation.";

                }
                else if (nn.Name.StartsWith("Scale"))
                {
                    //visibility
                    lblHdim.Content = "Numerator: ";
                    lblCdim.Content = "Denominator: ";
                    lblHdim.Visibility = System.Windows.Visibility.Visible;
                    txtHdim.Visibility = System.Windows.Visibility.Visible;
                    lblAct.Visibility = System.Windows.Visibility.Collapsed;
                    cmbAct.Visibility = System.Windows.Visibility.Collapsed;
                    //second row
                    lblCdim.Visibility = System.Windows.Visibility.Visible;
                    txtCdim.Visibility = System.Windows.Visibility.Visible;
                    isChecked1.Visibility = System.Windows.Visibility.Collapsed;
                    isChecked2.Visibility = System.Windows.Visibility.Collapsed;
                    //last row
                    lblDrop.Visibility = System.Windows.Visibility.Collapsed;
                    txtDrop.Visibility = System.Windows.Visibility.Collapsed;
                    lblPerc.Visibility = System.Windows.Visibility.Collapsed;
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Visible;
                    lblDesc.Content = "Description: Multiplication of the fraction (Num./Denom.) and input data.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Visible;
                    lblDesc.Content = "Description: Drop value is in percentage. Only integer value is allowed.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
                }
                else if (nn.Name.StartsWith("Conv1D"))
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
                }
                else if (nn.Name.StartsWith("Polling1D"))
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
                }
                else if (nn.Name.StartsWith("Conv2D"))
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
                }
                else if (nn.Name.StartsWith("Polling2D"))
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
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
                    //description row
                    lblDesc.Visibility = System.Windows.Visibility.Collapsed;
                    lblDesc.Content = "Description:.";
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

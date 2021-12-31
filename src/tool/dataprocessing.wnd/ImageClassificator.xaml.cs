using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataProcessing.Core;

namespace DataProcessing.Wnd
{
    
    /// <summary>
    /// Interaction logic for ImageClassificator.xaml
    /// </summary>
    public partial class ImageClassificator : System.Windows.Controls.UserControl
    {
        public ImageClassificator()
        {
            InitializeComponent();
            this.DataContextChanged += ImageClassificator_DataContextChanged;
        }

        private void ImageClassificator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext!=null && (DataContext is ImageClassificatorModel))
            {
                imageLabelList.ItemsSource = ((ImageClassificatorModel)DataContext).Labels;
                
            }
                
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ImageClassificatorModel;
            if (model == null || model.Labels == null)
            {
                model = new ImageClassificatorModel();
                this.DataContext = model;
            }

            model.Labels.Add(new ImageLabelItem());
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ImageClassificatorModel;
            if (model == null || model.Labels == null)
                return;
           if (imageLabelList.SelectedItem!=null && System.Windows.Forms.DialogResult.Yes == System.Windows.Forms.MessageBox.Show("Confirm deletion", "ANNdotNET", System.Windows.Forms.MessageBoxButtons.YesNo))
                model.Labels.Remove((ImageLabelItem)imageLabelList.SelectedItem);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]*(?:\.[0-9]*)?$");
        }

        public ANNDataSet GetDataSet()
        {
            try
            {
                var Model = this.DataContext as ImageClassificatorModel;
                if (Model != null && Model.Labels != null)
                {
                    ANNDataSet ds = new ANNDataSet();
                    ds.MetaData = new MetaColumn[]
                    {
                    new MetaColumn(){ Id=0, Index=0, Name="Image", Type="Image", MissingValue="None", Param=VariableType.Feature.ToString(),  },
                    new MetaColumn(){ Id=0, Index=0, Name="Label", Type="Category", MissingValue="None", Param=VariableType.Label.ToString(),  }
                    };
                    ds.Data = new List<List<string>>();
                    foreach (var l in Model.Labels)
                    {
                        if (string.IsNullOrEmpty(l.Label) || string.IsNullOrEmpty(l.Folder)
                            || Model.Channels <= 0 || Model.Width <= 0 || Model.Height <= 0)
                            throw new Exception("One or more image classification data parameters has invalid value!");

                        var strRow = new List<string>() { l.Label, l.Folder, l.Query, Model.Channels.ToString(),
                            Model.Height.ToString(), Model.Width.ToString(),Model.DataAugmentation.ToString()

                        };
                        ds.Data.Add(strRow);
                    }

                    //data1.TestRows = int.Parse(txtValidationCount.Text);
                    ds.RowsToValidation = int.Parse(txtValidationCount.Text);
                    ds.RowsToTest = int.Parse(txtTestCount.Text);

                    ds.IsPrecentige = radionPercentige.IsChecked.Value;
                    ds.RandomizeData = checkRandomizeDataset.IsChecked.Value;

                    return ds;
                }
                else
                    return null;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public ImageClassificatorModel LoadDataSet(ANNDataSet dataSet)
        {
            try
            {
                if (dataSet == null || dataSet.Data == null || dataSet.MetaData == null)
                {
                    ResetPanel();
                    return null;
                }

                var model = loadDataToModel(dataSet);
                //set 
                //txtValidationCount.Text = dataSet.TestRows.ToString();
                txtValidationCount.Text = dataSet.RowsToValidation.ToString();
                txtTestCount.Text = dataSet.RowsToTest.ToString();
                radionNumber.IsChecked = !dataSet.IsPrecentige;
                radionPercentige.IsChecked = dataSet.IsPrecentige;

                return model;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        private ImageClassificatorModel loadDataToModel(ANNDataSet dataSet)
        {
            try
            {
                var model = new ImageClassificatorModel();
                if (dataSet != null && dataSet.Data.Count > 0)
                {
                    //
                    var row = dataSet.Data.First();
                    model.Channels = int.Parse(row[3]);
                    model.Height = int.Parse(row[4]);
                    model.Width = int.Parse(row[5]);

                    model.DataAugmentation = int.Parse(row[6]);


                    //extract each image label
                    foreach (var r in dataSet.Data)
                    {
                        var itm = new ImageLabelItem();
                        itm.Label = r[0];
                        itm.Folder = r[1];
                        itm.Query = r[2]=="n/a"?"":r[2];
                        model.Labels.Add(itm);
                    }
                    return model;
                }
                else
                    return null;


            }
            catch (Exception)
            {

                throw;
            }
           
        }
        private void ResetPanel()
        {
            var model  = this.DataContext as ImageClassificatorModel;
            if(model!=null && model.Labels!=null)
                model.Labels.Clear();
        }
    }

   
}

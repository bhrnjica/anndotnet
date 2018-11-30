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

namespace DataProcessing.Wnd
{
    
    /// <summary>
    /// Interaction logic for ImageClassificator.xaml
    /// </summary>
    public partial class ImageClassificator : UserControl
    {
        ObservableCollection<ImageLabelItem> Labels { get; set; }
        public ImageClassificator()
        {
            InitializeComponent();
            Labels = new ObservableCollection<ImageLabelItem>();
            imageLabelList.ItemsSource = Labels;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e)
        {
            Labels.Add(new ImageLabelItem());
        }

        private void Button_Click_Remove(object sender, RoutedEventArgs e)
        {
            if(imageLabelList.SelectedItem!=null && MessageBoxResult.Yes == MessageBox.Show("Confirm deletion", "ANNdotNET", MessageBoxButton.YesNo))
             Labels.Remove((ImageLabelItem)imageLabelList.SelectedItem);
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]*(?:\.[0-9]*)?$");
        }
    }

    public class ImageLabelItem
    {
        public string Label { get; set; }
        public string Folder { get; set; }
        public string Query { get; set; }
    }
}

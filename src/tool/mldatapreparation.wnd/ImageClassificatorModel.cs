using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing.Wnd
{
    public class ImageLabelItem
    {
        public string Label { get; set; }
        public string Folder { get; set; }
        public string Query { get; set; }
    }
    public class ImageClassificatorModel
    {
        //Scale image params
        public int Channels { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int DataAugmentation { get; set; }
        
        public ObservableCollection<ImageLabelItem> Labels { get; set; }
        
        public ImageClassificatorModel()
        {
            Labels = new ObservableCollection<ImageLabelItem>();
        }
    }
}

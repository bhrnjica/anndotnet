using System;
using Anndotnet.App.ViewModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;


namespace Anndotnet.App;
public class CustomColorizer : DocumentColorizingTransformer
{
    private readonly DataParserViewModel _dataParserViewModel;
    private readonly Random random = new Random();
    public CustomColorizer(DataParserViewModel dataParserViewModel)
    {
        _dataParserViewModel = dataParserViewModel;
    }
    protected override void ColorizeLine(DocumentLine line)
    {
        int startOffset = line.Offset;
        int endOffset = line.EndOffset;

        if (startOffset < 0 || endOffset < 0)
            return;

        int length = endOffset - startOffset;
        string text = CurrentContext.Document.GetText(startOffset, length);

   
        // Check if it's a comment line
        if (text.StartsWith("!"))
        {
            // Colorize comments in green
            ChangeLinePart(startOffset, startOffset + length, (VisualLineElement element) =>
            {
                element.TextRunProperties.SetForegroundBrush(Brushes.Green);
            });
        }
        else
        {
            // Split the line by the column separator (comma or pipe)
            string[] columns = text.Split(new[] { _dataParserViewModel.DataParser.ColumnSeparator });

            // Colorize header line with different colors for each column
            //if (_dataParserViewModel.DataParser.HasHeader)
            {
                int currentOffset = startOffset;
                foreach (string column in columns)
                {
                    int columnLength = column.Length; // Include the separator
                    ChangeLinePart(currentOffset, currentOffset + columnLength, (VisualLineElement element) =>
                    {
                        // Use different colors for each column
                        element.TextRunProperties.SetForegroundBrush(GetColumnColor(column));
                    });

                    currentOffset += columnLength;
                }
            }
            //else
            //{
            //    // Colorize content rows with grayed column color
            //    int currentOffset = startOffset;
            //    foreach (string column in columns)
            //    {
            //        int columnLength = column.Length; // Include the separator
            //        ChangeLinePart(currentOffset, currentOffset + columnLength, (VisualLineElement element) =>
            //        {
            //            // Use grayed color for content rows
            //            element.TextRunProperties.SetForegroundBrush(Brushes.Gray);
            //        });

            //        currentOffset += columnLength;
            //    }
            //}
        }
    }

    private IBrush GetColumnColor(string columnName)
    {
        byte[] rgb = new byte[3];
        random.NextBytes(rgb);
        var color = Color.FromArgb(100, rgb[0], rgb[1], rgb[2]);
        return new SolidColorBrush(color);
    }

}

public partial class DataParserView : UserControl
{
    public DataParserView()
    {

        InitializeComponent();

        Loaded+= (o, e) =>
        {
            TextEditor.TextArea.TextView.LineTransformers.Add(new CustomColorizer(this.DataContext as DataParserViewModel));
        };     

        
    }

}
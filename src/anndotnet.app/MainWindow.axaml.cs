using Anndotnet.App.ViewModel;
using Avalonia.Controls;

namespace Anndotnet.App
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel mainViewModel)
        {

            this.DataContext = mainViewModel;
            InitializeComponent();
        }
    }
}
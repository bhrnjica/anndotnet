using anndotnet.wnd.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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
using System.Windows.Threading;
using System.IO.Compression;
using System.IO;

namespace anndotnet.wnd.Pages
{
    /// <summary>
    /// Interaction logic for StartPageEx.xaml
    /// </summary>
    public partial class StartPageEx
    {
        private string m_defaultBranch = "master";
        //private string m_defaultBranch = "master";
        public Action<Exception> ReportException { get; internal set; }

        public StartPageEx()
        {
            InitializeComponent();
            this.Loaded += StartPage_Loaded;
            this.DataContextChanged += StartPage_DataContextChanged;
        }

        private void StartPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //for the old DataContex we should save the state
                if (e.OldValue != null)
                {
                    //set wait cursor 
                    MainWindow.SetCursor(true);

                    //start.Dispose();
                }
                //for new model we should show previously stored state
                if (e.NewValue != null)
                {

                    //restore cursor 
                    MainWindow.SetCursor(false);
                }
            }
            catch (System.Exception ex)
            {
                //restore cursor 
                MainWindow.SetCursor(false);

                Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(

                    () =>
                    {
                        var appCnt = App.Current.MainWindow.DataContext as AppController;
                        appCnt.ReportException(ex);
                    }

                ));
            }
        }

        private async void StartPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = new List<ExamplesModel>();
                var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("anndotnet"));
                //var cnt = await client.Repository.Content.GetAllContents("bhrnjica", "anndotnet", "Examples");
                var cnt = await client.Repository.Content.GetAllContentsByRef("bhrnjica", "anndotnet", "Examples", m_defaultBranch);
                //
                foreach (var f in cnt.Where(x=>x.Name.EndsWith("zip")))
                {
                    var em = new ExamplesModel();
                    em.Name = f.Name;
                    em.Path = f.Path;
                    var pathReadm = f.Path.Replace(f.Name, System.IO.Path.GetFileNameWithoutExtension(f.Name)+".txt");
                    var fileWithContent = await client.Repository.Content.GetAllContentsByRef("bhrnjica", "anndotnet", pathReadm, m_defaultBranch);
                    em.Description = fileWithContent.FirstOrDefault()?.Content;
                    model.Add(em);
                }
                listView.ItemsSource = model;
            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }

            
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        /// <summary>
        /// Event to open example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestOpenExample(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                var strPat = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string strPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(strPat), e.Uri.OriginalString);
                AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
                appCOnt.OpenProject(strPath);
            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }
            
        }

        /// <summary>
        /// Method downloads the example from the github location, store on Example folder of the exe file 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MainWindow.SetCursor(true);
                var strPat = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exampleDir = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(strPat), "Examples\\");
                var example = ((ListViewItem)sender).Content as ExamplesModel;

                //create folder in case doesnt exist
                var fullPathFolder = $"{exampleDir}{System.IO.Path.GetFileNameWithoutExtension(example.Name)}\\";
                if (!System.IO.Directory.Exists(exampleDir))
                {
                    System.IO.Directory.CreateDirectory(exampleDir);
                }

                
                //var zipFile = "";
                if(string.IsNullOrEmpty(example.Name))
                {
                    MessageBox.Show("File not found.","ANNDotNET");
                    return;
                }
                //var remoteUrl = $"https://github.com/bhrnjica/anndotnet/raw/{m_defaultBranch}/{example.Name}";
                var remoteUrl = $"https://raw.githubusercontent.com/bhrnjica/anndotnet/{m_defaultBranch}/Examples/{example.Name}";
                var fullPath = exampleDir + example.Name;
                //
                var fi = new System.IO.FileInfo(fullPath);
                if(!fi.Exists)
                    await downloadFile(remoteUrl, fullPath);
                //exctract the zip file
                using (var strm = File.OpenRead(fullPath))
                using (ZipArchive a = new ZipArchive(strm))
                {
                    a.Entries.Where(o => o.Name == string.Empty && !System.IO.Directory.Exists(System.IO.Path.Combine(exampleDir, o.FullName))).ToList().ForEach(o => Directory.CreateDirectory(System.IO.Path.Combine(exampleDir, o.FullName)));
                    a.Entries.Where(o => o.Name != string.Empty).ToList().ForEach(xx => xx.ExtractToFile(System.IO.Path.Combine(exampleDir, xx.FullName), true));
                }
                //open annproject
                var annFile = Directory.GetFiles(fullPathFolder).Where(x => System.IO.Path.GetExtension(x).Equals(".ann", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if(string.IsNullOrEmpty(annFile))
                {
                    MessageBox.Show("Annproject file cannot be found.", "ANNDotNET");
                    return;
                }
                AppController appCOnt = App.Current.MainWindow.DataContext as AppController;
                MainWindow.SetCursor(false);
                appCOnt.OpenProject(annFile);
            }
            catch (Exception ex)
            {
                if (ReportException != null)
                    ReportException(ex);
            }
            finally
            {
                MainWindow.SetCursor(false);
            }
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="serverPath">The server path.</param>
        /// <param name="localPath">The local path.</param>
        private static async Task downloadFile(string serverPath, string localPath)
        {
            using (System.Net.WebClient wb = new System.Net.WebClient())
            {
                wb.DownloadProgressChanged += downloadProgressChanged;
                await wb.DownloadFileTaskAsync(new Uri(serverPath), localPath);
            }

        }

        private static void downloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            
        }
    }
}

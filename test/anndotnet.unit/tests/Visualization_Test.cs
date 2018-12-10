using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core;
using Xunit;

namespace anndotnet.unit
{

    public class VisualizationTests
    {

        [Fact]
        public void export_to_csv_regression_test01()
        {
            DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();
            //var strPath = "C:\\sc\\Datasets\\ResNet18_ImageNet_CNTK.model";
            var strPath = "C:\\sc\\github\\anndotnet\\src\\tool\\anndotnet.wnd\\Resources\\Iris\\IrisProject\\FFModel\\models\\model_at_983of1000_epochs_TimeSpan_636729018198856673";
            vizualizeModel(strPath);
            //
            
            Assert.Equal(8, 8);


        }
        public static string GenerateNetworkGraph(Function model)
        {
            try
            {
                var fg = new NetToGraph();
                var dotStr = fg.ToGraph(model);
                return dotStr;
            }
            catch (Exception)
            {

                throw;
            }
        }
        static void vizualizeModel(string cntkModelPath)
        {

            try
            {
                Function model = Function.Load(cntkModelPath, DeviceDescriptor.UseDefaultDevice(), ModelFormat.CNTKv2);
                //generate graph and shows it
                var dotString = GenerateNetworkGraph(model);
                // Save it to a temp folder 
                string tempDotPath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".dot";
                string tempImagePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
                File.WriteAllText(tempDotPath, dotString);

                try
                {
                    //execute the proces
                    using (Process graphVizprocess = new Process())
                    {
                        graphVizprocess.StartInfo.FileName = "dot.exe";
                        graphVizprocess.StartInfo.Arguments = "-Tpng " + tempDotPath + " -o " + tempImagePath;
                        graphVizprocess.Start();
                        graphVizprocess.WaitForExit();
                    }

                }
                catch (Exception)
                {
                    var exx = new Exception("Seems Graphviz is not installed and registered in system variable.");
                    throw exx;
                }

                try
                {
                    ProcessStartInfo Info = new ProcessStartInfo()
                    {
                        FileName = "mspaint.exe",
                        //WindowStyle = ProcessWindowStyle.Maximized,
                        Arguments = tempImagePath
                    };
                    Process.Start(Info);
                }
                catch (Exception)
                {
                    ProcessStartInfo Info = new ProcessStartInfo()
                    {
                        FileName = "mspaint.exe",
                        //WindowStyle = ProcessWindowStyle.Maximized,
                        Arguments = tempImagePath
                    };
                    Process.Start(Info);
                }


            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}

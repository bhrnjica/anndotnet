using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core;
using Xunit;

namespace anndotnet.unit
{

    public class ExportResultTests
    {

        [Fact]
        public void export_to_csv_regression_test01()
        {
            DeviceDescriptor device = DeviceDescriptor.UseDefaultDevice();
            var strPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\ml_config_file_test.txt";

            //
            var dicMParameters = MLFactory.LoadMLConfiguration(strPath);
            Assert.Equal(11, dicMParameters.Count);


        }   
    }
}

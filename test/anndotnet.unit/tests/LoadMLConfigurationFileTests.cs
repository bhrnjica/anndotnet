using ANNdotNET.Core;
using System;
using System.IO;
using Xunit;

namespace anndotnet.unit
{

    public class LoadMLConfigurationFileTests
    {
        [Fact]
        public void loadMLConfigFile_Test01()
        {
            var strPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\ml_config_file_test.txt";

           //
            var dicMParameters = MLFactory.LoadMLConfiguration(strPath);
            Assert.Equal(11, dicMParameters.Count);
            
            //List of ml config keywords
            Assert.True(dicMParameters.ContainsKey("features"));
            Assert.True(dicMParameters.ContainsKey("labels"));

            Assert.True(dicMParameters.ContainsKey("network"));
            Assert.True(dicMParameters.ContainsKey("learning"));
            Assert.True(dicMParameters.ContainsKey("training"));

            Assert.True(dicMParameters.ContainsKey("trainingPath"));
            Assert.True(dicMParameters.ContainsKey("validationPath"));
            Assert.True(dicMParameters.ContainsKey("testPath"));

            Assert.True(dicMParameters.ContainsKey("modelTempFile"));
            Assert.True(dicMParameters.ContainsKey("modelFinalFile"));
            Assert.True(dicMParameters.ContainsKey("modelEvalResult"));


        }

        [Fact]
       // [ExpectedException(typeof(FileNotFoundException))]
        public void loadMLConfigFile_Test02()
        {
            try
            {
            
                var strPath = "C:\\sc\\github\\anndotnet\\test\\anndotnet.unit\\data\\xcxml_config_file_test.txt";
                //var strPath = "../../../../data/xcxml_config_file_test.txt";

                //
                var dicMParameters = MLFactory.LoadMLConfiguration(strPath);
            }
            catch (Exception ex)
            {

                Assert.StartsWith("Could not find file", ex.Message);
            }
           
            
        }


    }
}

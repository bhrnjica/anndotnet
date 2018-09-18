using ANNdotNET.Core;
using System;
using Xunit;

namespace anndotnet.unit
{

    public class StreamConfigurationTests
    {
        [Fact]
       
        public void createStreamConfiguration_test01_exception()
        {

            try
            {
                //one feature variable with 3 dimensions
                //one label variable with 1 dimensions
                var strLines = new string[2]
                {
                "|feature 3",
                "|label 1 0 0"
                };

                MLFactory f = new MLFactory();
                f.CreateIOVariables(strLines[0], strLines[1], CNTK.DataType.Float);
            }
            catch (Exception ex)
            {
                Assert.Equal("One of variables were not formatted properly!", ex.Message);
            }    
        }

        [Fact]
      //  [ExpectedException(typeof(Exception), "One of variable was not formatted properly!")]
        public void createStreamConfiguration_test02_exception()
        {
            try
            {
                //one feature variable with 3 dimensions
                //one label variable with 1 dimensions
                var strLines = new string[2]
                {
                "|feature 3 0 0",
                "|label  0"
                };

                MLFactory f = new MLFactory();
                f.CreateIOVariables(strLines[0], strLines[1], CNTK.DataType.Float);
            }
            catch (Exception ex)
            {
                Assert.Equal("One of variables were not formatted properly!", ex.Message);
            }
        }

        [Fact]
        public void createStreamConfiguration_test02()
        {
            //one feature variable with 5 dimensions
            //one label variable with 3 dimensions which represent one-hot encoding
            var strLines = new string[2]
            {
                "|feature 3 0",
                "|label 3 0"
            };

            MLFactory f = new MLFactory();
            //setup stream configuration
            f.CreateIOVariables(strLines[0], strLines[1], CNTK.DataType.Float);

            //
            Assert.Equal(2, f.StreamConfigurations.Count);
            //first feature
            Assert.Equal("feature", f.StreamConfigurations[0].m_streamName);
            Assert.Equal(f.StreamConfigurations[0].m_dim, (uint)3);
            Assert.False(f.StreamConfigurations[0].m_isSparse);

            //second label
            Assert.Equal("label", f.StreamConfigurations[1].m_streamName);
            Assert.Equal(f.StreamConfigurations[1].m_dim, (uint)3);
            Assert.False(f.StreamConfigurations[1].m_isSparse);

        }


        [Fact]
        public void CreateStreamConfiguration_test03()
        {
            //3 feature variables with different dimensions
            //one label variable with 1 dimensions
            var strLines = new string[2]
            {   //name  dimension dynamicAxes isSparse    
                "|year 3 1 |month 12 1 |shop 52 1 |item 5100 1 |cnt_past3m 3 0",
                "|label 1 0"
            };

            MLFactory f = new MLFactory();
            //setup stream configuration
             f.CreateIOVariables(strLines[0], strLines[1], CNTK.DataType.Float);

            //
            Assert.Equal(6, f.StreamConfigurations.Count);
            //first feature
            Assert.Equal("year",f.StreamConfigurations[0].m_streamName);
            Assert.Equal(f.StreamConfigurations[0].m_dim, (uint)3);
            Assert.True(f.StreamConfigurations[0].m_isSparse);

            //second feature
            Assert.Equal("month", f.StreamConfigurations[1].m_streamName);
            Assert.Equal(f.StreamConfigurations[1].m_dim, (uint)12);
            Assert.True(f.StreamConfigurations[1].m_isSparse);

            //third feature
            Assert.Equal("shop", f.StreamConfigurations[2].m_streamName);
            Assert.Equal(f.StreamConfigurations[2].m_dim, (uint)52);
            Assert.True(f.StreamConfigurations[2].m_isSparse);

            //fourth feature
            Assert.Equal("item", f.StreamConfigurations[3].m_streamName);
            Assert.Equal(f.StreamConfigurations[3].m_dim, (uint)5100);
            Assert.True(f.StreamConfigurations[3].m_isSparse);

            //fifth feature
            Assert.Equal("cnt_past3m", f.StreamConfigurations[4].m_streamName);
            Assert.Equal(f.StreamConfigurations[4].m_dim, (uint)3);
            Assert.False(f.StreamConfigurations[4].m_isSparse);

            //first label
            Assert.Equal("label", f.StreamConfigurations[5].m_streamName);
            Assert.Equal(f.StreamConfigurations[5].m_dim, (uint)1);
            Assert.False(f.StreamConfigurations[5].m_isSparse);

        }
    }
}

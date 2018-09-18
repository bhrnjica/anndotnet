using System;
using System.Linq;
using ANNdotNET.Core;
using CNTK;
using NNetwork.Core;
using Xunit;

namespace anndotnet.unit
{

    public class InputOutputDefinitionTests
    {
       
        [Fact]
        public void createStreamConfiguration_test03()
        {
            //stream configuration to distinct features and labels in the file
            StreamConfiguration[] streamConfig = new StreamConfiguration[]
               {
                   new StreamConfiguration("year", 3,true),
                   new StreamConfiguration("month", 12,true),
                   new StreamConfiguration("shop", 56, true),
                   new StreamConfiguration("item", 5100,true),
                   new StreamConfiguration("cnt_past3m", 3),
                   new StreamConfiguration("item_cnt_month", 1)
               };
            string strFeature = "|year 3 1 |month 12 1 |shop 56 1 |item 5100 1 |cnt_past3m 3 0";
            string strLabels = "|item_cnt_month 1 0";

            MLFactory f = new MLFactory();
          
            //setup stream configuration
            f.CreateIOVariables(strFeature,strLabels,DataType.Float);

            //
            Assert.Equal(5, f.InputVariables.Count);
            Assert.Single(f.OutputVariables);
            //first feature
            Assert.Equal("year", f.InputVariables[0].Name);
            Assert.Equal(3, f.InputVariables[0].Shape.Dimensions[0]);
            Assert.True(f.InputVariables[0].IsSparse);

            //second feature
            Assert.Equal("month", f.InputVariables[1].Name);
            Assert.Equal(12, f.InputVariables[1].Shape.Dimensions[0]);
            Assert.True(f.InputVariables[1].IsSparse);

            //third feature
            Assert.Equal("shop", f.InputVariables[2].Name);
            Assert.Equal(56, f.InputVariables[2].Shape.Dimensions[0]);
            Assert.True(f.InputVariables[2].IsSparse);

            //fourth feature
            Assert.Equal("item", f.InputVariables[3].Name);
            Assert.Equal(5100, f.InputVariables[3].Shape.Dimensions[0]);
            Assert.True(f.InputVariables[3].IsSparse);

            //fifth feature
            Assert.Equal("cnt_past3m", f.InputVariables[4].Name);
            Assert.Equal(3, f.InputVariables[4].Shape.Dimensions[0]);
            Assert.False(f.InputVariables[4].IsSparse);

            //first label
            Assert.Equal("item_cnt_month", f.OutputVariables[0].Name);
            Assert.Equal(1, f.OutputVariables[0].Shape.Dimensions[0]);
            Assert.False(f.OutputVariables[0].IsSparse);

        }


      
    }
}

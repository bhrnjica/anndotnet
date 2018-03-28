using ExcelDna.Integration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;
using System.IO;
using System.Windows.Forms;

namespace ANNdotNET.ExcelAddIn
{
    public class ANNdotNETModel
    {
        //Example of usage in Excel: =ANNdotNETEval(A1:A34, modelPath)
        [ExcelFunction(Description = "ANNdotNETEval - ANNdotNET model.")]
        public static object ANNdotNETEval(object arg, string modelPath)
        {
            try
            {
                //First convert object in to array
                object[,] obj = (object[,])arg;
                
                //create list to convert values
                List<float> calculatedOutput = new List<float>();
                
                //
                foreach (var s in obj)
                {
                    var ss = float.Parse(s.ToString(), CultureInfo.InvariantCulture);
                    calculatedOutput.Add(ss);
                }
                //
                return EvaluateModel(calculatedOutput.ToArray(), modelPath);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }

        }
        private static float EvaluateModel(float[] vector, string modelPath)
        {
            //
            FileInfo fi = new FileInfo(modelPath);
            if (!fi.Exists)
            {
                throw new Exception($"The '{fi.FullName}' does not exist. Make sure the model is places at this location.");
            }

            //load the model from disk
            var ffnn_model = Function.Load(fi.FullName, DeviceDescriptor.CPUDevice);

            //extract features and label from the model
            Variable feature = ffnn_model.Arguments[0];
            Variable label = ffnn_model.Output;

            Value xValues = Value.CreateBatch<float>(new int[] { feature.Shape[0] }, vector, DeviceDescriptor.CPUDevice);
            //Value yValues = - we don't need it, because we are going to calculate it

            //map the variables and values
            var inputDataMap = new Dictionary<Variable, Value>();
            inputDataMap.Add(feature, xValues);
            var outputDataMap = new Dictionary<Variable, Value>();
            outputDataMap.Add(label, null);

            //evaluate the model
            ffnn_model.Evaluate(inputDataMap, outputDataMap, DeviceDescriptor.CPUDevice);
            //extract the result  as one hot vector
            var outputData = outputDataMap[label].GetDenseData<float>(label);

            //parse output and return appropriate value based on problem type
            if (outputData[0].Count > 2)
            {
                float lValue = outputData[0].IndexOf(outputData[0].Max());
                return lValue;
            }
            else if (outputData[0].Count == 2)
            {
                float lValue = outputData[0].IndexOf(outputData[0].Max());
                return lValue;
            }
            else
            {
                var lValue = outputData[0].First();
                return lValue;
            }
        }

    }
}

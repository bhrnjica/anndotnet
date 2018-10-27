using NNetwork.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CNTK;
using System.IO;
using System.Globalization;
using System.Drawing;
using GPdotNet.MathStuff;
using System.Threading.Tasks;

namespace ANNdotNET.Core
{
    public class MLExport
    {
       
        public static async Task<string> PrintPerformance(string mlconfigPath, DeviceDescriptor device)
        {
            try
            {
                var er = await MLEvaluator.EvaluateMLConfig(mlconfigPath, device, DataSetType.Testing ,EvaluationType.Results);

                if (er.Actual == null)
                    throw new Exception("Export has failed. No testing nor validation datatset to export.");

                //
                List<string> strLine = new List<string>();

                //include label categorical values in the export
                if (er.OutputClasses != null && er.OutputClasses.Count > 1)
                {
                    var ss = "!#OutputClasses(";
                    for (int i = 0; i < er.OutputClasses.Count; i++)
                    {
                        ss += $"[{i}={er.OutputClasses[i]}],";
                    }
                    var outputClassesStr = ss.Substring(0, ss.Length - 1) + ")";
                    strLine.Add(outputClassesStr);
                }
                //make header
                var headerStr = string.Join(";", er.Header);
                strLine.Add(headerStr);

                //prepare for saving
                for (int i = 0; i < er.Actual.Count; i++)
                    strLine.Add($"{er.Actual[i].ToString(CultureInfo.InvariantCulture)};{er.Predicted[i].ToString(CultureInfo.InvariantCulture)}");


                return "";
            }
            catch (Exception)
            {

                throw;
            }
        }
        public static async Task<bool> ExportToCSV(string mlconfigPath, DeviceDescriptor device, string filePathExport)
        {
            try
            {
                var er = await MLEvaluator.EvaluateMLConfig(mlconfigPath, device, DataSetType.Testing, EvaluationType.Results);

                if (er.Actual == null)
                    throw new Exception("Export has failed. No testing nor validation datatset to export.");

                //
                List<string> strLine = new List<string>();

                //include label categorical values in the export
                if (er.OutputClasses != null && er.OutputClasses.Count > 1)
                {
                    var ss = "!#OutputClasses(";
                    for (int i = 0; i < er.OutputClasses.Count; i++)
                    {
                        ss += $"[{i}={er.OutputClasses[i]}],";
                    }
                    var outputClassesStr = ss.Substring(0, ss.Length - 1) + ")";
                    strLine.Add(outputClassesStr);
                }
                //make header
                var headerStr = string.Join(";", er.Header);
                strLine.Add(headerStr);

                //prepare for saving
                for (int i = 0; i < er.Actual.Count; i++)
                    strLine.Add($"{er.Actual[i].ToString(CultureInfo.InvariantCulture)};{er.Predicted[i].ToString(CultureInfo.InvariantCulture)}");

                //store content to file
                //
                await Task.Run(()=> File.WriteAllLines(filePathExport, strLine.ToArray()));
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static Dictionary<string, List<object>> TransformResult(List<List<float>> ActualT, List<List<float>> PredictedT,
          List<List<float>> ActualV, List<List<float>> PredictedV)
        {

            var dic = new Dictionary<string, List<object>>();
            //get output for training data set
            List<double> actualT = new List<double>();
            List<double> predictedT = new List<double>();
            List<double> actualV = new List<double>();
            List<double> predictedV = new List<double>();
            //
            if (ActualT != null && ActualT.Count > 0)
            {


                //category output
                for (int i = 0; i < ActualT.Count; i++)
                {
                    float act = 0;
                    float pred = 0;
                    //category output
                    if (ActualT[i].Count > 2)
                    {
                        act = ActualT[i].IndexOf(ActualT[i].Max());
                        pred = PredictedT[i].IndexOf(PredictedT[i].Max());
                    }
                    else if (ActualT[i].Count == 2)
                    {
                        act = ActualT[i].IndexOf(ActualT[i].Max());
                        pred = PredictedT[i][1];
                    }
                    else
                    {
                        act = ActualT[i].First();
                        pred = PredictedT[i].First();
                    }


                    actualT.Add(act);
                    predictedT.Add(pred);
                }
            }

            //
            if (ActualV != null && ActualV.Count > 0)
            {


                //category output
                for (int i = 0; i < ActualV.Count; i++)
                {
                    float act = 0;
                    float pred = 0;
                    //category output
                    if (ActualV[i].Count > 2)
                    {
                        act = ActualV[i].IndexOf(ActualV[i].Max());
                        pred = PredictedV[i].IndexOf(PredictedV[i].Max());
                    }
                    else if (ActualV[i].Count == 2)
                    {
                        act = ActualV[i].IndexOf(ActualT[i].Max());
                        pred = (1.0f - PredictedV[i][0]);
                    }
                    else
                    {
                        act = ActualV[i].First();
                        pred = PredictedV[i].First();
                    }


                    actualV.Add(act);
                    predictedV.Add(pred);
                }
            }
           //add train data
            if (actualT != null)
            {
                //add data sets
                dic.Add("obs_train", actualT.Select(x => (object)x).ToList<object>());
                dic.Add("prd_train", predictedT.Select(x => (object)x).ToList<object>());
            }

            //add validation dataset
            if (actualV != null)
            {
                dic.Add("obs_test", actualV.Select(x => (object)x).ToList<object>());
                dic.Add("prd_test", predictedV.Select(x => (object)x).ToList<object>());
            }

            return dic;

        }
    }
}

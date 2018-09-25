using CNTK;
using System;
using System.Collections.Generic;
using System.Text;

namespace ANNdotNET.Core
{
    /// <summary>
    /// The class implements data returned when the model is evaluated 
    /// </summary>
    public class EvaluationResult
    {
        public List<string> Header { get; set; }
        public List<string> OutputClasses { get; set; }

        public Dictionary<string, List<List<float>>> DataSet { get; set; }

        public List<float> Actual { get; set; }
        public List<float> Predicted { get; set; }
        public List<List<float>> ActualEx { get; set; }
        public List<List<float>> PredictedEx { get; set; }
    }

    public class EvaluationParameters
    {
        //public string DataFilePath { get; set; }
        public uint MinibatchSize { get; set; }
        public MinibatchSourceEx MBSource { get; set; }
        public List<Variable> Input { get; set; }
        public List<Variable> Ouptut { get; set; }

        //  public StreamConfiguration[] StrmsConfig { get; set; }
    }
}
    
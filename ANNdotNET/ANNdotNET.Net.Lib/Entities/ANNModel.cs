using CNTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ANNdotNET.Net.Lib;
using ANNdotNET.Export.Lib;

namespace ANNdotNET.Net.Lib.Entities
{
    /// <summary>
    /// Implements model class which consists of the properties needed to training CNTK model.
    /// </summary>
    public class ANNModel
    {
        public bool IsRunnig { get; internal set; }
        //guid value of the model
        public string Guid { get; internal set; }
        //project holds the model
        public ANNProject Parent { get; set; }
        //project name
        public string Name { get; internal set; }
        //path of the file containing training and validation dataset
        public string DataFileName { get; internal set; }
        //Model data needed for Network definition
        public ActiveModelData ModelData { get; set; }

        //Loss values through training iteration 
        public List<float> LossData { get; set; }
        //Evaluation values through training iteration
        public List<float> EvalData { get; set; }
        //Actual values of training data set
        public List<List<float>> ActualT { get; set; }
        //predicted values of training data set
        public List<List<float>> PredictedT { get; set; }
        //number of training dataset samples
        public uint TrainCount { get; internal set; }
        //actual values of validation dataset
        public List<List<float>> ActualV { get; set; }
        //predicted values of validation dataset
        public List<List<float>> PredictedV { get; set; }
        //number of validation dataset samples
        public uint TestCount { get; internal set; }
        //randomized training data set before training
        public bool Randomize { get; internal set; }
        //number of features (including decoded)
        public int InputDim { get; internal set; }
        //number of output labels
        public int OutputDim { get; internal set; }
        
        public string Label { get; internal set; }
        //Class names in case of categorical model
        public List<string> Classes { get; internal set; }

        public List<List<float>> TrainInput { get; internal set; }
        public List<List<float>> TestInput { get; internal set; }
        //CNTK model 
        public Function CNTKModel { get; set; }
        public Trainer Trainer { get; set; }

        public ANNModel()
        {
            ActualV = new List<List<float>>();
            ActualT = new List<List<float>>();
            PredictedV = new List<List<float>>();
            PredictedT = new List<List<float>>();
            TrainInput = new List<List<float>>();
            TestInput = new List<List<float>>();
            LossData = new List<float>();
            EvalData = new List<float>();
        }
        internal void Run(ActiveModelData setData, Action<int, float, float, (List<List<float>>, List<List<float>>, List<List<float>>), (List<List<float>>, List<List<float>>, List<List<float>>)> report, CancellationToken token)
        {
            try
            {
                ModelData = new ActiveModelData(setData);
                Factory fac = new Factory();
                fac.Iterations = (uint) setData.IterValue;
                fac.Init(this, DataFileName);

                fac.report = report;

                //run training
                Task.Run(() =>
                fac.Train(token)
                );
            }
            catch (Exception)
            {

                throw;
            }
        }

        internal void Close()
        {
            
        }

        
        public Dictionary<string, List<object>> EvaluateResults()
        {
            var dic = new Dictionary<string, List<object>>();
            //
            if (ActualT != null && ActualT.Count > 0)
            {
                //get output for training data set
                List<double> actualT = new List<double>();
                List<double> predictedT = new List<double>();
                List<double> actualV = new List<double>();
                List<double> predictedV = new List<double>();
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
                        pred = (1.0f - PredictedT[i][0]);
                    }
                    else
                    {
                        act = ActualT[i].First();
                        pred = PredictedT[i].First();
                    }


                    actualT.Add(act);
                    predictedT.Add(pred);
                }

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

                //
                if (OutputDim > 1)
                    dic.Add("Classes", Classes.ToList<object>());


                //add data sets
                dic.Add("obs_train", actualT.Select(x => (object)x).ToList<object>());
                dic.Add("prd_train", predictedT.Select(x => (object)x).ToList<object>());
                //add test dataset
                if (actualV != null)
                {
                    dic.Add("obs_test", actualV.Select(x => (object)x).ToList<object>());
                    dic.Add("prd_test", predictedV.Select(x => (object)x).ToList<object>());
                }

                return dic;

            }

            return null;
        }

        public void ExportCNTK(string filepath)
        {
            try
            {
                CNTKModel.Save(filepath);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        public void ExportONNX(string filepath)
        {
            try
            {
                CNTKModel.Save(filepath);
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Saving current state of the trainer. This is used when training process is continued 
        /// </summary>
        /// <param name="trainer"></param>
        /// <param name="model"></param>
        private void saveSnapShot(Trainer trainer, Function model)
        {
            ////save checkpoint for the model
            //var tempPath = Path.GetTempPath();
            //var tempFile = Path.GetRandomFileName();

            //var tempItem = Path.Combine(tempPath, tempFile);
            //trainer.SaveCheckpoint(tempItem);
            //var newPath = Path.Combine(tempPath, DateTime.Now.Ticks.ToString());//, tempFile);
            //Directory.CreateDirectory(newPath);
            //var newPathItem = Path.Combine(newPath, tempFile);
            //File.Move(tempItem, newPathItem);
            //File.Move(tempItem + ".ckp", newPathItem + ".ckp");
            //savedModelChechState = newPathItem;

            Trainer = trainer;
            CNTKModel = model;
        }

        public void ExportToE(string filepath)
        {
            if (TrainInput == null || TrainInput.Count == 0)
                return;
            try
            {
                var train = transformData(TrainInput, ActualT);
                var test = transformData(TestInput, ActualV);

                ExportToExcel.Export(train, test, filepath, "ANNdotNETEval({0}:{1})");
                
                //save cntk model in document folder
                var docFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var strPath = Path.Combine(docFolderPath, "/annmodel/anndotnet-cntk-model.model");
                ExportCNTK(strPath);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        float[][] transformData(List<List<float>> input, List<List<float>> label)
        {
            if (input != null && input.Count > 0)
            {
                //get output for training data set
                var trData = new List<float[]>();

                //category output
                for (int i = 0; i < input.Count; i++)
                {
                    var row = input[i].ToList();
                   
                    //extract label
                    //category output
                    if (ActualT[i].Count > 2)
                    {
                       var lValue= ActualT[i].IndexOf(ActualT[i].Max());
                        row.Add(lValue);
                    }
                    else if (ActualT[i].Count == 2)
                    {
                        var lValue = ActualT[i].IndexOf(ActualT[i].Max());
                        row.Add(lValue);
                    }
                    else
                    {
                        var lValue = ActualT[i].First();
                        row.Add(lValue);
                    }
                    //
                    trData.Add(row.ToArray());
                }

                //
                return trData.ToArray();

            }
            return null;
        }
    }
}
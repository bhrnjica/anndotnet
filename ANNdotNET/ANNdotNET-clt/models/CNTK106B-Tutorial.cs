using ANNdotNET.Net.Lib;
using CNTK;
using libSVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnndotNET.Net.Lib.Util;
namespace ANNdotNET_clt.models
{
    public class CNTK106B_Tutorial
    {
        static string filePath = @"../../../data/solar.csv";
        // set fast mode to true
        static bool isFast = true;

        // Training parameters
        //static int TRAINING_SIZE = 8000;
        //static int VALIDATION_SIZE = 1000;
        //static int TESTING_SIZE = 1000;
        //static int BATCH_SIZE = 100;
        static int EPOCHS = isFast ? 10 : 100;

        // We keep upto 14 inputs from a day
        static int TIMESTEPS = 14;

        // Specify the internal-state dimensions of the LSTM cell
        static int H_DIMS = 15;
        static float DROPRATRE = 0.2f;

        //# 20000 is the maximum total output in our dataset. We normalize all values with
        //# this so our inputs are between 0.0 and 1.0 range.
        static int NORMALIZE = 20000;

        // process batches of 10 days
        static int BATCH_SIZE = TIMESTEPS * 10;

        //input dimension
        static int IN = 1;

        //output dimension
        static int OUT = 1;


        /// <summary>
        /// Build and train a RNN model.
        /// </summary>
        /// <param name="device">CPU or GPU device to train and run the model</param>
        public static void Train(DeviceDescriptor device)
        {
            
           
            //load data in to memory
            //var xdata = Numpy.LinSpace(0, 100.0, 10000).Select(x => (float)x).ToArray<float>();
            (Dictionary<string, List<List<float>>> X, Dictionary<string, List<List<float>>> Y) = load_Solar_Dataset(filePath, NORMALIZE,14,0.1f,0.1f);

           


            ////split dataset on train, validate and test parts
            //var featureSet = ds.Item1["train"];
            //var labelSet = ds["label"];

            // Create input variable with dynamic size 
            var featuresName = "feature";
            var feature = Variable.InputVariable(new int[] { IN }, DataType.Float, featuresName, null, false /*isSparse*/);

            //out dim is also 1
            var labelsName = "label";
            var label = Variable.InputVariable(new int[] { OUT }, DataType.Float, labelsName, new List<Axis>() { Axis.DefaultBatchAxis() }, false /*isSparse*/);
            

            //create model
            var z = createModel(feature, device);
            
            var trainingLoss = CNTKLib.SquaredError(z, label, "squarederrorLoss");
            var prediction = CNTKLib.SquaredError(z, label, "squarederrorEval");
            var modelParams = z.Parameters();


            //learning rate
            double learning_rate = 0.05;
            var learning_schedule = new TrainingParameterScheduleDouble(learning_rate);

            //momentum schedule
            var momentum_schedule = CNTKLib.MomentumAsTimeConstantSchedule(0.9);
            var zParams = new ParameterVector(z.Parameters().ToList());

            //Create adam optimizer
            IList<Learner> parameterLearners = new List<Learner>() {
                CNTKLib.FSAdaGradLearner(zParams, learning_schedule, momentum_schedule)  };

            ////create trainer
            var trainer = Trainer.CreateTrainer(z, trainingLoss, prediction, parameterLearners);

            //training
            //loss_summary = []

            // train the model
            int outputFrequencyInMinibatches = 5;
            int it = 0;
            for (int i = 1; i <= EPOCHS; i++)
            {
                //get the next minibatch amount of data
                foreach (var miniBatchData in nextBatch(X, Y, "train"))
                {
                    var xValues = Value.CreateBatchOfSequences<float>(new NDShape(1, IN), miniBatchData.faetures, device);
                    var yValues = Value.CreateBatchOfSequences<float>(new NDShape(1, OUT), miniBatchData.label, device);

                    ////Combine variables and data in to Dictionary for the training
                    var batchData = new Dictionary<Variable, Value>();
                    batchData.Add(feature, xValues);
                    batchData.Add(label, yValues);

                    //train minibarch data
                    trainer.TrainMinibatch(batchData, false, device);
                }

                //output training process
                PrintTrainingProgress(trainer, i, outputFrequencyInMinibatches);
            }
        }

        

        private static Function createModel(Variable feature, DeviceDescriptor device)
        {
            int cellDim = H_DIMS;
            //create rnn object
            var lstmNN = new LSTMReccurentNN(H_DIMS, cellDim, device);

            //First create embedded layer, which defines number of input and the number of embedding layers 
            var embededL = lstmNN.Embedding(feature, H_DIMS);

            //create recurrence
            var lastLayer = lstmNN.CreateRecurrence(embededL, OUT, "timeSeriesOutput");
            var dropoutLay = CNTKLib.Dropout(lastLayer, DROPRATRE);
            var outLayer = lstmNN.CreateDenseLayer(dropoutLay, 1);
            return outLayer;
        }

        public static void PrintTrainingProgress(Trainer trainer, int minibatchIdx, int outputFrequencyInMinibatches)
        {
            if ((minibatchIdx % outputFrequencyInMinibatches) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
            {
                float trainLossValue = (float)trainer.PreviousMinibatchLossAverage();
                float evaluationValue = (float)trainer.PreviousMinibatchEvaluationAverage();
                Console.WriteLine($"Minibatch: {minibatchIdx} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
            }

        }

        private static IEnumerable<(List<List<float>> faetures, List<List<float>> label)> nextBatch(Dictionary<string, List<List<float>>> xData, Dictionary<string, List<List<float>>> yData, string dataset)
        {
            List<List<float>> asBatch(List<List<float>> data, int start, int count)
            {
                var lst = new List<List<float>>();
                for (int i = start; i < start + count; i++)
                {
                    if (i >= data.Count)
                        break;

                    lst.Add(data[i]);
                }
                return lst;
            }

            List<List<float>> XX = xData[dataset];
            var YY = yData[dataset];


            for (int i = 0; i < XX.Count - BATCH_SIZE; i += BATCH_SIZE)
                yield return (asBatch(XX, i, BATCH_SIZE), asBatch(YY, i, BATCH_SIZE));
        }
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fun"></param>
        /// <param name="x0"></param>
        /// <param name="timeSteps"></param>
        /// <param name="timeShift"></param>
        /// <returns></returns>
        static (Dictionary<string, List<List<float>>>, Dictionary<string, List<List<float>>>) 
            load_Solar_Dataset(string filePath,float normalize , int time_steps, float val_size= 0.1f, float test_size = 0.1f, char delimiter = ',')
        {

            // 1. extract the data from the file
            var str = File.ReadAllLines(filePath);
            var data = new List<List<object>>();
            var header = new List<string>();
            foreach (var l in str)
            {
                //header of dataset
                if (l.StartsWith("!"))
                {
                    header = new List<string>(l.Split(delimiter));
                    continue;
                }
                var row = new List<object>();
                var strRow = l.Split(delimiter);
                Debug.Assert(strRow.Length ==3);

                //skip first line it is header
                for (int i=0; i < strRow.Length; i++)
                {
                    var c = strRow[i];

                    //first row we expect date
                    if(i==0)
                    {
                        var dt = c.ParseDateTime("yyyy-MM-dd HH:mm:ss");
                        row.Add(dt);
                    }
                    else if(float.TryParse(c,NumberStyles.Any, CultureInfo.InvariantCulture, out float val))
                    {
                        row.Add(val);
                    }
                    else
                    {
                        //this should not happen
                        row.Add(0);
                    }
                }
                
                data.Add(row);
            }

            // 2. find max value for each day and merge into the current dataset
            // add new columns into current ds
            header.Add("solar.current.max");
            header.Add("solar.total.max");
            header.Add("date");

            // Normalize the data values and add new columns
            //
            foreach (var row in data)
            {
                //get date value only
                var d = (DateTime)row[0];
                var date = new DateTime(d.Year, d.Month, d.Day);
                //
                float cv = Convert.ToSingle(row[1]);
                var ct = Convert.ToSingle(row[2]);
                row[1] = cv / normalize;
                row[2] = ct / normalize;
                //put initial values to aggregated columns
                row.Add(0);
                row.Add(0);
                row.Add(date);
            }

            //calculate values for new columns
            foreach(var group in data.GroupBy(x=>x[5]))
            {
                var maxCurrent = group.Max(x=>x[1]);
                var maxTotal = group.Max(x => x[2]);
                foreach(var row in group)
                {
                    row[3] = maxCurrent;
                    row[4] = maxTotal;
                }
            }

            //declare datasets
            var result_x = new Dictionary<string, List<List<float>>>();
            result_x.Add("train", null);
            result_x.Add("val", null);
            result_x.Add("test", null);

            //label
            var result_y = new Dictionary<string, List<List<float>>>();
            result_y.Add("train", null);
            result_y.Add("val", null);
            result_y.Add("test", null);

            // split the dataset into train, validatation and test sets on day boundaries
            float groupCount = data.GroupBy(x => x[5]).Count();
            val_size = (int)(groupCount * val_size);
            test_size = (int)(groupCount * test_size);
            float next_val = 0;
            float next_test = 0;

            // generate sequences a day at a time
            int j = 0; //counter
            foreach (var day in data.GroupBy(x => x[5]))
            {
                //2 - "solar.total" column
                //4- solar.total.max
                var total = day.Select(x=>(float)x[2]).ToList();
                var max_total_for_day = day.Select(x => (float)x[4]).ToList();
                //
                var current_set = "train";
                //if we have less than 8 data points for a day we skip over the
                // day assuming something is missing in the raw data
                if (day.Count() < 8)
                    continue;

                if (j >= next_val)
                {
                    current_set = "val";
                    next_val = j + (int)(groupCount / val_size);
                }

                else if (j >= next_test)
                {
                    current_set = "test";
                    next_test = j + (int)(groupCount / test_size);
                }
                else
                    current_set = "train";

                //sequence generation
                foreach(var num in Enumerable.Range(2, total.Count))
                {
                    //create current sequences
                    if (result_x[current_set] == null)
                        result_x[current_set] = new List<List<float>>();
                    result_x[current_set].Add(total.Take(num).ToList());

                    if (result_y[current_set] == null)
                        result_y[current_set] = new List<List<float>>();
                    result_y[current_set].Add(new List<float>(max_total_for_day.Take(1)));

                    //
                    if (num >= time_steps)
                        break;

                }
                //increase counter
                j++;
            }

            //return data sets            
            return (result_x,result_y);
        }


    }
}

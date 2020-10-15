using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;
using Anndotnet.Core.Data;
using Anndotnet.Core.Interface;
using Anndotnet.Core.Progress;
using NumSharp;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace Anndotnet.Core.Trainers
{
    public class TVTrainer : ITrainer
    {
        ConfigProto _config;
        Operation _init;
        DataFeed _train;
        DataFeed _valid;
        NDArray X;
        NDArray Y;
        int _percentageSplit;


        public TVTrainer(NDArray x, NDArray y, int percentageSplit = 20, bool shuffle = false)
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true
            };

            X = x;
            Y = y;
            _percentageSplit = percentageSplit;

            initTrainer(1,shuffle);

        }

        private void initTrainer(int seed, bool shuffle = false)
        {
            //

            (_train, _valid) = Split(seed, shuffle);

            // Initialize the variables (i.e. assign their default value)
            _init = tf.global_variables_initializer();
        }

        internal (DataFeed train, DataFeed validation) Split(int seed, bool shuffle = false)
        {
            int testSize = (X.shape[0] * _percentageSplit) / 100;
            int trainSize = X.shape[0] - testSize;

            //generate indexes
            var random = new Random(seed);
            var lst = Enumerable.Range(0, X.shape[0]);
            var trainIds = shuffle ? lst.OrderBy(t => random.Next()).ToArray().Take(trainSize) : lst.Take(trainSize);
            var testIds = lst.Except(trainIds);

            //create ndarrays
            var trArray = np.array(trainIds);
            var teArray = np.array(testIds);
            //
            var trainX = X[trArray];
            var testX = X[teArray];
            var trainY = Y[trArray];
            var testY = Y[teArray];

            return (new DataFeed(trainX, trainY), new DataFeed(testX, testY));

        }

        public bool RunOffline(Tensor x, Tensor y, Learner lr, TrainingParameters tr)
        {
            //check for progress
            if (tr.Progress == null)
            {
                var pt = new ProgressTVTraining();
                tr.Progress = pt.Run;
            }

            using (var sess = tf.Session(_config))
            {

                sess.run(_init);

                // check the accuracy before training
                var (x_input, y_input) = _train.GetFullBatch();

                //report of zero epoch
                var (eval, loss) = sess.run((lr.Evals.First(), lr.Loss), (x, x_input), (y, y_input));
                tr.Progress(new TrainingProgress() {ProgressType= ProgressType.Initialization, Iteration = 0, TrainEval=eval, TrainLoss= loss });

                // training
                foreach (var i in range(1, tr.Epochs))
                {
                    // by sampling some input data (fetching)
                    (x_input, y_input) = _train.GetFullBatch();

                    //train and back propagate error
                    sess.run(lr.Optimizer, (x, x_input), (y, y_input));

                    // We regularly check the loss
                    if (i % tr.ProgressStep == 0)
                    {
                        var (x_inputV, y_inputV) = _valid.GetFullBatch();
                        //
                        var (TEval, TLoss) = sess.run((lr.Evals.First(), lr.Loss), (x, x_input), (y, y_input));
                        var (VEval, VLoss) = sess.run((lr.Evals.First(), lr.Loss), (x, x_inputV), (y, y_inputV));

                        //report progress
                        tr.Progress(new TrainingProgress() 
                            { ProgressType = ProgressType.Training, Iteration = i, 
                                TrainEval = TEval, TrainLoss = TLoss, ValidEval = VEval, ValidLoss = VLoss
                        });
                    }
                        
                }

                // Finally, we check our final accuracy
                var tranData = _train.GetFullBatch();
                var (TEvala, TLossa) = sess.run((lr.Evals.First(), lr.Loss), (x, tranData.xBatch), (y, tranData.yBatch));

                //Evaluate validation set
                var validData = _valid.GetFullBatch();
                var (VEvala, VLossa) = sess.run((lr.Evals.First(), lr.Loss), (x, validData.xBatch), (y, validData.yBatch));

                //report progress
                tr.Progress(new TrainingProgress() 
                        { 
                            ProgressType = ProgressType.Completed, 
                            Iteration = tr.Epochs, TrainEval = TEvala, TrainLoss = TLossa,
                                                  ValidEval = VEvala,ValidLoss = VLossa
                });
            }

            return true;
        }

        public bool Run(Tensor x, Tensor y, Learner lr, TrainingParameters tr)
        {
            //check for progress
            if (tr.Progress == null)
            {
                var pt = new ProgressTVTraining();
                tr.Progress = pt.Run;
            }

            //
            using (var sess = tf.Session())
            {
                // Run the initializer
                sess.run(_init);

                int batchCount = 0;
                // Training cycle
                foreach (var e in range(1, tr.Epochs))
                {

                    // Loop over all batches
                    foreach (var (x_in, y_in) in _train.GetNextBatch(tr.MinibatchSize))
                    {
                        
                        // Run optimization op (backprop)
                        sess.run(lr.Optimizer, (x, x_in),(y, y_in));

                        //batch counting
                        batchCount++;
                    }

                    var (x_input, y_input) = _train.GetFullBatch();
                    var (x_inputV, y_inputV) = _valid.GetFullBatch();

                    var funs = new List<Tensor>();
                    funs.Add(lr.Loss);
                    funs.AddRange(lr.Evals);

                    var results = sess.run(funs.ToArray(), (x, x_input), (y, y_input));
                    //
                    var (TEval, TLoss) = sess.run((lr.Evals.First(), lr.Loss), (x, x_input), (y, y_input));
                    var (VEval, VLoss) = sess.run((lr.Evals.First(), lr.Loss), (x, x_inputV), (y, y_inputV));

                    //report progress
                    tr.Progress(new TrainingProgress()
                    {
                        ProgressType = ProgressType.Training,
                        Iteration = e,
                        TrainEval = TEval,
                        TrainLoss = TLoss,
                        ValidEval = VEval,
                        ValidLoss = VLoss
                    });
                }

                return true;
            }
        }
    }
}

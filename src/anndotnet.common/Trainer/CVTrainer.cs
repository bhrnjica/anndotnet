using NumSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow;
using static Tensorflow.Binding;

namespace AnnDotNET.Common
{
    public class CVTrainer : ITrainer
    {
        ConfigProto _config;
        Operation _init;
        NDArray X;
        NDArray Y;
        (DataFeed train, DataFeed valid)[] _cvData;
        int _kFold;
        public CVTrainer(NDArray x, NDArray y, int kFold=5)
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true
            };

            X = x;
            Y = y;
            _kFold = kFold;
            initTrainer();
        }

        private void initTrainer()
        {
            //
            _cvData = new (DataFeed train, DataFeed valid)[_kFold];
            float percentage = 100.0f / _kFold;
            int testSize = (int)((X.shape[0] * percentage) / 100);
            int trainSize = X.shape[0] - testSize;

            //create folds
            for (int i=0; i<_kFold; i++)
              _cvData[i] = Split(trainSize, testSize, i);

            // Initialize the variables (i.e. assign their default value)
            _init = tf.global_variables_initializer();
        }

        public bool Run(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr)
        {
            //check for progress
            if(tr.Progress==null)
            {
                var pt = new ProgressCVTraining();
                tr.Progress = pt.Run;
            }
            //
            using (var sess = tf.Session())
            {
                // Run the initializer
                sess.run(_init);

                for (int f = 0; f < _cvData.Length; f++)
                {
                    var feed = _cvData[f];

                    // Training cycle
                    foreach (var epoch in range(1, tr.Epochs))
                    {
                        int batchCount = 0;
                        // Loop over all batches
                        foreach (var batch in feed.train.GetNextBatch(tr.MinibatchSize))
                        {
                            var (xTrain, yTrain) = batch;

                            // Run optimization op (backprop)
                            sess.run(lr.Learner, (x, xTrain), (y, yTrain));

                            //batch counting
                            batchCount++;
                        }

                        // Display logs per epoch step
                        if (epoch % tr.ProgressStep == 0)
                        {
                            //get data
                            var (xTrain, yTrain) = feed.train.GetFullBatch();
                            var (xValid, yValid) = feed.valid.GetFullBatch();

                            //report progress
                            reportProgress(sess, x, y, xTrain, yTrain, xValid, yValid, f+1, epoch, lr, tr);
                        }

                    }
                }
            }
            return true;
        }

        public bool RunOffline(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr)
        {
            //check for progress
            if (tr.Progress == null)
            {
                var pt = new ProgressCVTraining();
                tr.Progress = pt.Run;
            }

            using (var sess = tf.Session(_config))
            {
                sess.run(_init);

                //enumerate folds
                for(int f = 0; f <_cvData.Length; f++)
                {
                    var feed = _cvData[f];
                    var (xTrain, yTrain) = feed.train.GetFullBatch();
                    var (xValid, yValid) = feed.valid.GetFullBatch();

                    //report progress
                    reportProgress(sess,x,y,xTrain,yTrain,xValid,yValid, f, 0, lr, tr);

                    // training
                    foreach (var i in range(1, tr.Epochs))
                    {
                        // by sampling some input data (fetching)
                        (xTrain, yTrain) = feed.train.GetFullBatch();
                        
                        //train and back propagate error
                        sess.run(lr.Learner, (x, xTrain), (y, yTrain));

                        // progress about training
                        if (i % tr.ProgressStep == 0)
                        {
                            (xValid, yValid) = feed.valid.GetFullBatch();

                            //report progress
                            reportProgress(sess, x, y, xTrain, yTrain, xValid, yValid, f+1, 0, lr, tr);
                        }

                    }

                    // Finally, we check our final accuracy
                    (xTrain, yTrain) = feed.train.GetFullBatch();
                    (xValid, yValid) = feed.valid.GetFullBatch();
                    //report progress
                    reportProgress(sess, x, y, xTrain, yTrain, xValid, yValid, f+1, -1, lr, tr);
                }

            }

            return true;
        }


        internal (DataFeed train, DataFeed validation) Split(int trainSize, int testSize, int index)
        {
            //generate indexes
            var lst = Enumerable.Range(0, X.shape[0]);
            var trainIds = lst.Skip(index * testSize).Take(trainSize);
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

        private void reportProgress(Session sess,Tensor x, Tensor y, NDArray xTrain, NDArray yTrain, NDArray xValid, NDArray yValid, int f, int i, AnnLearner lr, TrainingParameters tr)
        {

            //
            var (TEval, TLoss) = sess.run((lr.Eval, lr.Loss), (x, xTrain), (y, yTrain));
            var (VEval, VLoss) = sess.run((lr.Eval, lr.Loss), (x, xValid), (y, yValid));

            //report progress
            tr.Progress(new TrainingProgress()
            {
                ProgressType = i==0 ? ProgressType.Initialization: i < 0 ? ProgressType.Completed : ProgressType.Training,
                FoldIndex = f,
                Iteration = i,
                TrainEval = TEval,
                TrainLoss = TLoss,
                ValidEval = VEval,
                ValidLoss = VLoss
            });
        }
    }
}

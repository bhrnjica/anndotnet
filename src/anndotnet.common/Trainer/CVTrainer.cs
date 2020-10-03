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

            return (new DataFeed(trainX, trainY, 1), new DataFeed(testX, testY, 1));
        }
        public bool RunOffline(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr)
        {
            using (var sess = tf.Session(_config))
            {

                sess.run(_init);
                foreach(var feed in _cvData)
                {
                    // check the accuracy before training
                    var (x_input, y_input) = feed.train.GetFullBatch();

                    //report of zero epoch
                    var (eval, loss) = sess.run((lr.Eval, lr.Loss), (x, x_input), (y, y_input));
                    tr.Progress(new TrainingProgress() { ProgressType = ProgressType.Initialization, Iteration = 0, Eval = eval, Loss = loss });

                    // training
                    foreach (var i in range(1, tr.Epoch))
                    {
                        // by sampling some input data (fetching)
                        (x_input, y_input) = feed.train.GetFullBatch();

                        //train and back propagate error
                        sess.run(lr.Learner, (x, x_input), (y, y_input));

                        // We regularly check the loss
                        if (i % tr.ProgressStep == 0)
                        {
                            (eval, loss) = sess.run((lr.Eval, lr.Loss), (x, x_input), (y, y_input));
                            tr.Progress(new TrainingProgress() { ProgressType = ProgressType.Training, Iteration = i, Eval = eval, Loss = loss });
                        }

                    }

                    // Finally, we check our final accuracy
                    (x_input, y_input) = feed.train.GetFullBatch();

                    sess.run(lr.Eval, (x, x_input), (y, y_input));
                    tr.Progress(new TrainingProgress() { ProgressType = ProgressType.Completed, Iteration = tr.Epoch, Eval = eval, Loss = loss });
                }
                
            }

            return true;
        }

        public bool Run(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr)
        {
            //
            using (var sess = tf.Session())
            {
                // Run the initializer
                sess.run(_init);

                foreach (var feed in _cvData)
                {
                    int batchCount = 0;
                    // Training cycle
                    foreach (var epoch in range(1, tr.Epoch))
                    {

                        // Loop over all batches
                        foreach (var batch in feed.train.GetNextBatch())
                        {
                            var (batch_xs, batch_ys) = batch;

                            // Run optimization op (backprop)
                            sess.run(lr.Learner, (x, batch_xs), (y, batch_ys));

                            //batch counting
                            batchCount++;
                        }

                        // Compute average values
                        //calculate loss and evaluation function
                        var fullBatch = feed.train.GetFullBatch();
                        (float eval, float loss) = sess.run((lr.Eval, lr.Loss), (x, fullBatch.xBatch), (y, fullBatch.yBatch));

                        // Display logs per epoch step
                        if (epoch % tr.ProgressStep == 0)
                            tr.Progress(new TrainingProgress() { Iteration = epoch, Eval = eval, Loss = loss });

                    }
                }
            }
            return true;
        }
    }
}

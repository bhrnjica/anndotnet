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
    public class Trainer : ITrainer
    {
        ConfigProto _config;
        Operation _init;
        public Trainer()
        {
            _config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true
            };

            // Initialize the variables (i.e. assign their default value)
             _init = tf.global_variables_initializer();
        }
        public bool TrainOffline(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr, DataFeed dFeed)
        {
            using (var sess = tf.Session(_config))
            {

                sess.run(_init);

                // check the accuracy before training
                var (x_input, y_input) = dFeed.GetFullBatch();

                //report of zero epoch
                var (eval, loss) = sess.run((lr.Eval, lr.Loss), (x, x_input), (y, y_input));
                tr.Progress(new TrainingProgress() {ProgressType= ProgressType.Initialization, Iteration = 0, Eval=eval, Loss= loss });

                // training
                foreach (var i in range(1, tr.Epoch))
                {
                    // by sampling some input data (fetching)
                    (x_input, y_input) = dFeed.GetFullBatch();

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
                (x_input, y_input) = dFeed.GetFullBatch();

                sess.run(lr.Eval, (x, x_input), (y, y_input));
                tr.Progress(new TrainingProgress() { ProgressType = ProgressType.Completed, Iteration = tr.Epoch, Eval = eval, Loss = loss });
            }

            return true;
        }

        public bool Train(Tensor x, Tensor y, AnnLearner lr, TrainingParameters tr, DataFeed dFeed)
        {
            //
            using (var sess = tf.Session())
            {
                // Run the initializer
                sess.run(_init);

                int batchCount = 0;
                // Training cycle
                foreach (var epoch in range(1, tr.Epoch))
                {

                    // Loop over all batches
                    foreach (var batch in dFeed.GetNextBatch())
                    {
                        var (batch_xs, batch_ys) = batch;

                        // Run optimization op (backprop)
                        sess.run(lr.Learner, (x, batch_xs),(y, batch_ys));

                        //batch counting
                        batchCount++;
                    }

                    // Compute average values
                    //calculate loss and evaluation function
                    var fullBatch = dFeed.GetFullBatch();
                    (float eval, float loss) = sess.run((lr.Eval, lr.Loss), (x, fullBatch.xBatch), (y, fullBatch.yBatch));


                    // Display logs per epoch step
                    if (epoch % tr.ProgressStep == 0)
                        tr.Progress(new TrainingProgress() { Iteration = epoch, Eval = eval, Loss = loss });

                }

                return true;
            }
        }
    }
}

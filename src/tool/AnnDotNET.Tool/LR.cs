using System;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;

namespace AnnDotNET.Tool
{
    public class LR
    {
        static NDArray dsX;
        static NDArray dsY;
        static int nSamples;
        static  void PrepareData()
        {
            // Prepare training Data
            dsX = np.array(3.3f, 4.4f, 5.5f, 6.71f, 6.93f, 4.168f, 9.779f, 6.182f, 7.59f, 2.167f, 7.042f, 10.791f, 5.313f, 7.997f, 5.654f, 9.27f, 3.1f);
            dsY = np.array(1.7f, 2.76f, 2.09f, 3.19f, 1.694f, 1.573f, 3.366f, 2.596f, 2.53f, 1.221f, 2.827f, 3.465f, 1.65f, 2.904f, 2.42f, 2.94f, 1.3f);
            nSamples = dsX.shape[0];
        }

        static private (NDArray, NDArray) get_next_batch(NDArray x, NDArray y, int start, int end)
        {
            var x_batch = x[$"{start}:{end}"];
            var y_batch = y[$"{start}:{end}"];
            return (x_batch, y_batch);
        }

        private static (NDArray, NDArray) randomize(NDArray x, NDArray y)
        {
            var perm = np.random.permutation(y.shape[0]);

            np.random.shuffle(perm);
            return (dsX[perm], dsY[perm]);
        }

        public static void Run()
        {
            //define random number under NumPy
            var rng = np.random;
            rng.Seed = 1;
            int display_freq = 5;

            float learning_rate = 01f;
            int batch_size = 10;
            int epochs = 100;

            // Prepare training Data
            PrepareData();
            //var train_X = np.array(3.3f, 4.4f, 5.5f, 6.71f, 6.93f, 4.168f, 9.779f, 6.182f, 7.59f, 2.167f, 7.042f, 10.791f, 5.313f, 7.997f, 5.654f, 9.27f, 3.1f);
            //var train_Y = np.array(1.7f, 2.76f, 2.09f, 3.19f, 1.694f, 1.573f, 3.366f, 2.596f, 2.53f, 1.221f, 2.827f, 3.465f, 1.65f, 2.904f, 2.42f, 2.94f, 1.3f);
            //var n_samples = train_X.shape[0];



            // tf Graph Input
            var X = tf.placeholder(tf.float32);
            var Y = tf.placeholder(tf.float32);

            // Set model weights
            var init = tf.global_variables_initializer();
            var W = tf.Variable(rng.randn<float>(), name: "weight");
            var b = tf.Variable(rng.randn<float>(), name: "bias");
          
            
            // Construct a linear model
            var pred = tf.add(tf.multiply(X, W), b);

            // Mean squared error
            var cost = tf.reduce_sum(tf.pow(pred - Y, 2.0f)) / (2.0f * nSamples);
            var cost2 = tf.sqrt(cost/nSamples);

            // Gradient descent
            // Note, minimize() knows to modify W and b because Variable objects are trainable=True by default
            var optimizer = tf.train.GradientDescentOptimizer(learning_rate).minimize(cost);


            // Number of training iterations in each epoch
            var num_tr_iter = (int)dsY.size / batch_size;
            using (var sess = tf.Session())
            {
                sess.run(init);
                sess.run(W.initializer);
                sess.run(b.initializer);

                float loss_val = 100.0f;
                float accuracy_val = 0f;

                foreach (var epoch in range(epochs))
                {
                    print($"Training epoch: {epoch + 1}");

                    // Randomly shuffle the training data at the beginning of each epoch 
                    var (x_train, y_train) = randomize(dsX, dsY);

                    foreach (var iteration in range(num_tr_iter))
                    {
                        var start = iteration * batch_size;
                        var end = (iteration + 1) * batch_size;
                        var (x_batch, y_batch) = get_next_batch(x_train, y_train, start, end);

                        // Run optimization op (backprop)
                        sess.run(optimizer, new FeedItem(X, x_batch), new FeedItem(Y, y_batch));

                        if (iteration % display_freq == 0)
                        {
                            // Calculate and display the batch loss and accuracy
                            var result = sess.run(new[] { cost, cost2 }, new FeedItem(X, x_batch), new FeedItem(Y, y_batch));
                            loss_val = result[0];
                            accuracy_val = result[1];
                            print($"iter {iteration.ToString("000")}: MSE={loss_val.ToString("0.0000")}, RMSE={accuracy_val.ToString("P")}");
                        }
                    }

                    // Run validation after every epoch
                    var results1 = sess.run(new[] { cost, cost2 }, new FeedItem(X, dsX), new FeedItem(Y, dsY));
                    loss_val = results1[0];
                    accuracy_val = results1[1];
                    print("---------------------------------------------------------");
                    print($"Epoch: {epoch + 1}, validation MSE: {loss_val.ToString("0.0000")}, validation RMSE: {accuracy_val.ToString("P")}");
                    print("---------------------------------------------------------");
                }
            };


        }
    }
}

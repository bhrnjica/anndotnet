using System;
using System.Linq;
using Daany;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
using Daany.Ext;
using System.Diagnostics;

namespace AnnDotNET.Tool
{
    class AnnTrainer
    {
        public Tensor Eval { get; set; }
        public Tensor Loss { get; set; }
        public Operation Learner { get; set; }
    }
    class Ann
    {
        //placeholders
        Tensor x { get; set; }
        Tensor y { get; set; }
        //model
        Tensor z { get; set; }
    }

    class Program
    {

        static void Main(string[] args)
        {
            (var X_data, var Y_data) = PrepareData();

            (Tensor x, Tensor y) = createPlacehoders(4, 3);

            int outDim = y.shape.Last();
            Tensor z = createModel(x, outDim);

            var tr = createTrainer(y, z);

            Train(x, y, tr, X_data, Y_data);

            return;

        }

        public static void Train(Tensor x, Tensor y, AnnTrainer tr, NDArray X_data, NDArray Y_data)
        {
            var sw = new Stopwatch();
            sw.Start();

            var config = new ConfigProto
            {
                IntraOpParallelismThreads = 1,
                InterOpParallelismThreads = 1,
                LogDevicePlacement = true
            };

            using (var sess = tf.Session(config))
            {
                // init variables
                sess.run(tf.global_variables_initializer());

                // check the accuracy before training
                var (x_input, y_input) = (X_data, Y_data);//sess.run((x_inputs_data, y_inputs_data));
                var loss = sess.run(tr.Loss, (x, x_input), (y, y_input));
                print($"iter:{0} - loss:{loss}");

                // training
                foreach (var i in range(50))
                {
                    // by sampling some input data (fetching)
                    (x_input, y_input) = (X_data, Y_data);//sess.run((x_inputs_data, y_inputs_data));
                    var (_, loss0) = sess.run((tr.Learner, tr.Loss), (x, x_input), (y, y_input));

                    // We regularly check the loss
                    if (i % 5 == 0)
                        print($"iter:{i} - loss:{loss0}");
                }

                // Finally, we check our final accuracy
                (x_input, y_input) = (X_data, Y_data);//sess.run((x_inputs_data, y_inputs_data));
                sess.run(tr.Eval, (x, x_input), (y, y_input));
            }

            print($"Time taken: {sw.Elapsed.TotalSeconds}s");
        }


        public static (NDArray, NDArray) PrepareData()
        {

            //read the iris data and create DataFrame object
            var df = DataFrame.FromCsv("iris.txt", sep: '\t');

            //prepare the data
            var features = new string[] { "sepal_length", "sepal_width", "petal_length", "petal_width" };
            var label = "species";
            //
            return df.PrepareData(features, label);

        }




        private static AnnTrainer createTrainer(Tensor y, Tensor z)
        {
            var tr = new AnnTrainer();

            tf_with(tf.variable_scope("Loss"), delegate
            {

                var losses = tf.nn.sigmoid_cross_entropy_with_logits(tf.cast(y, tf.float32), z);
                tr.Loss = tf.reduce_mean(losses);
            });

            tf_with(tf.variable_scope("Evaluation"), delegate
            {
                var y_pred = tf.cast(z > 0, tf.int32);
                tr.Eval = tf.reduce_mean(tf.cast(tf.equal(y_pred, y), tf.float32));
                // accuracy = tf.Print(accuracy, data =[accuracy], message = "accuracy:")
            });

            // We add the training operation, ...
            var adam = tf.train.AdamOptimizer(0.01f);
            tr.Learner = adam.minimize(tr.Loss, name: "train_op");

            return tr;
        }

        private static Tensor createModel(Tensor x, int outDim)
        {
            Tensor z = null;

            tf_with(tf.variable_scope("FullyConnected"), delegate
            {
                var w = tf.get_variable("w", shape: (x.shape.Last(), 5), initializer: tf.random_normal_initializer(stddev: 0.1f));
                var b = tf.get_variable("b", shape: 5, initializer: tf.constant_initializer(0.1));
                z = tf.matmul(x, w) + b;
                var yyy = tf.nn.relu(z);

                var w2 = tf.get_variable("w2", shape: (5, outDim), initializer: tf.random_normal_initializer(stddev: 0.1f));
                var b2 = tf.get_variable("b2", shape: outDim, initializer: tf.constant_initializer(0.1));
                z = tf.matmul(yyy, w2) + b2;
            });

            return z;
        }

        private static (Tensor x, Tensor y) createPlacehoders(int xDim, int yDim)
        {
            Tensor X = null;
            Tensor Y = null;
            //
            tf_with(tf.variable_scope("placeholders"), delegate
            {
                X = tf.placeholder(tf.float32, shape: (-1, xDim));
                Y = tf.placeholder(tf.int32, shape: (-1, yDim));
            });

            return (X, Y);
        }
    }
}

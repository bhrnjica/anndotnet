using System;
using System.Linq;
using Daany;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
using Daany.Ext;
using System.Diagnostics;
using AnnDotNET.Common;
using ann.vdn;
using System.IO;

namespace AnnDotNET.Tool
{
   
    class Program
    {

        static void Main(string[] args)
        {
            (var X_data, var Y_data) = PrepareData();

            var dFeed = new DataFeed(X_data, Y_data, 50);

            Placeholders plcHolder = new Placeholders();
            (Tensor x, Tensor y) = plcHolder.Create(4, 3);

            //create network
            int outDim = y.shape.Last();
            NetworkModel model = new NetworkModel();
            Tensor z = model.Create(x, outDim);
            
            //define learner
            var learner = new ClassificationLearner();
            var lr = learner.Create(y, z, new TrainingParameters());

            //training process
            Trainer tr = new Trainer();
            tr.TrainOffline(x, y, lr, new TrainingParameters() { Progress = Progress }, dFeed);
 
            //evaluation


            //prediction
            return;

        }

        public static void Progress(TrainingProgress tp)
        {
            if(tp.ProgressType== ProgressType.Initialization)
                Console.WriteLine($"_________________________________________________________");

            Console.WriteLine($"Iteration={tp.Iteration}, Loss={Math.Round(tp.Loss, 3)}, Eval={Math.Round(tp.Eval, 3)}");

            if (tp.ProgressType == ProgressType.Completed)
                Console.WriteLine($"_________________________________________________________");
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


        
        public static void Predict(DataFeed dFeed)
        {
            var graph = new Graph().as_default();
            using (var sess = tf.Session(graph))
            {
                graph.Import(Path.Join(".resources/logistic_regression", "model.pb"));

                // restoring the model
                // var saver = tf.train.import_meta_graph("logistic_regression/tensorflowModel.ckpt.meta");
                // saver.restore(sess, tf.train.latest_checkpoint('logistic_regression'));
                var pred = graph.OperationByName("Softmax");
                var output = pred.outputs[0];
                var x = graph.OperationByName("Placeholder");
                var input = x.outputs[0];

                // predict
                var (batch_xs, batch_ys) = dFeed.GetFullBatch();
                var results = sess.run(output, new FeedItem(input, batch_xs[np.arange(1)]));

                if (results[0].argmax() == (batch_ys[0] as NDArray).argmax())
                    print("predicted OK!");
                else
                    throw new ValueError("predict error, should be 90% accuracy");
            }
        }

    }
}

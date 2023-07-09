///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using Tensorflow;
using static Tensorflow.Binding;

namespace AnnDotNet.Vnd;

public class NetworkModel
{

    public Tensor Create(Tensor x, int outDim)
    {
        Tensor z = null;
        int? random_seed = 1;
        tf_with(tf.variable_scope("FullyConnected"), delegate
        {
            var initValue = tf.random_uniform(shape: (x.shape.dims.Last(), 5), seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            var initValueb = tf.random_uniform(shape: 5, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            var w1 = tf.Variable<Tensor>(initValue, name: "w1");
            var b1 = tf.Variable<Tensor>(initValueb, name: "b1");

            z = tf.matmul(x, w1) + b1;
            var yyy = tf.nn.relu(z);

            var initValue2 = tf.random_uniform(shape: (5, outDim), seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            var initValueb2 = tf.random_uniform(shape: outDim, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            //var initValue2 = tf.random_normal(shape: (5, outDim), stddev: 0.1f, mean: 0f, seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            //var initValueb2 = tf.random_normal(shape: outDim, stddev: 0.1f, mean: 0f, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            var w2 = tf.Variable<Tensor>(initValue2, name: "w2");
            var b2 = tf.Variable<Tensor>(initValueb2, name: "b2");

            z = tf.matmul(yyy, w2) + b2;
        });

        return z;
    }

    public Tensor CreateLogisticRegression(Tensor x)
    {
        Tensor z = null;
        int? random_seed = 1;
        tf_with(tf.variable_scope("FullyConnected"), delegate
        {
            var initValue = tf.random_uniform(shape: (x.shape.dims.Last(), 5), seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            var initValueb = tf.random_uniform(shape: 5, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            var w1 = tf.Variable<Tensor>(initValue, name: "w1");
            var b1 = tf.Variable<Tensor>(initValueb, name: "b1");

            z = tf.matmul(x, w1) + b1;
            var yyy = tf.nn.softmax(z);


        });

        return z;
    }

    public Tensor CreatAlexNetNetwork()
    {
        throw new NotImplementedException();
    }

    public Tensor CreateResNet()
    {
        throw new NotImplementedException();
    }
    public Tensor CreateSimpleRegression(Tensor x)
    {
        Tensor z = null;
        int? random_seed = 1;
        tf_with(tf.variable_scope("FullyConnected"), delegate
        {
            var initValue = tf.random_uniform(shape: (x.shape.dims.Last(), 5), seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            var initValueb = tf.random_uniform(shape: 5, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            var w1 = tf.Variable<Tensor>(initValue, name: "w1");
            var b1 = tf.Variable<Tensor>(initValueb, name: "b1");

            z = tf.matmul(x, w1) + b1;
            var yyy = tf.nn.tanh(z);

            var initValue2 = tf.random_uniform(shape: (5, 1), seed: random_seed, dtype: TF_DataType.TF_FLOAT);
            var initValueb2 = tf.random_uniform(shape: 1, seed: random_seed, dtype: TF_DataType.TF_FLOAT);

            var w2 = tf.Variable<Tensor>(initValue2, name: "w2");
            var b2 = tf.Variable<Tensor>(initValueb2, name: "b2");

            z = tf.matmul(yyy, w2) + b2;
        });

        return z;
    }

    public bool Save(Session sess, string filePath)
    {
            
        var saver = tf.train.Saver();
        var save_path = saver.save(sess, ".resources/logistic_regression/model.ckpt");
        tf.train.write_graph(sess.graph, ".resources/logistic_regression", "model.pbtxt", as_text: true);

        FreezeGraph.freeze_graph(input_graph: ".resources/logistic_regression/model.pbtxt",
            input_saver: "",
            input_binary: false,
            input_checkpoint: ".resources/logistic_regression/model.ckpt",
            output_node_names: "Softmax",
            restore_op_name: "save/restore_all",
            filename_tensor_name: "save/Const:0",
            output_graph: ".resources/logistic_regression/model.pb",
            clear_devices: true,
            initializer_nodes: "");

        return true;
    }
}
using AnnDotNet.Core.TensorflowEx;
using NUnit.Framework;
using System;
using System.Linq;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace AnnDotNet.test;

public class EvaluationFunctionTests
{
    ConfigProto _config;
       
    [SetUp]
    public void Setup()
    {
        _config = new ConfigProto
        {
            IntraOpParallelismThreads = 1,
            InterOpParallelismThreads = 1,
            LogDevicePlacement = true
        };

    }

    [Test]
    public void Test1()
    {
        tf.enable_eager_execution();
        // Define tensor constants.
        var a = tf.constant(2f);
        var b = tf.constant(3f);
        var c = tf.constant(5f);
        var np0 = new NDArray(new[] { 2f, 3f, 4f, 5f }, new Shape(4));
        //var inX = new NDArray(new[] { a, b, c });

        // .
        var mean = tf.reduce_mean(np0,0);
        Assert.IsTrue(mean.ToArray<float>()[0] == 3.5);

        var sum = tf.reduce_sum(new NDArray(new[] { a, b, c }));

        Assert.IsTrue(sum.ToArray<float>()[0] == 10);

    }

    [Test]
    public void AbsoluteErrorTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(6, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");

        var actual = new NDArray(new[] { 4.0f, 2.0f, 1.0f, 0.0f, 5.0f, 1.0f }, shape);
        var predicted = new NDArray(new[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.8f, 0.2f }, shape);

        var ce = FunctionEx.AbsoluteError(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 1.83f);
        }
    }

    [Test]
    public void RootMeanSquaredErrorTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(6, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");

        var actual = new NDArray(new[] { 4.0f, 2.0f, 1.0f, 0.0f, 5.0f, 1.0f }, shape);
        var predicted = new NDArray(new[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.8f, 0.2f }, shape);

        var ce = FunctionEx.RootMeanSquaredError(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 2.32f);
        }
    }

    [Test]
    public void SquaredErrorTest()
    {
        tf.compat.v1.disable_eager_execution();
        var  shape= new Shape(6, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");

        var actual = new NDArray(new[] { 4.0f, 2.0f, 1.0f , 0.0f, 5.0f, 1.0f  }, shape);
        var predicted = new NDArray(new[] { 1.0f, 0.0f, 0.0f , 0.0f, 0.8f, 0.2f  }, shape);

        var ce = FunctionEx.SquaredError(y, z);
           
        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));
               
            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0],2);
                
            Assert.IsTrue(calcValue == 32.28f);
        }
        
    }

    [Test]
    public void MeanSquaredErrorTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(6, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");

        var actual = new NDArray(new[] { 4.0f, 2.0f, 1.0f, 0.0f, 5.0f, 1.0f }, shape);
        var predicted = new NDArray(new[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.8f, 0.2f }, shape);

        var ce = FunctionEx.MeanSquaredError(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 5.38f);
        }
    }

    //"Multi-class cross -entropy"
    [Test]
    public void MultiClassCrossEntropyTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(3, 2);
        var shape1 = new Shape(-1, 2);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");
        var q = tf.nn.softmax(z);
        var logit = new NDArray(new[] { 0.20f, 0.80f, 0.70f, 0.30f, 0.50f, 0.50f }, shape);
        var labels = new NDArray(new[] { 0.0f, 1.0f, 1.0f, 0.0f, 1.0f, 0.0f }, shape);

        var ce = -tf.reduce_sum(y * tf.log(q), axis: 1); 
        var ce1 = tf.nn.softmax_cross_entropy_with_logits(labels: y, logits: z);

        var ce2 = FunctionEx.MultiClassCrossEntropy(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, labels), (z, logit));
            var vv2 = sess.run(ce1, (y, labels), (z, logit));
            var vv3 = sess.run(ce2, (y, labels), (z, logit));

            // Access tensors value.
            float calcValue1 = vv1.ToArray<float>().Average();
            float calcValue2 = vv2.ToArray<float>().Average();
            float calcValue3 = (float) Math.Round(vv3.ToArray<float>()[0],7);
            Assert.IsTrue(calcValue1 == calcValue2);
            Assert.IsTrue(calcValue2 == calcValue3);
        }
    }


    [Test]
    public void BinaryClassEntropy()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(2, 5);
        var shape1 = new Shape(-1, 5);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");
        var q = tf.nn.sigmoid(z);
             

        var labels = new NDArray(new[] { 0f, 0f, 0f, 1.0f, 0f, 1.0f, 0f, 0f, 0f, 0f}, shape);
        var logit = new NDArray(new[] { 0.20f, 0.20f, 0.20f, 0.20f, 0.2f, 0.3f, 0.3f, 0.2f, 0.1f, 0.1f }, shape);

        var be1 = -(y * tf.log(q));
        var be2 = y * -tf.log(q) + (1f - y) *( -tf.log(1f - q));
        var be3 = y * -tf.log(tf.sigmoid(z)) + (1f - y) * -tf.log(1f - tf.sigmoid(z));
        var be4 = tf.nn.sigmoid_cross_entropy_with_logits(labels: y, logits: z);
        var be5 = FunctionEx.BinaryCrossEntropy(y, z);// 

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(be1, (y, labels), (z, logit));
            var vv2 = sess.run(be2, (y, labels), (z, logit));
            var vv3 = sess.run(be3, (y, labels), (z, logit));
            var vv4 = sess.run(be4, (y, labels), (z, logit));
            var vv5 = sess.run(be5, (y, labels), (z, logit));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv5.ToArray<float>()[0],2);
            // float calcValue1 = ce1.ToArray<float>()[0];
            Assert.IsTrue(calcValue == 0.75f);
        }
    }

    [Test]
    public void BinaryAccuracy()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(6, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual =    new NDArray(new[] { 0.0f, 1.00f, 0.00f, 1.00f, 1.00f, 0.00f }, shape);
        var predicted = new NDArray(new[] { 0.20f, 0.80f, 0.70f, 0.30f, 0.51f, 0.50f }, shape);

        var ce = FunctionEx.Accuracy(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 0.50f);
        }
    }

    [Test]
    public void AccuracyTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(3, 2);
        var shape1 = new Shape(-1, 2);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");

  
        var actual    = new NDArray(new[] {  0.0f, 1.00f, 0.00f, 1.00f, 1.00f, 0.00f }, shape);
        var predicted = new NDArray(new[] { 0.20f, 0.80f, 0.70f, 0.30f, 0.50f, 0.50f }, shape);

        var ce = FunctionEx.Accuracy(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 0.67f);
        }
    }

    public void ErrorTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(3, 2);
        var shape1 = new Shape(-1, 2);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] { 0.0f, 1.00f, 0.00f, 1.00f, 1.00f, 0.00f }, shape);
        var predicted = new NDArray(new[] { 0.20f, 0.80f, 0.70f, 0.30f, 0.50f, 0.50f }, shape);

        var ce = FunctionEx.ClassificationError(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 1.0f- 0.67f);
        }
    }

    [Test]
    public void AccuracyTest1()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(3, 3);
        var shape1 = new Shape(-1, 3);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(   new[] { 0.0f, 1.00f, 0.00f, 1.00f, 0.00f, 0.00f, 0.00f, 0.00f, 1.00f }, shape);
        var predicted = new NDArray(new[] { 0.10f, 0.80f, 0.10f, 0.40f, 0.30f, 0.30f,0.40f, 0.50f, 0.10f }, shape);

        var ce = FunctionEx.Accuracy(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            float calcValue = (float)Math.Round(vv1.ToArray<float>()[0], 2);

            Assert.IsTrue(calcValue == 0.67f);
        }
    }

    [Test]
    public void ConfusionMatrixTest1()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 3);
        var shape1 = new Shape(-1, 3);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,0f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,0f,1f,
            0f,1f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,1f,0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,0.100f,0.100f,
            0.600f,0.200f,0.200f,
            0.100f,0.100f,0.800f,
            0.900f,0.055f,0.045f,
            0.100f,0.350f,0.550f,
            0.600f,0.200f,0.200f,
            0.300f,0.600f,0.100f,
            0.100f,0.200f,0.700f,
            0.900f,0.090f,0.010f,
            0.800f,0.100f,0.100f,
        }, shape);

        var ce = FunctionEx.ConfusionMatrix(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 3f);
            Assert.IsTrue(calcValue[1] == 0f);
            Assert.IsTrue(calcValue[2] == 0f);
            Assert.IsTrue(calcValue[3] == 3f);
            Assert.IsTrue(calcValue[4] == 1f);
            Assert.IsTrue(calcValue[5] == 0f);
            Assert.IsTrue(calcValue[6] == 0f);
            Assert.IsTrue(calcValue[7] == 0f);
            Assert.IsTrue(calcValue[8] == 3f);
        }
    }

    [Test]
    public void ConfusionMatrixTest2()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,
            0f,
            0f,
            1f,
            0f,
            0f,
            0f,
            0f,
            1f,
            0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,
            0.600f,
            0.100f,
            0.900f,
            0.100f,
            0.600f,
            0.300f,
            0.100f,
            0.900f,
            0.800f,
        }, shape);

        var ce = FunctionEx.ConfusionMatrix(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 3f);
            Assert.IsTrue(calcValue[1] == 0f);
            Assert.IsTrue(calcValue[2] == 3f);
            Assert.IsTrue(calcValue[3] == 4f);
        }
    }


    [Test]
    public void PrecisionTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 3);
        var shape1 = new Shape(-1, 3);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,0f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,0f,1f,
            0f,1f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,1f,0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,0.100f,0.100f,
            0.600f,0.200f,0.200f,
            0.100f,0.100f,0.800f,
            0.900f,0.055f,0.045f,
            0.100f,0.350f,0.550f,
            0.600f,0.200f,0.200f,
            0.300f,0.600f,0.100f,
            0.100f,0.200f,0.700f,
            0.900f,0.090f,0.010f,
            0.800f,0.100f,0.100f,
        }, shape);

        var ce = FunctionEx.Precision(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 0.5f);
            Assert.IsTrue(calcValue[1] == 1f);
            Assert.IsTrue(calcValue[2] == 1f);
               
        }
    }

    [Test]
    public void RecallTest()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 3);
        var shape1 = new Shape(-1, 3);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,0f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,0f,1f,
            0f,1f,0f,
            0f,1f,0f,
            0f,0f,1f,
            1f,0f,0f,
            0f,1f,0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,0.100f,0.100f,
            0.600f,0.200f,0.200f,
            0.100f,0.100f,0.800f,
            0.900f,0.055f,0.045f,
            0.100f,0.350f,0.550f,
            0.600f,0.200f,0.200f,
            0.300f,0.600f,0.100f,
            0.100f,0.200f,0.700f,
            0.900f,0.090f,0.010f,
            0.800f,0.100f,0.100f,
        }, shape);

        var ce = FunctionEx.Recall(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 1f);
            Assert.IsTrue(calcValue[1] == 0.25f);
            Assert.IsTrue(calcValue[2] == 1f);

        }
    }

    [Test]
    public void PrecisionTest2()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,
            0f,
            0f,
            1f,
            0f,
            0f,
            0f,
            0f,
            1f,
            0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,
            0.600f,
            0.100f,
            0.900f,
            0.100f,
            0.600f,
            0.300f,
            0.100f,
            0.900f,
            0.800f,
        }, shape);

        var ce = FunctionEx.Precision(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 0.5f);
                
        }
    }

    [Test]
    public void RecallTest2()
    {
        tf.compat.v1.disable_eager_execution();
        var shape = new Shape(10, 1);
        var shape1 = new Shape(-1, 1);
        var y = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "y");
        var z = tf.placeholder(TF_DataType.TF_FLOAT, shape1, "z");


        var actual = new NDArray(new[] {
            1f,
            0f,
            0f,
            1f,
            0f,
            0f,
            0f,
            0f,
            1f,
            0f,
        }, shape);
        var predicted = new NDArray(new[] {
            0.800f,
            0.600f,
            0.100f,
            0.900f,
            0.100f,
            0.600f,
            0.300f,
            0.100f,
            0.900f,
            0.800f,
        }, shape);

        var ce = FunctionEx.Recall(y, z);

        //evaluate functions
        using (var sess = tf.Session(_config))
        {
            sess.run(tf.global_variables_initializer());
            var vv1 = sess.run(ce, (y, actual), (z, predicted));

            // Access tensors value.
            var calcValue = vv1.ToArray<float>();

            Assert.IsTrue(calcValue[0] == 1f);

        }
    }

}
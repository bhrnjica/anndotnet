using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NumSharp;
using Tensorflow;
using static Tensorflow.Binding;
using Anndotnet.Core.TensorflowEx;

namespace anndotnet.test
{
    public class EvaluationFunctionTests
    {
        ConfigProto _config;
        Operation _init;


        [SetUp]
        public void Setup()
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

        [Test]
        public void Test1()
        {

            // Define tensor constants.
            var a = tf.constant(2f);
            var b = tf.constant(3f);
            var c = tf.constant(5f);
            var np0 = new NDArray(new[] { 2f, 3f, 4f, 5f }, new Shape(4));
            //var inX = new NDArray(new[] { a, b, c });

            // .
            var mean = tf.reduce_mean(np0,0);
            Assert.IsTrue(mean.ToArray<float>()[0] == 3.5);

            var sum = tf.reduce_sum(new[] { a, b, c });
            Assert.IsTrue(sum.ToArray<float>()[0] == 3.5);

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
                sess.run(_init);
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
                sess.run(_init);
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
                sess.run(_init);
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
                sess.run(_init);
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
                sess.run(_init);
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
                sess.run(_init);
                var vv1 = sess.run(be1, (y, labels), (z, logit));
                var vv2 = sess.run(be2, (y, labels), (z, logit));
                var vv3 = sess.run(be3, (y, labels), (z, logit));
                var vv4 = sess.run(be4, (y, labels), (z, logit));
                var vv5 = sess.run(be5, (y, labels), (z, logit));

                // Access tensors value.
                float calcValue = vv1.ToArray<float>()[0];
               // float calcValue1 = ce1.ToArray<float>()[0];
                Assert.IsTrue(calcValue == 0.247f);
            }



        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnDotNet.Core.Interfaces;
using static TorchSharp.torch.nn;
using static TorchSharp.torch;
using TorchSharp;
using AnnDotNet.Core.Entities;
using AnnDotNet.Vnd.Layers;

namespace Anndotnet.Core.Model
{
    internal class Model : Module<Tensor, Tensor>
    {
        private readonly List<ILayer> _layers;
        
        private Module<Tensor, Tensor> conv1 = Conv2d(1, 32, 3);
        private Module<Tensor, Tensor> conv2 = Conv2d(32, 64, 3);
        private Module<Tensor, Tensor> fc1 = Linear(9216, 128);
        private Module<Tensor, Tensor> fc2 = Linear(128, 10);

        // These don't have any parameters, so the only reason to instantiate
        // them is performance, since they will be used over and over.
        private Module<Tensor, Tensor> pool1 = MaxPool2d(kernelSize: new long[] { 2, 2 });

        private Module<Tensor, Tensor> relu1 = ReLU();
        private Module<Tensor, Tensor> relu2 = ReLU();
        private Module<Tensor, Tensor> relu3 = ReLU();

        private Module<Tensor, Tensor> dropout1 = Dropout(0.25);
        private Module<Tensor, Tensor> dropout2 = Dropout(0.5);

        private Module<Tensor, Tensor> flatten = Flatten();
        private Module<Tensor, Tensor> logsm = LogSoftmax(1);

        public Model(string name, ILayer[] layers, torch.Device device = null) : base(name)
        {
            _layers = layers.ToList();

            RegisterComponents();

            if (device != null && device.type == DeviceType.CUDA)
            {
                this.to(device);
            }
        }

        public override Tensor forward(Tensor input)
        {

            // All these modules are private to the model and won't have hooks,
            // so we can use 'forward' instead of 'call'
            var l11 = conv1.forward(input);
            var l12 = relu1.forward(l11);

            var l21 = conv2.forward(l12);
            var l22 = relu2.forward(l21);
            var l23 = pool1.forward(l22);
            var l24 = dropout1.forward(l23);

            var x = flatten.forward(l24);

            var l31 = fc1.forward(x);
            var l32 = relu3.forward(l31);
            var l33 = dropout2.forward(l32);

            var l41 = fc2.forward(l33);

            return logsm.forward(l41);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                conv1.Dispose();
                conv2.Dispose();
                fc1.Dispose();
                fc2.Dispose();
                pool1.Dispose();
                relu1.Dispose();
                relu2.Dispose();
                relu3.Dispose();
                dropout1.Dispose();
                dropout2.Dispose();
                flatten.Dispose();
                logsm.Dispose();
                ClearModules();
            }
            base.Dispose(disposing);
        }

        public static Tensor CreateNetwork(List<ILayer> layers, Tensor inX, Tensor outY, int seed = 1234)
        {
            throw new NotImplementedException();


            //var init = ValueInitializer.GlorotNormal;

            //Tensor z = inX;

            //var l = new List<ILayer>();

            //foreach (var layer in layers)
            //{

            //    if (layer.Type == LayerType.Dense)
            //    {
            //        var nnl = layer as FCLayer ?? throw new ArgumentNullException("FCLayer cannot be null!");

            //        z = l.Dense(z, nnl.OutDim, init, layer.Name, seed: seed);
            //    }
            //    else if (layer.Type == LayerType.Drop)
            //    {
            //        var nnl = layer as DropLayer ?? throw new ArgumentNullException("DropLayer cannot be null!");
            //        z = l.Drop(z, nnl.DropPercentage / 100.0f, layer.Name, seed: seed);
            //    }
            //    else if (layer.Type == LayerType.Activation)
            //    {
            //        var nnl = layer as ActLayer ?? throw new ArgumentNullException("ActivationLayer cannot be null!");
            //        z = l.ActivationLayer(z, nnl.Activation, layer.Name);
            //    }
            //    else if (layer.Type == LayerType.Embedding)
            //    {
            //        var nnl = layer as EmbeddingLayer ?? throw new ArgumentNullException("EmbeddingLayer cannot be null!");
            //        z = l.Embedding(z, nnl.OutDim, init, layer.Name, seed: seed);

            //        //z = Embedding.Create(z, layer.Param1, type, device, 1, layer.Name);
            //    }
            //    //else if (layer.Type == LayerType.LSTM)
            //    //{
            //    //    var returnSequence = true;
            //    //    if (layers.IndexOf(lastLSTM) == layers.IndexOf(layer))
            //    //        returnSequence = false;
            //    //    net = RNN.RecurrenceLSTM(net, layer.Param1, layer.Param2, type, device, returnSequence, layer.FParam,
            //    //        layer.BParam2, layer.BParam1, 1);
            //    //}
            //    //else if (layer.Type == LayerType.NALU)
            //    //{
            //    //    var nalu = new NALU(net, layer.Param1, type, device, 1, layer.Name);
            //    //    net = nalu.H;
            //    //}
            //    //else if (layer.Type == LayerType.Conv1D)
            //    //{
            //    //    var cn = new Convolution();
            //    //    net = cn.Conv1D(net, layer.Param1, layer.Param2, type, device,
            //    //        layer.BParam1, layer.BParam2, layer.Name, 1);
            //    //}
            //    //else if (layer.Type == LayerType.Conv2D)
            //    //{
            //    //    var cn = new Convolution();
            //    //    net = cn.Conv2D(net, layer.Param1, new int[] { layer.Param2, layer.Param2 }, type, device,
            //    //        layer.BParam1/*padding*/, layer.BParam2/*bias*/, layer.Name, 1);
            //    //}
            //    //else if (layer.Type == LayerType.Pooling1D)
            //    //{
            //    //    var cn = new Convolution();
            //    //    var pType = PoolingType.Max;
            //    //    if (layer.FParam == Activation.Avg)
            //    //        pType = PoolingType.Average;

            //    //    //
            //    //    net = cn.Pooling1D(net, layer.Param1, type, pType, device, layer.Name, 1);
            //    //}
            //    //else if (layer.Type == LayerType.Pooling2D)
            //    //{
            //    //    var cn = new Convolution();
            //    //    var pType = PoolingType.Max;
            //    //    if (layer.FParam == Activation.Avg)
            //    //        pType = PoolingType.Average;

            //    //    //
            //    //    net = cn.Pooling2D(net, new int[] { layer.Param1, layer.Param1 }, layer.Param2,
            //    //        type, pType, device, layer.Name, 1);
            //    //}
            //    //else if (layer.Type == LayerType.CudaStackedLSTM)
            //    //{
            //    //    net = RNN.RecurreceCudaStackedLSTM(net, layer.Param1, layer.Param2, layer.BParam2, device);
            //    //}
            //    //else if (layer.Type == LayerType.CudaStackedGRU)
            //    //{
            //    //    net = RNN.RecurreceCudaStackedGRU(net, layer.Param1, layer.Param2, layer.BParam2, device);
            //    //}

            //}

            ////add last layer as Output 
            //z = l.Output(z, outY);
            //return z;
        }
    }
}

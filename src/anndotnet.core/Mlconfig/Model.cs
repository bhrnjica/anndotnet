using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;
using AnnDotNet.Core.Layers;
using static TorchSharp.torch.nn;
using static TorchSharp.torch;
using TorchSharp;


namespace Anndotnet.Core.Model
{
    public class AnnModel : Module<Tensor, Tensor>
    {
        private readonly List<Module<Tensor, Tensor>> _layers= new List<Module<Tensor, Tensor>>();
        private readonly int _inputDim;
        private readonly int _outputDim;

        
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

        public AnnModel(string name, List<ILayer> layers, int inputDim, int outputDim, Device device = null) : base(name)
        {
            
            _inputDim = inputDim;
            _outputDim = outputDim;

            _layers.Clear();
            ToTorchLayers(layers);

            RegisterComponents();

            if (device is { type: DeviceType.CUDA })
            {
                this.to(device);
            }
        }
        public override Tensor forward(Tensor input)
        {
            var z = input;

            foreach (var l in _layers)
            {
               z = l.forward(z);
            }

            return z;

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
                _layers.ForEach(l => l.Dispose());
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

        private void ToTorchLayers(List<ILayer> layers)
        {
            var inputDim = _inputDim;   
            foreach (var layer in layers)
            {
                if (layer is Dense)
                {
                    var l = (Dense)layer;
                    var linear = Linear(inputDim, l.OutputDim, l.HasBias);
                    _layers.Add(linear);
                    var activation = ToTorchLayer(l.Activation);
                    _layers.Add(activation);

                    inputDim = l.OutputDim;
                }
                else if (layer is Dropout)
                {
                    var l = (Dropout)layer;
                    var drop= Dropout(l.Rate);
                    _layers.Add(drop);
                }
                else
                {
                    throw new NotSupportedException("The layer is not supported.");
                }
            }
        }

        private Module<Tensor, Tensor> ToTorchLayer(Activation activation)
        {
            return activation switch
            {
                Activation.None => Identity(),
                Activation.ReLU => ReLU(),
                Activation.Sigmoid => Sigmoid(),
                Activation.Softmax => Softmax(0),
                Activation.TanH => Tanh(),
                _ => Identity()
            };
        }
    }
}

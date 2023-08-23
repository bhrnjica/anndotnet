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
        private readonly Module<Tensor, Tensor> network;
        private readonly int _inputDim;
        private readonly int _outputDim;


        public AnnModel(string name, List<ILayer> layers, int inputDim, int outputDim, Device device = null) : base(name)
        {
            
            _inputDim = inputDim;
            _outputDim = outputDim;

            _layers.Clear();

            ToTorchLayers(layers);

            network = Sequential(_layers.ToArray());

            RegisterComponents();

            if (device is { type: DeviceType.CUDA })
            {
                this.to(device);
            }
        }
        public override Tensor forward(Tensor input)
        {
            return network.forward(input);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                network.Dispose();
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
                Activation.Softmax => Softmax(dim:1),
                Activation.LogSoftmax => LogSoftmax(dim: 1),
                Activation.TanH => Tanh(),
                _ => Identity()
            };
        }
    }
}

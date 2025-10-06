////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anndotnet.Core.Entities;
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Layers;
using Anndotnet.Core.Util;
using static TorchSharp.torch.nn;
using static TorchSharp.torch;
using TorchSharp;


namespace Anndotnet.Core.Mlconfig
{
    public class AnnModel : Module<Tensor, Tensor>
    {
        private readonly List<Module<Tensor, Tensor>> _layers= new List<Module<Tensor, Tensor>>();
        private readonly Module<Tensor, Tensor> network;
        private readonly int _inputDim;
        private readonly int _outputDim;


        public AnnModel(string name, List<ILayer> layers, int inputDim, int outputDim, Device device = null, Anndotnet.Core.Entities.WeightInitMethod initMethod = Anndotnet.Core.Entities.WeightInitMethod.XavierUniform) : base(name)
        {
            
            _inputDim = inputDim;
            _outputDim = outputDim;

            _layers.Clear();

            ToTorchLayers(layers);

            network = Sequential(_layers.ToArray());

            RegisterComponents();
            
            // Apply weight initialization
            ModelUtils.InitializeWeights(this, initMethod);

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
                else if (layer is Embedding)
                {
                    var l = (Embedding)layer;
                    var embedding = Embedding(l.OutputDim, inputDim, padding_idx: l.PaddingIdX, max_norm: l.MaxNorm, norm_type: l.NormType);
                    _layers.Add(embedding);

                    inputDim = l.OutputDim;
                }
                else if (layer is BatchNormalization)
                {
                    var l = (BatchNormalization)layer;
                    var batchNorm = BatchNorm1d(l.NumFeatures, l.Eps, l.Momentum, l.Affine, l.TrackRunningStats);
                    _layers.Add(batchNorm);
                }
                else if (layer is LayerNormalization)
                {
                    var l = (LayerNormalization)layer;
                    var layerNorm = LayerNorm(l.NormalizedShape.Select(x => (long)x).ToArray(), l.Eps, l.ElementwiseAffine);
                    _layers.Add(layerNorm);
                }
                else if (layer is Conv1D)
                {
                    var l = (Conv1D)layer;
                    var conv = Conv1d(l.InChannels, l.OutChannels, l.KernelSize, l.Stride, l.Padding, l.Dilation, bias: l.HasBias);
                    _layers.Add(conv);
                    
                    var activation = ToTorchLayer(l.Activation);
                    _layers.Add(activation);
                    
                    inputDim = l.OutChannels;
                }
                else if (layer is Conv2D)
                {
                    var l = (Conv2D)layer;
                    var conv = Conv2d(l.InChannels, l.OutChannels, l.KernelSize, l.Stride, l.Padding, l.Dilation, bias: l.HasBias);
                    _layers.Add(conv);
                    
                    var activation = ToTorchLayer(l.Activation);
                    _layers.Add(activation);
                    
                    inputDim = l.OutChannels;
                }
                else if (layer is MaxPool1D)
                {
                    var l = (MaxPool1D)layer;
                    var pool = MaxPool1d(l.KernelSize, l.Stride, l.Padding, l.Dilation);
                    _layers.Add(pool);
                }
                else if (layer is MaxPool2D)
                {
                    var l = (MaxPool2D)layer;
                    var pool = MaxPool2d(l.KernelSize, l.Stride, l.Padding, l.Dilation);
                    _layers.Add(pool);
                }
                else if (layer is AvgPool2D)
                {
                    var l = (AvgPool2D)layer;
                    var pool = AvgPool2d(l.KernelSize, l.Stride, l.Padding);
                    _layers.Add(pool);
                }
                else if (layer is GlobalAvgPool)
                {
                    var adaptivePool = AdaptiveAvgPool2d(new long[] { 1, 1 });
                    _layers.Add(adaptivePool);
                }
                else if (layer is Flatten)
                {
                    var l = (Flatten)layer;
                    var flatten = torch.nn.Flatten(l.StartDim, l.EndDim);
                    _layers.Add(flatten);
                }
                else if (layer is Lstm)
                {
                    var l = (Lstm)layer;
                    var lstm = LSTM(l.InputSize, l.HiddenSize, l.Layers, l.HasBias, l.BatchFirst, l.DropRate, l.Bidirectional);
                    // Note: LSTM returns tuple, would need special handling
                }
                else if (layer is Dropout)
                {
                    var l = (Dropout)layer;
                    var drop = Dropout(l.Rate);
                    _layers.Add(drop);
                }
                else
                {
                    throw new NotSupportedException($"The layer type {layer.GetType().Name} is not supported.");
                }
            }
        }

        private Module<Tensor, Tensor> ToTorchLayer(Activation activation)
        {
            return activation switch
            {
                Activation.None => Identity(),
                Activation.ReLU => ReLU(),
                Activation.ELU => ELU(),

                Activation.TanH => Tanh(),
                Activation.Tanhshrink => Tanhshrink(),



                Activation.Sigmoid => Sigmoid(),
                Activation.Softmax => Softmax(dim:1),
                Activation.LogSoftmax => LogSoftmax(dim: 1),
                _ => Identity()
            };
        }
    }
}

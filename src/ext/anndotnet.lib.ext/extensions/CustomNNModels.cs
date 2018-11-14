//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                      //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using CNTK;
using NNetwork.Core.Common;
using NNetwork.Core.Network;
using NNetwork.Core.Network.Modules;
using System.Collections.Generic;
using System.Linq;

namespace ANNdotNET.Lib.Ext
{
    public static partial class CustomNNModels
    {
        /// <summary>
        /// This method is called from Desktop Application in order to run Custom network implementation. In order to change run custom 
        /// network model, call the method here.
        /// </summary>
        /// <param name="variables"></param>
        /// <param name="device"></param>
        public static Function CustomModelCallEntryPoint(List<Variable> variables, DeviceDescriptor device)
        {
            try
            {
                //Only one custom model is implemented for far
                return CustomNNModels.PredictFutureSalesModel(variables, device);
            }
            catch (System.Exception)
            {

                throw;
            }
           
        }
        /// <summary>
        /// Implementation of custom NN model
        /// </summary>
        /// <param name="data"></param>
        /// <param name="yearVar"></param>
        /// <param name="montVar"></param>
        /// <param name="shopVar"></param>
        /// <param name="itemVar"></param>
        /// <param name="cnt3Var"></param>
        /// <param name="label"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        private static Function PredictFutureSalesModel(List<Variable> variables, DeviceDescriptor device)
        {
            //define features and label vars
            Variable yearVar = variables[0];
            Variable montVar = variables[1];
            Variable shopVar = variables[2];
            Variable itemVar = variables[3];
            Variable cnt3Var = variables[4];
            Variable label   = variables[5];

            //create rnn object
            var ffNet = new FeedForwaredNN(device);

            //predefined parameters
            var H_DIMS = 11;
            var CELL_DIMS = 3;
            var DROPRATRE = 0.2f;
            var outDim = label.Shape.Dimensions.Last();

            //embedding layer and dimensionality reduction 
            var yearEmb = Embedding.Create(yearVar, yearVar.Shape.Dimensions[0] - 1, DataType.Float, device, 1, yearVar.Name+"_emb"); 
            var monthEmb = Embedding.Create(montVar, montVar.Shape.Dimensions[0]/2, DataType.Float, device, 1, montVar.Name + "_emb");
            var varshopEmb = Embedding.Create(shopVar, shopVar.Shape.Dimensions[0] / 2, DataType.Float, device, 1, shopVar.Name + "_emb");

            var itemEmb = Embedding.Create(itemVar, itemVar.Shape.Dimensions[0] / 2, DataType.Float, device, 1, itemVar.Name + "_emb");
            var itemEmb2 = Embedding.Create(itemEmb, itemEmb.Output.Shape.Dimensions[0] / 4, DataType.Float, device, 1, itemEmb.Name + "_emb");

            //join all embedding layers with input variable of previous product sales
            var emb = CNTKLib.Splice(new VariableVector() { yearEmb, monthEmb, varshopEmb, itemEmb2, cnt3Var }, new Axis(0));

            //create recurrence for time series on top of joined layer
            var lstmLayer = RNN.RecurrenceLSTM(emb,H_DIMS, CELL_DIMS,DataType.Float,device, false, Activation.TanH, true, true);

            //create dense on top of LSTM recurrence layers
            var denseLayer = ffNet.Dense(lstmLayer, 33, Activation.TanH);

            //create dropout layer on top of dense layer
            var dropoutLay = CNTKLib.Dropout(denseLayer, DROPRATRE);

            //create dense layer without activation function
            var outLayer = ffNet.Dense(dropoutLay, outDim, Activation.None, label.Name);
            // 
            return outLayer;
        }


    }
}

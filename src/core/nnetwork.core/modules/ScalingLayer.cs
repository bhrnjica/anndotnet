//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                       //
// Copyright 2017-2018 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                          //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using CNTK;
using System.Linq;
using System.Collections.Generic;

namespace NNetwork.Core.Network.Modules
{
    /// <summary>
    /// Implementation of Embedding layer. 
    /// </summary>
    public class ScalingLayer// : NetworkFoundation
    {
        /// <summary>
        /// Create scaling layer
        /// </summary>
        /// <param name="input"></param>
        /// <param name="embeddingDim"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Function Scale(Variable x, int value1, int value2, DeviceDescriptor device, string name= "scaleL")
        {
            var scaleFactor = CNTK.Constant.Scalar<float>(value1/(float)value2, device);
            var net  = (Variable)CNTK.CNTKLib.ElementTimes(scaleFactor, x);
            return net;
        }


        public static List<Function> ZScore(List<Variable> xVars, Dictionary<string, List<List<float>>> X, Dictionary<string, (float[] mean, float[] invStd)> stats, DeviceDescriptor device)
        {
            //
            var normalizedInputs = new List<Function>();
            foreach (var input in xVars)
            {
                var seqName = stats.Where(x => x.Value.mean.Length == input.Shape.Dimensions.Last()).Select(x => x.Key).FirstOrDefault();

                //
                var nd_mean = new NDArrayView(new int[] { stats[seqName].mean.Length }, stats[seqName].mean, device);
                var nd_invStd = new NDArrayView(new int[] { stats[seqName].invStd.Length }, stats[seqName].invStd, device);
                var m = new Constant(nd_mean, "mean");
                var s = new Constant(nd_invStd, "invStd");

                //
                var dif = CNTKLib.Minus(input, m);
                var fn = CNTKLib.ElementTimes(s, dif);

                //
                var normalizedinput = CNTKLib.PerDimMeanVarianceNormalize(input, nd_mean, nd_invStd, input.Name + "_norm");
                //
                normalizedInputs.Add(normalizedinput);

            }

            return normalizedInputs;
        }

        public static List<Function> MinMax(List<Variable> xVars, Dictionary<string, List<List<float>>> X, Dictionary<string, (float[] min, float[] max_minInv)> stats, DeviceDescriptor device)
        {
            //
            var normalizedInputs = new List<Function>();
            foreach (var input in xVars)
            {
                var seqName = stats.Where(x => x.Value.min.Length == input.Shape.Dimensions.Last()).Select(x => x.Key).FirstOrDefault();

                //
                var nd_min = new NDArrayView(new int[] { stats[seqName].min.Length }, stats[seqName].min, device);
                var nd_maxminInv = new NDArrayView(new int[] { stats[seqName].max_minInv.Length }, stats[seqName].max_minInv, device);

                var m = new Constant(nd_min, "min");
                var s = new Constant(nd_maxminInv, "maxminInv");

                //
                var dif = CNTKLib.Minus(input, m);
                var fn = CNTKLib.ElementTimes(s, dif);

                //
                //var normalizedinput = CNTKLib.PerDimMeanVarianceNormalize(input, nd_mean, nd_std, input.Name+"_norm");
                //
                normalizedInputs.Add(fn);

            }

            return normalizedInputs;
        }

    }
}

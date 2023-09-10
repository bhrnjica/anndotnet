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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daany.MathStuff.Stats;

namespace Anndotnet.Core.MlMetrics
{
    public class MulticlassClassifictionMetrics
    {
        public double LogLoss { get; }
        public double LogLossReduction { get; private set; }
        public double MacroAccuracy { get; }
        public double MicroAccuracy { get; }
        public double TopKAccuracy { get; }
        public int TopKPredictionCount { get; }
        public IReadOnlyList<double> TopKAccuracyForAllK { get; }
        public IReadOnlyList<double> PerClassLogLoss { get; }
        public ConfusionMatrix ConfusionMatrix { get; }
    }
}

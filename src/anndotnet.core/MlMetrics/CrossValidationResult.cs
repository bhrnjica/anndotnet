////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

namespace Anndotnet.Core.MlMetrics;

public sealed class CrossValidationResult<T> where T : class
{
    /// <summary>Metrics for this cross-validation fold.</summary>
    public readonly T Metrics;

    /// <summary>Model trained during cross-validation fold.</summary>
    public readonly object Model;

    /// <summary>The scored hold-out set for this fold.</summary>
    public readonly object ScoredHoldOutSet;

    /// <summary>Fold number.</summary>
    public readonly int Fold;
}


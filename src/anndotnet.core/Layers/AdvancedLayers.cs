////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using Anndotnet.Core.Entities;
using Anndotnet.Core.Layers;

namespace Anndotnet.Core.Layers;

/// <summary>
/// Batch normalization layer
/// </summary>
public record BatchNormalization : Base
{
    public int NumFeatures { get; set; }
    public float Eps { get; set; } = 1e-5f;
    public float Momentum { get; set; } = 0.1f;
    public bool Affine { get; set; } = true;
    public bool TrackRunningStats { get; set; } = true;

    public BatchNormalization()
    {
        Type = LayerType.BatchNormalization;
    }
}

/// <summary>
/// Layer normalization layer
/// </summary>
public record LayerNormalization : Base
{
    public int[] NormalizedShape { get; set; }
    public float Eps { get; set; } = 1e-5f;
    public bool ElementwiseAffine { get; set; } = true;

    public LayerNormalization()
    {
        Type = LayerType.LayerNormalization;
    }
}

/// <summary>
/// Convolutional 1D layer
/// </summary>
public record Conv1D : Base
{
    public int InChannels { get; set; }
    public int OutChannels { get; set; }
    public int KernelSize { get; set; }
    public int Stride { get; set; } = 1;
    public int Padding { get; set; } = 0;
    public int Dilation { get; set; } = 1;
    public bool HasBias { get; set; } = true;
    public Activation Activation { get; set; } = Activation.None;

    public Conv1D()
    {
        Type = LayerType.Conv1D;
    }
}

/// <summary>
/// Convolutional 2D layer
/// </summary>
public record Conv2D : Base
{
    public int InChannels { get; set; }
    public int OutChannels { get; set; }
    public int KernelSize { get; set; }
    public int Stride { get; set; } = 1;
    public int Padding { get; set; } = 0;
    public int Dilation { get; set; } = 1;
    public bool HasBias { get; set; } = true;
    public Activation Activation { get; set; } = Activation.None;

    public Conv2D()
    {
        Type = LayerType.Conv2D;
    }
}

/// <summary>
/// Max pooling 1D layer
/// </summary>
public record MaxPool1D : Base
{
    public int KernelSize { get; set; }
    public int Stride { get; set; }
    public int Padding { get; set; } = 0;
    public int Dilation { get; set; } = 1;
    public bool ReturnIndices { get; set; } = false;
    public bool CeilMode { get; set; } = false;

    public MaxPool1D()
    {
        Type = LayerType.MaxPool1D;
    }
}

/// <summary>
/// Max pooling 2D layer
/// </summary>
public record MaxPool2D : Base
{
    public int KernelSize { get; set; }
    public int Stride { get; set; }
    public int Padding { get; set; } = 0;
    public int Dilation { get; set; } = 1;
    public bool ReturnIndices { get; set; } = false;
    public bool CeilMode { get; set; } = false;

    public MaxPool2D()
    {
        Type = LayerType.MaxPool2D;
    }
}

/// <summary>
/// Average pooling 2D layer
/// </summary>
public record AvgPool2D : Base
{
    public int KernelSize { get; set; }
    public int Stride { get; set; }
    public int Padding { get; set; } = 0;
    public bool CountIncludePad { get; set; } = true;
    public bool CeilMode { get; set; } = false;

    public AvgPool2D()
    {
        Type = LayerType.AvgPool2D;
    }
}

/// <summary>
/// Global average pooling layer
/// </summary>
public record GlobalAvgPool : Base
{
    public GlobalAvgPool()
    {
        Type = LayerType.GlobalAvgPool;
    }
}

/// <summary>
/// Flatten layer
/// </summary>
public record Flatten : Base
{
    public int StartDim { get; set; } = 1;
    public int EndDim { get; set; } = -1;

    public Flatten()
    {
        Type = LayerType.Flatten;
    }
}

/// <summary>
/// Reshape layer
/// </summary>
public record Reshape : Base
{
    public long[] Shape { get; set; }

    public Reshape()
    {
        Type = LayerType.Reshape;
    }
}

/// <summary>
/// Attention layer (simplified)
/// </summary>
public record Attention : Base
{
    public int EmbedDim { get; set; }
    public int NumHeads { get; set; } = 1;
    public float Dropout { get; set; } = 0.0f;
    public bool HasBias { get; set; } = true;

    public Attention()
    {
        Type = LayerType.Attention;
    }
}
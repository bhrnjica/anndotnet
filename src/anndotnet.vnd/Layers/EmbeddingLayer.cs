﻿///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//                Created by anndotnet community, anndotnet.com              //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/anndotnet/anndotnet //
//                                                                           //
//             For feedback:https://github.com/anndotnet/anndotnet/issues    //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

using AnnDotNet.Core;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

namespace AnnDotNet.Vnd.Layers;

public class EmbeddingLayer : ILayer
{
    public int OutDim { get; set; }
    public string Name { get; set; }
    public LayerType Type { get; set; }
    public EmbeddingLayer() => Type = LayerType.Embedding;
}
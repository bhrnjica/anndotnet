///////////////////////////////////////////////////////////////////////////////
//               ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                           //
//            Copyright 2017-2021 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                           //
//                     Licensed under the MIT License                        //
//             See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                           //
//             For feedback:https://github.com/bhrnjica/anndotnet/issues     //
//                                                                           //
///////////////////////////////////////////////////////////////////////////////

namespace AnnDotNet.Core.Entities;

public enum EvalFunction
{
    AE,//absolute error
    MAE,//Mean absolute error
    SE,//squared error
    MSE,//mean squared error
    RMSE,//root mean squared error
    CAcc,//Classification accuracy
    CErr,//classification error
    BAcc,//binary accuracy
    BErr,//binary error
    
}

//https://neptune.ai/blog/pytorch-loss-functions
public enum LossFunction
{
    AE,//absolute error (torch.nn.L1Loss)
    MSE,//mean squared error (torch.nn.MSELoss)

    //Used only on models with the softmax function as an output activation layer.
    NLLLoss,//Negative Log-Likelihood Loss Function  (torch.nn.NLLLoss)

    BCE,//Binary Cross Entropy Loss Function (torch.nn.BCELoss)

    CCE,//Cross Entropy Loss Function (torch.nn.CrossEntropyLoss)
}
///////////////////////////////////////////////////////////////////////////////
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Anndotnet.Core.Util;
using AnnDotNet.Core.Data;
using AnnDotNet.Core.Entities;
using AnnDotNet.Core.Interfaces;

[assembly: InternalsVisibleTo("anndotnet.test")]
namespace AnnDotNet.Core.Trainers;

public class CvTrainer : ITrainer, IProgressTraining
{
    private int _currentFold;
    private readonly int _kFold;
    private readonly ITrainer[] _tvTrainer;
    private readonly IProgressTraining _progress;
    private readonly (DataLoader train, DataLoader validation)[] _cvData;

    public CvTrainer(Module<Tensor, Tensor> model, DataFeed trainData, TrainingParameters tParams, LearningParameters lParams, IProgressTraining progress, int seed = 1234)
    {
        _kFold = tParams.KFold;
        _progress = progress;
       
        _cvData = new (DataLoader train, DataLoader validation)[_kFold];
        CreateFolds(trainData, tParams.MiniBatchSize);

        _tvTrainer = new TvTrainer[_kFold];
        for (int i = 0; i < _kFold; i++)
        {
            _tvTrainer[i] = new TvTrainer(model, _cvData[i].train, _cvData[i].validation, tParams, lParams, this, seed);

        }

    }
    private void CreateFolds(DataFeed trainData,int batchSize)
    {
        float percentage = 100.0f / _kFold;

        int testSize = (int)(trainData.Count * percentage / 100.0f);
        int trainSize = (int)trainData.Count - testSize;

        //create folds
        for (int i = 0; i < _kFold; i++)
        {
            _cvData[i] = Split(trainData,batchSize ,trainSize, testSize, i);
        }

    }
    private (DataLoader train, DataLoader validation) Split(DataFeed data, int batchSize, int trainSize, int testSize, int index)
    {
        //generate indexes
        var lst = LongEnumerable.Range(0, data.Count).ToList();
        var n = index * testSize;

        var trainIds = lst.Take(n)
                .Concat(lst.Skip(n + testSize)
                .Take(lst.Count() - n - testSize)).ToList();

        var testIds = lst.Except(trainIds);

        var train = new DataLoader(data, batchSize, trainIds);
        var valid = new DataLoader(data, batchSize, testIds);
       
        return (train, valid);
    }

    public async Task<bool> RunAsync()
    {
        for (int i = 0; i < _kFold; i++)
        {
            _currentFold = i;
           await _tvTrainer[i].RunAsync();
        }
        return true;
    }

    public void Run(ProgressReport tp)
    {
        tp.Fold = _currentFold;
        tp.KFold = _kFold;
        _progress.Run(tp);
    }
}

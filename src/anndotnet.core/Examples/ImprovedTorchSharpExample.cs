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
using Anndotnet.Core.Interfaces;
using Anndotnet.Core.Layers;
using Anndotnet.Core.Mlconfig;
using Anndotnet.Core.Util;

namespace Anndotnet.Core.Examples;

/// <summary>
/// Example demonstrating improved TorchSharp features in ANNdotNET v2
/// </summary>
public class ImprovedTorchSharpExample
{
    /// <summary>
    /// Creates an advanced neural network with modern layers and techniques
    /// </summary>
    public static List<ILayer> CreateAdvancedNetwork()
    {
        return new List<ILayer>
        {
            // Input processing with batch normalization
            new Dense 
            { 
                Name = "Input", 
                OutputDim = 128, 
                HasBias = true, 
                Activation = Activation.ReLU,
                Type = LayerType.Dense
            },
            new BatchNormalization 
            { 
                Name = "BatchNorm1", 
                NumFeatures = 128,
                Type = LayerType.BatchNormalization
            },
            
            // Hidden layers with dropout for regularization
            new Dense 
            { 
                Name = "Hidden1", 
                OutputDim = 256, 
                HasBias = true, 
                Activation = Activation.ReLU,
                Type = LayerType.Dense
            },
            new Dropout 
            { 
                Name = "Dropout1", 
                Rate = 0.3f,
                Type = LayerType.Dropout
            },
            
            new Dense 
            { 
                Name = "Hidden2", 
                OutputDim = 128, 
                HasBias = true, 
                Activation = Activation.ReLU,
                Type = LayerType.Dense
            },
            new BatchNormalization 
            { 
                Name = "BatchNorm2", 
                NumFeatures = 128,
                Type = LayerType.BatchNormalization
            },
            new Dropout 
            { 
                Name = "Dropout2", 
                Rate = 0.2f,
                Type = LayerType.Dropout
            },
            
            // Output layer
            new Dense 
            { 
                Name = "Output", 
                OutputDim = 10, 
                HasBias = true, 
                Activation = Activation.Softmax,
                Type = LayerType.Dense
            }
        };
    }

    /// <summary>
    /// Creates a convolutional neural network for image processing
    /// </summary>
    public static List<ILayer> CreateConvolutionalNetwork()
    {
        return new List<ILayer>
        {
            // First convolutional block
            new Conv2D 
            { 
                Name = "Conv1", 
                InChannels = 3, 
                OutChannels = 32, 
                KernelSize = 3, 
                Stride = 1, 
                Padding = 1,
                Activation = Activation.ReLU,
                Type = LayerType.Conv2D
            },
            new BatchNormalization 
            { 
                Name = "BatchNorm1", 
                NumFeatures = 32,
                Type = LayerType.BatchNormalization
            },
            new MaxPool2D 
            { 
                Name = "MaxPool1", 
                KernelSize = 2, 
                Stride = 2,
                Type = LayerType.MaxPool2D
            },
            
            // Second convolutional block
            new Conv2D 
            { 
                Name = "Conv2", 
                InChannels = 32, 
                OutChannels = 64, 
                KernelSize = 3, 
                Stride = 1, 
                Padding = 1,
                Activation = Activation.ReLU,
                Type = LayerType.Conv2D
            },
            new BatchNormalization 
            { 
                Name = "BatchNorm2", 
                NumFeatures = 64,
                Type = LayerType.BatchNormalization
            },
            new MaxPool2D 
            { 
                Name = "MaxPool2", 
                KernelSize = 2, 
                Stride = 2,
                Type = LayerType.MaxPool2D
            },
            
            // Flatten and fully connected layers
            new GlobalAvgPool 
            { 
                Name = "GlobalAvgPool",
                Type = LayerType.GlobalAvgPool
            },
            new Flatten 
            { 
                Name = "Flatten",
                Type = LayerType.Flatten
            },
            new Dense 
            { 
                Name = "FC1", 
                OutputDim = 128, 
                HasBias = true, 
                Activation = Activation.ReLU,
                Type = LayerType.Dense
            },
            new Dropout 
            { 
                Name = "Dropout", 
                Rate = 0.5f,
                Type = LayerType.Dropout
            },
            new Dense 
            { 
                Name = "Output", 
                OutputDim = 10, 
                HasBias = true, 
                Activation = Activation.Softmax,
                Type = LayerType.Dense
            }
        };
    }

    /// <summary>
    /// Demonstrates improved training parameters with modern techniques
    /// </summary>
    public static (TrainingParameters tParams, LearningParameters lParams) CreateImprovedTrainingConfig()
    {
        var lParams = new LearningParameters
        {
            LearnerType = LearnerType.Adam,
            LossFunction = LossFunction.CCE,
            EvaluationFunctions = new List<EvalFunction> { EvalFunction.CAcc, EvalFunction.CErr },
            LearningRate = 0.001f,
            Beta1 = 0.9,
            Beta2 = 0.999,
            WeightDecay = 1e-4,
            
            // New improved features
            UseScheduler = true,
            SchedulerType = SchedulerType.CosineAnnealingLR,
            GradientClipNorm = 1.0,
            WeightInitMethod = WeightInitMethod.KaimingUniform,
            UseMixedPrecision = false // Not yet supported in current TorchSharp
        };

        var tParams = new TrainingParameters
        {
            TrainingType = TrainingType.TvTraining,
            EarlyStopping = EarlyStopping.ValidationLoss,
            EarlyStoppingPatience = 15,
            Epochs = 100,
            ProgressStep = 1,
            MiniBatchSize = 32,
            SplitPercentage = 80,
            ShuffleWhenTraining = true,
            ShuffleWhenSplit = true,
            Retrain = true,
            
            // New checkpoint features
            CheckpointPath = "./checkpoints",
            CheckpointFrequency = 10
        };

        return (tParams, lParams);
    }

    /// <summary>
    /// Demonstrates tensor utilities usage
    /// </summary>
    public static void DemonstrateTensorUtilities()
    {
        // Create sample data
        var data = new float[100, 10];
        var random = new Random(42);
        
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                data[i, j] = (float)random.NextDouble();
            }
        }

        // Create tensor with proper device placement
        using var tensor = TensorUtils.CreateTensor(data);
        
        // Normalize the data
        using var normalized = TensorUtils.MinMaxNormalize(tensor);
        
        // Split into training and validation
        var (train, validation) = TensorUtils.TrainValidationSplit(normalized, 0.8f, true, 42);
        
        using (train)
        using (validation)
        {
            Console.WriteLine($"Original shape: [{string.Join(", ", tensor.shape)}]");
            Console.WriteLine($"Train shape: [{string.Join(", ", train.shape)}]");
            Console.WriteLine($"Validation shape: [{string.Join(", ", validation.shape)}]");
            Console.WriteLine($"Memory usage: {TensorUtils.GetMemoryUsage(tensor)} bytes");
        }
    }

    /// <summary>
    /// Demonstrates model utilities usage
    /// </summary>
    public static void DemonstrateModelUtilities()
    {
        // Create a model
        var layers = CreateAdvancedNetwork();
        var device = torch.cuda.is_available() ? torch.CUDA : torch.CPU;
        var model = MlFactory.CreateNetwork("AdvancedModel", layers, 784, 10, device, WeightInitMethod.KaimingUniform);

        // Get model information
        var paramCount = ModelUtils.CountParameters(model);
        var trainableParams = ModelUtils.CountTrainableParameters(model);
        var memoryUsage = ModelUtils.GetModelMemoryUsage(model);
        var summary = ModelUtils.GetParameterSummary(model);

        Console.WriteLine($"Total parameters: {paramCount:N0}");
        Console.WriteLine($"Trainable parameters: {trainableParams:N0}");
        Console.WriteLine($"Memory usage: {memoryUsage:N0} bytes");
        Console.WriteLine($"Number of layers: {((List<Dictionary<string, object>>)summary["layers"]).Count}");

        // Demonstrate saving and loading (would need actual training context)
        // ModelUtils.SaveCheckpoint(model, optimizer, 10, 0.5f, new Dictionary<string, float> { ["accuracy"] = 0.95f }, "./model_checkpoint");

        model.Dispose();
    }
}
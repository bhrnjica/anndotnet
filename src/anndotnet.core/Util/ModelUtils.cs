////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System.Text.Json;
using TorchSharp.Modules;

namespace Anndotnet.Core.Util
{
    /// <summary>
    /// Utilities for model management, checkpointing, and optimization
    /// </summary>
    public static class ModelUtils
    {
        /// <summary>
        /// Saves a model checkpoint (simplified version)
        /// </summary>
        /// <param name="model">The model to save</param>
        /// <param name="optimizer">The optimizer to save (placeholder)</param>
        /// <param name="epoch">Current epoch</param>
        /// <param name="loss">Current loss</param>
        /// <param name="metrics">Current metrics</param>
        /// <param name="filePath">Path to save the checkpoint</param>
        public static void SaveCheckpoint(
            Module<Tensor, Tensor> model,
            torch.optim.Optimizer optimizer,
            int epoch,
            float loss,
            Dictionary<string, float> metrics,
            string filePath)
        {
            var checkpoint = new Dictionary<string, object>
            {
                ["epoch"] = epoch,
                ["loss"] = loss,
                ["metrics"] = metrics ?? new Dictionary<string, float>(),
                ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
            };

            // Save model state
            var modelStateFile = Path.ChangeExtension(filePath, ".model.dat");
            model.save(modelStateFile);

            // Note: Optimizer save/load not available in current TorchSharp version
            // var optimStateFile = Path.ChangeExtension(filePath, ".optim.dat");
            // optimizer.save(optimStateFile);

            // Save metadata
            var metadataFile = Path.ChangeExtension(filePath, ".metadata.json");
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(metadataFile, JsonSerializer.Serialize(checkpoint, jsonOptions));
        }

        /// <summary>
        /// Loads a model checkpoint (simplified version)
        /// </summary>
        /// <param name="model">The model to load state into</param>
        /// <param name="optimizer">The optimizer (placeholder)</param>
        /// <param name="filePath">Path to the checkpoint</param>
        /// <returns>Checkpoint metadata including epoch, loss, and metrics</returns>
        public static Dictionary<string, object> LoadCheckpoint(
            Module<Tensor, Tensor> model,
            torch.optim.Optimizer optimizer,
            string filePath)
        {
            // Load model state
            var modelStateFile = Path.ChangeExtension(filePath, ".model.dat");
            if (File.Exists(modelStateFile))
            {
                model.load(modelStateFile);
            }

            // Note: Optimizer save/load not available in current TorchSharp version
            // var optimStateFile = Path.ChangeExtension(filePath, ".optim.dat");
            // if (File.Exists(optimStateFile))
            // {
            //     optimizer.load(optimStateFile);
            // }

            // Load metadata
            var metadataFile = Path.ChangeExtension(filePath, ".metadata.json");
            if (File.Exists(metadataFile))
            {
                var jsonContent = File.ReadAllText(metadataFile);
                return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonContent) 
                       ?? new Dictionary<string, object>();
            }

            return new Dictionary<string, object>();
        }

        /// <summary>
        /// Counts the total number of parameters in a model
        /// </summary>
        /// <param name="model">The model to analyze</param>
        /// <returns>Total number of parameters</returns>
        public static long CountParameters(Module<Tensor, Tensor> model)
        {
            return model.parameters().Sum(p => p.numel());
        }

        /// <summary>
        /// Counts the number of trainable parameters in a model
        /// </summary>
        /// <param name="model">The model to analyze</param>
        /// <returns>Number of trainable parameters</returns>
        public static long CountTrainableParameters(Module<Tensor, Tensor> model)
        {
            return model.parameters().Where(p => p.requires_grad).Sum(p => p.numel());
        }

        /// <summary>
        /// Gets detailed parameter information for a model
        /// </summary>
        /// <param name="model">The model to analyze</param>
        /// <returns>Parameter summary</returns>
        public static Dictionary<string, object> GetParameterSummary(Module<Tensor, Tensor> model)
        {
            var parameters = model.named_parameters().ToList();
            var summary = new Dictionary<string, object>
            {
                ["total_parameters"] = CountParameters(model),
                ["trainable_parameters"] = CountTrainableParameters(model),
                ["layers"] = new List<Dictionary<string, object>>()
            };

            var layers = (List<Dictionary<string, object>>)summary["layers"];
            
            foreach (var (name, param) in parameters)
            {
                layers.Add(new Dictionary<string, object>
                {
                    ["name"] = name,
                    ["shape"] = param.shape.Select(s => (long)s).ToArray(),
                    ["parameters"] = param.numel(),
                    ["trainable"] = param.requires_grad,
                    ["dtype"] = param.dtype.ToString()
                });
            }

            return summary;
        }

        /// <summary>
        /// Applies gradient clipping to prevent exploding gradients
        /// </summary>
        /// <param name="model">The model whose gradients to clip</param>
        /// <param name="maxNorm">Maximum gradient norm</param>
        /// <param name="normType">Type of norm to use (default: 2.0 for L2 norm)</param>
        /// <returns>Total norm of the gradients</returns>
        public static double ClipGradientNorm(Module<Tensor, Tensor> model, double maxNorm, double normType = 2.0)
        {
            return torch.nn.utils.clip_grad_norm_(model.parameters(), maxNorm, normType);
        }

        /// <summary>
        /// Applies gradient clipping by value to prevent exploding gradients
        /// </summary>
        /// <param name="model">The model whose gradients to clip</param>
        /// <param name="clipValue">Maximum gradient value</param>
        public static void ClipGradientValue(Module<Tensor, Tensor> model, double clipValue)
        {
            torch.nn.utils.clip_grad_value_(model.parameters(), clipValue);
        }

        /// <summary>
        /// Freezes all parameters in a model (sets requires_grad = false)
        /// </summary>
        /// <param name="model">The model to freeze</param>
        public static void FreezeModel(Module<Tensor, Tensor> model)
        {
            foreach (var param in model.parameters())
            {
                param.requires_grad_(false);
            }
        }

        /// <summary>
        /// Unfreezes all parameters in a model (sets requires_grad = true)
        /// </summary>
        /// <param name="model">The model to unfreeze</param>
        public static void UnfreezeModel(Module<Tensor, Tensor> model)
        {
            foreach (var param in model.parameters())
            {
                param.requires_grad_(true);
            }
        }

        /// <summary>
        /// Applies weight initialization to a model
        /// </summary>
        /// <param name="model">The model to initialize</param>
        /// <param name="initMethod">Initialization method</param>
        public static void InitializeWeights(Module<Tensor, Tensor> model, Anndotnet.Core.Entities.WeightInitMethod initMethod = Anndotnet.Core.Entities.WeightInitMethod.XavierUniform)
        {
            foreach (var module in model.modules())
            {
                if (module is Linear linear)
                {
                    ApplyInitialization(linear.weight, initMethod);
                    if (linear.bias is not null)
                    {
                        torch.nn.init.zeros_(linear.bias);
                    }
                }
                else if (module is Conv2d conv)
                {
                    ApplyInitialization(conv.weight, initMethod);
                    if (conv.bias is not null)
                    {
                        torch.nn.init.zeros_(conv.bias);
                    }
                }
            }
        }

        private static void ApplyInitialization(Tensor tensor, Anndotnet.Core.Entities.WeightInitMethod method)
        {
            switch (method)
            {
                case Anndotnet.Core.Entities.WeightInitMethod.XavierUniform:
                    torch.nn.init.xavier_uniform_(tensor);
                    break;
                case Anndotnet.Core.Entities.WeightInitMethod.XavierNormal:
                    torch.nn.init.xavier_normal_(tensor);
                    break;
                case Anndotnet.Core.Entities.WeightInitMethod.KaimingUniform:
                    torch.nn.init.kaiming_uniform_(tensor);
                    break;
                case Anndotnet.Core.Entities.WeightInitMethod.KaimingNormal:
                    torch.nn.init.kaiming_normal_(tensor);
                    break;
                case Anndotnet.Core.Entities.WeightInitMethod.Normal:
                    torch.nn.init.normal_(tensor, 0.0, 0.02);
                    break;
                case Anndotnet.Core.Entities.WeightInitMethod.Uniform:
                    torch.nn.init.uniform_(tensor, -0.1, 0.1);
                    break;
                default:
                    torch.nn.init.xavier_uniform_(tensor);
                    break;
            }
        }

        /// <summary>
        /// Calculates model memory usage
        /// </summary>
        /// <param name="model">The model to analyze</param>
        /// <returns>Memory usage in bytes</returns>
        public static long GetModelMemoryUsage(Module<Tensor, Tensor> model)
        {
            long totalMemory = 0;
            foreach (var param in model.parameters())
            {
                totalMemory += param.numel() * param.element_size();
            }
            return totalMemory;
        }
    }
}
////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

namespace Anndotnet.Core.Util
{
    /// <summary>
    /// Tensor operation utilities for better performance and memory management
    /// </summary>
    public static class TensorUtils
    {
        /// <summary>
        /// Safely concatenates tensors with memory management
        /// </summary>
        /// <param name="tensors">List of tensors to concatenate</param>
        /// <param name="dimension">Dimension along which to concatenate</param>
        /// <returns>Concatenated tensor</returns>
        public static Tensor SafeConcatenate(IEnumerable<Tensor> tensors, int dimension = 0)
        {
            var tensorList = tensors.ToList();
            if (!tensorList.Any())
                throw new ArgumentException("No tensors provided for concatenation");

            if (tensorList.Count == 1)
                return torch.clone(tensorList[0]);

            return torch.cat(tensorList, dimension);
        }

        /// <summary>
        /// Creates a tensor from data with proper device placement
        /// </summary>
        /// <param name="data">Data array</param>
        /// <param name="device">Target device</param>
        /// <param name="dtype">Data type</param>
        /// <returns>Tensor on the specified device</returns>
        public static Tensor CreateTensor(float[] data, Device device = null, ScalarType dtype = ScalarType.Float32)
        {
            var tensor = torch.tensor(data, dtype: dtype);
            if (device != null)
            {
                tensor = tensor.to(device);
            }
            return tensor;
        }

        /// <summary>
        /// Creates a tensor from 2D data with proper device placement
        /// </summary>
        /// <param name="data">2D data array</param>
        /// <param name="device">Target device</param>
        /// <param name="dtype">Data type</param>
        /// <returns>Tensor on the specified device</returns>
        public static Tensor CreateTensor(float[,] data, Device device = null, ScalarType dtype = ScalarType.Float32)
        {
            var tensor = torch.tensor(data, dtype: dtype);
            if (device != null)
            {
                tensor = tensor.to(device);
            }
            return tensor;
        }

        /// <summary>
        /// Safely reshapes a tensor with validation
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="shape">Target shape</param>
        /// <returns>Reshaped tensor</returns>
        public static Tensor SafeReshape(Tensor tensor, params long[] shape)
        {
            var originalSize = tensor.numel();
            var targetSize = shape.Aggregate(1L, (a, b) => a * b);
            
            if (originalSize != targetSize)
            {
                throw new ArgumentException($"Cannot reshape tensor of size {originalSize} to shape [{string.Join(", ", shape)}] (size {targetSize})");
            }
            
            return tensor.reshape(shape);
        }

        /// <summary>
        /// Normalizes a tensor using min-max normalization
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="min">Minimum value for normalization range</param>
        /// <param name="max">Maximum value for normalization range</param>
        /// <returns>Normalized tensor</returns>
        public static Tensor MinMaxNormalize(Tensor tensor, float min = 0.0f, float max = 1.0f)
        {
            using var tensorMin = tensor.min();
            using var tensorMax = tensor.max();
            using var tensorRange = tensorMax - tensorMin;
            
            var normalized = (tensor - tensorMin) / tensorRange;
            return normalized * (max - min) + min;
        }

        /// <summary>
        /// Standardizes a tensor using z-score normalization
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="dim">Dimension along which to compute statistics</param>
        /// <returns>Standardized tensor</returns>
        public static Tensor Standardize(Tensor tensor, int? dim = null)
        {
            Tensor mean, std;
            
            if (dim.HasValue)
            {
                mean = tensor.mean(new long[] { dim.Value }, keepdim: true);
                std = tensor.std(new long[] { dim.Value }, keepdim: true);
            }
            else
            {
                mean = tensor.mean();
                std = tensor.std();
            }
            
            using (mean)
            using (std)
            {
                return (tensor - mean) / (std + 1e-8f);
            }
        }

        /// <summary>
        /// Applies one-hot encoding to a tensor of class indices
        /// </summary>
        /// <param name="tensor">Input tensor with class indices</param>
        /// <param name="numClasses">Number of classes</param>
        /// <returns>One-hot encoded tensor</returns>
        public static Tensor OneHotEncode(Tensor tensor, int numClasses)
        {
            if (tensor.dtype != ScalarType.Int64)
            {
                tensor = tensor.to_type(ScalarType.Int64);
            }
            
            return torch.nn.functional.one_hot(tensor, numClasses).to_type(ScalarType.Float32);
        }

        /// <summary>
        /// Splits a tensor into training and validation sets
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="trainRatio">Ratio of data for training (0.0 to 1.0)</param>
        /// <param name="shuffle">Whether to shuffle before splitting</param>
        /// <param name="seed">Random seed for reproducibility</param>
        /// <returns>Tuple of (train_tensor, validation_tensor)</returns>
        public static (Tensor train, Tensor validation) TrainValidationSplit(
            Tensor tensor, 
            float trainRatio = 0.8f, 
            bool shuffle = true, 
            int seed = 42)
        {
            if (trainRatio <= 0.0f || trainRatio >= 1.0f)
                throw new ArgumentException("Train ratio must be between 0 and 1");

            var totalSamples = tensor.shape[0];
            var trainSamples = (long)(totalSamples * trainRatio);
            
            Tensor indices = torch.arange(totalSamples, dtype: ScalarType.Int64);
            
            if (shuffle)
            {
                torch.random.manual_seed(seed);
                indices = indices[torch.randperm(totalSamples)];
            }
            
            var trainIndices = indices[torch.arange(trainSamples)];
            var validIndices = indices[torch.arange(trainSamples, totalSamples)];
            
            using (indices)
            using (trainIndices)
            using (validIndices)
            {
                var trainTensor = tensor.index_select(0, trainIndices);
                var validTensor = tensor.index_select(0, validIndices);
                
                return (trainTensor, validTensor);
            }
        }

        /// <summary>
        /// Computes accuracy between predictions and targets
        /// </summary>
        /// <param name="predictions">Model predictions</param>
        /// <param name="targets">True targets</param>
        /// <param name="topK">For top-k accuracy (default 1)</param>
        /// <returns>Accuracy as a float</returns>
        public static float ComputeAccuracy(Tensor predictions, Tensor targets, int topK = 1)
        {
            if (topK == 1)
            {
                using var predictedClasses = predictions.argmax(1);
                using var correct = predictedClasses.eq(targets);
                return correct.sum().ToSingle() / targets.shape[0];
            }
            else
            {
                using var topKPreds = predictions.topk(topK, dim: 1).indexes;
                using var targetsExpanded = targets.unsqueeze(1).expand_as(topKPreds);
                using var correct = topKPreds.eq(targetsExpanded).any(dim: 1);
                return correct.sum().ToSingle() / targets.shape[0];
            }
        }

        /// <summary>
        /// Applies dropout manually for custom implementations
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="rate">Dropout rate (0.0 to 1.0)</param>
        /// <param name="training">Whether in training mode</param>
        /// <returns>Tensor with dropout applied</returns>
        public static Tensor ApplyDropout(Tensor tensor, float rate, bool training = true)
        {
            if (!training || rate <= 0.0f)
                return tensor;

            if (rate >= 1.0f)
                return torch.zeros_like(tensor);

            using var mask = torch.rand_like(tensor).gt(rate);
            return tensor * mask.to_type(tensor.dtype) / (1.0f - rate);
        }

        /// <summary>
        /// Calculates tensor memory usage in bytes
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <returns>Memory usage in bytes</returns>
        public static long GetMemoryUsage(Tensor tensor)
        {
            return tensor.numel() * tensor.element_size();
        }

        /// <summary>
        /// Safely moves tensor to device with memory management
        /// </summary>
        /// <param name="tensor">Input tensor</param>
        /// <param name="device">Target device</param>
        /// <returns>Tensor on target device</returns>
        public static Tensor SafeToDevice(Tensor tensor, Device device)
        {
            if (tensor.device.Equals(device))
                return tensor;
                
            return tensor.to(device);
        }
    }
}
////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using TorchSharp;

namespace Anndotnet.Core.Entities
{
    /// <summary>
    /// Configuration for performance optimizations
    /// </summary>
    public class PerformanceConfig
    {
        /// <summary>
        /// Enable mixed precision training (if supported)
        /// </summary>
        public bool EnableMixedPrecision { get; set; } = false;

        /// <summary>
        /// Enable gradient clipping for stability
        /// </summary>
        public bool EnableGradientClipping { get; set; } = false;

        /// <summary>
        /// Maximum gradient norm for clipping
        /// </summary>
        public float GradientClippingThreshold { get; set; } = 1.0f;

        /// <summary>
        /// Enable memory optimization techniques
        /// </summary>
        public bool EnableMemoryOptimization { get; set; } = true;

        /// <summary>
        /// Clear GPU cache every N epochs
        /// </summary>
        public int MemoryCleanupFrequency { get; set; } = 10;

        /// <summary>
        /// Use asynchronous data transfer
        /// </summary>
        public bool UseAsyncDataTransfer { get; set; } = true;

        /// <summary>
        /// Tensor caching for repeated operations
        /// </summary>
        public bool EnableTensorCaching { get; set; } = true;

        /// <summary>
        /// Preferred device for training
        /// </summary>
        public Device PreferredDevice { get; set; }

        /// <summary>
        /// Enable GPU warmup before training
        /// </summary>
        public bool EnableGpuWarmup { get; set; } = true;

        /// <summary>
        /// Pre-allocate tensors for metrics calculation
        /// </summary>
        public bool PreallocateMetricTensors { get; set; } = true;

        /// <summary>
        /// Default performance configuration
        /// </summary>
        public static PerformanceConfig Default => new()
        {
            EnableMixedPrecision = false, // Conservative default
            EnableGradientClipping = false,
            EnableMemoryOptimization = true,
            MemoryCleanupFrequency = 10,
            UseAsyncDataTransfer = true,
            EnableTensorCaching = true,
            EnableGpuWarmup = true,
            PreallocateMetricTensors = true
        };

        /// <summary>
        /// High performance configuration for powerful hardware
        /// </summary>
        public static PerformanceConfig HighPerformance => new()
        {
            EnableMixedPrecision = true,
            EnableGradientClipping = true,
            GradientClippingThreshold = 0.5f,
            EnableMemoryOptimization = true,
            MemoryCleanupFrequency = 5,
            UseAsyncDataTransfer = true,
            EnableTensorCaching = true,
            EnableGpuWarmup = true,
            PreallocateMetricTensors = true
        };

        /// <summary>
        /// Memory conservative configuration for limited hardware
        /// </summary>
        public static PerformanceConfig MemoryConservative => new()
        {
            EnableMixedPrecision = false,
            EnableGradientClipping = false,
            EnableMemoryOptimization = true,
            MemoryCleanupFrequency = 1, // Clean every epoch
            UseAsyncDataTransfer = false,
            EnableTensorCaching = false,
            EnableGpuWarmup = false,
            PreallocateMetricTensors = false
        };
    }
}
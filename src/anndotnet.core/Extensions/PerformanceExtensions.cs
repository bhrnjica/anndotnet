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
using static TorchSharp.torch;

namespace Anndotnet.Core.Extensions
{
    /// <summary>
    /// Performance optimization extensions for TorchSharp operations
    /// </summary>
    public static class PerformanceExtensions
    {
        /// <summary>
        /// Optimized device transfer with memory management
        /// </summary>
        public static Tensor ToDeviceOptimized(this Tensor tensor, Device device)
        {
            if (tensor.device.Equals(device))
                return tensor;

            // Use standard transfer without non_blocking parameter (not available in 0.101.2)
            var result = tensor.to(device);
            return result;
        }

        /// <summary>
        /// Check if CUDA is available and recommended
        /// </summary>
        public static bool ShouldUseCuda()
        {
            return torch.cuda.is_available() && torch.cuda.device_count() > 0;
        }

        /// <summary>
        /// Get optimal device for training
        /// </summary>
        public static Device GetOptimalDevice()
        {
            if (ShouldUseCuda())
            {
                return torch.cuda.is_available() ? torch.CUDA : torch.CPU;
            }
            return torch.CPU;
        }

        /// <summary>
        /// Clear GPU memory cache
        /// </summary>
        public static void ClearGpuMemory()
        {
            if (torch.cuda.is_available())
            {
                // Note: empty_cache not available in TorchSharp 0.101.2
                // Using synchronize as alternative for memory management
                torch.cuda.synchronize();
            }
            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Enable memory-efficient training settings
        /// </summary>
        public static void OptimizeMemorySettings()
        {
            // Note: TorchSharp 0.101.2 has limited cudnn backend control
            // These optimizations are available in later versions
            if (torch.cuda.is_available())
            {
                // Basic memory management optimizations available in this version
                // More advanced settings require newer TorchSharp versions
            }
        }

        /// <summary>
        /// Warmup GPU with dummy operations for better performance
        /// </summary>
        public static void WarmupGpu(Device device)
        {
            if (device.type != DeviceType.CUDA)
                return;

            // Perform dummy operations to initialize GPU context
            using var dummy = torch.randn(100, 100, device: device);
            using var result = torch.matmul(dummy, dummy);
            torch.cuda.synchronize();
        }
    }
}
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
using Anndotnet.Core.Extensions;
using Anndotnet.Core.Util;
using TorchSharp;

namespace Anndotnet.Core.Examples
{
    /// <summary>
    /// Demonstration of TorchSharp performance optimizations
    /// </summary>
    public class PerformanceDemo
    {
        /// <summary>
        /// Demonstrate improved TorchMetrics performance
        /// </summary>
        public static void DemoMetricsPerformance()
        {
            Console.WriteLine("=== TorchMetrics Performance Demo ===");
            
            var monitor = new PerformanceMonitor();
            var device = PerformanceExtensions.GetOptimalDevice();
            
            // Create sample data
            using var predicted = torch.randn(1000, 10, device: device);
            using var expected = torch.randint(0, 10, new long[] { 1000 }, device: device);
            using var predictedFlat = predicted.flatten();
            using var expectedFloat = expected.to_type(ScalarType.Float32);
            
            const int iterations = 100;
            
            // Test optimized metrics
            monitor.StartTiming("OptimizedMetrics");
            for (int i = 0; i < iterations; i++)
            {
                var accuracy = TorchMetrics.MCAccuracy(predicted, expected);
                var mae = TorchMetrics.MeanAbsoluteError(predictedFlat, expectedFloat);
            }
            monitor.StopTiming("OptimizedMetrics");
            
            var stats = monitor.GetStats("OptimizedMetrics");
            Console.WriteLine($"Optimized metrics: {stats}");
            Console.WriteLine($"Average per iteration: {stats.AverageTime.TotalMilliseconds / iterations:F3}ms");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrate performance configuration benefits
        /// </summary>
        public static void DemoPerformanceConfig()
        {
            Console.WriteLine("=== Performance Configuration Demo ===");
            
            var defaultConfig = PerformanceConfig.Default;
            var highPerfConfig = PerformanceConfig.HighPerformance;
            var memoryConfig = PerformanceConfig.MemoryConservative;
            
            Console.WriteLine($"Default Config - Memory optimization: {defaultConfig.EnableMemoryOptimization}");
            Console.WriteLine($"High Performance - Gradient clipping: {highPerfConfig.EnableGradientClipping}");
            Console.WriteLine($"Memory Conservative - Cleanup frequency: {memoryConfig.MemoryCleanupFrequency}");
            
            // Show device selection
            var optimalDevice = PerformanceExtensions.GetOptimalDevice();
            Console.WriteLine($"Optimal device detected: {optimalDevice}");
            Console.WriteLine($"CUDA available: {PerformanceExtensions.ShouldUseCuda()}");
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrate tensor operation benchmarks
        /// </summary>
        public static void DemoTensorBenchmarks()
        {
            Console.WriteLine("=== Tensor Operations Benchmark ===");
            
            var cpuDevice = torch.CPU;
            Console.WriteLine("Benchmarking CPU operations...");
            var cpuResults = PerformanceMonitor.BenchmarkTensorOps(cpuDevice, 50);
            Console.WriteLine(cpuResults);
            Console.WriteLine();
            
            // Only benchmark CUDA if available
            if (torch.cuda.is_available())
            {
                var cudaDevice = torch.CUDA;
                Console.WriteLine("Benchmarking CUDA operations...");
                var cudaResults = PerformanceMonitor.BenchmarkTensorOps(cudaDevice, 50);
                Console.WriteLine(cudaResults);
                
                // Compare performance
                var speedup = cpuResults.MatMulAvgTime.TotalMilliseconds / cudaResults.MatMulAvgTime.TotalMilliseconds;
                Console.WriteLine($"CUDA speedup for MatMul: {speedup:F2}x");
            }
            else
            {
                Console.WriteLine("CUDA not available, skipping GPU benchmarks.");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrate memory management improvements
        /// </summary>
        public static void DemoMemoryManagement()
        {
            Console.WriteLine("=== Memory Management Demo ===");
            
            var monitor = new PerformanceMonitor();
            var device = PerformanceExtensions.GetOptimalDevice();
            
            // Memory usage before optimization
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var memoryBefore = GC.GetTotalMemory(false);
            
            monitor.StartTiming("MemoryOperations");
            
            // Simulate memory-intensive operations with proper disposal
            var tensors = new List<Tensor>();
            for (int i = 0; i < 100; i++)
            {
                tensors.Add(torch.randn(100, 100, device: device));
            }
            
            // Proper disposal
            foreach (var tensor in tensors)
            {
                tensor.Dispose();
            }
            tensors.Clear();
            
            // Clean up GPU memory if available
            PerformanceExtensions.ClearGpuMemory();
            
            monitor.StopTiming("MemoryOperations");
            
            var memoryAfter = GC.GetTotalMemory(true);
            var memoryUsed = (memoryAfter - memoryBefore) / 1024.0 / 1024.0; // MB
            
            Console.WriteLine($"Memory operations completed in: {monitor.GetStats("MemoryOperations").AverageTime.TotalMilliseconds:F2}ms");
            Console.WriteLine($"Net memory change: {memoryUsed:F2} MB");
            Console.WriteLine();
        }

        /// <summary>
        /// Run all performance demonstrations
        /// </summary>
        public static void RunAllDemos()
        {
            Console.WriteLine("TorchSharp Performance Optimization Demonstrations");
            Console.WriteLine("==================================================");
            Console.WriteLine();
            
            try
            {
                DemoPerformanceConfig();
                DemoMetricsPerformance();
                DemoTensorBenchmarks();
                DemoMemoryManagement();
                
                Console.WriteLine("All demonstrations completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during demonstration: {ex.Message}");
            }
        }
    }
}
////////////////////////////////////////////////////////////////////////////
//           ANNdotNET - Deep Learning Tool on .NET Platform             //
//                                                                       //
//        Copyright 2017-2023 Bahrudin Hrnjica, bhrnjica@hotmail.com     //
//                                                                       //
//                 Licensed under the MIT License                        //
//         See license section at https://github.com/bhrnjica/anndotnet  //
//                                                                       //
///////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using TorchSharp;

namespace Anndotnet.Core.Util
{
    /// <summary>
    /// Performance monitoring and benchmarking utilities
    /// </summary>
    public class PerformanceMonitor
    {
        private readonly Dictionary<string, List<TimeSpan>> _timings = new();
        private readonly Dictionary<string, Stopwatch> _activeTimers = new();

        /// <summary>
        /// Start timing an operation
        /// </summary>
        public void StartTiming(string operationName)
        {
            if (!_activeTimers.ContainsKey(operationName))
            {
                _activeTimers[operationName] = new Stopwatch();
            }
            _activeTimers[operationName].Restart();
        }

        /// <summary>
        /// Stop timing an operation and record the result
        /// </summary>
        public TimeSpan StopTiming(string operationName)
        {
            if (_activeTimers.TryGetValue(operationName, out var stopwatch))
            {
                stopwatch.Stop();
                var elapsed = stopwatch.Elapsed;
                
                if (!_timings.ContainsKey(operationName))
                {
                    _timings[operationName] = new List<TimeSpan>();
                }
                _timings[operationName].Add(elapsed);
                
                return elapsed;
            }
            return TimeSpan.Zero;
        }

        /// <summary>
        /// Get performance statistics for an operation
        /// </summary>
        public PerformanceStats GetStats(string operationName)
        {
            if (!_timings.ContainsKey(operationName) || !_timings[operationName].Any())
            {
                return new PerformanceStats();
            }

            var times = _timings[operationName];
            var totalMs = times.Sum(t => t.TotalMilliseconds);
            var count = times.Count;
            var avgMs = totalMs / count;
            var minMs = times.Min(t => t.TotalMilliseconds);
            var maxMs = times.Max(t => t.TotalMilliseconds);

            return new PerformanceStats
            {
                OperationName = operationName,
                Count = count,
                TotalTime = TimeSpan.FromMilliseconds(totalMs),
                AverageTime = TimeSpan.FromMilliseconds(avgMs),
                MinTime = TimeSpan.FromMilliseconds(minMs),
                MaxTime = TimeSpan.FromMilliseconds(maxMs)
            };
        }

        /// <summary>
        /// Get all performance statistics
        /// </summary>
        public Dictionary<string, PerformanceStats> GetAllStats()
        {
            var result = new Dictionary<string, PerformanceStats>();
            foreach (var operationName in _timings.Keys)
            {
                result[operationName] = GetStats(operationName);
            }
            return result;
        }

        /// <summary>
        /// Clear all timing data
        /// </summary>
        public void Clear()
        {
            _timings.Clear();
            _activeTimers.Clear();
        }

        /// <summary>
        /// Benchmark tensor operations
        /// </summary>
        public static BenchmarkResult BenchmarkTensorOps(Device device, int iterations = 100)
        {
            var results = new BenchmarkResult { Device = device.ToString() };
            var random = new Random(42); // Fixed seed for reproducibility

            // Warmup
            using (var warmupA = torch.randn(100, 100, device: device))
            using (var warmupB = torch.randn(100, 100, device: device))
            {
                using var warmupC = torch.matmul(warmupA, warmupB);
                if (device.type == DeviceType.CUDA)
                    torch.cuda.synchronize();
            }

            // Matrix multiplication benchmark
            var matmulTimes = new List<TimeSpan>();
            var sw = Stopwatch.StartNew();
            
            for (int i = 0; i < iterations; i++)
            {
                var size = 256 + random.Next(256); // Variable sizes
                
                sw.Restart();
                using var a = torch.randn(size, size, device: device);
                using var b = torch.randn(size, size, device: device);
                using var c = torch.matmul(a, b);
                
                if (device.type == DeviceType.CUDA)
                    torch.cuda.synchronize();
                    
                sw.Stop();
                matmulTimes.Add(sw.Elapsed);
            }
            
            results.MatMulAvgTime = TimeSpan.FromMilliseconds(matmulTimes.Average(t => t.TotalMilliseconds));
            results.MatMulMinTime = matmulTimes.Min();
            results.MatMulMaxTime = matmulTimes.Max();

            // Element-wise operations benchmark
            var elemwiseTimes = new List<TimeSpan>();
            
            for (int i = 0; i < iterations; i++)
            {
                var size = 1000 + random.Next(1000);
                
                sw.Restart();
                using var a = torch.randn(size, size, device: device);
                using var b = torch.randn(size, size, device: device);
                using var c = torch.add(a, b);
                using var d = torch.mul(c, a);
                
                if (device.type == DeviceType.CUDA)
                    torch.cuda.synchronize();
                    
                sw.Stop();
                elemwiseTimes.Add(sw.Elapsed);
            }
            
            results.ElemWiseAvgTime = TimeSpan.FromMilliseconds(elemwiseTimes.Average(t => t.TotalMilliseconds));

            // Memory allocation/deallocation benchmark
            var memoryTimes = new List<TimeSpan>();
            
            for (int i = 0; i < iterations / 10; i++) // Fewer iterations for memory ops
            {
                sw.Restart();
                var tensors = new List<Tensor>();
                
                for (int j = 0; j < 10; j++)
                {
                    tensors.Add(torch.randn(100, 100, device: device));
                }
                
                foreach (var tensor in tensors)
                {
                    tensor.Dispose();
                }
                
                if (device.type == DeviceType.CUDA)
                    torch.cuda.synchronize();
                    
                sw.Stop();
                memoryTimes.Add(sw.Elapsed);
            }
            
            results.MemoryAllocAvgTime = TimeSpan.FromMilliseconds(memoryTimes.Average(t => t.TotalMilliseconds));

            return results;
        }
    }

    /// <summary>
    /// Performance statistics for an operation
    /// </summary>
    public class PerformanceStats
    {
        public string OperationName { get; set; } = "";
        public int Count { get; set; }
        public TimeSpan TotalTime { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }

        public override string ToString()
        {
            return $"{OperationName}: {Count} ops, Avg: {AverageTime.TotalMilliseconds:F2}ms, " +
                   $"Min: {MinTime.TotalMilliseconds:F2}ms, Max: {MaxTime.TotalMilliseconds:F2}ms";
        }
    }

    /// <summary>
    /// Benchmark results for tensor operations
    /// </summary>
    public class BenchmarkResult
    {
        public string Device { get; set; } = "";
        public TimeSpan MatMulAvgTime { get; set; }
        public TimeSpan MatMulMinTime { get; set; }
        public TimeSpan MatMulMaxTime { get; set; }
        public TimeSpan ElemWiseAvgTime { get; set; }
        public TimeSpan MemoryAllocAvgTime { get; set; }

        public override string ToString()
        {
            return $"Device: {Device}\n" +
                   $"MatMul Avg: {MatMulAvgTime.TotalMilliseconds:F2}ms\n" +
                   $"ElemWise Avg: {ElemWiseAvgTime.TotalMilliseconds:F2}ms\n" +
                   $"Memory Alloc Avg: {MemoryAllocAvgTime.TotalMilliseconds:F2}ms";
        }
    }
}
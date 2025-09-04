using Anndotnet.Core.Examples;

Console.WriteLine("Starting TorchSharp Performance Optimization Demonstrations...");
Console.WriteLine();

try
{
    PerformanceDemo.RunAllDemos();
}
catch (Exception ex)
{
    Console.WriteLine($"Demo failed with error: {ex.Message}");
    Console.WriteLine("This may be expected in environments without GPU support.");
}

Console.WriteLine();
Console.WriteLine("Performance demonstrations completed!");
Console.WriteLine("See TORCHSHARP_PERFORMANCE_OPTIMIZATIONS.md for detailed information.");
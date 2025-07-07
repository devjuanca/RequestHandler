using BenchmarkDotNet.Running;
using Benchmarks.Benchmarks;

_ = BenchmarkRunner.Run<RequestHandlerBenchmarks>();

Console.ReadLine();
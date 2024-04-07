using BenchmarkDotNet.Running;
using MediatorBenchmark;

var summary1 = BenchmarkRunner.Run<MediatorBenchmarkClass>();

Console.ReadLine();

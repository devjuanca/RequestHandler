using BenchmarkDotNet.Attributes;
using CustomMediator;
using MediatorBenchmark.Handlers;
using MediatorBenchmark.Handlers.CustomMediator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorBenchmark;

[MemoryDiagnoser(true)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class MediatorBenchmarkClass
{
    private readonly IServiceProvider ServiceProvider;

    public MediatorBenchmarkClass()
    {
        var services = new ServiceCollection();

        services.AddMediatR(a => a.RegisterServicesFromAssembly(typeof(MediatorBenchmarkClass).Assembly));

        services.AddRequestHandlers(typeof(MediatorBenchmarkClass));

        ServiceProvider = services.BuildServiceProvider();
    }


    [Benchmark]
    public async Task UsingMediatr()
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        var weather = await mediator.Send(new CityRequest { CityName = "London" }, CancellationToken.None);
    }

    [Benchmark]
    public async Task UsingCustomMediator()
    {
        var handler = ServiceProvider.GetRequiredService<WeatherForecastHandler>();

        var weather = await handler.HandleAsync("London", CancellationToken.None);
    }

}

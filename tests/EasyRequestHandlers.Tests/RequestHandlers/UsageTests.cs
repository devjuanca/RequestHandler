using Microsoft.Extensions.DependencyInjection;
using EasyRequestHandlers.Request;
using Xunit;

namespace EasyRequestHandlers.Tests.RequestHandlers;

public class UsageTests
{
    // Test data and handlers - same as in RegistrationTests for simplicity
    public class TestRequest { public int Value { get; set; } }

    public class TestResponse { public int Result { get; set; } }
    
    public class TestRequestHandler : RequestHandler<TestRequest, TestResponse>
    {
        public override Task<TestResponse> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new TestResponse { Result = request.Value * 2 });
        }
    }

    public class NoInputTestHandler : RequestHandler<TestResponse>
    {
        public override Task<TestResponse> HandleAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new TestResponse { Result = 42 });
        }
    }

    [Fact]
    public async Task DirectHandler_ProcessesRequest()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddEasyRequestHandlers(typeof(UsageTests))
                .Build();

        var provider = services.BuildServiceProvider();

        var handler = provider.GetRequiredService<TestRequestHandler>();
        
        // Act
        var result = await handler.HandleAsync(new TestRequest { Value = 21 });
        
        // Assert
        Assert.Equal(42, result.Result);
    }

    [Fact]
    public async Task NoInputHandler_ReturnsResponse()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEasyRequestHandlers(typeof(UsageTests))
                .Build();
        var provider = services.BuildServiceProvider();
        var handler = provider.GetRequiredService<NoInputTestHandler>();
        
        // Act
        var result = await handler.HandleAsync();
        
        // Assert
        Assert.Equal(42, result.Result);
    }

    [Fact]
    public async Task Sender_ProcessesRequest()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddEasyRequestHandlers(typeof(UsageTests))
                .WithMediatorPattern()
                .Build();

        var provider = services.BuildServiceProvider();

        var sender = provider.GetRequiredService<ISender>();
        
        // Act
        var result = await sender.SendAsync<TestRequest, TestResponse>(new TestRequest { Value = 21 });
        
        // Assert
        Assert.Equal(42, result.Result);
    }

    [Fact]
    public async Task Sender_ProcessesNoInputRequest()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddEasyRequestHandlers(typeof(UsageTests))
                .WithMediatorPattern()
                .Build();

        var provider = services.BuildServiceProvider();

        var sender = provider.GetRequiredService<ISender>();
        
        // Act
        var result = await sender.SendAsync<TestResponse>();
        
        // Assert
        Assert.Equal(42, result.Result);
    }
}
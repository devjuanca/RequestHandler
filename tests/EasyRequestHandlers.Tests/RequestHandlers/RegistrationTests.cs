using Microsoft.Extensions.DependencyInjection;
using EasyRequestHandlers.Request;
using Xunit;

namespace EasyRequestHandlers.Tests.RequestHandlers;

public class RegistrationTests
{
    // Test data and handlers
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
    public void AddEasyRequestHandlers_RegistersHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddEasyRequestHandlers(typeof(RegistrationTests))
                .Build();
        var provider = services.BuildServiceProvider();
        
        // Assert
        var handler = provider.GetService<TestRequestHandler>();
        Assert.NotNull(handler);
        
        var noInputHandler = provider.GetService<NoInputTestHandler>();
        Assert.NotNull(noInputHandler);
    }

    [Fact]
    public void WithMediatorPattern_RegistersISender()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddEasyRequestHandlers(typeof(RegistrationTests))
                .WithMediatorPattern()
                .Build();
        var provider = services.BuildServiceProvider();
        
        // Assert
        var sender = provider.GetService<ISender>();
        Assert.NotNull(sender);
    }
}
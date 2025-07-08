using Microsoft.Extensions.DependencyInjection;
using EasyRequestHandlers.Events;
using Xunit;

namespace EasyRequestHandlers.Tests.Events;

public class RegistrationTests
{
    // Test event and handlers
    public class TestEvent 
    { 
        public string Message { get; set; } = string.Empty;
    }
    
    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            // Just a simple handler for testing registration
            return Task.CompletedTask;
        }
    }

    public class AnotherTestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            // Another handler for the same event
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void AddEasyEventHandlers_RegistersHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddEasyEventHandlers(typeof(RegistrationTests));
        var provider = services.BuildServiceProvider();
        
        // Assert
        var handlers = provider.GetServices<IEventHandler<TestEvent>>();
        Assert.Equal(2, handlers.Count());
    }

    [Fact]
    public void AddEasyEventHandlers_RegistersEventPublisher()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act
        services.AddEasyEventHandlers(typeof(RegistrationTests));

        services.AddLogging();

        var provider = services.BuildServiceProvider();
        
        // Assert
        var publisher = provider.GetService<IEventPublisher>();
        Assert.NotNull(publisher);
    }
}
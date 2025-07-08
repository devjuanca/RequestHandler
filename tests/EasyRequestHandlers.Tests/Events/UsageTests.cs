using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EasyRequestHandlers.Events;
using Xunit;

namespace EasyRequestHandlers.Tests.Events;

public class UsageTests
{
    // Test event and handlers with tracking
    public class TestEvent 
    { 
        public string Message { get; set; } = string.Empty;
    }
    
    public class TrackingEventHandler : IEventHandler<TestEvent>
    {
        public static bool WasHandled { get; set; }
        
        public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken)
        {
            WasHandled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task EventPublisher_InvokesHandlers()
    {
        // Arrange
        TrackingEventHandler.WasHandled = false;
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        
        // Use the extension method instead of direct registration
        services.AddEasyEventHandlers(typeof(UsageTests));
        
        // Register your handler (or could use the scanning in AddEasyEventHandlers)
        services.AddSingleton<IEventHandler<TestEvent>, TrackingEventHandler>();
        
        var provider = services.BuildServiceProvider();

        var publisher = provider.GetRequiredService<IEventPublisher>();
        
        // Act
        await publisher.PublishAsync(new TestEvent { Message = "Test message" });
        
        // Assert
        Assert.True(TrackingEventHandler.WasHandled);
    }

    [Fact]
    public async Task EventPublisher_WithParallelExecution_InvokesHandlers()
    {
        // Arrange
        TrackingEventHandler.WasHandled = false;
        
        var services = new ServiceCollection();
        
        services.AddLogging(builder => builder.AddConsole());

        services.AddEasyEventHandlers(typeof(UsageTests));

        services.AddSingleton<IEventHandler<TestEvent>, TrackingEventHandler>();
        
        var provider = services.BuildServiceProvider();
        
        var publisher = provider.GetRequiredService<IEventPublisher>();
        
        // Act
        await publisher.PublishAsync(new TestEvent { Message = "Test message" }, useParallelExecution: true);
        
        // Assert
        Assert.True(TrackingEventHandler.WasHandled);
    }
}
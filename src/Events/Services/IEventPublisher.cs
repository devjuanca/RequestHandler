using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RequestHandlers.Events.Services
{
    /// <summary>
    /// Provides methods for publishing events to registered handlers.
    /// </summary>
    public interface IEventPublisher
    {

        /// <summary>
        /// Publishes an event to all registered handlers asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being published.</typeparam>
        /// <param name="event">The event instance to be published.</param>
        /// <param name="useParallelExecution">Determines if the handlers should run simultaneously. Default: `false`</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PublishAsync<TEvent>(TEvent @event, bool useParallelExecution = false, CancellationToken cancellationToken = default) where TEvent : class;
    }

    /// <summary>
    /// Default implementation of the <see cref="IEventPublisher"/> interface.
    /// Handles the publishing of events by resolving handlers from the service provider and invoking them.
    /// </summary>

    internal sealed class EventPublisher : IEventPublisher
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<EventPublisher> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="scopeFactory">The factory used to create service scopes.</param>
        /// <param name="logger">The logger instance used to log messages.</param>
        public EventPublisher(IServiceScopeFactory scopeFactory, ILogger<EventPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Publishes an event to all registered handlers asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event being published.</typeparam>
        /// <param name="event">The event instance to be published.</param>
        /// <param name="useParallelExecution">Determines if the handlers should run simultaneously. Default: `false`</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PublishAsync<TEvent>(TEvent @event, bool useParallelExecution = false, CancellationToken cancellationToken = default) where TEvent : class
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var tasks = new List<Task>();

                var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>().ToArray();

                foreach (var handler in handlers)
                {
                    var handleTask = handler.HandleAsync(@event, cancellationToken);

                    if (useParallelExecution)
                    {
                        tasks.Add(handleTask);
                    }
                    else
                    {
                        try
                        {
                            await handleTask.ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "An error occurred while executing an event handler.");
                        }
                    }
                }

                if (tasks.Count > 0)
                {
                    await Task.WhenAll(tasks).ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            foreach (var ex in t.Exception.InnerExceptions)
                            {
                                _logger.LogError(ex, "An error ocurred while executing an event handler");
                            }
                        }
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some exception was catched");
            }
        }
    }
}
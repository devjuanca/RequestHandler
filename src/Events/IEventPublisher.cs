using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandlers.Events
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

                var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>().ToArray();

                var nonTransactionalHandlers = handlers.Where(a => !(a is ITransactionalEventHandler<TEvent>)).ToArray();

                var transactionalHandlers = handlers.Where(a => a is ITransactionalEventHandler<TEvent>).Select(a => a as ITransactionalEventHandler<TEvent>).OrderBy(a => a.Order).ToArray();

                if (nonTransactionalHandlers.Length > 0)
                {
                    await HandleNonTransactionalEvents(@event, nonTransactionalHandlers, useParallelExecution, cancellationToken);
                }

                if (transactionalHandlers.Length > 0)
                {
                    await HandleTransactionalEvents(@event, transactionalHandlers, cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Some exception was catched");
            }
        }

        private async Task HandleNonTransactionalEvents<TEvent>(TEvent @event, IReadOnlyList<IEventHandler<TEvent>> handlers, bool useParallelExecution, CancellationToken cancellationToken = default) where TEvent : class
        {
            var tasks = new List<Task>();

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

        private async Task HandleTransactionalEvents<TEvent>(TEvent @event, IReadOnlyList<ITransactionalEventHandler<TEvent>> handlers, CancellationToken cancellationToken = default) where TEvent : class
        {
            var executedTransactionalHandlers = new Stack<ITransactionalEventHandler<TEvent>>();

            try
            {
                foreach (var handler in handlers)
                {
                    executedTransactionalHandlers.Push(handler);

                    await handler.HandleAsync(@event, cancellationToken).ConfigureAwait(false);
                }

                while (executedTransactionalHandlers.Count > 0)
                {
                    var handler = executedTransactionalHandlers.Pop();

                    await handler.CommitAsync(@event, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {

                foreach (var handler in executedTransactionalHandlers.Reverse())
                {
                    await handler.RollbackAsync(@event, cancellationToken).ConfigureAwait(false);
                }

                _logger.LogError(ex, "An error occurred during event handling. All Handlers Rollback were executed.");
            }
        }
    }
}
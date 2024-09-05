using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandlers.Events
{
    public interface ITransactionalEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : class
    {
        /// <summary>
        /// Gets the order in which this handler should be executed relative to other handlers.
        /// Handlers with lower values are executed earlier.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Commits the changes made during the event handling process.
        /// </summary>
        /// <param name="event">The event instance that was handled.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitAsync(TEvent @event, CancellationToken cancellationToken);

        /// <summary>
        /// Rolls back the changes made during the event handling process.
        /// </summary>
        /// <param name="event">The event instance that was handled.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RollbackAsync(TEvent @event, CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandler.Events
{

    /// <summary>
    /// Defines a handler for a specific event type. 
    /// Implementations of this interface contain the logic to process events of type <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to handle.</typeparam>
    public interface IEventHandler<TEvent> where TEvent : class
    {

        /// <summary>
        /// Handles the event asynchronously.
        /// </summary>
        /// <param name="event">The event instance to be handled.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }
}

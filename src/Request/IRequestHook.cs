using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandlers.Request
{
    public interface IRequestHook<TRequest, TResponse>
    {
        Task OnExecutingAsync(TRequest request, CancellationToken cancellationToken = default);

        Task OnExecutedAsync(TRequest request, TResponse response, CancellationToken cancellationToken = default);
    }

    public interface IRequestPreHook<TRequest>
    {
        Task OnExecutingAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public interface IRequestPostHook<TRequest, TResponse>
    {
        Task OnExecutedAsync(TRequest request, TResponse response, CancellationToken cancellationToken = default);
    }
}
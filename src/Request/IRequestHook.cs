using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandlers.Request
{
    public interface IRequestHook<TRequest, TResponse>
    {
        Task OnExecutingAsync(TRequest request, CancellationToken cancellationToken);

        Task OnExecutedAsync(TRequest request, TResponse response, CancellationToken cancellationToken);
    }
}

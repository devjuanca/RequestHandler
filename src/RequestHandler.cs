using System.Threading;
using System.Threading.Tasks;

namespace CustomMediator
{
    public abstract class RequestHandler<TRequest, TResponse> : BaseHandler
    {
        public abstract ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    public abstract class RequestHandler<TResponse> : BaseHandler
    {
        public abstract ValueTask<TResponse> HandleAsync(CancellationToken cancellationToken = default);
    }
}

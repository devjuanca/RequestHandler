using EasyRequestHandlers.Request;
using MediatR;

namespace Benchmarks.Scenarios.SimpleRequest;

// Shared response type
public class SimpleResponse
{
    public int Result { get; set; }
}

// MediatR implementations
public class MediatRSimpleRequest : IRequest<SimpleResponse>
{
    public int Value { get; set; }
}

public class MediatRSimpleRequestHandler : IRequestHandler<MediatRSimpleRequest, SimpleResponse>
{
    public Task<SimpleResponse> Handle(MediatRSimpleRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SimpleResponse { Result = request.Value * 2 });
    }
}

// EasyRequestHandler implementations
public class EasySimpleRequest
{
    public int Value { get; set; }
}

public class EasySimpleRequestHandler : RequestHandler<EasySimpleRequest, SimpleResponse>
{
    public override Task<SimpleResponse> HandleAsync(EasySimpleRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new SimpleResponse { Result = request.Value * 2 });
    }
}
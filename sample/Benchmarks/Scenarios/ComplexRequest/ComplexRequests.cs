using EasyRequestHandlers.Request;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Benchmarks.Scenarios.ComplexRequest;

// Shared response type
public class ComplexResponse
{
    public string ProcessedData { get; set; } = string.Empty;
    public DateTime ProcessedTime { get; set; }
    public List<string> Tags { get; set; } = new();
}

// MediatR implementations
public class MediatRComplexRequest : IRequest<ComplexResponse>
{
    public string Data { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class MediatRComplexRequestHandler : IRequestHandler<MediatRComplexRequest, ComplexResponse>
{
    private readonly ILogger<MediatRComplexRequestHandler> _logger;

    public MediatRComplexRequestHandler(ILogger<MediatRComplexRequestHandler> logger)
    {
        _logger = logger;
    }

    public Task<ComplexResponse> Handle(MediatRComplexRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing complex request");
        
        // Simulate work
        var processedData = request.Data.ToUpperInvariant();
        var tags = request.Metadata.Values.ToList();
        
        var response = new ComplexResponse
        {
            ProcessedData = processedData,
            ProcessedTime = DateTime.UtcNow,
            Tags = tags
        };
        
        return Task.FromResult(response);
    }
}

// EasyRequestHandler implementations
public class EasyComplexRequest
{
    public string Data { get; set; } = string.Empty;
    public Dictionary<string, string> Metadata { get; set; } = new();
}

public class EasyComplexRequestHandler : RequestHandler<EasyComplexRequest, ComplexResponse>
{
    private readonly ILogger<EasyComplexRequestHandler> _logger;

    public EasyComplexRequestHandler(ILogger<EasyComplexRequestHandler> logger)
    {
        _logger = logger;
    }

    public override Task<ComplexResponse> HandleAsync(EasyComplexRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing complex request");
        
        // Simulate work
        var processedData = request.Data.ToUpperInvariant();
        var tags = request.Metadata.Values.ToList();
        
        var response = new ComplexResponse
        {
            ProcessedData = processedData,
            ProcessedTime = DateTime.UtcNow,
            Tags = tags
        };
        
        return Task.FromResult(response);
    }
}
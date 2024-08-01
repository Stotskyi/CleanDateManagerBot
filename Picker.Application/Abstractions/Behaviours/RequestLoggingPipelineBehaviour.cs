using MediatR;
using Microsoft.Extensions.Logging;
using Picker.Domain.Abstarctions;
using Serilog.Context;

namespace Picker.Application.Abstractions.Behaviours;

internal sealed class RequestLoggingPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class where TResponse : Result
{
    private readonly ILogger<RequestLoggingPipelineBehaviour<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehaviour(ILogger<RequestLoggingPipelineBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        _logger.LogInformation($"Processing request {requestName}");

        TResponse result = await next();

        if (result.IsSuccess)
        {
            _logger.LogInformation($"Completed request {requestName}");
        }
        else
        {
            using (LogContext.PushProperty("Error", result.Error, true))
            {
                _logger.LogInformation($"Completed request {requestName} with error");
            }
        }

        return result;
    }
}
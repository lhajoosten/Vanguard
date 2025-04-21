using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Vanguard.Application.Common.Services;

namespace Vanguard.Application.Common.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PerformanceBehavior(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next(cancellationToken);

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            // Log all requests that take more than 500ms
            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId > 0 ? _currentUserService.UserId.ToString() : "Anonymous";

                _logger.LogWarning("Long running request: {RequestName} ({ElapsedMilliseconds} milliseconds) for user {UserId}",
                    requestName, elapsedMilliseconds, userId);
            }

            return response;
        }
    }
}
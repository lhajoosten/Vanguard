using MediatR;
using Microsoft.Extensions.Logging;
using Vanguard.Application.Common.Interfaces;
using Vanguard.Core.Interfaces;

namespace Vanguard.Application.Common.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(
            ILogger<TransactionBehavior<TRequest, TResponse>> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Skip transaction for queries or commands that don't require it
            if (request is IReadOnlyRequest)
            {
                return await next(cancellationToken);
            }

            var requestName = typeof(TRequest).Name;

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                _logger.LogInformation("Beginning transaction for {RequestName}", requestName);

                var response = await next(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Committed transaction for {RequestName}", requestName);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling transaction for {RequestName}", requestName);

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogInformation("Rolled back transaction for {RequestName}", requestName);

                throw;
            }
        }
    }
}
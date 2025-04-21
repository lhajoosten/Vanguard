using FluentValidation;
using MediatR;
using Vanguard.Core.Results;
using ValidationException = FluentValidation.ValidationException;

namespace Vanguard.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next(cancellationToken);
            }

            // Run all validators
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // Collect validation failures
            var failures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                // Group validation errors by property
                var errorsByProperty = failures
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());

                // Create error message
                var errorMessage = string.Join("; ", errorsByProperty.Select(kvp =>
                    $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));

                // If the response is a Result, return a failure result
                if (typeof(TResponse).IsGenericType &&
                    (typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) ||
                     typeof(TResponse) == typeof(Result)))
                {
                    if (typeof(TResponse) == typeof(Result))
                    {
                        return (TResponse)(object)Result.Failure(errorMessage);
                    }
                    else
                    {
                        var genericType = typeof(TResponse).GetGenericArguments()[0];
                        var method = typeof(Result).GetMethod("Failure")?.MakeGenericMethod(genericType);
                        return (TResponse)method?.Invoke(null, [errorMessage])!;
                    }
                }

                // Otherwise throw validation exception
                throw new ValidationException(failures);
            }

            return await next(cancellationToken);
        }
    }
}
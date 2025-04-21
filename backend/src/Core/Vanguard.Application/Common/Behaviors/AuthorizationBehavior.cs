using MediatR;
using Vanguard.Application.Common.Authorization;
using Vanguard.Application.Common.Services;
using Vanguard.Core.Results;

namespace Vanguard.Application.Common.Behaviors
{
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthorizeService _authorizationService;

        public AuthorizationBehavior(
            ICurrentUserService currentUserService,
            IAuthorizeService authorizationService)
        {
            _currentUserService = currentUserService;
            _authorizationService = authorizationService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if the request implements IRequireAuthorization
            if (request is IRequireAuthorization authRequest)
            {
                var userId = _currentUserService.UserId;

                // Check if user is authenticated with a valid ID
                if (!_currentUserService.IsAuthenticated || userId <= 0)
                {
                    // If the response is a Result, return unauthorized
                    if (typeof(TResponse).IsGenericType &&
                        (typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) ||
                         typeof(TResponse) == typeof(Result)))
                    {
                        var unauthorizedMethod = typeof(Result).GetMethod("Failure");
                        if (typeof(TResponse) == typeof(Result))
                        {
                            return (TResponse)unauthorizedMethod!.Invoke(null, ["Unauthorized"]!)!;
                        }
                        else
                        {
                            var genericType = typeof(TResponse).GetGenericArguments()[0];
                            var unauthorizedGenericMethod = unauthorizedMethod!.MakeGenericMethod(genericType);
                            return (TResponse)unauthorizedGenericMethod.Invoke(null, ["Unauthorized"])!;
                        }
                    }
                    // Otherwise throw exception
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                foreach (var permission in authRequest.RequiredPermissions)
                {
                    if (!await _authorizationService.HasPermissionAsync(userId, permission, cancellationToken))
                    {
                        // If the response is a Result, return forbidden
                        if (typeof(TResponse).IsGenericType &&
                            (typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>) ||
                             typeof(TResponse) == typeof(Result)))
                        {
                            var forbiddenMethod = typeof(Result).GetMethod("Failure");
                            if (typeof(TResponse) == typeof(Result))
                            {
                                return (TResponse)forbiddenMethod!.Invoke(null, ["Forbidden"])!;
                            }
                            else
                            {
                                var genericType = typeof(TResponse).GetGenericArguments()[0];
                                var forbiddenGenericMethod = forbiddenMethod!.MakeGenericMethod(genericType);
                                return (TResponse)forbiddenGenericMethod.Invoke(null, ["Forbidden"])!;
                            }
                        }

                        // Otherwise throw exception
                        throw new UnauthorizedAccessException($"User does not have the required permission: {permission.Name}");
                    }
                }
            }

            // Continue with the request
            return await next(cancellationToken);
        }
    }
}
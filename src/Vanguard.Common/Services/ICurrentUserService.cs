using Vanguard.Common.Base;

namespace Vanguard.Common.Services
{
    public interface ICurrentUserService
    {
        UserId GetUserId();
        bool IsAuthenticated();
        bool IsInRole(string role);
    }
}

namespace Vanguard.Application.Common.Services
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Email { get; }
        string DisplayName { get; }
        bool IsAuthenticated { get; }
    }
}

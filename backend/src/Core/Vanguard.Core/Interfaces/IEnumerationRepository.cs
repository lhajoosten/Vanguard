using Vanguard.Core.Base;

namespace Vanguard.Core.Interfaces
{
    /// <summary>
    /// Repository interface for working with enumeration types.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    public interface IEnumerationRepository<T> where T : Enumeration
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<T> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}

namespace Vanguard.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for queries that don't modify data and don't need a transaction
    /// </summary>
    public interface IReadOnlyRequest { }
}
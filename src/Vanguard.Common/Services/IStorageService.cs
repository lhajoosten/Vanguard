namespace Vanguard.Common.Services
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType);
        Task<Stream> GetFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }
}

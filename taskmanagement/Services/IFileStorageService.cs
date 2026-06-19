using taskmanagement.Models.Entities;

namespace taskmanagement.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(Stream fileStream, string contentType, CancellationToken ct = default);

        Task<Stream> OpenReadAsync(string storageName, CancellationToken ct = default);

        Task<bool> ExistsAsync(string storageName, CancellationToken ct = default);

        Task DeleteAsync(string storageName, CancellationToken ct = default);
    }
}

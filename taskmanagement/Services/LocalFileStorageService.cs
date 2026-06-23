using Microsoft.Extensions.Options;
using taskmanagement.Options;

namespace taskmanagement.Services
{
        public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _root;

        public LocalFileStorageService(IOptions<FileStorageOptions> options)
        {
            _root = options.Value.RootPath;

            if (!Directory.Exists(_root))
                Directory.CreateDirectory(_root);
        }

        public async Task<string> SaveAsync(Stream fileStream, string contentType, CancellationToken ct = default)
        {
            var storageName = $"{Guid.NewGuid()}{GetExtensionFromContentType(contentType)}";
            var fullPath = Path.Combine(_root, storageName);

            using var output = File.Create(fullPath);
            await fileStream.CopyToAsync(output, ct);

            return storageName;
        }

        public Task<Stream> OpenReadAsync(string storageName, CancellationToken ct = default)
        {
            var path = Path.Combine(_root, storageName);
            Stream stream = File.OpenRead(path);
            return Task.FromResult(stream);
        }

        public Task<bool> ExistsAsync(string storageName, CancellationToken ct = default)
        {
            var path = Path.Combine(_root, storageName);
            return Task.FromResult(File.Exists(path));
        }

        public Task DeleteAsync(string storageName, CancellationToken ct = default)
        {
            var path = Path.Combine(_root, storageName);
            if (File.Exists(path))
                File.Delete(path);

            return Task.CompletedTask;
        }

        private string GetExtensionFromContentType(string contentType)
        {
            return contentType switch
            {
                "application/pdf" => ".pdf",
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                _ => ".bin"
            };
        }
    }
}


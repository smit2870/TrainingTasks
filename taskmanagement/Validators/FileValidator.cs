using Microsoft.Extensions.Options;
using taskmanagement.Options;
public class FileValidator
{
    private readonly FileStorageOptions _options;

    public FileValidator(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }

    public void Validate(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("Empty file not allowed");

        if (file.Length > _options.MaxFileSizeBytes)
            throw new Exception("File too large");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!_options.AllowedExtensions.Contains(ext))
            throw new Exception("Unsupported file type");
    }
}
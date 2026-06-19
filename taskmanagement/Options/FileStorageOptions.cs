namespace taskmanagement.Options
{
    public class FileStorageOptions
    {
        public string RootPath { get; set; } = default!;
        public long MaxFileSizeBytes { get; set; }
        public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    }
}

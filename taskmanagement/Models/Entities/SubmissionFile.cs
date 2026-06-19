namespace taskmanagement.Models.Entities
{
    public class SubmissionFile
    {
        public int Id { get; set; }

        public int SubmissionId { get; set; }
        public Submission Submission { get; set; } = default!;

        public string OriginalFileName { get; set; } = default!;
        public string StorageName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }
        public string Checksum { get; set; } = default!;

        public string UploadedBy { get; set; } = default!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
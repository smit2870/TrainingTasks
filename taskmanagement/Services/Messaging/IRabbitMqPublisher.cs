using taskmanagement.Models.Messaging;

namespace taskmanagement.Services
{
    public interface IRabbitMqPublisher
    {
        Task<bool> PublishAsync(SubmissionProcessingRequested message);
    }
}

using taskmanagement.Models.Messaging;

namespace SubmissionProcessor.Services;

public interface ISubmissionProcessingService
{
    Task ProcessAsync(SubmissionProcessingRequested message, CancellationToken cancellationToken);
}
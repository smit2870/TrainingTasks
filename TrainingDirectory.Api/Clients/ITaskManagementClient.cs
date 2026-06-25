using TrainingDirectory.Api.Models;

namespace TrainingDirectory.Api.Clients
{
    public interface ITaskManagementClient
    {
        Task<TraineeDto?> GetTraineeById(int id);
    }
}
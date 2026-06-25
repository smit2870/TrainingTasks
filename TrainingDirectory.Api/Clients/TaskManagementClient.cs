using System.Net.Http.Json;
using TrainingDirectory.Api.Models;

namespace TrainingDirectory.Api.Clients
{
    public class TaskManagementClient : ITaskManagementClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TaskManagementClient> _logger;

        public TaskManagementClient(HttpClient httpClient, ILogger<TaskManagementClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<TraineeDto?> GetTraineeById(int id)
        {
            

            var response = await _httpClient.GetAsync($"/api/Trainees/temp/{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Request failed with status {Status}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<TraineeDto>();
            
        }
    }
}
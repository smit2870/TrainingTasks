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

        public async Task<TraineeDto?> GetTraineeById(int id,string token, CancellationToken cancellationToken = default)
        {
            
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);

            var correlationId = Guid.NewGuid().ToString();
            _httpClient.DefaultRequestHeaders.Remove("X-Correlation-Id");
            _httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);

            try
            {
                var response = await _httpClient.GetAsync($"/api/trainees/{id}", cancellationToken);

                // --------------- Handling failure 
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(" *** Request failed. Status: {Status}, Body: {Body}",response.StatusCode, body);

                    // ---------------- Do NOT retry for client errors
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                        return null;
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<TraineeDto>(cancellationToken: cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning(" *** Request timed out");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " *** Error calling TaskManagement API");
            }

            // ------------------ Fallback
            _logger.LogError(" *** Request failed for traineeId={Id}", id);

            return new TraineeDto
            {
                Id = id,
                FirstName = "Unavailable",
                LastName = "",
                Email = "N/A",
                TechStack = "N/A",
                Status = "ServiceDown"
            };

        }

    }
}
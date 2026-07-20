using System.Net.Http.Headers;
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

        public async Task<TraineeDto?> GetTraineeById(int id, string token, CancellationToken cancellationToken = default)
        {
            var correlationId = Guid.NewGuid().ToString();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/trainees/{id}");

                request.Headers.Add("X-Correlation-Id", correlationId);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var cleanToken = token;

                    if (cleanToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        cleanToken = cleanToken["Bearer ".Length..].Trim();
                    }

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cleanToken);
                }

                using var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync(cancellationToken);

                    _logger.LogWarning("Request failed. Status: {Status}, Body: {Body}, CorrelationId: {CorrelationId}", response.StatusCode, body, correlationId);

                    // ---------------- Do NOT retry for client errors
                    if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                    {
                        return null;
                    }
                }
                else
                {
                    return await response.Content.ReadFromJsonAsync<TraineeDto>(cancellationToken: cancellationToken);
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(ex, "Request timed out for traineeId={Id}, CorrelationId={CorrelationId}", id, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling TaskManagement API for traineeId={Id}, CorrelationId={CorrelationId}", id, correlationId);
            }

            // ------------------ Fallback
            _logger.LogError("Request failed for traineeId={Id}, CorrelationId={CorrelationId}", id, correlationId);

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
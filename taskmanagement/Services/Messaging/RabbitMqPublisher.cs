using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using taskmanagement.Models.Messaging;

namespace taskmanagement.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IConfiguration _config;
        private readonly ILogger<RabbitMqPublisher> _logger;
        private readonly IRabbitMqConnection _connection;

        public RabbitMqPublisher(IConfiguration config, ILogger<RabbitMqPublisher> logger,IRabbitMqConnection connection)
        {
            _config = config;
            _logger = logger;
            _connection = connection;
        }

        public async Task<bool> PublishAsync(SubmissionProcessingRequested message)
        {
            try
            {
                
                var channel = await _connection.GetConnection().CreateChannelAsync();

                var queueName = _config["RabbitMq:QueueName"]!;

                await channel.QueueDeclareAsync(
                    queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "dlx-exchange" }
                    }

                );

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                var props = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: queueName,
                    mandatory: false,
                    basicProperties: props,
                    body: body
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ publish failed");
                return false;
            }
        }
    }
}
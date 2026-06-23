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

        public RabbitMqPublisher(IConfiguration config, ILogger<RabbitMqPublisher> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> PublishAsync(SubmissionProcessingRequested message)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = _config["RabbitMq:Host"],
                    Port = int.Parse(_config["RabbitMq:Port"]!),
                    UserName = _config["RabbitMq:UserName"],
                    Password = _config["RabbitMq:Password"],
                    VirtualHost = _config["RabbitMq:VirtualHost"]
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                var queueName = _config["RabbitMq:QueueName"]!;

                await channel.QueueDeclareAsync(
                    queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false
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
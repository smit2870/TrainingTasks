using RabbitMQ.Client;

namespace taskmanagement.Services
{
    public class RabbitMqConnection : IRabbitMqConnection, IDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqConnection(IConfiguration config)
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMq:Host"],
                Port = int.Parse(config["RabbitMq:Port"]!),
                UserName = config["RabbitMq:UserName"],
                Password = config["RabbitMq:Password"],
                VirtualHost = config["RabbitMq:VirtualHost"]
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public IConnection GetConnection() => _connection;

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
using RabbitMQ.Client;

namespace taskmanagement.Services
{
    public interface IRabbitMqConnection
    {
        IConnection GetConnection();
    }
}
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace SubmissionProcessor;

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;

    private IConnection _connection;
    private IChannel _channel;

    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _config = config;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _config["RabbitMq:Host"],
            Port = int.Parse(_config["RabbitMq:Port"] ?? "5672"),
            UserName = _config["RabbitMq:Username"],
            Password = _config["RabbitMq:Password"],
            VirtualHost = _config["RabbitMq:VirtualHost"] ?? "/"
        };

        _connection = await factory.CreateConnectionAsync();

        _channel = await _connection.CreateChannelAsync();

        var queue = _config["RabbitMq:QueueName"] ?? "default-queue";

        await _channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
        );

        await base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queue = _config["RabbitMq:QueueName"] ?? "default-queue";

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                var message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);

                _logger.LogInformation(
                    "Processing SubmissionId: {SubmissionId}, FileId: {FileId}",
                    message?.SubmissionId,
                    message?.FileId
                );

                await Task.Delay(2000);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed");

                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsumeAsync(
            queue: queue,
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("Worker started listening to queue: {Queue}", queue);

        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using taskmanagement.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using taskmanagement.Models.Messaging;
using SubmissionProcessor.Services;

namespace SubmissionProcessor;

public class Worker : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConnection? _connection;
    private IChannel? _channel;

    public Worker(
        IConfiguration config,
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMq:Host"],
            Port = int.Parse(_config["RabbitMq:Port"] ?? "5672"),
            UserName = _config["RabbitMq:Username"],
            Password = _config["RabbitMq:Password"],
            VirtualHost = _config["RabbitMq:VirtualHost"] ?? "/"
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var queue = _config["RabbitMq:QueueName"]!;

        await _channel.QueueDeclareAsync(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "dlx-exchange" }
            },
            cancellationToken: stoppingToken);

        
        await _channel.ExchangeDeclareAsync(
            exchange: "dlx-exchange",
            type: ExchangeType.Fanout
        );

        await _channel.QueueDeclareAsync(
            queue: "submission-processing-dlq",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        await _channel.QueueBindAsync(
            queue: "submission-processing-dlq",
            exchange: "dlx-exchange",
            routingKey: ""
        );

        await _channel.BasicQosAsync(0, 1, false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            SubmissionProcessingRequested? message = null;

            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.LogInformation("  ---  Message received: {Json}", json);

                message = JsonSerializer.Deserialize<SubmissionProcessingRequested>(json);

                if (message == null)
                    throw new Exception("Invalid message");

                using var scope = _scopeFactory.CreateScope();

                var processor = scope.ServiceProvider
                    .GetRequiredService<ISubmissionProcessingService>();
                await processor.ProcessAsync(message, stoppingToken);

                await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);

                _logger.LogInformation("  ----  Processed message {MessageId}", message.MessageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " ***  Processing failed");

                if (_channel != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var job = await db.ProcessingJobs
                        .FirstOrDefaultAsync(x => x.MessageId == message!.MessageId);

                    bool shouldRetry = job != null && job.Attempts < 3;

                    _logger.LogWarning("  ****  Retry decision for --- MsgId: {MsgId}, Attempt: {Attempt}, Retry: {Retry}",message?.MessageId, job?.Attempts, shouldRetry);

                    await _channel.BasicNackAsync(
                        ea.DeliveryTag,
                        false,
                        requeue: shouldRetry,
                        stoppingToken
                    );

                }
            }
        };

        await _channel.BasicConsumeAsync(queue, false, consumer, stoppingToken);

        _logger.LogInformation("  *** Listening to queue: {Queue}", queue);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync(cancellationToken);

        if (_connection != null)
            await _connection.CloseAsync(cancellationToken);

        await base.StopAsync(cancellationToken);
    }
}
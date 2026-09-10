using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using FCG.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FCG.Infrastructure.Services;

public sealed class SqsNotificationPublisher : INotificationPublisher
{
    private readonly IAmazonSQS? _sqsClient;
    private readonly string? _queueUrl;
    private readonly ILogger<SqsNotificationPublisher> _logger;

    public SqsNotificationPublisher(
        IConfiguration configuration,
        ILogger<SqsNotificationPublisher> logger,
        IAmazonSQS? sqsClient = null)
    {
        _logger = logger;
        _sqsClient = sqsClient;
        _queueUrl = configuration["Sqs:NotificationsQueueUrl"]
            ?? configuration["Sqs__NotificationsQueueUrl"];
    }

    public async Task PublishOrderPaidAsync(
        Guid orderId,
        Guid userId,
        IEnumerable<Guid> gameIds,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_queueUrl) || _sqsClient is null)
        {
            _logger.LogWarning(
                "Publicacao de notificacao ignorada: fila SQS nao configurada ou cliente SQS indisponivel (OrderId: {OrderId}).",
                orderId);
            return;
        }

        try
        {
            var payload = new
            {
                orderId,
                userId,
                gameIds = gameIds.ToList(),
                correlationId = Guid.NewGuid().ToString(),
                timestamp = DateTime.UtcNow
            };

            var messageBody = JsonSerializer.Serialize(payload);

            var request = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody
            };

            var response = await _sqsClient.SendMessageAsync(request, cancellationToken);

            _logger.LogInformation(
                "Notificacao do pedido {OrderId} publicada com sucesso no SQS (MessageId: {MessageId}).",
                orderId,
                response.MessageId);
        }
        catch (Exception ex)
        {
            // O envio de notificacao nao pode interromper nem reverter a conclusao do pagamento
            _logger.LogError(
                ex,
                "Falha ao enviar mensagem de notificacao para a fila SQS para o pedido {OrderId}.",
                orderId);
        }
    }
}

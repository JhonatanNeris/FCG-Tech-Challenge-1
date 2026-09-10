namespace FCG.Application.Interfaces;

public interface INotificationPublisher
{
    Task PublishOrderPaidAsync(Guid orderId, Guid userId, IEnumerable<Guid> gameIds, CancellationToken cancellationToken = default);
}

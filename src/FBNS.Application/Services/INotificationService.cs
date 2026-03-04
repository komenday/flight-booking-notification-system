using FBNS.Application.Models;

namespace FBNS.Application.Services;

public interface INotificationService
{
    Task HandleReservationCreatedAsync(ReservationCreatedEvent @event, CancellationToken cancellationToken);

    Task HandleReservationConfirmedAsync(ReservationConfirmedEvent @event, CancellationToken cancellationToken);

    Task HandleReservationCancelledAsync(ReservationCancelledEvent @event, CancellationToken cancellationToken);

    Task HandleReservationExpiredAsync(ReservationExpiredEvent @event, CancellationToken cancellationToken);
}
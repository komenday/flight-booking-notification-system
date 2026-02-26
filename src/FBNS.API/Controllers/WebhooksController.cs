using FBNS.API.DTOs;
using FBNS.Application.Models;
using FBNS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FBNS.API.Controllers;

[ApiController]
[Route("webhooks")]
[Produces("application/json")]
public class WebhooksController(
    INotificationService notificationService,
    ILogger<WebhooksController> logger)
    : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<WebhooksController> _logger = logger;

    [HttpPost("reservation-created")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReservationCreated([FromBody] ReservationCreatedEventDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received ReservationCreated webhook for {ReservationId}", dto.ReservationId);

        var reservationCreatedEvent = new ReservationCreatedEvent(
            dto.ReservationId,
            dto.FlightId,
            dto.FlightNumber,
            dto.SeatNumber,
            dto.PassengerFirstName,
            dto.PassengerLastName,
            dto.PassengerEmail,
            dto.ExpiresAt,
            dto.OccurredAt);

        await _notificationService.HandleReservationCreatedAsync(reservationCreatedEvent, cancellationToken);

        return Ok(new { message = "Notification sent successfully" });
    }

    [HttpPost("reservation-confirmed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReservationConfirmed([FromBody] ReservationConfirmedEventDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received ReservationConfirmed webhook for {ReservationId}", dto.ReservationId);

        var reservationConfirmedEvent = new ReservationConfirmedEvent(
            dto.ReservationId,
            dto.FlightId,
            dto.FlightNumber,
            dto.SeatNumber,
            dto.PassengerFirstName,
            dto.PassengerLastName,
            dto.PassengerEmail,
            dto.ConfirmedAt,
            dto.OccurredAt);

        await _notificationService.HandleReservationConfirmedAsync(reservationConfirmedEvent, cancellationToken);

        return Ok(new { message = "Notification sent successfully" });
    }

    [HttpPost("reservation-cancelled")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReservationCancelled([FromBody] ReservationCancelledEventDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received ReservationCancelled webhook for {ReservationId}", dto.ReservationId);

        var reservationCancelledEvent = new ReservationCancelledEvent(
            dto.ReservationId,
            dto.FlightId,
            dto.FlightNumber,
            dto.SeatNumber,
            dto.PassengerFirstName,
            dto.PassengerLastName,
            dto.PassengerEmail,
            dto.OccurredAt);

        await _notificationService.HandleReservationCancelledAsync(reservationCancelledEvent, cancellationToken);

        return Ok(new { message = "Notification sent successfully" });
    }

    [HttpPost("reservation-expired")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReservationExpired([FromBody] ReservationExpiredEventDto dto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received ReservationExpired webhook for {ReservationId}", dto.ReservationId);

        var reservationExpiredEvent = new ReservationExpiredEvent(
            dto.ReservationId,
            dto.FlightId,
            dto.FlightNumber,
            dto.SeatNumber,
            dto.PassengerFirstName,
            dto.PassengerLastName,
            dto.PassengerEmail,
            dto.OccurredAt);

        await _notificationService.HandleReservationExpiredAsync(reservationExpiredEvent, cancellationToken);

        return Ok(new { message = "Notification sent successfully" });
    }
}
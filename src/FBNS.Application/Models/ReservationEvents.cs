namespace FBNS.Application.Models;

public record ReservationCreatedEvent(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime ExpiresAt,
    DateTime OccurredAt);

public record ReservationConfirmedEvent(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime ConfirmedAt,
    DateTime OccurredAt);

public record ReservationCancelledEvent(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime OccurredAt);

public record ReservationExpiredEvent(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime OccurredAt);

namespace FBNS.API.DTOs;

public record ReservationConfirmedEventDto(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime ConfirmedAt,
    DateTime OccurredAt
) : ReservationEventDto(
    ReservationId,
    FlightId,
    FlightNumber,
    SeatNumber,
    PassengerFirstName,
    PassengerLastName,
    PassengerEmail,
    OccurredAt);

namespace FBNS.API.DTOs;

public record ReservationEventDto(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    DateTime OccurredAt
);

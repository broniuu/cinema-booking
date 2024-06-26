﻿namespace CinemaBooking.Web.Db.Entitites;

public class Seat
{
    public required Guid Id { get; set; }
    public int PositionX { get; set; }
    public int PositionY { get; set; }
    public required string SeatNumber { get; set; }
    public virtual Hall Hall { get; set; } = null!;
    public virtual required Guid HallId { get; set; }
    public bool IsForDisabled { get; set; }
    public virtual List<Reservation> Reservations { get; set; } = [];
}

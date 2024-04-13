using CinemaBooking.Web.Db.Entitites;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Web.Db;

public class CinemaDbContext(DbContextOptions<CinemaDbContext> options) : DbContext(options)
{
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Screening> Screenings => Set<Screening>();
    public DbSet<Seat> Seats => Set<Seat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnHallCreateing(modelBuilder);
        OnScreeningCreateing(modelBuilder);
        OnReservationCreateing(modelBuilder);
        OnSeatCreateing(modelBuilder);

    }

    private void OnHallCreateing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hall>()
             .HasKey(e => e.Id);
        modelBuilder.Entity<Hall>()
            .Property(e => e.Name)
            .IsRequired();
        modelBuilder.Entity<Hall>()
            .HasMany(e => e.Screenings)
            .WithOne(e => e.Hall)
            .HasForeignKey(e => e.HallId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();
        modelBuilder.Entity<Hall>()
            .HasMany(e => e.Seats)
            .WithOne(e => e.Hall)
            .HasForeignKey(e => e.HallId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();
    }

    private void OnScreeningCreateing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Screening>()
             .HasKey(e => e.Id);
        modelBuilder.Entity<Screening>()
            .Property(e => e.Name)
            .IsRequired();
        modelBuilder.Entity<Screening>()
            .Property(e => e.Date)
            .IsRequired();
        modelBuilder.Entity<Screening>()
            .Property(e => e.HallId)
            .IsRequired();
        modelBuilder.Entity<Screening>()
            .HasMany(e => e.Reservations)
            .WithOne(e => e.Screening)
            .HasForeignKey(e => e.ScreeningId)
            .HasPrincipalKey(e => e.Id)
            .IsRequired();
    }

    private void OnReservationCreateing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>()
             .HasKey(e => e.Id);
        modelBuilder.Entity<Reservation>()
            .Property(e => e.Name)
            .IsRequired();
        modelBuilder.Entity<Reservation>()
            .Property(e => e.Surname)
            .IsRequired();
        modelBuilder.Entity<Reservation>()
            .Property(e => e.PhoneNumber)
            .IsRequired();
        modelBuilder.Entity<Reservation>()
            .Property(e => e.ScreeningId)
            .IsRequired();
        modelBuilder.Entity<Reservation>()
            .Property(e => e.SeatId)
            .IsRequired();
    }

    private void OnSeatCreateing(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seat>()
             .HasKey(e => e.Id);
        modelBuilder.Entity<Seat>()
            .Property(e => e.PositionX)
            .IsRequired();
        modelBuilder.Entity<Seat>()
            .Property(e => e.PositionY)
            .IsRequired();
        modelBuilder.Entity<Seat>()
            .Property(e => e.SeatNumber)
            .IsRequired();
        modelBuilder.Entity<Seat>()
            .Property(e => e.HallId)
            .IsRequired();
        modelBuilder.Entity<Seat>()
            .HasOne(e => e.Reservation)
            .WithOne(e => e.Seat)
            .HasForeignKey<Reservation>();

    }
}

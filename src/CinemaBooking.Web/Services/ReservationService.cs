using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using Result;

namespace CinemaBooking.Web.Services;

public class ReservationService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Reservation> validator, ILogger<ReservationService> logger)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly GenericCudService<Reservation> _cudService = new(dbContextFactory, validator, logger);

    public virtual Task<Result<Reservation?>> AddAsync(Reservation reservation)
    {
        reservation.PhoneNumber = reservation.PhoneNumber.RemoveSpaces();
        return _cudService.AddAsync(reservation, "Error occured while adding reservation");
    }

    public virtual Task<Result<Reservation?>> UpdateAsync(Reservation reservation)
    {
        reservation.PhoneNumber = reservation.PhoneNumber.RemoveSpaces();
        return _cudService.UpdateAsync(reservation, "Error occured while updating reservation");
    }

    public virtual async Task<Result<bool>> RemoveAsync(Guid id)
    {
        var reservation = new Reservation { 
            Id = id, 
            Name = string.Empty,
            ScreeningId = Guid.Empty,
            SeatId = Guid.Empty,
            Surname = string.Empty,
            PhoneNumber = string.Empty,
        };
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        return await _cudService.RemoveAsync(dbContext, reservation, "Error occured while removing reservation");
    }
}
    

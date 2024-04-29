using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using LanguageExt.Common;

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

    public virtual Task<Result<bool>> RemoveAsync(Reservation reservation)
    {
        return _cudService.RemoveAsync(reservation, "Error occured while removing reservation");
    }
}
    

using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;
using LanguageExt.Common;

namespace CinemaBooking.Web.Services;

public class ReservationService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Reservation> validator)
{
    private readonly IDbContextFactory<CinemaDbContext> _dbContextFactory = dbContextFactory;
    private readonly GenericCudService<Reservation> _cudService = new(dbContextFactory, validator);

    public virtual Task<Result<Reservation?>> AddAsync(Reservation reservation)
    {
        reservation.PhoneNumber = reservation.PhoneNumber.RemoveSpaces();
        return _cudService.AddAsync(reservation);
    }
}
    

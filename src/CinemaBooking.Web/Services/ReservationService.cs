using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using CinemaBooking.Web.Db.Entitites;
using FluentValidation;

namespace CinemaBooking.Web.Services;

public class ReservationService(IDbContextFactory<CinemaDbContext> dbContextFactory, IValidator<Reservation> validator)
    : GenericCudService<Reservation>(dbContextFactory, validator);

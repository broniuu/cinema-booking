using Blazored.Toast;
using CinemaBooking.Web.Services;

namespace CinemaBooking.Web;

public static class Config
{
    public static IServiceCollection AddCinemaManagementServices(this IServiceCollection services)
    {
        return services.AddScoped<HallViewService>()
            .AddScoped<ScreeningService>();
    }
}

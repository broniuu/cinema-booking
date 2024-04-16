using Blazored.Toast;
using CinemaBooking.Web;
using CinemaBooking.Web.Components;
using CinemaBooking.Web.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCinemaManagementServices()
    .AddBlazoredToast()
    .AddRadzenComponents()
    .AddDbContextFactory<CinemaDbContext>(o =>
    {
        string dbPathDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CinemaBooking");
        Directory.CreateDirectory(dbPathDirectoryPath);
        var dbPath = Path.Combine(dbPathDirectoryPath, "cinemaBookingData.db");
        o.UseSqlite($"Data Source={dbPath};");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.MigrateDbAsync();
await app.FillInDatabaseAsync(app.Logger);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

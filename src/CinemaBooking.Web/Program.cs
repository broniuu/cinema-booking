using System.Runtime.CompilerServices;
using System.Text;
using CinemaBooking.Web;
using CinemaBooking.Web.Components;
using CinemaBooking.Web.Db;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Radzen;

[assembly: InternalsVisibleTo("CinemaBooking.Web.UnitTests")]

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register special encoding
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// Add services to the container.
builder.Services.AddLocalization()
    .AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCinemaManagementServices()
    .AddRadzenComponents()
    .AddSingleton(TimeProvider.System)
    .AddValidators()
    .AddDbContextFactory();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

var app = builder.Build();

var supportedCultures = new[] { "en-US", "pl-PL"};

app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.CreateAppDataDirectories();
await app.MigrateDbAsync();
if (app.Environment.IsDevelopment())
{
    await app.FillInDatabaseAsync(app.Logger);
}


app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();

using CinemaBooking.Web.Db;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CinemaBooking.Web.UnitTests.TestHelpers;
public class InMemorySqliteProvider
{
    private DbConnection? _connection;
    private DbContextOptions<CinemaDbContext>? _contextOptions;

    public void InitializeConnection()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var dbContext = new CinemaDbContext(_contextOptions);
        dbContext.Database.EnsureCreated();
    }

    public CinemaDbContext CreateDbContext()
    {
        if (_contextOptions is null)
        {
            throw new ArgumentNullException(nameof(_contextOptions));
        }
        return new(_contextOptions);
    } 

    public IDbContextFactory<CinemaDbContext> CreateDbContextFactory()
    {
        if ( _contextOptions is null ) {
            throw new ArgumentNullException(nameof(_contextOptions));
        }
        return new CinemaDbContextFactory(_contextOptions);
    }

    public void DisposeConnection() => _connection?.Dispose();

    private class CinemaDbContextFactory(DbContextOptions<CinemaDbContext> options) : IDbContextFactory<CinemaDbContext>
    {
        private readonly DbContextOptions<CinemaDbContext> _options = options;
        public CinemaDbContext CreateDbContext()
        {
            return new CinemaDbContext(_options);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class SqliteFootballContext : FootballContext
{
    public SqliteFootballContext(DbContextOptions<FootballContext> options) : base(options)
    {
    }
}
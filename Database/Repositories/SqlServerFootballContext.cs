using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class SqlServerFootballContext : FootballContext
{
    public SqlServerFootballContext(DbContextOptions<FootballContext> options) : base(options)
    {
    }
}
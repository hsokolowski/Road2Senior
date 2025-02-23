using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.Repositories
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FootballContext>
    {
        public FootballContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Road2Senior");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FootballContext>();
            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new FootballContext(optionsBuilder.Options);
        }
    }
}
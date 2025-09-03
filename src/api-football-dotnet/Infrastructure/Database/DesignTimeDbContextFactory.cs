using Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Database
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

            var databaseType = configuration["DatabaseType"];
            string connectionName = databaseType switch
            {
                "AzureKv" => "AzureConnection",
                "SqlServer" => "SqlServerConnection",
                _ => throw new Exception($"Unknown DatabaseType: {databaseType}")
            };
            
            var optionsBuilder = new DbContextOptionsBuilder<FootballContext>();
            var connectionString = configuration.GetConnectionString(connectionName);
            
            optionsBuilder.UseSqlServer(connectionString);

            return new FootballContext(optionsBuilder.Options);
        }
    }
}
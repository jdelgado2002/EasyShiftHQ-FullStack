using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace easyshifthq.EntityFrameworkCore;

public class EasyshifthqDbContextFactory : IDesignTimeDbContextFactory<EasyshifthqDbContext>
{
    public EasyshifthqDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<EasyshifthqDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new EasyshifthqDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

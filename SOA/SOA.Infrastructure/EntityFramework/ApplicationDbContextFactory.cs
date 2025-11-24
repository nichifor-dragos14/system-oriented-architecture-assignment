using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SOA.Infrastructure.EntityFramework;

internal class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=soa;Username=root;Password=root");

        ApplicationDbContext applicationContext = new(optionsBuilder.Options);

        return applicationContext;
    }
}
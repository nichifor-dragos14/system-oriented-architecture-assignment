using Microsoft.AspNetCore.Identity;
using SOA.Domain.Identity;
using SOA.Gateway.ExtensionMethods;
using SOA.Infrastructure.ExtensionMethods;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddInfrastructure(builder.Configuration)
       .AddWebApiServices(builder.Configuration);

var app = builder
    .Build()
    .UseWebApiPipeline();

app.MapGet("/health", () => "OK");

try
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var name in new[] { "Student", "Professor" })
        {
            if (!await roleManager.RoleExistsAsync(name))
            {
                await roleManager.CreateAsync(
                    new ApplicationRole
                    {
                        Name = name,
                        NormalizedName = name.ToUpperInvariant()
                    }
                );
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
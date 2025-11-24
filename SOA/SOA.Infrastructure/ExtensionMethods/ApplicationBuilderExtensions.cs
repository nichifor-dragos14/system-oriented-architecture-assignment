using Microsoft.AspNetCore.Builder;

namespace SOA.Infrastructure.ExtensionMethods;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app;
    }
}
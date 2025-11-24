using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SOA.Gateway.Authentication;
using SOA.Gateway.Clients;
using System.Text;

namespace SOA.Gateway.ExtensionMethods;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSection = configuration.GetSection("Jwt");
        var jwtKey = jwtSection.GetValue<string>("Key")
                          ?? throw new InvalidOperationException("Missing Jwt:Key");
        var jwtIssuer = jwtSection.GetValue<string>("Issuer")!;
        var jwtAudience = jwtSection.GetValue<string>("Audience")!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });
        services.AddAuthorization();

        services.AddScoped<TokenService>();

        services.AddHttpClient<StudentsServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:Students"]!);
        });

        services.AddHttpClient<GradesServiceClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Services:Grades"]!);
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}

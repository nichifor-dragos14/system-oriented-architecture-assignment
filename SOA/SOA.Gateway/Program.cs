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

app.MapGet("/api/notifications/events/gpa", async (IHttpClientFactory httpClientFactory, HttpContext httpContext) =>
{
    var client = httpClientFactory.CreateClient("Notifications");
    using var req = new HttpRequestMessage(HttpMethod.Get, "/events/gpa");
    using var response = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, httpContext.RequestAborted);

    httpContext.Response.StatusCode = (int)response.StatusCode;

    httpContext.Response.Headers["Content-Type"] = "text/event-stream";
    httpContext.Response.Headers["Cache-Control"] = "no-cache";
    httpContext.Response.Headers["X-Accel-Buffering"] = "no";

    static bool Skip(string key) =>
        key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase) ||
        key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase) ||
        key.Equals("Connection", StringComparison.OrdinalIgnoreCase) ||
        key.Equals("Keep-Alive", StringComparison.OrdinalIgnoreCase) ||
        key.Equals("Upgrade", StringComparison.OrdinalIgnoreCase) ||
        key.Equals("Proxy-Connection", StringComparison.OrdinalIgnoreCase);

    foreach (var header in response.Headers)
    {

        if (!Skip(header.Key))
        {
            httpContext.Response.Headers[header.Key] = header.Value.ToArray();
        }
    }

    foreach (var header in response.Content.Headers)
    {
        if (!Skip(header.Key))
        {
            httpContext.Response.Headers[header.Key] = header.Value.ToArray();
        }
    }
        
    await httpContext.Response.StartAsync();
    await response.Content.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
}).AllowAnonymous();

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
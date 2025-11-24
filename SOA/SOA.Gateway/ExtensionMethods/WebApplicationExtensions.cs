namespace SOA.Gateway.ExtensionMethods;

public static class WebApplicationExtensions
{
    public static WebApplication UseWebApiPipeline(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseRouting();

        app.MapControllers();

        return app;
    }
}
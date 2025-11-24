using Microsoft.AspNetCore.Mvc;
using SOA.NotificationService.Hubs;

namespace SOA.NotificationService.Controllers;

[ApiController]
[Route("events/gpa")]
public class GpaEventsController : ControllerBase
{
    private readonly GpaSseHub _gpaSseHub;
    public GpaEventsController(GpaSseHub gpaSseHub) => _gpaSseHub = gpaSseHub;

    [HttpGet]
    public async Task Get(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("X-Accel-Buffering", "no");

        var id = _gpaSseHub.RegisterClient(Response.Body);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Response.Body.WriteAsync(":\n"u8.ToArray(), cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
        }
        finally
        {
            _gpaSseHub.UnregisterClient(id);
        }
    }
}

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SOA.Domain.Events;

namespace SOA.Functions.Functions;

public class GpaHandler
{
    private readonly ILogger<GpaHandler> _logger;

    public GpaHandler(ILogger<GpaHandler> logger) => _logger = logger;

    [Function("GpaFunction")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "compute-gpa")] HttpRequestData request)
    {
        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            var gradeComputedGpaEvent = JsonSerializer.Deserialize<GradeCreatedGpaEvent>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (gradeComputedGpaEvent is null)
            {
                var badRequest = request.CreateResponse(HttpStatusCode.BadRequest);
                await badRequest.WriteStringAsync("Invalid payload.");

                return badRequest;
            }

            _logger.LogInformation("Received grades at {Course} for student {StudentId}", gradeComputedGpaEvent.Course, gradeComputedGpaEvent.StudentId);

            var gpa = ComputeAverage(gradeComputedGpaEvent.Values);

            var responsePayload = new
            {
                gpa
            };

            var ok = request.CreateResponse(HttpStatusCode.OK);
            ok.Headers.Add("Content-Type", "application/json");

            await ok.WriteStringAsync(JsonSerializer.Serialize(responsePayload));

            return ok;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to process GPA request");

            var error = request.CreateResponse(HttpStatusCode.InternalServerError);
            await error.WriteStringAsync("Internal error.");

            return error;
        }
    }

    private double ComputeAverage(List<int> grades)
    {
        return grades.Sum(g => g) / grades.Count;
    }
}

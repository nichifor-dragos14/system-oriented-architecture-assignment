using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOA.Gateway.Clients;

namespace SOA.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentsServiceClient _studentsServiceClient;

    public StudentsController(StudentsServiceClient studentsServiceClient)
    {
        _studentsServiceClient = studentsServiceClient;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetGradesForStudent(CancellationToken cancellationToken)
    {
        var students = await _studentsServiceClient.GetAllStudentsAsync(cancellationToken);

        return students is not null ? Ok(students) : StatusCode(502, "Upstream error while fetching students.");
    }
}

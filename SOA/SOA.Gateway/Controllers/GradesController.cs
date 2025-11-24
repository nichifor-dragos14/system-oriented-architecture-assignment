using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOA.Dto.Grade;
using SOA.Gateway.Clients;

namespace SOA.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class GradesController : ControllerBase
{
    private readonly GradesServiceClient _gradesServiceClient;

    public GradesController(GradesServiceClient gradesServiceClient)
    {
        _gradesServiceClient = gradesServiceClient;
    }

    [HttpGet("student/{studentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetGradesForStudent([FromRoute] Guid studentId, CancellationToken cancellationToken)
    {
        var grades = await _gradesServiceClient.GetGradesForStudentAsync(studentId, cancellationToken);

        return grades is not null ? Ok(grades) : StatusCode(502, "Upstream error while fetching grades.");
    }

    [HttpPost()]
    [Authorize(Roles = "Professor")]
    public async Task<IActionResult> AddGrade([FromBody] CreateGradeDto createGradeDto, CancellationToken cancellationToken)
    {
        var ok = await _gradesServiceClient.AddGradeAsync(createGradeDto, cancellationToken);

        return ok ? NoContent() : StatusCode(502, "Upstream error while adding grade.");
    }

    // PUT api/gateway/grades/{gradeId}
    [HttpPut("grades/{gradeId:guid}")]
    [Authorize(Roles = "Professor")]
    public async Task<IActionResult> UpdateGrade([FromRoute] Guid gradeId, [FromBody] UpdateGradeDto updateGradeDto, CancellationToken cancellationToken)
    {
        var ok = await _gradesServiceClient.UpdateGradeAsync(gradeId, updateGradeDto, cancellationToken);

        return ok ? NoContent() : StatusCode(502, "Upstream error while updating grade.");
    }

    // DELETE api/gateway/grades/{gradeId}
    [HttpDelete("grades/{gradeId:guid}")]
    [Authorize(Roles = "Professor")]
    public async Task<IActionResult> DeleteGrade([FromRoute] Guid gradeId, CancellationToken cancellationToken)
    {
        var ok = await _gradesServiceClient.DeleteGradeAsync(gradeId, cancellationToken);

        return ok ? NoContent() : StatusCode(502, "Upstream error while deleting grade.");
    }
}

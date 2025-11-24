using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOA.Domain.Events;
using SOA.Domain.Grade;
using SOA.Domain.Student;
using SOA.Dto.Grade;
using SOA.GradeService.EntityFramework;
using SOA.GradeService.Messaging;

namespace SOA.GradeService.Controllers;

[ApiController]
[Route("api/grades")]
public class GradeController : ControllerBase
{
    private readonly GradesDbContext _dbContext;
    private readonly GradeEventPublisher _gradeEventPublisher;

    public GradeController(
        GradesDbContext dbContext,
        GradeEventPublisher gradeEventPublisher
    )
    {
        _dbContext = dbContext;
        _gradeEventPublisher = gradeEventPublisher;
    }

    // GET /api/grades/student/{studentId}
    [HttpGet("student/{studentId:guid}")]
    public async Task<ActionResult<IEnumerable<GradeDto>>> GetGradesForStudent(Guid studentId)
    {
        var grades = await _dbContext.Grades
            .AsNoTracking()
            .Where(g => g.StudentId == studentId)
            .ToListAsync();

        return Ok(grades.Select(grade => new GradeDto
            {
                Id = grade.Id,
                Course = grade.Course,
                Value = grade.Value
            })
        );
    }

    // POST /api/grades
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGradeDto createGradeDto)
    {
        var grade = new Grade
        {
            Id = Guid.NewGuid(),
            Course = createGradeDto.Course,
            Value = createGradeDto.Value,
            StudentId = createGradeDto.StudentId
        };

        _dbContext.Grades.Add(grade);
        await _dbContext.SaveChangesAsync();

        var studentGrades = await _dbContext.Grades
            .AsNoTracking()
            .Where(g => g.StudentId == grade.StudentId && g.Course == grade.Course)
            .Select(g => g.Value)
            .ToListAsync();

        var gradeCreatedGpaEvent = new GradeCreatedGpaEvent(grade.Value, grade.StudentId, grade.Course, studentGrades, DateTime.UtcNow);

        _gradeEventPublisher.PublishGradeCreated(gradeCreatedGpaEvent);

        return NoContent();
    }

    // PUT /api/grades/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGradeDto updateGradeDto)
    {
        var existing = await _dbContext.Grades
            .FindAsync(id);

        if (existing is null)
        {
            return NotFound();
        }

        existing.Value = updateGradeDto.Value;
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/grades/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var grade = await _dbContext.Grades
            .FindAsync(id);

        if (grade is null) 
        { 
            return NotFound(); 
        }

        _dbContext.Grades.Remove(grade);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

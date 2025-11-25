using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOA.Domain.Student;
using SOA.Dto.Student;
using SOA.StudentService.EntityFramework;

namespace SOA.StudentService.Controllers;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly StudentsDbContext _dbContext;

    public StudentController(StudentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GET /api/students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        var students = await _dbContext.Students
            .AsNoTracking()
            .ToListAsync();

        return Ok(students
            .Where(student => student.Role == Role.Student)
            .Select(student => new StudentDto
                {
                    Id = student.Id,
                    Name = student.Name,
                    Email = student.Email,
                    Role = student.Role,
                }
            )
        );
    }

    // GET /api/students/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Student>> GetById(Guid id)
    {
        var student = await _dbContext.Students
            .FindAsync(id);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    // POST /api/students
    [HttpPost]
    public async Task<ActionResult<Student>> Create([FromBody] StudentDto studentDto)
    {
        var student = new Student
        {
            Id = studentDto.Id,
            Name = studentDto.Name,
            Email = studentDto.Email,
            Role = studentDto.Role,
        };

        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }
}

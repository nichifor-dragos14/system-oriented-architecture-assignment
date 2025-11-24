using SOA.Domain.Student;

namespace SOA.Dto.Student;

public class StudentDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Role Role { get; set; }
    public string? Email { get; set; }
}

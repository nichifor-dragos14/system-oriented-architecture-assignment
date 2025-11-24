namespace SOA.Domain.Student;

public class Student
{
    public Guid Id { get; set; }
    public Role Role { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;

    public List<Grade.Grade> Grades { get; set; } = [];
}

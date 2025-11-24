namespace SOA.Domain.Grade;

public class Grade
{
    public Guid Id { get; set; }
    public string Course { get; set; } = default!;
    public int Value { get; set; }

    public Guid StudentId { get; set; }
    public Student.Student Student { get; set; }
}

namespace SOA.Dto.Grade;

public class CreateGradeDto
{
    public Guid StudentId { get; set; }
    public string Course { get; set; } = string.Empty;
    public int Value { get; set; }
}

namespace SOA.Domain.Events;

public record GradeCreatedGpaEvent(
    int GivenGrade,
    Guid StudentId,
    string Course,
    List<int> Values,
    DateTime ComputedAt
);

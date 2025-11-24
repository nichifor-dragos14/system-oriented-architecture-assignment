using SOA.Domain.Student;

namespace SOA.Gateway.Authentication.Models;

public record RegisterRequest(
    string Email,
    string Password,
    string Name,
    Role Role
);

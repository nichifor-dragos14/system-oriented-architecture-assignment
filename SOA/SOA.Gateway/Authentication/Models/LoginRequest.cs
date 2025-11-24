namespace SOA.Gateway.Authentication.Models;

public record LoginRequest(
    string Email,
    string Password
);


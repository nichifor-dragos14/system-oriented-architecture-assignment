using Microsoft.IdentityModel.Tokens;
using SOA.Domain.Identity;
using SOA.Gateway.Clients;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SOA.Gateway.Authentication;

public sealed class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly StudentsServiceClient _studentsServiceClient;

    public TokenService(
        IConfiguration configuration,
        StudentsServiceClient studentsServiceClient
    )
    {
        _configuration = configuration;
        _studentsServiceClient = studentsServiceClient;
    }

    public async Task<string> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var profile = await _studentsServiceClient.GetStudentByIdAsync(user.Id, cancellationToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, profile is null ? user.UserName ?? user.Email ?? "" : $"{profile.Name}")
        };

        if (profile is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, profile.Role.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenMinutes"] ?? "60")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

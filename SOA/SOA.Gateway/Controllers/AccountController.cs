using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SOA.Domain.Identity;
using SOA.Dto.Student;
using SOA.Gateway.Authentication;
using SOA.Gateway.Authentication.Models;
using SOA.Gateway.Clients;

namespace SOA.Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<ApplicationRole> _roles;
    private readonly TokenService _tokenService;
    private readonly StudentsServiceClient _studentServiceClient;

    public AccountController(
        UserManager<ApplicationUser> users,
        RoleManager<ApplicationRole> roles,
        TokenService tokenService,
        StudentsServiceClient studentServiceClient
    )
    {
        _users = users;
        _roles = roles;
        _tokenService = tokenService;
        _studentServiceClient = studentServiceClient;
    }

    /// <summary> User registration </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var applicationUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email
        };

        var create = await _users.CreateAsync(applicationUser, request.Password);

        if (!create.Succeeded)
        {
            return BadRequest(string.Join("; ", create.Errors.Select(e => e.Description)));
        }

        var roleName = request.Role.ToString();

        if (!await _roles.RoleExistsAsync(roleName))
        {
            await _roles.CreateAsync(new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            });
        }

        await _users.AddToRoleAsync(applicationUser, roleName);

        var student = new StudentDto
        {
            Id = applicationUser.Id,
            Name = request.Name,
            Email = request.Email,
            Role = request.Role
        };

        await _studentServiceClient.CreateStudentAsync(student, cancellationToken);

        return Ok(new RegisterResponse(student.Id));
    }

    /// <summary> User login </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await _users.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var valid = await _users.CheckPasswordAsync(user, request.Password);

        if (!valid)
        {
            return Unauthorized("Invalid credentials.");
        }

        var jwt = await _tokenService.CreateAsync(user, cancellationToken);

        return Ok(new AuthResponse(jwt));
    }
}

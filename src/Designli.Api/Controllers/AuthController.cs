using Designli.Application.DTOs;
using Designli.Domain.Interfaces;
using Designli.Application.DTOs;
using Designli.Domain.Interfaces;
using Designli.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Designli.Api.Controllers;

/// <summary>
/// Controller for authentication and user management.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;

    public AuthController(IUserRepository users, IJwtTokenService jwt)
    {
        _users = users;
        _jwt = jwt;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <returns>A JWT token if successful.</returns>
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _users.GetByUsername(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _jwt.GenerateToken(user.Username);
        return Ok(new { token });
    }

    /// <summary>
    /// Retrieves a list of all registered users.
    /// </summary>
    /// <returns>A list of usernames.</returns>
    [Authorize]
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        return Ok(_users.GetAll().Select(x => x.Username));
    }
}
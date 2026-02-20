using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Utilities.Responses;

namespace Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration, IUserRepository userRepository) : ControllerBase
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string Token, string Username, string FullName, string Role, DateTime ExpiresAt);
    public record RegisterRequest(string Username, string Email, string Password, string FirstName, string LastName);

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse.Fail("Username y password son requeridos.", "VALIDATION_ERROR"));
        }

        var user = await userRepository.GetByUsernameAsync(request.Username);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(ApiResponse.Fail("Credenciales inválidas.", "INVALID_CREDENTIALS"));
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(ApiResponse.Fail("Credenciales inválidas.", "INVALID_CREDENTIALS"));
        }

        // Record login timestamp
        user.RecordLogin();
        await userRepository.SaveChangesAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!);
        var expiresAt = DateTime.UtcNow.AddHours(2);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Guest")
            ]),
            Expires = expiresAt,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var response = new LoginResponse(
            Token: tokenHandler.WriteToken(token),
            Username: user.Username,
            FullName: user.FullName,
            Role: user.Role?.Name ?? "Guest",
            ExpiresAt: expiresAt
        );

        return Ok(ApiResponse<LoginResponse>.Ok(response, "Inicio de sesión exitoso"));
    }

    /// <summary>
    /// Registers a new user (default role: Seller).
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(ApiResponse.Fail("Username, email y password son requeridos.", "VALIDATION_ERROR"));
        }

        if (request.Password.Length < 6)
        {
            return BadRequest(ApiResponse.Fail("El password debe tener al menos 6 caracteres.", "VALIDATION_ERROR"));
        }

        var exists = await userRepository.ExistsAsync(request.Username, request.Email);
        if (exists)
        {
            return Conflict(ApiResponse.Fail("El username o email ya está registrado.", "USER_EXISTS"));
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Default role: Seller (10000000-0000-0000-0000-000000000003)
        var defaultRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");

        var user = new Core.Domain.Entities.User(
            Guid.NewGuid(),
            request.Username,
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName,
            defaultRoleId
        );

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return Ok(ApiResponse.Ok("Usuario registrado exitosamente"));
    }
}
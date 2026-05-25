using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DoomscrollingAnalyzer.API.Data;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;
using DoomscrollingAnalyzer.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DoomscrollingAnalyzer.API.Services;

public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            Email = NormalizeEmail(request.Email),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        context.AppUsers.Add(user);
        await context.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await context.AppUsers.FirstOrDefaultAsync(appUser => appUser.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        return CreateAuthResponse(user);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);

        return context.AppUsers.AnyAsync(user => user.Email == normalizedEmail);
    }

    private AuthResponseDto CreateAuthResponse(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(GetTokenExpiryMinutes());

        return new AuthResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = GenerateJwtToken(user, expiresAt),
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(AppUser user, DateTime expiresAt)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = jwtSettings["Key"]
            ?? throw new InvalidOperationException("JWT setting 'JwtSettings:Key' is not configured.");
        var issuer = jwtSettings["Issuer"]
            ?? throw new InvalidOperationException("JWT setting 'JwtSettings:Issuer' is not configured.");
        var audience = jwtSettings["Audience"]
            ?? throw new InvalidOperationException("JWT setting 'JwtSettings:Audience' is not configured.");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private int GetTokenExpiryMinutes()
    {
        return configuration.GetValue("JwtSettings:ExpiresInMinutes", 60);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}

using System.IdentityModel.Tokens.Jwt;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Services;
using DoomscrollingAnalyzer.API.Tests.TestSupport;
using FluentAssertions;

namespace DoomscrollingAnalyzer.API.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUserWithHashedPassword()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new AuthService(context, TestConfigurationFactory.Create());

        var response = await service.RegisterAsync(new RegisterRequestDto
        {
            Username = "testuser",
            Email = "TestUser@Example.com",
            Password = "Password123!"
        });

        var user = context.AppUsers.Single();
        response.UserId.Should().Be(user.Id);
        response.Email.Should().Be("testuser@example.com");
        response.Token.Should().NotBeNullOrWhiteSpace();
        user.PasswordHash.Should().NotBe("Password123!");
        BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new AuthService(context, TestConfigurationFactory.Create());

        await service.RegisterAsync(new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });

        var response = await service.LoginAsync(new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        });

        response.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldGenerateJwtToken()
    {
        await using var context = TestDbContextFactory.Create();
        var service = new AuthService(context, TestConfigurationFactory.Create());

        await service.RegisterAsync(new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        });

        var response = await service.LoginAsync(new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        response.Should().NotBeNull();
        response!.Token.Should().NotBeNullOrWhiteSpace();

        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);
        token.Issuer.Should().Be("DoomscrollingAnalyzer.API.Tests");
        token.Audiences.Should().Contain("DoomscrollingAnalyzer.Tests");
        token.Claims.Should().Contain(claim => claim.Type == JwtRegisteredClaimNames.Email && claim.Value == "test@example.com");
    }
}

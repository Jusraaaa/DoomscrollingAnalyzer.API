using DoomscrollingAnalyzer.API.DTOs;

namespace DoomscrollingAnalyzer.API.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);

    Task<bool> EmailExistsAsync(string email);
}

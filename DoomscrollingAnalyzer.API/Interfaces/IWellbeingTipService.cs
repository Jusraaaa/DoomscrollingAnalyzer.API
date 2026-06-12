using DoomscrollingAnalyzer.API.DTOs;

namespace DoomscrollingAnalyzer.API.Interfaces;

public interface IWellbeingTipService
{
    Task<WellbeingTipDto> GetTipAsync(CancellationToken cancellationToken = default);
}

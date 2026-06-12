using System.Net.Http.Json;
using DoomscrollingAnalyzer.API.DTOs;
using DoomscrollingAnalyzer.API.Interfaces;

namespace DoomscrollingAnalyzer.API.Services;

public class WellbeingTipService(HttpClient httpClient, ILogger<WellbeingTipService> logger) : IWellbeingTipService
{
    private const string SourceName = "Quotable";

    private static readonly WellbeingTipDto FallbackTip = new()
    {
        Content = "Pause, breathe, and choose the next thing you give your attention to.",
        Author = "Doomscrolling Analyzer",
        Source = "Fallback"
    };

    public async Task<WellbeingTipDto> GetTipAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync("/random?maxLength=120", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Failed to fetch wellbeing tip from {Source}. Status code: {StatusCode}",
                    SourceName,
                    response.StatusCode);

                return FallbackTip;
            }

            var quote = await response.Content.ReadFromJsonAsync<QuotableQuoteResponse>(cancellationToken);
            if (quote is null || string.IsNullOrWhiteSpace(quote.Content))
            {
                logger.LogWarning("Fetched wellbeing tip from {Source}, but the response was empty", SourceName);

                return FallbackTip;
            }

            logger.LogInformation("Fetched wellbeing tip from {Source}", SourceName);

            return new WellbeingTipDto
            {
                Content = quote.Content.Trim(),
                Author = string.IsNullOrWhiteSpace(quote.Author) ? "Unknown" : quote.Author.Trim(),
                Source = SourceName
            };
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException or NotSupportedException)
        {
            logger.LogWarning(exception, "Failed to fetch wellbeing tip from {Source}; using fallback tip", SourceName);

            return FallbackTip;
        }
    }

    private sealed class QuotableQuoteResponse
    {
        public string? Content { get; set; }

        public string? Author { get; set; }
    }
}

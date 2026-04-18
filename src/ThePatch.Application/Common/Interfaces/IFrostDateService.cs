namespace ThePatch.Application.Common.Interfaces;

public interface IFrostDateService
{
    Task<FrostDateResult?> GetFrostDatesAsync(string zipCode, CancellationToken ct = default);
    Task<string?> GetHardinessZoneAsync(string zipCode, CancellationToken ct = default);
}

public record FrostDateResult(
    DateOnly LastSpringFrost,
    DateOnly FirstFallFrost,
    string HardinessZone
);

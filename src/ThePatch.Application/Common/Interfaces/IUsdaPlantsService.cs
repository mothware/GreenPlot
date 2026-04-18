namespace ThePatch.Application.Common.Interfaces;

public interface IUsdaPlantsService
{
    Task<UsdaPlantData?> LookupByCommonNameAsync(string commonName, CancellationToken ct = default);
    Task<UsdaPlantData?> LookupBySymbolAsync(string symbol, CancellationToken ct = default);
}

public record UsdaPlantData(
    string Symbol,
    string ScientificName,
    string CommonName,
    string Family,
    string? ZoneMin,
    string? ZoneMax,
    string? NativeStatus
);

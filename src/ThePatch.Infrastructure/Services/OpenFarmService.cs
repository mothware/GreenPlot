using ThePatch.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ThePatch.Infrastructure.Services;

public class OpenFarmService : IOpenFarmService
{
    private readonly HttpClient _http;
    private readonly ILogger<OpenFarmService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public OpenFarmService(HttpClient http, ILogger<OpenFarmService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<OpenFarmCropDto>> SearchCropsAsync(string query, CancellationToken ct = default)
    {
        try
        {
            var url = $"https://openfarm.cc/api/v1/crops?filter={Uri.EscapeDataString(query)}";
            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return [];

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);

            var results = new List<OpenFarmCropDto>();
            foreach (var item in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                var attrs = item.GetProperty("attributes");
                results.Add(MapCrop(item.GetProperty("id").GetString()!, attrs));
            }
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenFarm search failed for query: {Query}", query);
            return [];
        }
    }

    public async Task<OpenFarmCropDto?> GetCropAsync(string slug, CancellationToken ct = default)
    {
        try
        {
            var url = $"https://openfarm.cc/api/v1/crops/{slug}";
            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            var data = doc.RootElement.GetProperty("data");
            return MapCrop(data.GetProperty("id").GetString()!, data.GetProperty("attributes"));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenFarm get crop failed for slug: {Slug}", slug);
            return null;
        }
    }

    private static OpenFarmCropDto MapCrop(string slug, JsonElement attrs)
    {
        static string? SafeString(JsonElement el, string key) =>
            el.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;

        static int? SafeInt(JsonElement el, string key) =>
            el.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.Number
                ? prop.GetInt32()
                : null;

        return new OpenFarmCropDto(
            slug,
            SafeString(attrs, "name") ?? slug,
            SafeString(attrs, "description"),
            SafeString(attrs, "sun_requirements"),
            SafeString(attrs, "sowing_method"),
            SafeInt(attrs, "row_spacing"),
            SafeInt(attrs, "spread"),
            SafeInt(attrs, "height_maximum"),
            [],
            []);
    }
}

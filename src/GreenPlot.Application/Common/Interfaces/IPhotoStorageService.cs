namespace GreenPlot.Application.Common.Interfaces;

public interface IPhotoStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
}

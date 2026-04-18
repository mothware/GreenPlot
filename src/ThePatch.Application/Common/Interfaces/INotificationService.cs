namespace ThePatch.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendPushAsync(Guid userId, string title, string body, CancellationToken ct = default);
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}

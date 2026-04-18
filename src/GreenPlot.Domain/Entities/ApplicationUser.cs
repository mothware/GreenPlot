using GreenPlot.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GreenPlot.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? HomeZipCode { get; set; }
    public UnitSystem PreferredUnits { get; set; } = UnitSystem.Imperial;
    public string TimeZone { get; set; } = "America/New_York";
    public bool EmailDigestEnabled { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Garden> Gardens { get; set; } = new List<Garden>();
}

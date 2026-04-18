using Hangfire.Dashboard;

namespace GreenPlot.Api.Middleware;

public class HangfireDashboardAuthFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        // In production, restrict to admin role or specific IP
        return httpContext.User.Identity?.IsAuthenticated == true;
    }
}

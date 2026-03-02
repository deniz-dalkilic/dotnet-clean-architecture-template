using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Application.UseCases.AppInfo;

namespace Template.Api.Endpoints;

public static class AppInfoEndpoints
{
    public static IEndpointRouteBuilder MapAppInfoEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/app-info", (GetAppInfoService service) => Results.Ok(service.Execute()));
        return endpoints;
    }
}

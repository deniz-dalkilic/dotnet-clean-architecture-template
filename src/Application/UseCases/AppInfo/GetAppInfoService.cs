namespace Template.Application.UseCases.AppInfo;

public sealed class GetAppInfoService
{
    public AppInfoResponse Execute() => new("Template", "1.0.0");
}

public sealed record AppInfoResponse(string Name, string Version);

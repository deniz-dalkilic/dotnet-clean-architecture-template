using Microsoft.Extensions.Logging;
using Quartz;

namespace Template.Infrastructure.Jobs;

public sealed class HeartbeatJob(ILogger<HeartbeatJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Quartz heartbeat job executed at {TimestampUtc}", DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}

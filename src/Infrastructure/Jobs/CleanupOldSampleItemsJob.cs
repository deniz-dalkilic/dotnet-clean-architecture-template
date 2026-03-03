using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Template.Application.Abstractions;

namespace Template.Infrastructure.Jobs;

public sealed class CleanupOldSampleItemsJob(
    IAppDbContext dbContext,
    IOptions<CleanupOldSampleItemsOptions> options,
    ILogger<CleanupOldSampleItemsJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var maxAgeDays = Math.Max(1, options.Value.MaxAgeDays);
        var cutoffUtc = DateTime.UtcNow.AddDays(-maxAgeDays);

        var deletedRows = await dbContext.SampleItems
            .Where(item => item.CreatedAtUtc < cutoffUtc)
            .ExecuteDeleteAsync(context.CancellationToken);

        logger.LogInformation(
            "CleanupOldSampleItemsJob removed {DeletedRows} sample items older than {MaxAgeDays} day(s) (cutoff: {CutoffUtc})",
            deletedRows,
            maxAgeDays,
            cutoffUtc);
    }
}

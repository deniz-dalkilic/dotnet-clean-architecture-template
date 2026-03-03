namespace Template.Infrastructure.Jobs;

public sealed class CleanupOldSampleItemsOptions
{
    public const string SectionName = "BackgroundJobs:CleanupOldSampleItems";

    public string Cron { get; set; } = "0 0 * ? * *";
    public int MaxAgeDays { get; set; } = 30;
}

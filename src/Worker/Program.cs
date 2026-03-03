using Quartz;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Template.Infrastructure.DependencyInjection;
using Template.Infrastructure.Jobs;
using Template.Worker.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<CleanupOldSampleItemsOptions>(
    builder.Configuration.GetSection(CleanupOldSampleItemsOptions.SectionName));

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("template-worker"))
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddOtlpExporter());

builder.Services.AddQuartz(q =>
{
    var options = builder.Configuration
        .GetSection(CleanupOldSampleItemsOptions.SectionName)
        .Get<CleanupOldSampleItemsOptions>() ?? new CleanupOldSampleItemsOptions();

    var jobKey = new JobKey(nameof(CleanupOldSampleItemsJob));

    q.AddJob<CleanupOldSampleItemsJob>(job => job.WithIdentity(jobKey));
    q.AddTrigger(trigger => trigger
        .ForJob(jobKey)
        .WithIdentity($"{nameof(CleanupOldSampleItemsJob)}-trigger")
        .WithCronSchedule(options.Cron));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
builder.Services.AddHostedService<SampleItemCreatedConsumerService>();

builder.Services.AddSerilog(config => config.WriteTo.Console());

var host = builder.Build();
await host.RunAsync();

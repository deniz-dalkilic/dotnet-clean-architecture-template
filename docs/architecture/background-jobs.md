# Background Jobs

`Template.Worker` uses Quartz.NET to run recurring jobs.

## CleanupOldSampleItemsJob

The worker registers `CleanupOldSampleItemsJob`, which deletes rows from `sample_items` older than a configured age.

Configuration (`src/Worker/appsettings*.json`):

```json
{
  "BackgroundJobs": {
    "CleanupOldSampleItems": {
      "Cron": "0 0 * ? * *",
      "MaxAgeDays": 30
    }
  }
}
```

- `Cron` controls when the job runs (Quartz cron expression).
- `MaxAgeDays` controls retention. Items older than `UtcNow - MaxAgeDays` are deleted.

The default cron (`0 0 * ? * *`) runs hourly at minute `0`.

## Idempotency

The cleanup query is naturally idempotent:

- It only deletes data that matches `CreatedAtUtc < cutoff`.
- Re-running the job with the same cutoff deletes `0` rows after previous cleanup.

This keeps behavior safe during retries, restarts, or manual re-triggers.

## Retries

Quartz can re-run a job after failures depending on scheduler/job configuration and service lifecycle.

Recommended approach for this template:

1. Keep background jobs side-effect safe (idempotent deletes/upserts where possible).
2. Log inputs and outcomes (`cutoff`, number of deleted rows) for observability.
3. For advanced retry behavior, use Quartz listeners or reschedule failed executions explicitly.

For database transient faults, rely on your DB provider resiliency strategy and ensure failures are visible in logs/alerts.

# Observability

This template is **OSS-first** for observability and uses OpenTelemetry + Grafana stack components.

## What is included

- **Logs**: Serilog
  - Always written to console.
  - Optionally shipped to Grafana Loki when `Serilog:Loki:Endpoint` is configured.
- **Traces + Metrics**: OpenTelemetry SDK in both API and Worker.
  - Exported with OTLP (`OpenTelemetry:OtlpEndpoint`).
- **Resource attributes** on telemetry
  - `service.name`
  - `service.version`
  - `deployment.environment`
- **RabbitMQ spans**
  - Producer spans on publish in infrastructure event bus.
  - Consumer spans while processing messages in worker.

## Configuration

### API (`src/Api/appsettings*.json`)

```json
{
  "OpenTelemetry": {
    "ServiceName": "template-api",
    "OtlpEndpoint": "http://localhost:4317"
  },
  "Serilog": {
    "Loki": {
      "Endpoint": "http://localhost:3100"
    }
  }
}
```

### Worker (`src/Worker/appsettings*.json`)

```json
{
  "OpenTelemetry": {
    "ServiceName": "template-worker",
    "OtlpEndpoint": "http://localhost:4317"
  },
  "Serilog": {
    "Loki": {
      "Endpoint": "http://localhost:3100"
    }
  }
}
```

## OTLP and Collector

Both API and Worker export OTLP directly using `OpenTelemetry:OtlpEndpoint`. In local development, this typically points to the OpenTelemetry Collector (`http://localhost:4317`).

Collector responsibilities:

- Receives OTLP traces/metrics from services.
- Forwards traces to Tempo.
- Forwards metrics to Prometheus-compatible backend.
- Can be extended to fan out to additional backends without changing app code.

See local collector configuration under:

- `infra/observability/otel-collector/config.yaml`

## Local Grafana observability stack

The repository includes a ready local stack:

- Grafana
- Loki
- Tempo
- Prometheus
- OpenTelemetry Collector
- Promtail (for log shipping scenarios)

See:

- `infra/observability/docker-compose.yml`
- `infra/observability/grafana/provisioning/datasources/datasources.yml`
- `infra/observability/loki/local-config.yaml`
- `infra/observability/tempo.yaml`
- `infra/observability/prometheus/prometheus.yml`

## Notes

- Loki sink is intentionally conditional: if no Loki endpoint is configured, logs still go to console.
- OTLP exporters are configured from app settings, enabling easy environment-specific overrides.
- Optional proprietary sink example (Seq) is listed as commented package version in `Directory.Packages.props` and is not enabled by default.

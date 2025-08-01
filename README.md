# GHLearning-EasyAppMetric

[![.NET](https://github.com/gordon-hung/GHLearning-EasyAppMetric/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gordon-hung/GHLearning-EasyAppMetric/actions/workflows/dotnet.yml)

[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/gordon-hung/GHLearning-EasyAppMetric)

An [OpenTelemetry Prometheus exporter](https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/metrics/sdk_exporters/prometheus.md) for configuring an ASP.NET Core application with an endpoint for Prometheus to scrape.Gordon Hung Learning Easy App Metric

### Steps to enable OpenTelemetry.Exporter.Prometheus.AspNetCore

#### Step 1: Install Package

```shell
dotnet add package --prerelease OpenTelemetry.Exporter.Prometheus.AspNetCore
```

#### Step 2: Configure OpenTelemetry MeterProvider

* When using [OpenTelemetry.Extensions.Hosting](https://github.com/open-telemetry/opentelemetry-dotnet/blob/coreunstable-1.12.0-beta.1/src/OpenTelemetry.Extensions.Hosting/README.md) package on .NET 6.0+:
  ```csharp
  services.AddOpenTelemetry()
      .WithMetrics(builder => builder
          .AddPrometheusExporter());
  ```
* Or configure directly:
  Call the `MeterProviderBuilder.AddPrometheusExporter` extension to register the Prometheus exporter.
  ```csharp
  var meterProvider = Sdk.CreateMeterProviderBuilder()
      .AddPrometheusExporter()
      .Build();
  builder.Services.AddSingleton(meterProvider);
  ```

#### Step 3: Configure Prometheus Scraping Endpoint

* Register Prometheus scraping middleware using the `UseOpenTelemetryPrometheusScrapingEndpoint` extension method on `IApplicationBuilder` :

  ```csharp
  var builder = WebApplication.CreateBuilder(args);
  var app = builder.Build();
  app.UseOpenTelemetryPrometheusScrapingEndpoint();
  ```

  Overloads of the `UseOpenTelemetryPrometheusScrapingEndpoint` extension are provided to change the path or for more advanced configuration a predicate function can be used:

  ```csharp
  app.UseOpenTelemetryPrometheusScrapingEndpoint(
          context => context.Request.Path == "/internal/metrics"
              && context.Connection.LocalPort == 5067);
  ```

  This can be used in combination with [configuring multiple ports on the ASP.NET application](https://learn.microsoft.com/aspnet/core/fundamentals/servers/kestrel/endpoints) to expose the scraping endpoint on a different port.

### Configuration

The `PrometheusExporter` can be configured using the `PrometheusAspNetCoreOptions` properties.

#### ScrapeEndpointPath

Defines the path for the Prometheus scrape endpoint for the middleware registered by `UseOpenTelemetryPrometheusScrapingEndpoint`. Default value: `"/metrics"`.

#### ScrapeResponseCacheDurationMilliseconds

Configures scrape endpoint response caching. Multiple scrape requests within the cache duration time period will receive the same previously generated response. The default value is `300`. Set to `0` to disable response caching.

### Troubleshooting

This component uses an [EventSource](https://docs.microsoft.com/dotnet/api/system.diagnostics.tracing.eventsource) with the name "OpenTelemetry-Exporter-Prometheus" for its internal logging. Please refer to [SDK troubleshooting](https://github.com/open-telemetry/opentelemetry-dotnet/blob/coreunstable-1.12.0-beta.1/src/OpenTelemetry/README.md#troubleshooting) for instructions on seeing these internal logs.

### References

* [OpenTelemetry Project](https://opentelemetry.io/)
* [Prometheus](https://prometheus.io/)

### Steps to enable OpenTelemetry.Exporter.Prometheus.AspNetCore

#### Step 1: Install Package

```shell
dotnet add package --prerelease OpenTelemetry.Exporter.Prometheus.AspNetCore
```

#### Step 2: Configure OpenTelemetry MeterProvider

* When using [OpenTelemetry.Extensions.Hosting](https://github.com/open-telemetry/opentelemetry-dotnet/blob/coreunstable-1.12.0-beta.1/src/OpenTelemetry.Extensions.Hosting/README.md) package on .NET 6.0+:
  ```csharp
  services.AddOpenTelemetry()
      .WithMetrics(builder => builder
          .AddPrometheusExporter());
  ```
* Or configure directly:
  Call the `MeterProviderBuilder.AddPrometheusExporter` extension to register the Prometheus exporter.
  ```csharp
  var meterProvider = Sdk.CreateMeterProviderBuilder()
      .AddPrometheusExporter()
      .Build();
  builder.Services.AddSingleton(meterProvider);
  ```

#### Step 3: Configure Prometheus Scraping Endpoint

* Register Prometheus scraping middleware using the `UseOpenTelemetryPrometheusScrapingEndpoint` extension method on `IApplicationBuilder` :

  ```csharp
  var builder = WebApplication.CreateBuilder(args);
  var app = builder.Build();
  app.UseOpenTelemetryPrometheusScrapingEndpoint();
  ```

  Overloads of the `UseOpenTelemetryPrometheusScrapingEndpoint` extension are provided to change the path or for more advanced configuration a predicate function can be used:

  ```csharp
  app.UseOpenTelemetryPrometheusScrapingEndpoint(
          context => context.Request.Path == "/internal/metrics"
              && context.Connection.LocalPort == 5067);
  ```

  This can be used in combination with [configuring multiple ports on the ASP.NET application](https://learn.microsoft.com/aspnet/core/fundamentals/servers/kestrel/endpoints) to expose the scraping endpoint on a different port.

### Configuration

The `PrometheusExporter` can be configured using the `PrometheusAspNetCoreOptions` properties.

#### ScrapeEndpointPath

Defines the path for the Prometheus scrape endpoint for the middleware registered by `UseOpenTelemetryPrometheusScrapingEndpoint`. Default value: `"/metrics"`.

#### ScrapeResponseCacheDurationMilliseconds

Configures scrape endpoint response caching. Multiple scrape requests within the cache duration time period will receive the same previously generated response. The default value is `300`. Set to `0` to disable response caching.

### Troubleshooting

This component uses an [EventSource](https://docs.microsoft.com/dotnet/api/system.diagnostics.tracing.eventsource) with the name "OpenTelemetry-Exporter-Prometheus" for its internal logging. Please refer to [SDK troubleshooting](https://github.com/open-telemetry/opentelemetry-dotnet/blob/coreunstable-1.12.0-beta.1/src/OpenTelemetry/README.md#troubleshooting) for instructions on seeing these internal logs.

### References

* [OpenTelemetry Project](https://opentelemetry.io/)
* [Prometheus](https://prometheus.io/)

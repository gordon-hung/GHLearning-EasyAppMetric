using System.Net.Mime;
using System.Text.Json.Serialization;
using GHLearning.EasyAppMetric.WebApi.Metrics;
using GHLearning.EasyAppMetric.WebApi.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<Random>();

builder.Services
	.AddSingleton<AppMetric>()
	.AddSingleton<IAppMetric>(p => p.GetRequiredService<AppMetric>())
	.AddHostedService(p => p.GetRequiredService<AppMetric>())
	.AddOpenTelemetry()
	.WithMetrics(metrics => metrics
	.ConfigureResource(resource => resource.AddService(
		serviceName: builder.Configuration["ServiceName"]!.ToLower(),
		serviceNamespace: typeof(Program).Assembly.GetName().Name,
		serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown"))
	.AddMeter(builder.Environment.ApplicationName)
	.AddPrometheusExporter());

//Learn more about configuring HealthChecks at https://learn.microsoft.com/zh-tw/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-9.0
builder.Services
	.AddHealthChecks()
	.AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
	app.MapOpenApi();
	app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));// swagger/
	app.UseReDoc(options => options.SpecUrl("/openapi/v1.json"));//api-docs/
	app.MapScalarApiReference();//scalar/v1
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<AppMetricMiddleware>();

app.MapControllers();

app.MapHealthChecks("/live", new HealthCheckOptions
{
	Predicate = check => check.Tags.Contains("live"),
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
	Predicate = _ => true,
	ResultStatusCodes =
	{
		[HealthStatus.Healthy] = StatusCodes.Status200OK,
		[HealthStatus.Degraded] = StatusCodes.Status200OK,
		[HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
	}
});

app.MapPrometheusScrapingEndpoint("/metrics").AllowAnonymous();

app.Run();

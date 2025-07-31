using System.Diagnostics;
using System.Net;
using GHLearning.EasyAppMetric.WebApi.Metrics;

namespace GHLearning.EasyAppMetric.WebApi.Middlewares;

public class AppMetricMiddleware(
	ILogger<AppMetricMiddleware> logger,
	IWebHostEnvironment environment,
	IConfiguration configuration,
	IAppMetric appMetric,
	RequestDelegate next)
{
	public async Task InvokeAsync(HttpContext context)
	{
		var serviceName = configuration.GetValue<string>("ServiceName") ?? "UnknownService";
		var applicationName = environment.ApplicationName ?? "UnknownApplication";
		var environmentName = environment.EnvironmentName ?? "UnknownEnvironment";
		var requestPath = context.Request.Path;

		if (context.Request.Path.StartsWithSegments("/metrics", StringComparison.OrdinalIgnoreCase) ||
			context.Request.Path.StartsWithSegments("/api/info", StringComparison.OrdinalIgnoreCase))
		{
			// 如果是 /metrics 路徑，直接跳過日誌紀錄
			await next(context).ConfigureAwait(false);
			return;
		}

		if (!context.GetRouteData().Values.ContainsKey("controller"))
		{
			// 如果沒有找到 "controller" 路由參數，則認為不是控制器路徑
			await next(context).ConfigureAwait(false);
			return;
		}

		var stopwatch = Stopwatch.StartNew();
		logger.LogInformation(
			"Service: {serviceName},Application: {applicationName}, Environment: {environmentName}, Request Path: {requestPath}, Start Time: { StartTime}",
			serviceName,
			applicationName,
			environmentName,
			requestPath,
			DateTimeOffset.UtcNow);
		appMetric.LogReceive(serviceName, applicationName, environmentName, requestPath);
		try
		{
			await next(context).ConfigureAwait(false);
			stopwatch.Stop();
			logger.LogInformation(
				"Service: {serviceName},Application: {applicationName}, Environment: {environmentName}, Request Path: {requestPath}, Status Code: {context.Response.StatusCode}, Duration: {stopwatch.ElapsedMilliseconds} ms",
				serviceName,
				applicationName,
				environmentName,
				requestPath,
				context.Response.StatusCode,
				stopwatch.ElapsedMilliseconds);
			appMetric.LogResponse(serviceName, applicationName, environmentName, requestPath, context.Response.StatusCode);
		}
		catch (Exception ex)
		{
			stopwatch.Stop();
			logger.LogError(
				ex,
				"Service: {serviceName},Application: {applicationName}, Environment: {environmentName}, Request Path: {requestPath}, Status Code: {context.Response.StatusCode}, Duration: {stopwatch.ElapsedMilliseconds} ms",
				serviceName,
				applicationName,
				environmentName,
				requestPath,
				(int)HttpStatusCode.InternalServerError,
				stopwatch.ElapsedMilliseconds);
			appMetric.LogResponse(serviceName, applicationName, environmentName, requestPath, (int)HttpStatusCode.InternalServerError);
			throw;
		}
	}
}

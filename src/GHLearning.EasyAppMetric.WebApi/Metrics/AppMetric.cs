using System.Diagnostics.Metrics;
using GHLearning.EasyAppMetric.WebApi.Metrics;

namespace GHLearning.EasyAppMetric.WebApi.Metrics;

internal sealed class AppMetric : IAppMetric, IHostedService
{
    // 定義常數，用於避免字串拼接
    private const string MetricPrefix = "api_request_response_status";
    private const string ServiceNameKey = "service_name";
    private const string ApplicationNameKey = "application_name";
    private const string EnvironmentNameKey = "environment_name";
    private const string RequestPathKey = "request_path";
    private const string StatusCodeKey = "status_code";

    private readonly Counter<long> _appStartCounter;
    private readonly Counter<long> _appStopCounter;
    private readonly Counter<long> _receiveCounter;
    private readonly Counter<long> _responseCounter;

    public AppMetric(IHostEnvironment hostEnvironment, IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(name: hostEnvironment.ApplicationName, version: "1.0.0");

        // 創建計數器
        _appStartCounter = meter.CreateCounter<long>($"{MetricPrefix}_start");
        _appStopCounter = meter.CreateCounter<long>($"{MetricPrefix}_stop");
        _receiveCounter = meter.CreateCounter<long>($"{MetricPrefix}_receive");
        _responseCounter = meter.CreateCounter<long>($"{MetricPrefix}_response");
    }
    public void LogReceive(string serviceName, string applicationName, string environmentName, string requestPath)
    {
        KeyValuePair<string, object>[] labels =
        [
            new(ServiceNameKey, serviceName),
            new(ApplicationNameKey, applicationName),
            new(EnvironmentNameKey, environmentName),
            new(RequestPathKey, requestPath)
        ];

        _receiveCounter.Add(1, labels!);
    }

    public void LogResponse(string serviceName, string applicationName, string environmentName, string requestPath, int statusCode)
    {
        KeyValuePair<string, object>[] labels =
        [
            new(ServiceNameKey, serviceName),
            new(ApplicationNameKey, applicationName),
            new(EnvironmentNameKey, environmentName),
            new(RequestPathKey, requestPath),
            new(StatusCodeKey, statusCode)
        ];

        _responseCounter.Add(1, labels!);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appStartCounter.Add(1);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _appStopCounter.Add(1);
        return Task.CompletedTask;
    }
}

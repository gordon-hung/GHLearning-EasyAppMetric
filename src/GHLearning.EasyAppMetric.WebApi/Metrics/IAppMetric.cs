namespace GHLearning.EasyAppMetric.WebApi.Metrics;

public interface IAppMetric
{
    void LogReceive(string serviceName, string applicationName, string environmentName, string requestPath);
    void LogResponse(string serviceName, string applicationName, string environmentName, string requestPath, int statusCode);
}

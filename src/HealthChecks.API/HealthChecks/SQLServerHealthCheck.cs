using HealthChecks.API.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Data;

namespace HealthChecks.API.HealthChecks
{
    public class SQLServerHealthCheck : IHealthCheck
    {
        private readonly IConnectionFactory connectionFactory;

        public SQLServerHealthCheck(IConnectionFactory _connectionFactory)
        {
            this.connectionFactory = _connectionFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using var connection = connectionFactory.CreateConnection("sqlserver");
            await connection.OpenAsync();
            if (connection.State == ConnectionState.Open)
            {
                return HealthCheckResult.Healthy("Successful connection to sqlserver database!.");
            }
            else
            {
                return HealthCheckResult.Unhealthy("Un-successful connection to sqlserver database!.");
            }
        }
    }
}

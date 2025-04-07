using Microsoft.Data.SqlClient;
using Npgsql;
using System.Data.Common;

namespace HealthChecks.API.Common
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration configuration;
        public ConnectionFactory(IConfiguration _configuration) {
            configuration = _configuration;
        }

        public DbConnection CreateConnection(string databaseType)
        {
            var connection = configuration.GetConnectionString(databaseType);

            return databaseType.ToLower() switch
            {
                "sqlserver" => new SqlConnection(connection),
                "postgresql" => new NpgsqlConnection(connection),
                _ => throw new ArgumentException("Unsupported Database Type"),
            };
        }
    }
}

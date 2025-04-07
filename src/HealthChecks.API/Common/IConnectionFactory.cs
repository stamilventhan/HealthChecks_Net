using System.Data.Common;

namespace HealthChecks.API.Common
{
    public interface IConnectionFactory
    {
        DbConnection CreateConnection(string databaseType);
    }
}

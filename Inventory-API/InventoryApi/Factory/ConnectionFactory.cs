using Microsoft.Data.SqlClient;
using Shared.Constants;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace InventoryApi.Factory
{
    public class DapperDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDictionary<DatabaseConnections, string> _connectionDict;

        public DapperDbConnectionFactory(IDictionary<DatabaseConnections, string> connectionDict)
        {
            _connectionDict = connectionDict;
        }

        public DbConnection CreateConnection(string connectionName)
        {
            string? connectionString = null;

            if (Enum.TryParse(connectionName, true, out DatabaseConnections result) && _connectionDict.TryGetValue(result, out connectionString))
            {
                return new SqlConnection(connectionString);
            }

            throw new ArgumentNullException();
        }
    }
}

using Microsoft.Data.SqlClient;
using Shared.Constants;
using System.Data.Common;

namespace Infrastructure.Factory
{
    public class DapperDbConnectionFactory(IDictionary<string, string> connectionDict) : IDbConnectionFactory
    {
        private readonly IDictionary<string, string> _connectionDict = connectionDict;

        public DbConnection CreateConnection(string connection)
        {
            if (_connectionDict.TryGetValue(connection, out string? connectionString))
            {
                return new SqlConnection(connectionString);
            }

            throw new InvalidOperationException($"No connection string registered for '{connection}'.");
        }
    }
}

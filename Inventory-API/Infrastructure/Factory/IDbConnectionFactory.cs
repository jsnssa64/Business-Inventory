using System.Data.Common;

namespace Infrastructure.Factory
{
    public interface IDbConnectionFactory
    {
        DbConnection CreateConnection(string connection);
    }
}

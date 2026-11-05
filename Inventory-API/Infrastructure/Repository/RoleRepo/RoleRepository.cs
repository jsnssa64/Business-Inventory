using Dapper;
using System.Data;
using Domain.Interface;

namespace Infrastructure.Repository.RoleRepo
{
    public class RoleRepository(IDbConnectionFactory dbConnectionFactory) : IRoleRepository
    {
        [Obsolete]
        public async Task<RoleModel> CreateRole(CreateRoleModel createRole)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(CreateRoleModel.RoleName), createRole.RoleName);
            parameters.Add(nameof(CreateRoleModel.IsDefault), createRole.IsDefault);
            parameters.Add(nameof(RoleNameModel.RoleName), dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await conn.ExecuteScalarAsync<int>("dbo.CreateRole", parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new DbUpdateException($"Role: Not created");

            return new RoleModel
            {
                RoleName = createRole.RoleName,
                IsDefault = createRole.IsDefault
            };
        }

        [Obsolete]
        public async Task<bool> IsValidRole(RoleNameModel roleNameModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(RoleNameModel.RoleName), roleNameModel.RoleName);
            parameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                
            await conn.ExecuteScalarAsync("dbo.IsValidRole", parameters, commandType: CommandType.StoredProcedure);

            var roleExists = parameters.Get<int>("ReturnValue");

            return roleExists > 0;
        }

        [Obsolete]
        public async Task<IEnumerable<RoleModel>> GetRoles()
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var result = await conn.QueryAsync<dynamic>("dbo.GetRoles", new DynamicParameters(), commandType: CommandType.StoredProcedure);

            if (result.IsNullOrEmpty())
                throw new DbUpdateException("No roles found");

            return result.Select((role) => new RoleModel()
            {
                RoleName = role.RoleName,
                IsDefault = role.IsDefault
            });
        }

        [Obsolete]
        public async Task<RoleModel> GetDefaultRole()
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();

            var result = await conn.QuerySingleAsync<dynamic>("dbo.GetDefaultRole", new DynamicParameters(), commandType: CommandType.StoredProcedure);

            if (result is null)
                throw new DbUpdateException($"Role not found");

            return new RoleModel()
            {
                RoleName = result.RoleName,
                IsDefault = result.IsDefault
            };
        }
    }
}

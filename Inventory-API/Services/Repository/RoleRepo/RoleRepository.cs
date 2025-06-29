using Dapper;
using System.Data.Entity.Infrastructure;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using InventoryApi.Constants;
using Services.DataModel.Role;

namespace InventoryApi.Repository.RoleRepo
{
    public class RoleRepository: IRoleRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;

        public RoleRepository(
            IDbConnectionFactory dbConnectionFactory
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        [Obsolete]
        public async Task<RoleModel> CreateRole(CreateRoleModel createRole)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

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
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to Create new Role: {ex.Message}");
            }
        }

        [Obsolete]
        public async Task<bool> IsValidRole(RoleNameModel roleNameModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(RoleNameModel.RoleName), roleNameModel.RoleName);
                parameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                
                await conn.ExecuteScalarAsync("dbo.IsValidRole", parameters, commandType: CommandType.StoredProcedure);

                var roleExists = parameters.Get<int>("ReturnValue");

                return roleExists > 0;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve role: {ex.Message}");
            }
        }

        [Obsolete]
        public async Task<IEnumerable<RoleModel>> GetRoles()
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var result = await conn.QueryAsync<dynamic>("dbo.GetRoles", new DynamicParameters(), commandType: CommandType.StoredProcedure);

                if (result.IsNullOrEmpty())
                    throw new DbUpdateException("No roles found");

                return result.Select((role) => new RoleModel()
                {
                    RoleName = role.RoleName,
                    IsDefault = role.IsDefault
                });
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        [Obsolete]
        public async Task<RoleModel> GetDefaultRole()
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

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
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }
    }
}

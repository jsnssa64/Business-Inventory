using Dapper;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using System.Data;
using Microsoft.IdentityModel.Tokens;
using InventoryApi.Repository.Data.Role;

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

        public async Task<RoleModel> CreateRole(CreateRoleModel createRole)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(CreateRoleModel.RoleName), createRole.RoleName);
                parameters.Add(nameof(CreateRoleModel.IsDefault), createRole.IsDefault);
                parameters.Add(nameof(RoleIdModel.PublicRoleId), dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.ExecuteAsync("dbo.CreateRole", parameters, commandType: CommandType.StoredProcedure);

                if (result <= 0)
                    throw new DbUpdateException($"Role: Not created");

                return new RoleModel
                {
                    PublicRoleId = parameters.Get<Guid>(nameof(RoleIdModel.PublicRoleId)),
                    RoleName = createRole.RoleName,
                    IsDefault = createRole.IsDefault
                };
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to Create new Role: {ex.Message}");
            }
        }

        public async Task<bool> IsValidRole(RoleIdModel roleIdModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(RoleIdModel.PublicRoleId), roleIdModel.PublicRoleId);
                parameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                await conn.ExecuteAsync("dbo.IsValidRole", parameters, commandType: CommandType.StoredProcedure);

                var roleExists = parameters.Get<int>("ReturnValue");

                return roleExists > 0;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve role: {ex.Message}");
            }
        }

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
                    IsDefault = role.IsDefault,
                    PublicRoleId = role.PublicRoleId
                });
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

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
                    IsDefault = result.IsDefault,
                    PublicRoleId = result.PublicRoleId
                };
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }
    }
}

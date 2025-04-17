using Dapper;
using Domain.User;
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

        public async Task<Role> CreateRole(RoleNameModel roleModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(RoleNameModel.RoleName), roleModel);
                parameters.Add(nameof(RoleIdModel.RolePublicId), dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.ExecuteAsync("dbo.CreateRole", parameters, commandType: CommandType.StoredProcedure);

                if (result <= 0)
                    throw new DbUpdateException($"Role: Not created");

                return new Role
                {
                    Id = parameters.Get<string>(nameof(RoleIdModel.RolePublicId)),
                    Rolename = roleModel.RoleName,
                };
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to Create new Role: {ex.Message}");
            }
        }

        public async Task<bool> IsValidRole(string rolePublicId)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add("RolePublicId", rolePublicId);
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

        public async Task<IEnumerable<Role>> GetRoles()
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();

                var result = await conn.QueryAsync<Role>("dbo.GetRoles", new DynamicParameters(), commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<string> GetDefaultRole()
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();

                var result = await conn.QuerySingleAsync<string>("dbo.GetDefaultRole", new DynamicParameters(), commandType: CommandType.StoredProcedure);

                if (result.IsNullOrEmpty())
                    throw new DbUpdateException($"Role not found");

                return result;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }
    }
}

using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.User;
using InventoryApi.Factory;

namespace InventoryApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private IDbConnectionFactory _dbConnectionFactory;

        public UserRepository(
            IDbConnectionFactory dbConnectionFactory
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<User> RegisterUserLogin(User user)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(user.UserName), user.UserName);
                parameters.Add(nameof(user.Email), user.Email);
                parameters.Add(nameof(user.EncryptedPassword), user.EncryptedPassword);
                parameters.Add(nameof(user.Id), dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.ExecuteAsync("dbo.RegisterUserLogin", parameters, commandType: CommandType.StoredProcedure);

                user.Id = parameters.Get<int>(nameof(user.Id));

                if (result == -1)
                    throw new DbUpdateException($"Unable to process request");
                if (user.Id <= 0)
                    throw new DbUpdateException($"No User was registered");                

                return user;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task<User> RegisterUserDetails(User user, UserDetails userDetails)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(user.Id), user.Id);
                parameters.Add(nameof(userDetails.FullName), userDetails.FullName);
                parameters.Add(nameof(userDetails.EmailAddress), userDetails.EmailAddress);
                parameters.Add(nameof(userDetails.DOB), userDetails.DOB);
                parameters.Add(nameof(userDetails.userAddress.FirstLineAddress), userDetails.userAddress?.FirstLineAddress);
                parameters.Add(nameof(userDetails.userAddress.SecondLineAddress), userDetails.userAddress?.SecondLineAddress);
                parameters.Add(nameof(userDetails.userAddress.PostCode), userDetails.userAddress?.PostCode);
                parameters.Add(nameof(userDetails.ContactNumber), userDetails.ContactNumber);
                parameters.Add(nameof(userDetails.Gender), userDetails.Gender);

                var result = await conn.ExecuteAsync("dbo.UpdateUserDetails", parameters, commandType: CommandType.StoredProcedure);

                if (result == -1)
                    throw new DbUpdateException($"Unable to process request");

                return user;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task<bool> AssignRoleToUser(UserRole userRole)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserRole.UserId), userRole.UserId);
                parameters.Add(nameof(UserRole.Role), userRole.Role.ToString());

                var result = await conn.ExecuteAsync("dbo.AssignRoleToUser", parameters, commandType: CommandType.StoredProcedure);

                if (result <= 0)
                    throw new DbUpdateException($"No Changes To Role");

                return true;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to assign User to Role: {ex.Message}");
            }
        }
    }
}

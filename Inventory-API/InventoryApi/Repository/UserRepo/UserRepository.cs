using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.User;
using InventoryApi.Factory;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Repository.Data.User;

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

        public async Task<int> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(UserRegistrationModel.Email), userRegisterModel.Email);
                parameters.Add(nameof(UserRegistrationModel.FirstName), userRegisterModel.FirstName);
                parameters.Add(nameof(UserRegistrationModel.LastName), userRegisterModel.LastName);
                parameters.Add(nameof(UserRegistrationModel.Password), userRegisterModel.Password);
                parameters.Add(nameof(UserRegistrationModel.UserId), dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await conn.ExecuteAsync("dbo.CreateUser", parameters, commandType: CommandType.StoredProcedure);

                var userId = parameters.Get<int>(nameof(UserRegistrationModel.UserId));

                if (userId <= 0 || result < 0)
                    throw new DbUpdateException($"No User was registered");
                
                return userId;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(UserDetailsModel.Email), useDetailsModel.Email);
                parameters.Add(nameof(UserDetailsModel.FirstName), useDetailsModel.FirstName);
                parameters.Add(nameof(UserDetailsModel.LastName), useDetailsModel.LastName);
                parameters.Add(nameof(UserDetailsModel.ContactNumber), useDetailsModel.ContactNumber);
                parameters.Add(nameof(UserDetailsModel.Gender), useDetailsModel.Gender);
                parameters.Add(nameof(UserDetailsModel.DOB), useDetailsModel.DOB);
                parameters.Add(nameof(UserDetailsModel.FirstLineAddress), useDetailsModel.FirstLineAddress);
                parameters.Add(nameof(UserDetailsModel.SecondLineAddress), useDetailsModel.SecondLineAddress);
                parameters.Add(nameof(UserDetailsModel.Country), useDetailsModel.Country);
                parameters.Add(nameof(UserDetailsModel.PostCode), useDetailsModel.PostCode);
                
                var result = await conn.ExecuteAsync("dbo.UpdateUserDetails", parameters, commandType: CommandType.StoredProcedure);

                if (result < 0)
                    throw new DbUpdateException($"Unable to process request");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task ActivateUser(UserIdentifierModel userIdentifier)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                
                var result = await conn.ExecuteAsync("dbo.ActivateUser", parameters, commandType: CommandType.StoredProcedure);

                if (result < 0)
                    throw new DbUpdateException($"Unable to activate user");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to assign User to Role: {ex.Message}");
            }
        }

        public async Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleIdentifierModel roleIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(RoleIdentifierModel.RolePublicId), roleIdentifierModel.RolePublicId);

                var result = await conn.ExecuteAsync("dbo.AssignUserToRole", parameters, commandType: CommandType.StoredProcedure);

                if (result < 0)
                    throw new DbUpdateException($"No Changes To Role");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to assign User to Role: {ex.Message}");
            }
        }

        public async Task CreateNewPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(PasswordModel.Password), passwordModel.Password);

                var result = await conn.ExecuteAsync("dbo.CreateNewPassword", parameters, commandType: CommandType.StoredProcedure);

                if (result < 0)
                    throw new DbUpdateException($"{nameof(PasswordModel.Password)} Unchanged");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to Create new Password: {ex.Message}");
            }
        }

        public async Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel)
        {
            var status = (statusModel.Enabled) ? "enabled" : "disabled";
            var proc = (statusModel.Enabled) ? "dbo.EnableUser" : "dbo.DisableUser";
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                
                var result = await conn.ExecuteAsync(proc, parameters, commandType: CommandType.StoredProcedure);
                
                if (result < 0)
                    throw new DbUpdateException($"User: Unable to {status} user");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to {status} user: {ex.Message}");
            }
        }

        public async Task<User> GetUser(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>("dbo.GetUser", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                    throw new DbUpdateException($"User not found for username: {userIdentifierModel.Username}");

                var user = new User().Map(result);

                return user;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<(User, UserDetails)> GetUserDetails(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>("dbo.GetUserDetails", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                    throw new DbUpdateException($"User details not found for username: {userIdentifierModel.Username}");

                var user = new User();
                var userDetails = new UserDetails();
                userDetails.Map(result);
                user.Map(result);

                return (user, userDetails);
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }
    }
}

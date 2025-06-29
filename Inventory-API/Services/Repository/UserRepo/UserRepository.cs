using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.Entities.User;
using InventoryApi.Constants;
using Services.DataModel.Role;
using Services.DataModel.User;

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

        public async Task<UserIdModel> CreateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel, IDbTransaction? dbTransaction = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(UserRegistrationModel.Email), userRegisterModel.Email);
                parameters.Add(nameof(UserRegistrationModel.FirstName), userRegisterModel.FirstName);
                parameters.Add(nameof(UserRegistrationModel.LastName), userRegisterModel.LastName);
                parameters.Add(nameof(UserRegistrationModel.Password), userRegisterModel.EncryptedPassword);
                parameters.Add(nameof(UserRegistrationModel.UserId), dbType: DbType.Int32, direction: ParameterDirection.Output);

                var result = await dbConnection.ExecuteScalarAsync<int>("dbo.CreateUser", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

                var userId = parameters.Get<int>(nameof(UserRegistrationModel.UserId));

                if (userId <= 0 || result != 0)
                    throw new DbUpdateException($"No User was registered");

                return new UserIdModel()
                {
                    UserId = userId
                };
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

                var result = await conn.ExecuteScalarAsync<int>("dbo.UpdateUserDetails", parameters, commandType: CommandType.StoredProcedure);

                if (result != 0)
                    throw new DbUpdateException($"Unable to process request");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to register new User: {ex.Message}");
            }
        }

        public async Task ActivateUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, IDbTransaction? dbTransaction = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                
                var result = await dbConnection.ExecuteScalarAsync<int>("dbo.ActivateUser", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

                if (result != 0)
                    throw new DbUpdateException($"Unable to activate user");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to assign User to Role: {ex.Message}");
            }
        }

        public async Task AssignRoleToUser(IDbConnection dbConnection, UserIdentifierModel userIdentifier, RoleNameModel roleNameModel, IDbTransaction? dbTransaction = null)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(RoleNameModel.RoleName), roleNameModel.RoleName);

                var result = await dbConnection.ExecuteScalarAsync<int>("dbo.AssignUserToRole", parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

                if (result != 0)
                    throw new DbUpdateException($"No Changes To Role");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to assign User to Role: {ex.Message}");
            }
        }

        public async Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                parameters.Add(nameof(PasswordModel.NewPassword), passwordModel.NewPassword);

                var result = await conn.ExecuteScalarAsync<int>("dbo.ResetPassword", parameters, commandType: CommandType.StoredProcedure);

                if (result != 0)
                    throw new DbUpdateException($"{nameof(PasswordModel.NewPassword)} Unchanged");
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
                
                var result = await conn.ExecuteScalarAsync<int>(proc, parameters, commandType: CommandType.StoredProcedure);
                
                if (result != 0)
                    throw new DbUpdateException($"User: Unable to {status} user");
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to {status} user: {ex.Message}");
            }
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();

                var result = await conn.QueryAsync<User>("dbo.GetAllUsers", parameters, commandType: CommandType.StoredProcedure);

                if (result is null)
                    throw new DbUpdateException($"Users not found");

                return result;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<UserWithPassword> GetUser(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

                var result = await conn.QueryFirstAsync<dynamic>("dbo.GetUser", parameters, commandType: CommandType.StoredProcedure);

                if (result is null)
                    throw new DbUpdateException($"User not found for username: {userIdentifierModel.Username}");

                var user = new UserWithPassword();
                user.Map(result);

                return user;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<User> GetUserByEmail(UserEmailModel userEmailModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserEmailModel.Email), userEmailModel.Email);

                var result = await conn.QueryFirstAsync<dynamic>("dbo.GetUserByEmail", parameters, commandType: CommandType.StoredProcedure);

                if (result is null)
                    throw new DbUpdateException($"User not found for username: {userEmailModel.Email}");

                var user = new User();
                user.Map(result);

                return user;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserEmailModel.Email), userEmailModel.Email);

                var result = await conn.ExecuteScalarAsync<int>("dbo.IsValidUserEmail", parameters, commandType: CommandType.StoredProcedure);

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

                var result = await conn.ExecuteScalarAsync<int>("dbo.IsValidUserUsername", parameters, commandType: CommandType.StoredProcedure);

                return result == 1;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }

        public async Task<UserDetails> GetUserDetails(UserIdentifierModel userIdentifierModel)
        {
            try
            {
                using IDbConnection conn = _dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

                var parameters = new DynamicParameters();
                parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>("dbo.GetUserDetails", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                    throw new DbUpdateException($"User details not found for username: {userIdentifierModel.Username}");

                var userDetails = new UserDetails();
                userDetails.Map(result);

                return userDetails;
            }
            catch (Exception ex)
            {
                throw new DbUpdateException($"Failed to retrieve user details: {ex.Message}");
            }
        }
    }
}

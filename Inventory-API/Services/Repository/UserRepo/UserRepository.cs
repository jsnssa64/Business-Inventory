using System.Data;
using System.Data.Entity.Infrastructure;
using Dapper;
using Domain.Entities.User;
using Services.DataModel.Role;
using Services.DataModel.User;

namespace Services.Repository.UserRepo
{
    public class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
    {
        private static class StoredProc
        {
            public const string CreateUser = "dbo.CreateUser";
            public const string UpdateUserDetails = "dbo.UpdateUserDetails";
            public const string ActivateUser = "dbo.ActivateUser";
            public const string AssignUserToRole = "dbo.AssignUserToRole";
            public const string ResetPassword = "dbo.ResetPassword";
            public const string EnableUser = "dbo.EnableUser";
            public const string DisableUser = "dbo.DisableUser";
            public const string GetAllUsers = "dbo.GetAllUsers";
            public const string GetUser = "dbo.GetUser";
            public const string GetUserByEmail = "dbo.GetUserByEmail";
            public const string IsValidUserEmail = "dbo.IsValidUserEmail";
            public const string IsValidUserUsername = "dbo.IsValidUserUsername";
            public const string GetUserDetails = "dbo.GetUserDetails";
        };

        public async Task<UserIdModel> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            return await CreateUser(userIdentifier, userRegisterModel, conn);
        }

        public async Task<UserIdModel> CreateUser(UserIdentifierModel userIdentifier, UserRegistrationModel userRegisterModel, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
            parameters.Add(nameof(UserRegistrationModel.Email), userRegisterModel.Email);
            parameters.Add(nameof(UserRegistrationModel.FirstName), userRegisterModel.FirstName);
            parameters.Add(nameof(UserRegistrationModel.LastName), userRegisterModel.LastName);
            parameters.Add(nameof(UserRegistrationModel.Password), userRegisterModel.EncryptedPassword);
            parameters.Add(nameof(UserRegistrationModel.UserId), dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await dbConnection.ExecuteScalarAsync<int>(StoredProc.CreateUser, parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            var userId = parameters.Get<int>(nameof(UserRegistrationModel.UserId));

            if (userId <= 0 || result != 0)
                throw new DbUpdateException($"No User was registered");

            return new UserIdModel()
            {
                UserId = userId
            };
        }

        public async Task UpdateUserDetails(UserIdentifierModel userIdentifier, UserDetailsModel useDetailsModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

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

            var result = await conn.ExecuteScalarAsync<int>(StoredProc.UpdateUserDetails, parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new DbUpdateException($"Unable to process request");
        }

        public async Task ActivateUser(UserIdentifierModel userIdentifier)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            await ActivateUser(userIdentifier, conn);
        }

        public async Task ActivateUser(UserIdentifierModel userIdentifier, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                
            var result = await dbConnection.ExecuteScalarAsync<int>(StoredProc.ActivateUser, parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
                throw new DbUpdateException($"Unable to activate user");
        }

        public async Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleNameModel roleNameModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());
            await AssignRoleToUser(userIdentifier, roleNameModel, conn);
        }

        public async Task AssignRoleToUser(UserIdentifierModel userIdentifier, RoleNameModel roleNameModel, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
            parameters.Add(nameof(RoleNameModel.RoleName), roleNameModel.RoleName);

            var result = await dbConnection.ExecuteScalarAsync<int>(StoredProc.AssignUserToRole, parameters, commandType: CommandType.StoredProcedure, transaction: dbTransaction);

            if (result != 0)
                throw new DbUpdateException($"No Changes To Role");
        }

        public async Task ResetPassword(UserIdentifierModel userIdentifier, PasswordModel passwordModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
            parameters.Add(nameof(PasswordModel.NewPassword), passwordModel.NewPassword);

            var result = await conn.ExecuteScalarAsync<int>(StoredProc.ResetPassword, parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new DbUpdateException($"{nameof(PasswordModel.NewPassword)} Unchanged");
        }

        public async Task SetUserStatus(UserIdentifierModel userIdentifier, StatusModel statusModel)
        {
            var status = (statusModel.Enabled) ? "enabled" : "disabled";
            var proc = (statusModel.Enabled) ? StoredProc.EnableUser : StoredProc.DisableUser;
            
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifier.Username);
                
            var result = await conn.ExecuteScalarAsync<int>(proc, parameters, commandType: CommandType.StoredProcedure);

            if (result != 0)
                throw new DbUpdateException($"Unable to {status} user");
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();

            var result = await conn.QueryAsync<User>(StoredProc.GetAllUsers, parameters, commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<UserWithPassword?> GetUser(UserIdentifierModel userIdentifierModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.QueryAsync<dynamic>(StoredProc.GetUser, parameters, commandType: CommandType.StoredProcedure);

            if (result is null)
                return null;

            var user = new UserWithPassword();
            user.Map(result);

            return user;
        }

        public async Task<User?> GetUserByEmail(UserEmailModel userEmailModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserEmailModel.Email), userEmailModel.Email);

            var result = await conn.QueryAsync<dynamic>(StoredProc.GetUserByEmail, parameters, commandType: CommandType.StoredProcedure);

            if (result is null)
                return null;

            var user = new User();
            user.Map(result);

            return user;
        }

        public async Task<bool> IsValidUserByEmail(UserEmailModel userEmailModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            var parameters = new DynamicParameters();
            parameters.Add(nameof(UserEmailModel.Email), userEmailModel.Email);

            var result = await conn.ExecuteScalarAsync<int>(StoredProc.IsValidUserEmail, parameters, commandType: CommandType.StoredProcedure);

            return result == 1;
        }

        public async Task<bool> IsValidUserByUsername(UserIdentifierModel userIdentifierModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.ExecuteScalarAsync<int>(StoredProc.IsValidUserUsername, parameters, commandType: CommandType.StoredProcedure);

            return result == 1;
        }

        public async Task<UserDetails> GetUserDetails(UserIdentifierModel userIdentifierModel)
        {
            using IDbConnection conn = dbConnectionFactory.CreateConnection(DatabaseConnections.InventoryDb.ToString());

            DynamicParameters parameters = new();
            parameters.Add(nameof(UserIdentifierModel.Username), userIdentifierModel.Username);

            var result = await conn.QueryAsync<dynamic>(StoredProc.GetUserDetails, parameters, commandType: CommandType.StoredProcedure);



            var userDetails = new UserDetails();
            userDetails.Map(result);

            return userDetails;
        }
    }
}

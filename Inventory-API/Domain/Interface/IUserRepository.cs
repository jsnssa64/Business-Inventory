using Domain.Entities.User;
using Domain.ValueObjects.User;
using Domain.ValueObjects.User.Role;

namespace Domain.Interface
{
    public interface IUserRepository
    {
        Task<UserId> Create(UserDetails userDetails, UserRoleLevel userRole, UserEncryptedPassword userPassword);
        Task UpdateUserDetails(UserId userId, UserDetails userDetails);
        Task ActivateUser(UserId userId);
        Task AssignRoleToUser(UserId userId, UserRoleLevel userRole);
        Task SetPassword(UserId userId, UserEncryptedPassword passwordModel);
        Task SetUserStatus(UserId userId, UserStatus status);
        Task<IEnumerable<User?>> GetAllUsers();
        Task<UserDetails> GetUser(UserId userIdentifier);
        Task<User?> GetUserByEmail(string userEmail);
        Task<bool> IsValidUserByEmail(UserId userId);
        Task<bool> IsValidUserByUsername(UserId userId);
        Task<UserDetails> GetUserDetails(UserId userId);
    }
}

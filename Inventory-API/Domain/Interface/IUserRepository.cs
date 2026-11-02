using Domain.Entities.User;
using Domain.ValueObjects.User;
using Domain.ValueObjects.User.Role;

namespace Domain.Interface
{
    public interface IUserRepository
    {
        Task<UserIdentity> Create(UserId userIdentifier, UserDetails userDetails, UserRoleLevel userRole, UserPassword userPassword);
        Task UpdateUserDetails(UserId userId, UserDetails userDetails);
        Task ActivateUser(UserId userId);
        Task AssignRoleToUser(UserId userId, UserRoleLevel userRole);
        Task ResetPassword(UserId userId, UserPassword passwordModel);
        Task SetUserStatus(UserId userId, UserStatus status);
        Task<IEnumerable<User?>> GetAllUsers();
        Task<UserPassword> GetUser(UserId userIdentifier);
        Task<User?> GetUserByEmail(UserId userId);
        Task<bool> IsValidUserByEmail(UserId userId);
        Task<bool> IsValidUserByUsername(UserId userId);
        Task<UserDetails> GetUserDetails(UserId userId);
    }
}

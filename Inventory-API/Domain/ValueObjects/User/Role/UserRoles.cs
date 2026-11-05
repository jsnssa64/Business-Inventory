namespace Domain.ValueObjects.User.Role
{
    public class UserRole(UserRoleLevel userRole)
    {
        public UserRoleLevel Role { get; init; } = userRole;

        public static bool IsHigherThenMinimumRole(UserRoleLevel userRole, UserRoleLevel minimumRole)
        {
            return (int)userRole >= (int)minimumRole;
        }

        public static bool IsValidRole(string? role, out UserRoleLevel parsedRole)
        {
            parsedRole = UserRoleLevel.Guest;

            if (string.IsNullOrEmpty(role))
                return false;

            return Enum.TryParse(role, true, out parsedRole);
        }

        public static UserRoleLevel GetUserRole(string? roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Role name cannot be null or empty.", nameof(roleName));
            }

            return Enum.TryParse<UserRoleLevel>(roleName, true, out var parsedRole) ? parsedRole : throw new InvalidCastException("Invalid role name.");
        }
    }
}

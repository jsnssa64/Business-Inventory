namespace Domain.User
{
    public static class Roles
    {
        public static RoleLevel DefaultRole = RoleLevel.User;

        public static List<RoleLevel> AllRoles = new List<RoleLevel>() { 
            RoleLevel.Guest, 
            RoleLevel.User, 
            RoleLevel.Admin 
        };
        
        public enum RoleLevel
        {
            Guest = 1,
            User = 2,
            Admin = 3
        }

        public static bool IsValidRoleLevel(string? role)
        {
            if(role == null)
                return false;
            return Enum.TryParse<RoleLevel>(role, true, out var result);
        }
    }
}
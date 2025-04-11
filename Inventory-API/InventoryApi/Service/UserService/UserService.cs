using Domain.User;
using InventoryApi.Repository;
using InventoryApi.Service.SecurityService;

namespace InventoryApi.Service.UserService
{
    public class UserService: IUserService
    {
        private IUserRepository _userRepository;
        private ISecurityService _securityService;

        public UserService(IUserRepository userRepository, ISecurityService securityService) {
            _userRepository = userRepository;
            _securityService = securityService;
        }

        public void RegisterUser(User user)
        {
            if(user == null) throw new ArgumentNullException("user");

            //  Encrypt Password



            _userRepository.RegisterUserLogin(user);
            

        }

        public void AssignUserToRole(int userId, Role role)
        {
            //  

        }
    }
}

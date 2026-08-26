using AuthenticationGRPCLibrary;
using Google.Api;

namespace AuthService.Services
{
    public class AuthenticationService : Authentication.AuthenticationBase
    {
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            _logger = logger;
        }



    }
}
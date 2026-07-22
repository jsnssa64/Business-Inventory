using AuthenticationGRPCLibrary;
using Google.Api;

namespace AuthService.Services
{
    public class AuthenticationService : TestingAuthentication.TestingAuthenticationBase
    {
        private readonly ILogger<AuthenticationService> _logger;
        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            _logger = logger;
        }



    }
}
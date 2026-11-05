using Domain.ValueObjects.User.Role;
using Infrastructure.DataModel.Webhook;
using InventoryApi.Authorization;
using InventoryApi.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Security;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(UserRoleLevel.Client)]
    public class WebhookController : BaseController
    {
        private readonly ISecurityService _securityService;
        private readonly IWebhookRepository _webhookRepository;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IWebhookRepository webhookRepository, ISecurityService securityService, ILogger<WebhookController> logger)
        {
            _securityService = securityService;
            _webhookRepository = webhookRepository;
            _logger = logger;
        }

        [HttpGet("RegisterWebhook")]
        public async Task<IActionResult> RegisterWebhook(string webhookURL, string subscriptionType)
        {
            if (!Enum.TryParse<SubscriptionType>(subscriptionType, out var parsedSubscriptionType))
                throw new Exception("Invalid subscription type provided.");

            var sharedSecret = _securityService.GenerateSecureSecret();

            var subscriptionModel = new Subscription
            {
                userName = GetUserIdFromClaim(),
                webhookUrl = new Uri(webhookURL),
                secret = sharedSecret,
                subscriptionType = parsedSubscriptionType
            };

            await _webhookRepository.Subscribe(subscriptionModel);

            // Fix: Return a single object instead of passing multiple arguments to Ok()
            return Ok(new { Message = "Webhook registered successfully.", SharedSecret = sharedSecret });
        }
    }
}

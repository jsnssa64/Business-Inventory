using InventoryApi.Authentication;
using InventoryApi.Controllers.CustomController;
using InventoryApi.Repository.Webhook;
using InventoryApi.Service.SecurityService;
using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Webhook;
using static Domain.User.Roles;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [MinimumRole(RoleLevel.User)]
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
            try
            {
                if (!Enum.TryParse<SubscriptionType>(subscriptionType, out var parsedSubscriptionType))
                    throw new Exception("Invalid subscription type provided.");

                var sharedSecret = _securityService.GenerateSecureSecret();

                var subscriptionModel = new Subscription
                {
                    userName = GetUsername(),
                    webhookUrl = new Uri(webhookURL),
                    secret = sharedSecret,
                    subscriptionType = parsedSubscriptionType
                };

                await _webhookRepository.Subscribe(subscriptionModel);

                // Fix: Return a single object instead of passing multiple arguments to Ok()
                return Ok(new { Message = "Webhook registered successfully.", SharedSecret = sharedSecret });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering subscription");
                return StatusCode(500);
            }
        }
    }
}

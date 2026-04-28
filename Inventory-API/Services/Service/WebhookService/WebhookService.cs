using System.Data;
using System.Text.Json;
using System.Text;
using Services.DataModel.Webhook;
using Microsoft.Extensions.Logging;
using Services.Repository.Webhook;
using Services.Service.SecurityService;

namespace Services.Service.UserService
{
    public class WebhookService : IWebhookService
    {
        private ISecurityService _securityService;
        private IWebhookRepository _webhookRepository;
        private ILogger<WebhookService> _logger;
        private const string HubSignatureHeader = "X-Hub-Signature-256";

        public WebhookService(IWebhookRepository webhookRepository, ISecurityService securityService, ILogger<WebhookService> logger) {
            _securityService = securityService;
            _webhookRepository = webhookRepository;
            _logger = logger;
        }

        public async Task WebHookTest(string userName, SubscriptionType subscriptionType)
        {
            var webhooks = await _webhookRepository.GetWebhookByAction(subscriptionType);

            await PostToWebhook(webhooks.Select(webhook => new WebhookPost()
            {
                SharedSecret = webhook.SharedSecret,
                WebhookURI = webhook.WebhookURI,
                payload = new ActionDetails()
                {
                    action = subscriptionType.ToString(),
                    actionTriggerTime = DateTime.UtcNow
                }
            }), subscriptionType);

        }

        public async Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType)
        {
            try
            {
                using HttpClient client = new HttpClient();

                var tasks = new List<Task<HttpResponseMessage>>();

                foreach (var webhook in webhooks)
                {
                    var jsonPayload = JsonSerializer.Serialize(webhook.payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, webhook.WebhookURI);
                    request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    request.Headers.Add(HubSignatureHeader, $"sha256={_securityService.GetHashFromPayload(jsonPayload, webhook.SharedSecret)}");

                    tasks.Add(client.SendAsync(request));
                }

                var responses = await Task.WhenAll(tasks);

                foreach (var response in responses)
                {
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting to webhook");
            }
        }
    }
}

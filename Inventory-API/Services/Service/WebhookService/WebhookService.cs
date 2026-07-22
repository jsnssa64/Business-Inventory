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

        public async Task PostWebhookBySubscription(string userName, SubscriptionType subscriptionType)
        {
            var webhooks = await _webhookRepository.GetWebhooksByAction(subscriptionType);

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
                    try
                    {
                        var jsonPayload = JsonSerializer.Serialize(webhook.payload);

                        if (string.IsNullOrEmpty(jsonPayload))
                        {
                            // Record failure to serialize payload, but continue with other webhooks
                            _logger.LogError("Failed to serialize payload for webhook {WebhookURI}. Skipping.", webhook.WebhookURI);
                            continue;
                        }

                        var request = new HttpRequestMessage(HttpMethod.Post, webhook.WebhookURI);
                        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        request.Headers.Add(HubSignatureHeader, $"sha256={_securityService.GetHashFromPayload(jsonPayload, webhook.SharedSecret)}");

                        tasks.Add(client.SendAsync(request));
                    }
                    catch (Exception ex)
                    {
                        // Keep going even if one webhook fails, but log the error
                        _logger.LogError(ex, "Failed to create request for webhook {WebhookURI}. Message: {Message}", webhook.WebhookURI, ex.Message);
                    }
                }

                var responses = await Task.WhenAll(tasks);

                foreach (var response in responses)
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException ex)
                    {
                        // Keep going even if one webhook fails, but log the error
                        _logger.LogError(ex, "Failed to post to webhook. Status Code: {StatusCode}, Message: {Message}", response.StatusCode, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting to webhook");
            }
        }
    }
}

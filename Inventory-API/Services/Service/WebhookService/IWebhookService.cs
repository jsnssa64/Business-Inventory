using Microsoft.AspNetCore.Mvc;
using Services.DataModel.Webhook;

namespace InventoryApi.Service.UserService
{
    public interface IWebhookService
    {
        Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType);
    }
}
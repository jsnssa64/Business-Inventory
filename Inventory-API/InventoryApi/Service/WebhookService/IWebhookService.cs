using InventoryApi.Repository.Data.Webhook;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Service.UserService
{
    public interface IWebhookService
    {
        Task PostToWebhook(IEnumerable<WebhookPost> webhooks, SubscriptionType subscriptionType);
    }
}
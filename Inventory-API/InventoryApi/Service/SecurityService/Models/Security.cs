namespace InventoryApi.Service.SecurityService.Models
{
    public class Security
    {
        public Token AccessToken { get; set; }
        public Token RefreshToken { get; set; }
        public string Audience { get; set; }
        public string Issuer {  get; set; }
    }
}

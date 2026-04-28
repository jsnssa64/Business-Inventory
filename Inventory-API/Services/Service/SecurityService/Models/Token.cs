namespace Services.Service.SecurityService.Models
{
    public class Token
    {
        public string? Key { get; set; }
        public int Expiry { get; set; }
    }
}

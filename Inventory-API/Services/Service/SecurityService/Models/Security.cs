namespace Services.Service.SecurityService.Models
{
    public class Security
    {
        public required Token AccessToken { get; set; }
        public required Token RefreshToken { get; set; }
        public required Token ConfirmationToken { get; set; }
        public required Token ResetPasswordToken { get; set; }
        public required string Audience { get; set; }
        public required string Issuer { get; set; }
    }
}

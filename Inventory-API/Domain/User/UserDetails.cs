using System.ComponentModel.DataAnnotations;

namespace Domain.User
{
    public class UserDetails
    {
        public string? FullName { get; set; }
        public EmailAddressAttribute? EmailAddress { get; set; }
        public UserAddress? userAddress { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public PhoneAttribute? ContactNumber { get; set; }
    }

    public class UserAddress
    {
        public required string FirstLineAddress { get; set; }
        public string? SecondLineAddress { get; set; }
        public required string PostCode { get; set; }
    }
}

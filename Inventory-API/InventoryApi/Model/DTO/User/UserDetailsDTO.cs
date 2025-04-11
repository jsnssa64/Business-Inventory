using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Model.DTO.User
{
    public class UserDetailsDTO
    {
        public required string FullName { get; set; }
        public required EmailAddressAttribute EmailAddress { get; set; }
        public required string FirstLineAddress { get; set; }
        public required string SecondLineAddress { get; set; }
        public required string PostCode { get; set; }
        public required string Gender { get; set; }
        public DateOnly DOB { get; set; }
        public required PhoneAttribute PhoneNumber { get; set; }
    }
}

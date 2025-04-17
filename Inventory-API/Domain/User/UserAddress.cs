using System.ComponentModel.DataAnnotations;

namespace Domain.User
{
    public class UserAddress
    {
        public required string FirstLineAddress { get; set; }
        public string? SecondLineAddress { get; set; }
        public required string Country { get; set; }
        public required string PostCode { get; set; }

        public void Map(dynamic usrdets)
        {
            this.FirstLineAddress = usrdets.FirstLineAddress;
            this.SecondLineAddress = usrdets.SecondLineAddress;
            this.Country = usrdets.Country;
            this.PostCode = usrdets.PostCode;
        }
    }
}

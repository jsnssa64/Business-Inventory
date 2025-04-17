using System.ComponentModel.DataAnnotations;

namespace Domain.User
{
    public class UserDetails
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserAddress? userAddress { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public string? ContactNumber { get; set; }
    
        public void Map(dynamic usrdets)
        {
            this.FirstName = usrdets.FirstName;
            this.LastName = usrdets.LastName;

            if(this.userAddress is null)
            {
                this.userAddress = new UserAddress() { 
                    Country = usrdets.Country,
                    FirstLineAddress = usrdets.FirstLineAddress,
                    SecondLineAddress = usrdets.SecondLineAddress,
                    PostCode = usrdets.PostCode,
                };  
            }

            this.ContactNumber = usrdets.ContactNumber;
            this.DOB = usrdets.DOB;
            this.Gender = usrdets.Gender;
        }
    }
}

namespace Domain.User
{
    public class UserAddress
    {
        public string? FirstLineAddress { get; set; }
        public string? SecondLineAddress { get; set; }
        public string? Country { get; set; }
        public string? PostCode { get; set; }

        public void Map(dynamic usrdets)
        {
            try
            {
                this.FirstLineAddress = usrdets.FirstLineAddress;
                this.SecondLineAddress = usrdets.SecondLineAddress;
                this.Country = usrdets.Country;
                this.PostCode = usrdets.PostCode;
            }
            catch
            {
                throw new Exception("Unable to convert to UserAddress");
            }
        }
    }
}

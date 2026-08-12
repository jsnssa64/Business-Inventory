namespace Domain.ValueObjects.User
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
                FirstLineAddress = usrdets.FirstLineAddress;
                SecondLineAddress = usrdets.SecondLineAddress;
                Country = usrdets.Country;
                PostCode = usrdets.PostCode;
            }
            catch
            {
                throw new Exception("Unable to convert to UserAddress");
            }
        }
    }
}

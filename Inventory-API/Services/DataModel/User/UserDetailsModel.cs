namespace Services.DataModel.User
{
    public class UserDetailsModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? ContactNumber { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DOB { get; set; }
        public string? FirstLineAddress { get; set; }
        public string? SecondLineAddress { get; set; }
        public string? Country { get; set; }
        public string? PostCode { get; set; }
    }
}

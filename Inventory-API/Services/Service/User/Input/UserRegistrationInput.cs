namespace Services.Service.User.Input;

public record UserRegistrationInput(
    string Username,
    string Email,
    string Password,
    string Role
)
{
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
    public string ContactNumber { get; internal set; }
    public DateOnly? DOB { get; internal set; }
    public string Gender { get; internal set; }
    public object AddressLine1 { get; internal set; }
    public object AddressLine2 { get; internal set; }
    public object City { get; internal set; }
    public object State { get; internal set; }
    public string Country { get; internal set; }
    public object PostalCode { get; internal set; }
}
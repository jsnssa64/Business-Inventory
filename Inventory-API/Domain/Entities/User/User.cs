using Domain.ValueObjects.User;

namespace Domain.Entities.User
{
    public record User(string Username, string Email, UserRole Role);
}
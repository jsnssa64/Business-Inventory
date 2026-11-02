namespace Services.Service.User.Dto;

public record UserPasswordDto(
    string UserId,
    string Password
// TODO: decide whether create returns the full product or id only - if id only, reconsider the name (rule 7: <Noun>Dto)
);

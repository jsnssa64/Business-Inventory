CREATE PROCEDURE dbo.GetUserDetails
    @Username VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
           u.Id,
           u.Username,
           u.Email,
           ud.ContactNumber,
           ud.FirstName,
           ud.LastName,
           ud.FirstLineAddress,
           ud.SecondLineAddress,
           ud.Country,
           ud.PostCode,
           ud.DOB,
           ud.Country,
           ud.Gender
      FROM dbo.[User] u
      JOIN dbo.UserDetails ud
        ON u.Id = ud.UserId
     WHERE u.UserName = @Username 
           AND u.[Disabled] = 0
           AND u.Confirmed = 1;
END
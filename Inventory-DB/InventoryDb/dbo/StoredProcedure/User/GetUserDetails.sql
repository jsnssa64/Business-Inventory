CREATE PROCEDURE dbo.GetUserDetails
    @Username VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @UserId INT;

    EXEC dbo.GetActiveUserIdByUsername @Username, @UserId OUTPUT;

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
           ud.Gender
      FROM dbo.[User] u
      JOIN dbo.UserDetails ud
        ON u.Id = ud.UserId
     WHERE u.Id = @UserId
END
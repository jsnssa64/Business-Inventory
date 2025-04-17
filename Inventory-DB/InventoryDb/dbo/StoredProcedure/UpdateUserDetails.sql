CREATE PROCEDURE [dbo].[UpdateUserDetails]
    @Username VARCHAR(100),
    @Email VARCHAR(50) = NULL,
	@FirstName VARCHAR(50) = NULL,
	@LastName VARCHAR(50) = NULL,
	@ContactNumber VARCHAR(50) = NULL,
	@Gender VARCHAR(50) = NULL,
	@DOB DATE = NULL,
	@FirstLineAddress VARCHAR(150) = NULL,
	@SecondLineAddress VARCHAR(150) = NULL,
	@Country VARCHAR(50) = NULL,
	@PostCode VARCHAR(50) = NULL
AS
	SET NOCOUNT ON;
    SET XACT_ABORT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION
            
            DECLARE @UserId INT;
            -- Get internal ids
            EXEC dbo.GetUserId @Username, @UserId OUTPUT;

            -- Make sure the product exists for this user
            IF (@UserId IS NULL)
                THROW 50002, 'Product not found or does not belong to user.', 1;
            
            --  Declare Existing Values
            DECLARE @CurrentEmail VARCHAR(50),
                @CurrentFirstName VARCHAR(50),
	            @CurrentLastName VARCHAR(50),
	            @CurrentContactNumber VARCHAR(50),
	            @CurrentGender VARCHAR(50),
	            @CurrentDOB DATE,
	            @CurrentFirstLineAddress VARCHAR(150),
	            @CurrentSecondLineAddress VARCHAR(150),
	            @CurrentCountry VARCHAR(50),
	            @CurrentPostCode VARCHAR(50);

            --  Populate Existing Values
            SELECT 
                @CurrentEmail = u.Email,
                @CurrentFirstName = ud.FirstName,
                @CurrentLastName = ud.LastName,
                @CurrentContactNumber = ud.ContactNumber,
                @CurrentGender = ud.Gender,
                @CurrentDOB = ud.DOB,
                @CurrentFirstLineAddress = ud.FirstLineAddress,
                @CurrentSecondLineAddress = ud.SecondLineAddress,
                @CurrentCountry = ud.Country,
                @CurrentPostCode = ud.PostCode
            FROM dbo.UserDetails ud
            JOIN dbo.[User] u ON ud.UserId = u.Id
            WHERE ud.UserId = @UserId;

            UPDATE dbo.[User]
            SET 
                Email = ISNULL(@Email, @CurrentEmail)
            WHERE Id = @UserId;

            UPDATE dbo.UserDetails
            SET 
	            @FirstName = ISNULL(@FirstName, @CurrentFirstName),
	            @LastName = ISNULL(@LastName, @CurrentLastName),
	            @ContactNumber = ISNULL(@ContactNumber, @CurrentContactNumber),
	            @Gender = ISNULL(@Gender, @CurrentGender),
	            @DOB = ISNULL(@DOB, @CurrentDOB),
	            @FirstLineAddress = ISNULL(@FirstLineAddress, @CurrentFirstLineAddress),
	            @SecondLineAddress = ISNULL(@SecondLineAddress, @CurrentSecondLineAddress),
	            @Country = ISNULL(@Country, @CurrentCountry),
	            @PostCode = ISNULL(@PostCode, @CurrentPostCode)
            WHERE UserId = @UserId 
                AND UserId = @UserId;
        COMMIT TRANSACTION;
        RETURN 0;  -- success
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 -- Only transaction when child started the transaction
            ROLLBACK TRANSACTION;

        -- Optional: log the error here
        DECLARE @errMessage VARCHAR(100) = 'Error: ' + ERROR_MESSAGE();
        THROW 50001, @errMessage, 1;
    END CATCH
RETURN 0

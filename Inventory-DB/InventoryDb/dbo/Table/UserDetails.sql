CREATE TABLE [dbo].[UserDetails]
(
	UserId INT NOT NULL,
	FirstName VARCHAR(50) NOT NULL,
	LastName VARCHAR(50) NOT NULL,
	ContactNumber VARCHAR(50) NULL,
	Gender VARCHAR(50) NULL,
	DOB DATE NULL,
	FirstLineAddress VARCHAR(150) NULL,
	SecondLineAddress VARCHAR(150) NULL,
	Country VARCHAR(50) NULL,
	PostCode VARCHAR(50) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
	CONSTRAINT [FK_UserDetails_User] FOREIGN KEY (UserId) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [PK_UserDetails] PRIMARY KEY (UserId)
)

GO

CREATE NONCLUSTERED INDEX IX_UserDetails_UserId_Include
ON UserDetails(UserId)
INCLUDE (ContactNumber, FirstName, LastName, FirstLineAddress, SecondLineAddress, DOB, Country, Gender);

GO
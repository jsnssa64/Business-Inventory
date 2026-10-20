CREATE TABLE [org].[Business]
(
	[BusinessId] INT NOT NULL IDENTITY(1,1),
	[BusinessPublicId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_Business_BusinessPublicId] DEFAULT NEWID(),
	[Name] NVARCHAR(100) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_Business_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NULL,
	[DeactivatedAt] DATETIME2 NULL,
	CONSTRAINT [PK_Business] PRIMARY KEY ([BusinessId]),
	CONSTRAINT [UQ_Business_BusinessPublicId] UNIQUE ([BusinessPublicId]),
	CONSTRAINT [UQ_Business_Name] UNIQUE ([Name])
)

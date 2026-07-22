CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL IDENTITY(1,1),		--	Internal ID
	PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), -- External/public-facing ID
	[Name] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(100) NOT NULL,
	Quantity INT NOT NULL DEFAULT 0,
	EnabledPrice BIT NOT NULL DEFAULT 0,
	UserId INT NOT NULL,
	[Disabled] BIT NOT NULL DEFAULT 0,
	[Version] INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    DisabledAt DATETIME NULL,
	CONSTRAINT [FK_Product_User] FOREIGN KEY (UserId) REFERENCES [dbo].[User](Id),
	CONSTRAINT [PK_Product] PRIMARY KEY (Id)
)

GO 

CREATE NONCLUSTERED INDEX IX_User_Products ON Product(UserId, [Id], [Version])
INCLUDE(PublicId, [Name], [Description], Quantity)

GO
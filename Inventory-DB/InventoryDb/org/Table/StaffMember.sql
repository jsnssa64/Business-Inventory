CREATE TABLE [org].[StaffMember]
(
	[StaffMemberId] INT NOT NULL IDENTITY(1,1),
	[StaffMemberPublicId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_StaffMember_StaffMemberPublicId] DEFAULT NEWID(),
	[BusinessId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Email] NVARCHAR(254) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_StaffMember_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NULL,
	[DeactivatedAt] DATETIME2 NULL,
	CONSTRAINT [PK_StaffMember] PRIMARY KEY ([StaffMemberId]),
	CONSTRAINT [UQ_StaffMember_StaffMemberPublicId] UNIQUE ([StaffMemberPublicId]),
	CONSTRAINT [UQ_StaffMember_Email] UNIQUE ([Email]),
	CONSTRAINT [FK_StaffMember_Business] FOREIGN KEY ([BusinessId]) REFERENCES [org].[Business]([BusinessId])
)

GO

CREATE NONCLUSTERED INDEX [IX_StaffMember_BusinessId] ON [org].[StaffMember]([BusinessId])

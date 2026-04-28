CREATE TABLE [dbo].[ServiceSubscription]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	WebhookURI VARCHAR(100) NOT NULL,
	TriggerAction VARCHAR(100) NOT NULL,
	UserId INT NOT NULL,
	SharedSecret VARCHAR(100) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
	CONSTRAINT [FK_User_Subscription] FOREIGN KEY (UserId) REFERENCES [dbo].[User]([Id]),
	CONSTRAINT [PK_ServiceSubscription] PRIMARY KEY (Id)
)

GO
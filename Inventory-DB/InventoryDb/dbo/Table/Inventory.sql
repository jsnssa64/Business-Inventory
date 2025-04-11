CREATE TABLE [dbo].[Inventory]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Quantity INT NOT NULL,
	ProductId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
	CONSTRAINT [FK_Inventory_Product] FOREIGN KEY (ProductId) REFERENCES [dbo].[Product]([Id])
)

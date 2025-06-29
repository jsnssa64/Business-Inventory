CREATE TABLE [dbo].[Inventory]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ProductId INT UNIQUE NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
	Quantity INT NOT NULL DEFAULT 0,
	CONSTRAINT [FK_Inventory_Product] FOREIGN KEY (ProductId) REFERENCES [dbo].[Product]([Id])
)

GO

CREATE NONCLUSTERED INDEX IX_Inventory_Product ON Inventory(ProductId) 
INCLUDE (Quantity)
WHERE Quantity > 0;
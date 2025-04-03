CREATE TABLE [dbo].[Inventory]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Quantity INT NOT NULL,
	InventoryItemId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
	CONSTRAINT [FK_Inventory_InventoryItem] FOREIGN KEY (InventoryItemId) REFERENCES [dbo].[InventoryItem]([Id])
)

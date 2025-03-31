CREATE TABLE [dbo].[Inventory]
(
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Qty INT NOT NULL,
	[InventoryItemId] INT NOT NULL
	CONSTRAINT [FK_Inventory_InventoryItem] FOREIGN KEY ([InventoryItemId]) REFERENCES [dbo].[InventoryItem]([Id])
)

CREATE TABLE [supplierregistry].[SupplierAccess]
(
	[SupplierId] INT NOT NULL,
	[BusinessPublicId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_SupplierAccess] PRIMARY KEY ([SupplierId], [BusinessPublicId]),
	CONSTRAINT [FK_SupplierAccess_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [supplierregistry].[Supplier]([SupplierId])
)

GO

CREATE NONCLUSTERED INDEX [IX_SupplierAccess_BusinessPublicId] ON [supplierregistry].[SupplierAccess]([BusinessPublicId])

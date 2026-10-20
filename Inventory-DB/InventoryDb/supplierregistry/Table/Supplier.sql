CREATE TABLE [supplierregistry].[Supplier]
(
	[SupplierId] INT NOT NULL IDENTITY(1,1),
	[SupplierPublicId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_SupplierRegistrySupplier_SupplierPublicId] DEFAULT NEWID(),
	[Name] NVARCHAR(100) NOT NULL,
	[IsRestricted] BIT NOT NULL CONSTRAINT [DF_SupplierRegistrySupplier_IsRestricted] DEFAULT 0,
	[LiveAt] DATETIME2 NULL,
	[CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_SupplierRegistrySupplier_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NULL,
	[DeactivatedAt] DATETIME2 NULL,
	CONSTRAINT [PK_SupplierRegistrySupplier] PRIMARY KEY ([SupplierId]),
	CONSTRAINT [UQ_SupplierRegistrySupplier_SupplierPublicId] UNIQUE ([SupplierPublicId]),
	CONSTRAINT [UQ_SupplierRegistrySupplier_Name] UNIQUE ([Name])
)

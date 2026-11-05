CREATE TABLE [hostedcatalogue].[Supplier]
(
	[SupplierId] INT NOT NULL IDENTITY(1,1),
	[SupplierPublicId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_HostedCatalogueSupplier_SupplierPublicId] DEFAULT NEWID(),
	[Name] NVARCHAR(100) NOT NULL,
	[CurrencyCode] CHAR(3) NOT NULL,
	[CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_HostedCatalogueSupplier_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NULL,
	[DeactivatedAt] DATETIME2 NULL,
	CONSTRAINT [PK_HostedCatalogueSupplier] PRIMARY KEY ([SupplierId]),
	CONSTRAINT [UQ_HostedCatalogueSupplier_SupplierPublicId] UNIQUE ([SupplierPublicId]),
	CONSTRAINT [UQ_HostedCatalogueSupplier_Name] UNIQUE ([Name])
)

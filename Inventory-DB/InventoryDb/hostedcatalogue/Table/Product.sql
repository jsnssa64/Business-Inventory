CREATE TABLE [hostedcatalogue].[Product]
(
	[ProductId] INT NOT NULL IDENTITY(1,1),
	[ProductPublicId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_HostedCatalogueProduct_ProductPublicId] DEFAULT NEWID(),
	[SupplierId] INT NOT NULL,
	[Sku] NVARCHAR(50) NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(500) NULL,
	[ItemsPerPack] INT NOT NULL,
	[ItemContentAmount] DECIMAL(10,3) NOT NULL,
	[ItemContentUnitCode] VARCHAR(3) NOT NULL,
	[ListPriceAmount] DECIMAL(19,4) NOT NULL,
	[DiscontinuedAt] DATETIME2 NULL,
	[CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_HostedCatalogueProduct_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[UpdatedAt] DATETIME2 NULL,
	CONSTRAINT [PK_HostedCatalogueProduct] PRIMARY KEY ([ProductId]),
	CONSTRAINT [UQ_HostedCatalogueProduct_ProductPublicId] UNIQUE ([ProductPublicId]),
	CONSTRAINT [UQ_HostedCatalogueProduct_SupplierId_Sku] UNIQUE ([SupplierId], [Sku]),
	CONSTRAINT [FK_HostedCatalogueProduct_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [hostedcatalogue].[Supplier]([SupplierId])
)

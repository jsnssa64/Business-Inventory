CREATE TABLE [dbo].ProductPrice
(
	ProductId INT NOT NULL PRIMARY KEY,
	Price DECIMAL(19, 4) NOT NULL,
	CurrencyCode CHAR(3) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
	CONSTRAINT [FK_ProductPrice_Price] FOREIGN KEY (ProductId) REFERENCES [dbo].Product(Id)
)

GO 

CREATE NONCLUSTERED INDEX IX_ProductPrice_Product ON ProductPrice(ProductId)
INCLUDE(Price, CurrencyCode)

GO
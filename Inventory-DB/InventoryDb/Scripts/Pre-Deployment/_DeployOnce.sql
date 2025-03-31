IF NOT EXISTS (SELECT * FROM dbo.CheckTableExist('DeployOnce'))
BEGIN
	CREATE TABLE [dbo].[_deployOnce](
		[Id] INT NOT NULL,
		[FileName] [nchar](10) NULL
	) ON [PRIMARY]
END
GO
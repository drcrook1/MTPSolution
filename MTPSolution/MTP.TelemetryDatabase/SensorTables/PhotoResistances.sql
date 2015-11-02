CREATE TABLE [dbo].[PhotoResistances]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Location] NCHAR(25) NOT NULL, 
    [LightRatio] FLOAT NOT NULL, 
    [CollectionTime] DATETIME NOT NULL,
	[DeviceId] NCHAR(25) NULL
)

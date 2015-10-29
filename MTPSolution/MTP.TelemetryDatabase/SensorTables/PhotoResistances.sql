CREATE TABLE [dbo].[PhotoResistances]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Location] NCHAR(25) NOT NULL, 
    [PhotoResistance] INT NOT NULL, 
    [CollectionTime] DATETIME NOT NULL
)

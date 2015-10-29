CREATE TABLE [dbo].[Temperatures]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Location] NCHAR(25) NOT NULL, 
    [Temperature] FLOAT NOT NULL, 
    [CollectionTime] DATETIME NOT NULL
)

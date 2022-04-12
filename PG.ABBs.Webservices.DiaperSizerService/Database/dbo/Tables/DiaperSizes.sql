CREATE TABLE [dbo].[DiaperSizes]
(
	[DiaperSizeID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
    [Size] INT NOT NULL, 
    [AverageNumberOfDiapers] NVARCHAR(25) NULL
)

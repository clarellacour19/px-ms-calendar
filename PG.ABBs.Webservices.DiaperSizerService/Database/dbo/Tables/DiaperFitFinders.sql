CREATE TABLE [dbo].[DiaperFitFinders]
(
	[DiaperFitFinderID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
	[MarketInfoID] UNIQUEIDENTIFIER NOT NULL,
	[BabyWeightValue] INT NOT NULL, 
	[BabyWeightDescription] NVARCHAR(25) NOT NULL, 
    [WeightRange] NVARCHAR(25) NULL, 
    [AverageDiapersPerDay] NVARCHAR(10) NULL, 
    [LastAroundMonths] NUMERIC(3, 1) NOT NULL,
	CONSTRAINT FK_DiaperFitFinders_MarketInfos_MarketInfoID FOREIGN KEY (MarketInfoID) REFERENCES MarketInfos(MarketInfoID)
    
)

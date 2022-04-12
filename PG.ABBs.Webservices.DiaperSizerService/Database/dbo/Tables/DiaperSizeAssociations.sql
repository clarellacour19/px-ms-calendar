CREATE TABLE [dbo].[DiaperSizeAssociations]
(
	[DiaperSizeAssociationID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT newid(), 
    [DiaperFitFinderID] UNIQUEIDENTIFIER NOT NULL, 
    [DiaperSizeID] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT FK_DiaperSizeAssociations_DiaperFitFinders_DiaperFitFinderID FOREIGN KEY (DiaperFitFinderID) REFERENCES DiaperFitFinders(DiaperFitFinderID),
	CONSTRAINT FK_DiaperSizeAssociations_DiaperSizes_DiaperSizeID FOREIGN KEY (DiaperSizeID) REFERENCES DiaperSizes(DiaperSizeID)
)

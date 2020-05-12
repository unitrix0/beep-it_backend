USE [Beep]
GO

/****** Object:  StoredProcedure [dbo].[CreateDemoDataForUser]    Script Date: 11.05.2020 20:53:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		M. Rebsamen
-- Create date: 09.05.2020
-- Description:	Erzeugt Demo Daten für die Angegebene UserId
-- =============================================
CREATE OR ALTER   PROCEDURE [dbo].[CreateDemoDataForUser]
-- Add the parameters for the stored procedure here
--<@Param2, sysname, @p2> <Datatype_For_Param2, , int> = <Default_Value_For_Param2, , 0>
@UserId int
AS
BEGIN
DECLARE @environmentId INT;
SET @environmentId = (SELECT Id FROM Environments WHERE UserId = @UserId)


-- Create ArticleUserSettings
INSERT INTO ArticleUserSettings (ArticleId, EnvironmentId, KeepStockAmount, ArticleGroupId, UsualLifetime)
SELECT Id,
	@environmentId,
	(SELECT abs(convert(varbinary, NewId()) % 5) as nr),
	1,
	0
FROM Articles
	
-- Create Stock Entries
INSERT INTO StockEntries (ArticleId, EnvironmentId)
SELECT Id,
	@environmentId
FROM Articles

-- StockEntryValues
INSERT INTO StockEntryValues (ArticleId, EnvironmentId, IsOpened, AmountOnStock, ExpireDate, OpenedOn, AmountRemaining)
SELECT Id as ArticleId,
	@environmentId as EnvironmentId,
	1,
	1 as AmountOnStock,
	(SELECT DATEADD(m,(SELECT abs(convert(varbinary, NewId()) % 12)), GETDATE())) as ExpireDate,
	'0001-01-01 00:00:00.0000000' as OpenedOn,
	1 as AmountRemaining
FROM Articles
	
INSERT INTO StockEntryValues (ArticleId, EnvironmentId, IsOpened, AmountOnStock, ExpireDate, OpenedOn, AmountRemaining)
SELECT Id as ArticleId,
	@environmentId as EnvironmentId,
	0,
	(SELECT abs(convert(varbinary, NewId()) %5)+1 as nr) as AmountOnStock,
	(SELECT DATEADD(m,(SELECT abs(convert(varbinary, NewId()) % 12)), GETDATE())) as ExpireDate,
	'0001-01-01 00:00:00.0000000' as OpenedOn,
	1 as AmountRemaining
FROM Articles


UPDATE StockEntryValues 
Set AmountRemaining = ROUND(1.0/(SELECT abs(convert(varbinary, NewId()) %3)+2 as nr),2)
WHERE IsOpened = 1 AND EnvironmentId = @environmentId

-- Article Group
INSERT INTO ArticleGroups (Name, UserId, KeepStockAmount)
VALUES ('Confiture', @UserId,
		(SELECT Sum(AmountOnStock)+1 FROM StockEntryValues sev
		INNER JOIN Articles a on a.Id = sev.ArticleId
		WHERE a.Name like 'Favorit Konfitüre%' AND sev.EnvironmentId = @environmentId))

UPDATE ArticleUserSettings 
	SET ArticleGroupId = (SELECT Id FROM ArticleGroups WHERE Name = 'Confiture' AND UserId = @UserId)
	WHERE ArticleId IN (SELECT Id FROM Articles WHERE Name like 'Favorit Konfitüre%')
	  AND EnvironmentId = @environmentId
END
GO


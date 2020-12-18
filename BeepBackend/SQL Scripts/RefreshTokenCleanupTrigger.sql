/****** Object:  Trigger [dbo].[CleanupAfterInsert]    Script Date: 21.07.2020 13:40:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Marco Rebsamen
-- Create date: 21.07.2020
-- Description:	Entfernt verwendete/abgelaufene Refresh Tokens
-- =============================================
CREATE TRIGGER [dbo].[CleanupAfterInsert]
   ON  [dbo].[RefreshTokens]
   AFTER INSERT
AS 
BEGIN
    DELETE FROM RefreshTokens where Used = 1 or Invalidated = 1 or ExpiryDate < DATEADD(m,-1,GETUTCDATE())
END
GO

ALTER TABLE [dbo].[RefreshTokens] ENABLE TRIGGER [CleanupAfterInsert]
GO


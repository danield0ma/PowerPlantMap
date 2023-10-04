USE [PPM]
GO
/****** Object:  StoredProcedure [dbo].[GetPastActivity]    Script Date: 15/10/2022 19:49:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetPastActivity] @GID nvarchar(20), @start smalldatetime, @end smalldatetime

AS

SELECT * FROM PastActivity
WHERE GeneratorID = @GID AND PeriodStart >= @start AND PeriodStart < @end
ORDER BY 2 ASC
USE [PPM]
GO
/****** Object:  StoredProcedure [dbo].[GetLastDataTime]    Script Date: 31/10/2022 16:38:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetLastDataTime] @PPID nvarchar(4)

AS

SELECT TOP 1 PeriodStart
FROM PastActivity PA
GROUP BY PeriodStart
HAVING COUNT(GeneratorID) >= (
		SELECT COUNT(GeneratorID) - 5
		FROM Generators
	)
ORDER BY PeriodStart DESC
--		INNER JOIN BlocGenerators BG ON PA.GeneratorID = BG.GeneratorID
--		INNER JOIN Blocs B ON BG.BlocID = B.BlocID
--		INNER JOIN PPBlocs PPB ON B.BlocID = PPB.BlocID
--WHERE PPB.PowerPlantID = @PPID
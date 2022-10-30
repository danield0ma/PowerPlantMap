USE [PPM]
GO
/****** Object:  StoredProcedure [dbo].[GetLastDataTime]    Script Date: 30/10/2022 18:02:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetLastDataTime] @PPID nvarchar(4)

AS

SELECT TOP 1 PeriodEnd
FROM PastActivity PA
GROUP BY PeriodEnd
HAVING COUNT(GeneratorID) >= (
		SELECT COUNT(GeneratorID) - 5
		FROM Generators
	)
ORDER BY PeriodEnd DESC
--		INNER JOIN BlocGenerators BG ON PA.GeneratorID = BG.GeneratorID
--		INNER JOIN Blocs B ON BG.BlocID = B.BlocID
--		INNER JOIN PPBlocs PPB ON B.BlocID = PPB.BlocID
--WHERE PPB.PowerPlantID = @PPID
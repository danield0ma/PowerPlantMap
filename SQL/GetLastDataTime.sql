CREATE PROCEDURE [dbo].[GetLastDataTime] @PPID nvarchar(4)

AS

SELECT MAX(PeriodEnd)
FROM PastActivity PA INNER JOIN BlocGenerators BG ON PA.GeneratorID = BG.GeneratorID
		INNER JOIN Blocs B ON BG.BlocID = B.BlocID
		INNER JOIN PPBlocs PPB ON B.BlocID = PPB.BlocID
WHERE PPB.PowerPlantID = @PPID
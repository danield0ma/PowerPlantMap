CREATE PROCEDURE [dbo].[GetGeneratorsOfPowerPlant] @PPID nvarchar(4)

AS

SELECT BG.GeneratorID
FROM BlocGenerators BG INNER JOIN Blocs B ON B.BlocID = BG.BlocID
					INNER JOIN PPBlocs PPB ON B.BlocID = PPB.BlocID
WHERE PPB.PowerPlantID = @PPID
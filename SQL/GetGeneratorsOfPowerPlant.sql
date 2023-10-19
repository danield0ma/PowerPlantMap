CREATE PROCEDURE [dbo].[GetGeneratorsOfPowerPlant] @PowerPlantId nvarchar(4)

AS

SELECT BG.GeneratorID
FROM BlocGenerators BG INNER JOIN Blocs B ON B.BlocID = BG.BlocID
					INNER JOIN PowerPlantBlocs PPB ON B.BlocId = PPB.BlocId
WHERE PPB.PowerPlantId = @PowerPlantId
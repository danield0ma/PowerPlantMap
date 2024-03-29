CREATE PROCEDURE [dbo].[GetPowerPlantDetails] @PowerPlantId varchar(4)

AS

SELECT B.BlocId, B.BlocType, B.MaxBlocCapacity, B.CommissionDate, G.GeneratorId, G.MaxCapacity
FROM PowerPlantBlocs PPB INNER JOIN Blocs B ON PPB.BlocId = B.BlocId
			   INNER JOIN BlocGenerators BG ON B.BlocID = BG.BlocID
			   INNER JOIN  Generators G ON BG.GeneratorId = G.GeneratorId
WHERE PowerPlantId = @PowerPlantId
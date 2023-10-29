CREATE PROCEDURE [dbo].[GetPowerPlantModel] @PowerPlantId nvarchar(4)

AS

SELECT PP.PowerPlantId, PP.Name, PP.Description, PP.OperatorCompany, PP.Webpage, PP.Image, PP.Longitude, PP.Latitude,
	PP.Color, PP.Address, PP.IsCountry, B.BlocId, B.BlocType, B.MaxBlocCapacity, B.CommissionDate, G.GeneratorId, G.MaxCapacity
FROM PowerPlants PP
	INNER JOIN PowerPlantBlocs PPB ON PP.PowerPlantId = PPB.PowerPlantId
	INNER JOIN Blocs B ON PPB.BlocId = B.BlocId
	INNER JOIN BlocGenerators BG ON B.BlocID = BG.BlocID
	INNER JOIN  Generators G ON BG.GeneratorId = G.GeneratorId
WHERE PP.PowerPlantId = @PowerPlantId
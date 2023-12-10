CREATE PROCEDURE [dbo].[DeletePowerPlant] @PowerPlantId nvarchar(4)

AS

DELETE FROM PowerPlants
WHERE PowerPlantId = @PowerPlantId
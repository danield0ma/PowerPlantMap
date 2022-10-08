CREATE PROCEDURE [dbo].[GetBasicsOfPowerPlant] @id varchar(4)

AS

SELECT * FROM PowerPlants
WHERE PowerPlantID = @id
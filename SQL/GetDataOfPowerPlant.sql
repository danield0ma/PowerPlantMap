CREATE PROCEDURE [dbo].[GetDataOfPowerPlant] @Id varchar(4)

AS

SELECT * FROM PowerPlants
WHERE PowerPlantId = @Id
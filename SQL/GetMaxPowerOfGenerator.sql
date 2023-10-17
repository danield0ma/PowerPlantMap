CREATE PROCEDURE [dbo].[GetMaxPowerOfGenerator] @generatorId nvarchar(20)

AS

SELECT MaxCapacity
FROM Generators
WHERE GeneratorID = @generatorId
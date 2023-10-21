CREATE PROCEDURE [dbo].[GetMaxPowerOfGenerator] @GeneratorId nvarchar(20)

AS

SELECT MaxCapacity
FROM Generators
WHERE GeneratorId = @GeneratorId
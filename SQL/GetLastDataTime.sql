CREATE PROCEDURE [dbo].[GetLastDataTime]

AS

SELECT TOP 1 PeriodStart
FROM PastActivity PA
GROUP BY PeriodStart
HAVING COUNT(GeneratorID) >= (
    SELECT COUNT(GeneratorID) - 5
    FROM Generators
)
ORDER BY PeriodStart DESC
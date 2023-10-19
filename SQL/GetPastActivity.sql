CREATE PROCEDURE [dbo].[GetPastActivity] @GeneratorId nvarchar(20), @PeriodStart smalldatetime, @PeriodEnd smalldatetime

AS

SELECT * FROM PastActivity
WHERE GeneratorID = @GeneratorId AND PeriodStart >= @PeriodStart AND PeriodStart < @PeriodEnd
ORDER BY 2 ASC
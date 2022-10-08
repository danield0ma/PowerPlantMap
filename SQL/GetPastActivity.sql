CREATE PROCEDURE [dbo].[GetPastActivity] @GID nvarchar(20)

AS

SELECT TOP(3) * FROM PastActivity
WHERE GeneratorID = @GID
ORDER BY 2 DESC
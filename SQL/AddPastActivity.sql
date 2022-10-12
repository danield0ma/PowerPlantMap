CREATE PROCEDURE [dbo].[AddPastActivity] @GID nvarchar(20),
		@start smalldatetime, @end smalldatetime, @power smallint

AS

INSERT INTO PastActivity (GeneratorID, PeriodStart, PeriodEnd, ActualPower)
VALUES (@GID, @start, @end, @power)
CREATE PROCEDURE [dbo].[AddPastActivity] @GID nvarchar(20),
		@start smalldatetime, @power smallint

AS

INSERT INTO PastActivity (GeneratorID, PeriodStart, ActualPower)
VALUES (@GID, @start, @power)
CREATE PROCEDURE [dbo].[AddPastActivity] @GeneratorId nvarchar(20), @PeriodStart smalldatetime, @ActualPower smallint

AS

INSERT INTO PastActivity (GeneratorId, PeriodStart, ActualPower)
VALUES (@GeneratorId, @PeriodStart, @ActualPower)
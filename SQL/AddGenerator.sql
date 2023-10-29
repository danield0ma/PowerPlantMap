CREATE PROCEDURE [dbo].[AddGenerator] @GeneratorId nvarchar(20), @MaxCapacity smallint

AS

INSERT INTO Generators(GeneratorId, MaxCapacity)
VALUES (@GeneratorId, @MaxCapacity)
CREATE PROCEDURE [dbo].[DeleteGenerator] @GeneratorId nvarchar(20)

AS

DELETE FROM Generators
WHERE GeneratorId = @GeneratorId

DELETE FROM BlocGenerators
WHERE GeneratorId = @GeneratorId
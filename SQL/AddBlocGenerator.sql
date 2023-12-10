CREATE PROCEDURE [dbo].[AddBlocGenerator] @BlocId nvarchar(20), @GeneratorId nvarchar(20)

AS

INSERT INTO BlocGenerators(BlocId, GeneratorId)
VALUES (@BlocId, @GeneratorId)
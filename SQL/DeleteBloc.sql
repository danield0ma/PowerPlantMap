CREATE PROCEDURE [dbo].[DeleteBloc] @BlocId nvarchar(20)

AS

DELETE FROM Blocs
WHERE BlocId = @BlocId

DELETE FROM PowerPlantBlocs
WHERE BlocId = @BlocId
CREATE PROCEDURE [dbo].[AddPowerPlantBloc] @PowerPlantId nvarchar(4), @BlocId nvarchar(20)

AS

INSERT INTO PowerPlantBlocs(PowerPlantId, BlocId)
VALUES (@PowerPlantId, @BlocId)
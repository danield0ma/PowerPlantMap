CREATE PROCEDURE [dbo].[AddPowerPlant] @PowerPlantId nvarchar(4), @Name nvarchar(20), @Description nvarchar(50),
	@OperatorCompany nvarchar(50), @Webpage nvarchar(70), @Image nvarchar(20), @Longitude float = NULL, @Latitude float = NULL,
	@Color char(6), @Address nvarchar(50), @IsCountry bit

AS

INSERT INTO PowerPlants (PowerPlantId, Name, Description, OperatorCompany, Webpage, Image, Longitude, Latitude, Color, Address, IsCountry)
VALUES (@PowerPlantId, @Name, @Description, @OperatorCompany, @Webpage, @Image, @Longitude, @Latitude, @Color, @Address, @IsCountry)
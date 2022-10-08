USE [PPM]
GO
/****** Object:  StoredProcedure [dbo].[GetPowerPlantDetails]    Script Date: 08/10/2022 16:05:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetPowerPlantDetails] @PowerPlantID varchar(4)

AS

SELECT B.BlocID, B.BlocType, B.MaxBlocCapacity, B.ComissionDate, G.GeneratorID, G.MaxCapacity
FROM PPBlocs P INNER JOIN Blocs B ON P.BlocID = B.BlocID
			   INNER JOIN BlocGenerators BG ON B.BlocID = BG.BlocID
			   INNER JOIN  Generators G ON BG.GeneratorID = G.GeneratorID
WHERE PowerPlantID = @PowerPlantID
USE [PPM]
GO
/****** Object:  StoredProcedure [dbo].[GetLastDataTime]    Script Date: 22/10/2022 17:05:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetLastDataTime] @PPID nvarchar(4)

AS

SELECT MAX(PeriodEnd)
FROM PastActivity PA 
--		INNER JOIN BlocGenerators BG ON PA.GeneratorID = BG.GeneratorID
--		INNER JOIN Blocs B ON BG.BlocID = B.BlocID
--		INNER JOIN PPBlocs PPB ON B.BlocID = PPB.BlocID
--WHERE PPB.PowerPlantID = @PPID
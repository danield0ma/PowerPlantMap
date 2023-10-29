CREATE PROCEDURE [dbo].[AddBloc] @BlocId nvarchar(20), @BlocType nvarchar(20), @MaxBlocCapacity int, @CommissionDate int

AS

INSERT INTO Blocs(BlocId, BlocType, MaxBlocCapacity, CommissionDate)
VALUES (@BlocId, @BlocType, @MaxBlocCapacity, @CommissionDate)
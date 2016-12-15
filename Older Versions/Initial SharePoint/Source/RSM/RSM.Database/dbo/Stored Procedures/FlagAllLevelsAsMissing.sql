

CREATE PROCEDURE [dbo].[FlagAllLevelsAsMissing]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   UPDATE AccessLevels SET Missing = 1
END


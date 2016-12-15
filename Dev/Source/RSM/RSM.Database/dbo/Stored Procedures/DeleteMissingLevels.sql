

CREATE PROCEDURE [dbo].[DeleteMissingLevels]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DELETE FROM AccessLevelRoles where AccessLevelID in (SELECT AccessLevelID from AccessLevels where Missing = 1)
	DELETE FROM AccessLevels where Missing = 1
END


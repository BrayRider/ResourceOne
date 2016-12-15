

CREATE PROCEDURE [dbo].[DeleteLevel]
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DELETE FROM AccessLevelRoles where AccessLevelID = @ID
	DELETE FROM AccessLevels where AccessLevelID = @ID
END



CREATE PROCEDURE [dbo].[DeleteRole]
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM JCLRole WHERE RoleID = @ID
	UPDATE People SET NeedsRulePass = 1 WHERE PersonID in (SELECT PersonID FROM PeopleRoles WHERE RoleID = @ID)
	DELETE FROM PeopleRoles WHERE RoleID = @ID
	DELETE FROM Roles WHERE RoleID = @ID
END

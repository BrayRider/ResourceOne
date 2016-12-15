

CREATE PROCEDURE [dbo].[DeleteRule]
	@ID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DELETE FROM JCLRole WHERE RuleID = @ID
    DELETE FROM JCLRoleRule WHERE ID = @ID 
END


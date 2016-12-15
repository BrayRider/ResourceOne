




CREATE procedure [dbo].[RolesAvialableForRule]
	@RuleID int
as
-- Procedure Name : RolesAvialableForRule
-- Created By : Donavan Stanley
-- Created On : 2/23/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select RoleID, RoleName, RoleDesc
		from Roles 
		where Roles.RoleID not in (select RoleID from JCLRole where JCLRole.RuleID = @RuleID)
		order by Roles.RoleName 

end












CREATE procedure [dbo].[RulesWithRole]
	@RoleID int
as
-- Procedure Name : [RulesWithRole]
-- Created By : Donavan Stanley
-- Created On : 2/8/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select * 
		from JCLRole   
		where RuleID   in (select RuleID  from JCLRoleRule  where RoleID = @RoleID)
		 

end







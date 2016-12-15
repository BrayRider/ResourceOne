



CREATE procedure [dbo].[RolesAvialableForPerson]
	@PersonID int
as
-- Procedure Name : RolesAvialableForPerson
-- Created By : Donavan Stanley
-- Created On : 2/8/2011 8:16:53 AM
begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select RoleID, RoleName, RoleDesc
		from Roles 
		where Roles.RoleID not in (select RoleID from PeopleRoles where PeopleRoles.PersonID = @PersonID)
		order by Roles.RoleName 

end




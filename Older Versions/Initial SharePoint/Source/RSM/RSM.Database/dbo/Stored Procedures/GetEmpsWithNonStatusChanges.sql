






CREATE procedure [dbo].[GetEmpsWithNonStatusChanges]

as

begin
	-- local variables

	-- set nocount on and default isolation level
	set nocount on
	set transaction isolation level Read Committed
	
		select * 
		from dbo.EmpWithNonStatusChanges
		 

end








CREATE FUNCTION dbo.PeoplePendingUpload 
(
)
RETURNS int
AS
BEGIN
	
	
	RETURN (select count(personID) from People where NeedsUpload = 1)

END

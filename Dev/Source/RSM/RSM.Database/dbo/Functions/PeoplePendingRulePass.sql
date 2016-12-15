
	CREATE FUNCTION dbo.PeoplePendingRulePass 
(
)
RETURNS int
AS
BEGIN
	
	
	RETURN (select count(personID) from People where NeedsRulePass = 1)

END

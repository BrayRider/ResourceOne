

CREATE VIEW [dbo].[NewLocations]
AS
SELECT     distinct Facility
FROM         dbo.People
WHERE     (Facility NOT IN
                          (SELECT     LocationName
                            FROM          dbo.Locations))



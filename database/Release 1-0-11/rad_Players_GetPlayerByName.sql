IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rad_Players_GetPlayerByName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[rad_Players_GetPlayerByName]
GO


CREATE PROCEDURE dbo.rad_Players_GetPlayerByName
(
	@WorldName			nvarchar(64),
	@FirstName			nvarchar(32)
)
AS
BEGIN
	DECLARE @PlayerId int
	
	SELECT @PlayerId = PlayerId FROM dbo.rad_Players WHERE FirstName LIKE @FirstName + '%'

	-- Player
	SELECT
		*
	FROM
		dbo.rad_vw_Players
	WHERE	
		ObjectId = @PlayerId

	-- Owned Objects
	SELECT
		ObjectId,
		WorldId,
		OwnerPlayerId AS OwnerObjectId,
		TemplateId,
		TypeName,
		ObjectName,
		Properties,
		TemplateProperties
	FROM
		dbo.rad_vw_Objects
	WHERE	
		OwnerPlayerId = @PlayerId

	-- Objects Owned one level further such as items in a backpack
	SELECT
		o.ObjectId,
		o.WorldId,
		o.OwnerObjectId,
		o.TemplateId,
		o.TypeName,
		o.ObjectName,
		o.Properties,
		o.TemplateProperties
	FROM
		dbo.rad_vw_Objects o
		INNER JOIN dbo.rad_Objects p ON o.OwnerObjectId = p.ObjectId
	WHERE	
		p.OwnerPlayerId = @PlayerId
END
GO

exec rad_Players_GetPlayerByName 'Perenthia', 'Aly'
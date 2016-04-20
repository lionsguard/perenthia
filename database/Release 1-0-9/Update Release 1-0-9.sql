-- Create new Table rad_FileUpdates
create table dbo.rad_FileUpdates
(
    FileUpdateId        int not null identity(1,1) primary key clustered,
    WorldId             int not null,
    FileUpdateName      nvarchar(64) not null,
    LastUpdateDate      datetime not null
)
go

alter table dbo.rad_FileUpdates add constraint fk_FileUpdates_WorldId foreign key (WorldId) references dbo.rad_Worlds(WorldId)
go

insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'skills', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'skillgroups', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'races', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'terrain', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'delcor', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'angarath road', getdate())
insert into dbo.rad_FileUpdates (WorldId, FileUpdateName, LastUpdateDate) values (1, 'city of angarath', getdate())
go

CREATE PROCEDURE dbo.rad_FileUpdates_GetAll
(
    @WorldName          nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId    int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    SELECT * FROM dbo.rad_FileUpdates WHERE WorldId = @WorldId
END
go

CREATE PROCEDURE dbo.rad_FileUpdates_Save
(
	@WorldName			nvarchar(64),
	@FileUpdateName		nvarchar(64),
	@LastUpdateDate		datetime
)
AS
BEGIN
    DECLARE @WorldId    int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
	IF EXISTS(SELECT 1 FROM dbo.rad_FileUpdates WHERE WorldId = @WorldId AND FileUpdateName = @FileUpdateName)
	BEGIN
		UPDATE 
			dbo.rad_FileUpdates
		SET
			LastUpdateDate = @LastUpdateDate
		WHERE
			WorldId = @WorldId
			AND FileUpdateName = @FileUpdateName
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_FileUpdates
		(
			WorldId,
			FileUpdateName,
			LastUpdateDate
		)
		VALUES
		(
			@WorldId,
			@FileUpdateName,
			@LastUpdateDate
		)
	END
END
go

ALTER PROCEDURE [dbo].[rad_Maps_Update]
(
	@WorldName			nvarchar(64),
	@MapName			nvarchar(64),
	@StartX				int,
	@StartY				int,
	@EndX				int,
	@EndY				int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @Width	int,
			@Height int

	SET @Width = @EndX - @StartX
	SET @Height = @EndY - @StartY
	
	IF EXISTS(SELECT 1 FROM dbo.rad_Maps WHERE MapName = @MapName)
	BEGIN
		UPDATE
			dbo.rad_Maps
		SET
			X		= @StartX,
			Y		= @StartY,
			Width	= @Width,
			Height	= @Height
		WHERE
			MapName = @MapName
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_Maps
		(
			WorldId,
			MapName,
			X,
			Y,
			Width,
			Height
		)
		VALUES
		(
			@WorldId,
			@MapName,
			@StartX,
			@StartY,
			@Width,
			@Height
		)
	END

	DECLARE @dt datetime
	SET @dt = GETDATE()
	EXEC dbo.rad_FileUpdates_Save @WorldName = @WorldName, @FileUpdateName = @MapName, @LastUpdateDate = @dt
END
go


ALTER PROCEDURE [dbo].[rad_Maps_Save]
(
	@WorldName			nvarchar(64),
	@MapName			nvarchar(64),
	@X					int,
	@Y					int,
	@Width				int,
	@Height				int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	IF EXISTS(SELECT 1 FROM dbo.rad_Maps WHERE MapName = @MapName)
	BEGIN
		UPDATE
			dbo.rad_Maps
		SET
			X		= @X,
			Y		= @Y,
			Width	= @Width,
			Height	= @Height
		WHERE
			MapName = @MapName
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_Maps
		(
			WorldId,
			MapName,
			X,
			Y,
			Width,
			Height
		)
		VALUES
		(
			@WorldId,
			@MapName,
			@X,
			@Y,
			@Width,
			@Height
		)
	END

	DECLARE @dt datetime
	SET @dt = GETDATE()
	EXEC dbo.rad_FileUpdates_Save @WorldName = @WorldName, @FileUpdateName = @MapName, @LastUpdateDate = @dt
END
go

ALTER PROCEDURE [dbo].[rad_Objects_GetMap]
(
	@WorldName			nvarchar(64),
	@X					int,
	@Y					int,
	@Z					int,
	@Width				int,
	@Height				int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @startX int,
			@startY int,
			@endX int,
			@endY int

    SET @startX = @X
    SET @startY = @Y
    SET @endX = @X + @Width
    SET @endY = @Y + @Height

--	DECLARE @objects TABLE
--	(
--		ObjectId	int primary key
--	)
--
--	DECLARE @OwnedObjects TABLE
--	(
--		ObjectId	int primary key
--	)
--
--	--SELECT 
--	--	@startX = X, 
--	--	@endX = (X + Width),
--	--	@startY = Y,
--	--	@endY = (Y + Height)
--	--FROM
--	--	dbo.rad_Maps
--	--WHERE
--	--	WorldId = @WorldId
--	--	AND (@X >= X AND @X <= (X + Width))
--	--	AND (@Y >= Y AND @Y <= (Y + Height))
--
--	INSERT INTO @objects (ObjectId)
--	SELECT
--		ObjectId
--	FROM
--		dbo.rad_Places
--	WHERE	
--		X BETWEEN @startX AND @endX
--		AND Y BETWEEN @startY AND @endY
--		-- Ignore the Z component, pull all levels within the map range.

	-- Places
	SELECT
		ObjectId,
		WorldId,
		OwnerObjectId,
		NULL AS TemplateId,
		TypeName,
		ObjectName,
		Properties,
		NULL AS TemplateProperties,
		X,
		Y,
		Z
	FROM
	    dbo.rad_vw_Places
	WHERE
		WorldId = @WorldId
		AND X BETWEEN @startX AND @endX
		AND Y BETWEEN @startY AND @endY
	    --ObjectId IN (SELECT ObjectId FROM @objects)


--	-- Load all non-player actors into a temp table.
--	INSERT INTO @OwnedObjects (ObjectId)
--	SELECT
--		ObjectId
--	FROM
--		dbo.rad_vw_Objects
--	WHERE	
--		OwnerObjectId IN (SELECT ObjectId FROM @objects)
----		OR
----		(
----			Properties.value('(//property[@name="ObjectType"]/text())[1]', 'nvarchar(32)') IN ('Actor','Mobile')
----			-- Avatars will have the x,y specified
----			AND Properties.value('(//property[@name="X"]/text())[1]', 'int') BETWEEN @startX AND @endX
----			AND Properties.value('(//property[@name="Y"]/text())[1]', 'int') BETWEEN @startY AND @endY
----			-- Exclude players, they should be loaded when they login and move around
----			AND Properties.exist('(//property[@name="UserName"])') = 0
----			AND (
----					Properties.value('(//property[@name="IsTemplate"]/text())[1]', 'nvarchar(5)') IS NULL
----					OR
----					Properties.value('(//property[@name="IsTemplate"]/text())[1]', 'nvarchar(5)') = 'false'
----				)
----		)
--
--	-- All non-player actors on the map.
--	SELECT
--		ObjectId,
--		WorldId,
--		OwnerObjectId,
--		TemplateId,
--		TypeName,
--		ObjectName,
--		Properties,
--		TemplateProperties
--	FROM
--		dbo.rad_vw_Objects
--	WHERE	
--		ObjectId IN (SELECT ObjectId FROM @OwnedObjects)
--
--	-- All actors owned by the non-player actors in the current place. (Children)
--	SELECT
--		ObjectId,
--		WorldId,
--		OwnerObjectId,
--		TemplateId,
--		TypeName,
--		ObjectName,
--		Properties,
--		TemplateProperties
--	FROM
--		dbo.rad_vw_Objects
--	WHERE	
--		OwnerObjectId IN (SELECT ObjectId FROM @OwnedObjects)
--
--	-- All actors owned by actors owned by the non-player actors in the current place (Grandchildren)
--	SELECT
--		o.ObjectId,
--		o.WorldId,
--		o.OwnerObjectId,
--		o.TemplateId,
--		o.TypeName,
--		o.ObjectName,
--		o.Properties,
--		o.TemplateProperties
--	FROM
--		dbo.rad_vw_Objects o
--		INNER JOIN dbo.rad_Objects p ON o.OwnerObjectId = p.ObjectId
--	WHERE	
--		p.OwnerObjectId IN (SELECT ObjectId FROM @OwnedObjects)
END

go 

ALTER PROCEDURE [dbo].[rad_Objects_SavePlace]
(
	@WorldName			nvarchar(64),
	@ObjectName			nvarchar(64),
	@TypeName			nvarchar(256),
	@OwnerObjectId		int = null,
	@Properties			xml,
	@X					int,
	@Y					int,
	@Z					int,
	@ObjectId			int output
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	EXEC dbo.rad_Objects_Save @WorldName, @ObjectName, @TypeName, @OwnerObjectId, 0, 0, @Properties, @ObjectId OUT

	IF NOT EXISTS(SELECT 1 FROM dbo.rad_Places WHERE ObjectId = @ObjectId)
	BEGIN
		INSERT INTO dbo.rad_Places (ObjectId, WorldId, X, Y, Z) VALUES (@ObjectId, @WorldId, @X, @Y, @Z)
	END

	DECLARE @MapName nvarchar(64)
	SELECT 
		@MapName = MapName
	FROM
		dbo.rad_Maps
	WHERE
		WorldId = @WorldId
		AND (@X >= X AND @X <= (X + Width))
		AND (@Y >= Y AND @Y <= (Y + Height))

	-- Update file updates table.
	DECLARE @dt datetime
	SET @dt = GETDATE()
	EXEC dbo.rad_FileUpdates_Save @WorldName = @WorldName, @FileUpdateName = @MapName, @LastUpdateDate = @dt
END
go

CREATE PROCEDURE [dbo].[rad_Objects_GetPlaceById]
(
	@WorldName			nvarchar(64),
	@ObjectId			int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @OwnedObjects TABLE
	(
		ObjectId		int primary key
	)

	-- Load all non-player actors into a temp table.
	INSERT INTO @OwnedObjects (ObjectId)
	SELECT
		ObjectId
	FROM
		dbo.rad_Objects
	WHERE	
		OwnerObjectId = @ObjectId
		AND TemplateId IS NOT NULL

	-- Place Details
	SELECT
		ObjectId,
		WorldId,
		OwnerObjectId,
		NULL AS TemplateId,
		TypeName,
		ObjectName,
		Properties,
		NULL AS TemplateProperties,
		X,
		Y,
		Z
	FROM
		dbo.rad_vw_Places
	WHERE	
		ObjectId = @ObjectId

	-- All non-player actors in the current place.
	SELECT
		ObjectId,
		WorldId,
		OwnerObjectId,
		TemplateId,
		TypeName,
		ObjectName,
		Properties,
		TemplateProperties
	FROM
		dbo.rad_vw_Objects
	WHERE	
		ObjectId IN (SELECT ObjectId FROM @OwnedObjects)

	-- All actors owned by the non-player actors in the current place. (Children)
	SELECT
		ObjectId,
		WorldId,
		OwnerObjectId,
		TemplateId,
		TypeName,
		ObjectName,
		Properties,
		TemplateProperties
	FROM
		dbo.rad_vw_Objects
	WHERE	
		OwnerObjectId IN (SELECT ObjectId FROM @OwnedObjects)

	-- All actors owned by actors owned by the non-player actors in the current place (Grandchildren)
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
		p.OwnerObjectId IN (SELECT ObjectId FROM @OwnedObjects)
END

go

ALTER PROCEDURE [dbo].[rad_Objects_GetPlace]
(
	@WorldName			nvarchar(64),
	@X					int,
	@Y					int,
	@Z					int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @OwnedObjects TABLE
	(
		ObjectId		int primary key
	)

	DECLARE @ObjectId int
	SELECT 
		@ObjectId = o.ObjectId 
	FROM
		dbo.rad_Objects o
		INNER JOIN dbo.rad_Places p ON o.ObjectId = p.ObjectId
	WHERE
		o.WorldId = @WorldId
		AND p.X = @X
		AND p.Y = @Y
		AND p.Z = @Z

	EXEC dbo.rad_Objects_GetPlaceById @WorldName, @ObjectId
END

go

-- Insert new command FILEUPDATES
insert into dbo.rad_Commands (WorldId, CommandName, RequiredRole, Syntax, Arguments, Help, Isvisible) values (1, 'FILEUPDATES', 'Mortal', '', '', '', 0)
go
insert into dbo.rad_Commands (WorldId, CommandName, RequiredRole, Syntax, Arguments, Help, Isvisible) values (1, 'ISONLINE', 'Mortal', '', '', '', 0)
go

exec dbo.rad_Maps_Update 'Perenthia', 'Delcor', 445, 460, 468, 511
go
exec dbo.rad_Maps_Update 'Perenthia', 'Angarath Road', 468, 460, 654, 513
go
exec dbo.rad_Maps_Update 'Perenthia', 'City of Angarath', 654, 460, 753, 528
go

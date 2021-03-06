USE [nobility]
GO
/****** Object:  Table [dbo].[rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Worlds](
	[WorldId] [int] IDENTITY(1,1) NOT NULL,
	[WorldName] [nvarchar](64) NOT NULL,
	[IsOnline] [bit] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[Config] [xml] NULL,
 CONSTRAINT [PK_rad_Worlds] PRIMARY KEY CLUSTERED 
(
	[WorldId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_Types]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Types](
	[TypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_rad_Types] PRIMARY KEY CLUSTERED 
(
	[TypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_temp_objects]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_temp_objects](
	[RowNum] [int] IDENTITY(1,1) NOT NULL,
	[ObjectId] [int] NOT NULL,
	[NewObjectId] [int] NULL,
	[ObjectName] [nvarchar](64) NOT NULL,
	[TypeName] [nvarchar](256) NOT NULL,
	[OwnerObjectId] [int] NULL,
	[TemplateId] [int] NULL,
	[X] [int] NULL,
	[Y] [int] NULL,
	[Z] [int] NULL,
	[Properties] [xml] NULL,
 CONSTRAINT [PK__rad_temp_objects__7330EBF5] PRIMARY KEY CLUSTERED 
(
	[RowNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_Players_Deleted]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Players_Deleted](
	[DeletedPlayerId] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[UserDetailId] [int] NOT NULL,
	[WorldId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[FirstName] [nvarchar](32) NOT NULL,
	[LastName] [nvarchar](32) NULL,
	[Properties] [xml] NOT NULL,
	[DateDeleted] [datetime] NULL,
	[OwnedObjects] [xml] NULL,
 CONSTRAINT [PK_rad_Players_Deleted] PRIMARY KEY CLUSTERED 
(
	[DeletedPlayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_ObjectTypes]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_ObjectTypes](
	[ObjectTypeId] [int] IDENTITY(1,1) NOT NULL,
	[ObjectTypeName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_rad_ObjectTypes] PRIMARY KEY CLUSTERED 
(
	[ObjectTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_LogType]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_LogType](
	[LogTypeId] [tinyint] NOT NULL,
	[LogTypeName] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_rad_LogType] PRIMARY KEY CLUSTERED 
(
	[LogTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_HouseholdRelationTypes]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_HouseholdRelationTypes](
	[RelationTypeId] [int] NOT NULL,
	[RelationTypeName] [nvarchar](32) NULL,
PRIMARY KEY CLUSTERED 
(
	[RelationTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Games_GetGameId]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Games_GetGameId]
(
	@GameName		nvarchar(64),
	@GameId			int output
)
AS
BEGIN
	SELECT @GameId = GameId FROM dbo.rad_Games WHERE GameName = @GameName
END
GO
/****** Object:  UserDefinedFunction [dbo].[rad_fn_Split]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[rad_fn_Split](@String nvarchar(4000), @Delimiter char(1))
RETURNS @Results TABLE (Items nvarchar(4000))
AS
BEGIN
	DECLARE @index int
	DECLARE @slice nvarchar(4000)

	SELECT @index = 1
	IF @String IS NULL RETURN

	WHILE @index != 0
	BEGIN
		SELECT @index = CHARINDEX(@Delimiter,@String)
		
		IF @index !=0
			SELECT @slice = LEFT(@String,@index - 1)
		ELSE
			SELECT @slice = @String

		INSERT INTO @Results (Items) VALUES (@slice)
		SELECT @String = RIGHT(@String,LEN(@String) - @index)
		
		IF LEN(@String) = 0 BREAK
	END 
	RETURN
END
GO
/****** Object:  Table [dbo].[rad_Households]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Households](
	[HouseholdId] [int] IDENTITY(2580,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[HouseholdName] [nvarchar](20) NOT NULL,
	[ImageUri] [nvarchar](256) NULL,
	[Properties] [xml] NOT NULL,
	[DateCreated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[HouseholdId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_FileUpdates]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_FileUpdates](
	[FileUpdateId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[FileUpdateName] [nvarchar](64) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FileUpdateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_Logs]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Logs](
	[LogId] [bigint] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[LogTypeId] [tinyint] NOT NULL,
	[LogText] [nvarchar](max) NOT NULL,
	[LogDate] [datetime] NULL,
 CONSTRAINT [PK_rad_Logs] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_Commands]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Commands](
	[CommandId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[CommandName] [nvarchar](64) NOT NULL,
	[RequiredRole] [nvarchar](256) NOT NULL,
	[Syntax] [nvarchar](512) NOT NULL,
	[Arguments] [nvarchar](512) NOT NULL,
	[Help] [nvarchar](max) NOT NULL,
	[IsVisible] [bit] NULL,
 CONSTRAINT [PK_rad_Commands] PRIMARY KEY CLUSTERED 
(
	[CommandId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Worlds_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Worlds_Save]
(
	@WorldName			nvarchar(64),
	@IsOnline			bit,
	@Description		nvarchar(2000) = null,
	@WorldId			int output
)
AS
BEGIN
	IF @WorldId > 0
	BEGIN
		UPDATE 
			dbo.rad_Worlds
		SET
			WorldName		= @WorldName,
			IsOnline		= @IsOnline,
			Description		= @Description
		WHERE
			WorldId = @WorldId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_Worlds
		(
			WorldName,
			IsOnline,
			Description
		)
		VALUES
		(
			@WorldName,
			@IsOnline,
			@Description
		)
		SET @WorldId = SCOPE_IDENTITY()
	END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Worlds_GetWorldId]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Worlds_GetWorldId]
(
	@WorldName		nvarchar(64),
	@WorldId		int output
)
AS
BEGIN
	SELECT @WorldId = WorldId FROM dbo.rad_Worlds WHERE WorldName = @WorldName
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Worlds_GetWorld]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Worlds_GetWorld]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	SELECT * FROM dbo.rad_Worlds WHERE WorldName = @WorldName
END
GO
/****** Object:  Table [dbo].[rad_Terrain]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Terrain](
	[TerrainId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[TerrainName] [nvarchar](32) NOT NULL,
	[WalkType] [int] NOT NULL,
	[Color] [int] NOT NULL,
	[ImageUrl] [nvarchar](256) NULL,
 CONSTRAINT [PK_rad_Terrain] PRIMARY KEY CLUSTERED 
(
	[TerrainId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_SkillTypes]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_SkillTypes](
	[SkillTypeId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[SkillTypeName] [nvarchar](32) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_rad_SkillTypes] PRIMARY KEY CLUSTERED 
(
	[SkillTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_UserDetails]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_UserDetails](
	[UserDetailId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[Properties] [xml] NOT NULL,
 CONSTRAINT [PK_rad_UserDetails] PRIMARY KEY CLUSTERED 
(
	[UserDetailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Types_GetTypeId]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Types_GetTypeId]
(
	@TypeName	nvarchar(256),
	@TypeId		int output
)
AS
BEGIN
	SELECT @TypeId = TypeId FROM dbo.rad_Types WHERE TypeName = @TypeName
	IF @TypeId IS NULL
	BEGIN
		INSERT INTO dbo.rad_Types (TypeName) VALUES (@TypeName)
		SET @TypeId = SCOPE_IDENTITY()
	END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_UserDetails_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_UserDetails_Save]
(
	@WorldName		nvarchar(64),
	@UserName		nvarchar(256),
	@Properties		xml
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	IF EXISTS(SELECT 1 FROM dbo.rad_UserDetails WHERE WorldId = @WorldId AND UserName = @UserName)
	BEGIN
		UPDATE dbo.rad_UserDetails SET Properties = @Properties WHERE WorldId = @WorldId AND UserName = @UserName
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_UserDetails
		(
			WorldId,
			UserName,
			Properties
		)
		VALUES
		(
			@WorldId,
			@UserName,
			@Properties
		)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_GetPaged]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetPaged]
(
    @WorldName			nvarchar(64),
    @StartRowIndex		int,
    @MaxRows			int,
    @SortExpression		nvarchar(128),
    @SortDirection		nvarchar(4)
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

    SET @StartRowIndex = @StartRowIndex + 1
    
    DECLARE @sql nvarchar(max)
    SET @sql = N'SELECT 
		*
    FROM
		(
		SELECT '
		
	IF @SortExpression IS NULL OR LEN(@SortExpression) = 0
		SET @SortExpression = 'Level'
		
	IF @SortExpression = 'Level'
	BEGIN
		SET @sql = @sql + N'ROW_NUMBER() OVER(ORDER BY Properties.value(''(/properties/property[@name="Level"])[1]'', ''int'') ' + @SortDirection + ') AS RowNum,'
	END
	IF @SortExpression = 'Name'
	BEGIN
		SET @sql = @sql + N'ROW_NUMBER() OVER(ORDER BY ObjectName ' + @SortDirection + ') AS RowNum,'
	END
	--IF @SortExpression = 'Household'
	--BEGIN
	
	--END
	
	SET @sql = @sql + N' 
			*
		FROM
			dbo.rad_vw_Players
		WHERE	
			WorldId = ' + CONVERT(nvarchar, @WorldId) + '
		) AS Players
	WHERE 
		RowNum BETWEEN ' + CONVERT(nvarchar, @StartRowIndex) + ' AND (' + CONVERT(nvarchar, @StartRowIndex) + ' + ' + CONVERT(nvarchar, @MaxRows) + ') - 1
		'
		
	EXEC (@sql)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Terrain_GetTerrain]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Terrain_GetTerrain]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	SELECT * FROM dbo.rad_Terrain WHERE WorldId = @WorldId ORDER BY TerrainName ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Logs_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Logs_Save]
(
	@WorldName		nvarchar(64),
	@LogTypeId		tinyint,
	@LogText		nvarchar(max)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	INSERT INTO dbo.rad_Logs(WorldId, LogTypeId, LogText) VALUES (@WorldId, @LogTypeId, @LogText)
END
GO
/****** Object:  Table [dbo].[rad_Skills]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Skills](
	[SkillId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[SkillTypeId] [int] NOT NULL,
	[SkillName] [nvarchar](64) NOT NULL,
	[Description] [nvarchar](2000) NULL,
 CONSTRAINT [PK_rad_Skills] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_HouseholdRelations]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_HouseholdRelations](
	[PrimaryHouseholdId] [int] NOT NULL,
	[SecondaryHouseholdId] [int] NOT NULL,
	[RelationTypeId] [int] NOT NULL,
 CONSTRAINT [PK_rad_HouseholdRelations] PRIMARY KEY CLUSTERED 
(
	[PrimaryHouseholdId] ASC,
	[SecondaryHouseholdId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_HouseholdRanks]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_HouseholdRanks](
	[RankId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[HouseholdId] [int] NULL,
	[RankName] [nvarchar](20) NOT NULL,
	[RankOrder] [int] NOT NULL,
	[ImageUri] [nvarchar](256) NULL,
	[Permissions] [int] NOT NULL,
	[TitleMale] [nvarchar](20) NULL,
	[TitleFemale] [nvarchar](20) NULL,
	[RequiredMemberCount] [int] NULL,
	[EmblemCost] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RankId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_FileUpdates_Save]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_FileUpdates_Save]
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
GO
/****** Object:  StoredProcedure [dbo].[rad_FileUpdates_GetAll]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_FileUpdates_GetAll]
(
    @WorldName          nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId    int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    SELECT * FROM dbo.rad_FileUpdates WHERE WorldId = @WorldId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Commands_GetCommands]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Commands_GetCommands]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	SELECT * FROM dbo.rad_Commands WHERE WorldId = @WorldId ORDER BY CommandName ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_Save]
(
    @WorldName          nvarchar(64),
    @HouseholdName      nvarchar(20),
	@ImageUri			nvarchar(256),
    @Properties         xml,
    @HouseholdId        int output
)
AS
BEGIN
    IF @HouseholdId > 0
    BEGIN
        UPDATE
            dbo.rad_Households
        SET
			ImageUri			= @ImageUri,
            Properties          = @Properties
        WHERE
            HouseholdId = @HouseholdId
    END
    ELSE
    BEGIN
        DECLARE @WorldId int
        EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
        
        INSERT INTO dbo.rad_Households
        (
            WorldId,
            HouseholdName,
            ImageUri,
            Properties 
        )
        VALUES
        (
            @WorldId,
            @HouseholdName,
            @ImageUri,
            @Properties 
        )
        SET @HouseholdId = SCOPE_IDENTITY()
    END
END
GO
/****** Object:  Table [dbo].[rad_Maps]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Maps](
	[MapId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[MapName] [nvarchar](64) NOT NULL,
	[X] [int] NOT NULL,
	[Y] [int] NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[DefaultTerrainId] [int] NULL,
	[TypeId] [int] NULL,
 CONSTRAINT [PK_rad_Maps] PRIMARY KEY CLUSTERED 
(
	[MapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Maps_Update]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Maps_Update]
(
	@WorldName			nvarchar(64),
	@MapName			nvarchar(64),
	@StartX				int,
	@StartY				int,
	@EndX				int,
	@EndY				int,
	@TypeName			nvarchar(256) = null
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	DECLARE @TypeId int
	EXEC dbo.rad_Types_GetTypeId @TypeName = @TypeName, @TypeId = @TypeId OUT

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
			Height	= @Height,
			TypeId	= @TypeId
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
			Height,
			TypeId
		)
		VALUES
		(
			@WorldId,
			@MapName,
			@StartX,
			@StartY,
			@Width,
			@Height,
			@TypeId
		)
	END

	DECLARE @dt datetime
	SET @dt = GETDATE()
	EXEC dbo.rad_FileUpdates_Save @WorldName = @WorldName, @FileUpdateName = @MapName, @LastUpdateDate = @dt
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Maps_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Maps_Save]
(
	@WorldName			nvarchar(64),
	@MapName			nvarchar(64),
	@X					int,
	@Y					int,
	@Width				int,
	@Height				int,
	@TypeName			nvarchar(256) = null
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	DECLARE @TypeId int
	EXEC dbo.rad_Types_GetTypeId @TypeName = @TypeName, @TypeId = @TypeId OUT
	
	IF EXISTS(SELECT 1 FROM dbo.rad_Maps WHERE MapName = @MapName)
	BEGIN
		UPDATE
			dbo.rad_Maps
		SET
			X		= @X,
			Y		= @Y,
			Width	= @Width,
			Height	= @Height,
			TypeId	= @TypeId
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
			Height,
			TypeId
		)
		VALUES
		(
			@WorldId,
			@MapName,
			@X,
			@Y,
			@Width,
			@Height,
			@TypeId
		)
	END

	DECLARE @dt datetime
	SET @dt = GETDATE()
	EXEC dbo.rad_FileUpdates_Save @WorldName = @WorldName, @FileUpdateName = @MapName, @LastUpdateDate = @dt
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Maps_GetAllMaps]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Maps_GetAllMaps]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	SELECT * FROM dbo.rad_Maps WHERE WorldId = @WorldId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetHeadOfHouseholdRank]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetHeadOfHouseholdRank]
(
	@WorldName			nvarchar(64),
	@RankOrder			int
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    SELECT TOP 1
        *
    FROM
        dbo.rad_HouseholdRanks
    WHERE
        WorldId = @WorldId
        AND HouseholdId IS NULL
        AND RankOrder >= @RankOrder
    ORDER BY
		RankOrder ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetRanks]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetRanks]
(
    @HouseholdId        int
)
AS
BEGIN
    SELECT
        *
    FROM
        dbo.rad_HouseholdRanks
    WHERE
        HouseholdId = @HouseholdId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_SaveRank]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_SaveRank]
(
    @WorldName              nvarchar(64),
    @HouseholdId            int,
    @RankName               nvarchar(20),
    @RankOrder              int,
    @ImageUri               nvarchar(256) = null,
    @Permissions            int,
    @TitleMale              nvarchar(20) = null,
    @TitleFemale            nvarchar(20) = null,
    @RequiredMemberCount    int = 0,
    @EmblemCost             int = 0,
    @RankId                 int output
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    IF @RankId > 0
    BEGIN
        UPDATE
            dbo.rad_HouseholdRanks
        SET
            HouseholdId             = @HouseholdId,
            RankName                = @RankName,
            RankOrder               = @RankOrder,
            ImageUri                = @ImageUri,
            Permissions             = @Permissions,
            TitleMale               = @TitleMale,
            TitleFemale             = @TitleFemale,
            RequiredMemberCount     = @RequiredMemberCount,
            EmblemCost              = @EmblemCost
        WHERE
            RankId = @RankId
    END
    ELSE
    BEGIN
        INSERT INTO dbo.rad_HouseholdRanks
        (
            WorldId,
            HouseholdId,
            RankName,
            RankOrder,
            ImageUri,
            Permissions,
            TitleMale,
            TitleFemale,
            RequiredMemberCount,
            EmblemCost
        )
        VALUES
        (
            @WorldId,
            @HouseholdId,
            @RankName,
            @RankOrder,
            @ImageUri,
            @Permissions,
            @TitleMale,
            @TitleFemale,
            @RequiredMemberCount,
            @EmblemCost
        )
        SET @RankId = SCOPE_IDENTITY()
    END
END
GO
/****** Object:  Table [dbo].[rad_Players]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Players](
	[PlayerId] [int] IDENTITY(250350,1) NOT NULL,
	[UserDetailId] [int] NOT NULL,
	[WorldId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[FirstName] [nvarchar](32) NOT NULL,
	[LastName] [nvarchar](32) NULL,
	[IsDeleted] [bit] NULL,
	[Properties] [xml] NOT NULL,
	[HouseholdId] [int] NULL,
	[RankId] [int] NULL,
 CONSTRAINT [PK_rad_Players] PRIMARY KEY CLUSTERED 
(
	[PlayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Skills_GetSkills]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Skills_GetSkills]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
	
	SELECT 
		s.*, 
		st.SkillTypeName 
	FROM 
		dbo.rad_Skills s
		INNER JOIN dbo.rad_SkillTypes st on s.SkillTypeId = st.SkillTypeId
	WHERE 
		s.WorldId = @WorldId 
	ORDER BY 
		st.SkillTypeName ASC,
		s.SkillName ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_UserDetails_GetOrCreate]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_UserDetails_GetOrCreate]
(
	@WorldName		nvarchar(64),
	@UserName		nvarchar(256),
	@UserDetailId	int output
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	IF NOT EXISTS(SELECT 1 FROM dbo.rad_UserDetails WHERE WorldId = @WorldId AND UserName = @UserName)
	BEGIN
		EXECUTE dbo.rad_UserDetails_Save
			   @WorldName = @WorldName
			  ,@UserName = @UserName
			  ,@Properties = '<properties/>'
	END

	SELECT @UserDetailId = UserDetailId FROM dbo.rad_UserDetails WHERE WorldId = @WorldId AND UserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[rad_UserDetails_GetUserDetail]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_UserDetails_GetUserDetail]
(
	@WorldName		nvarchar(64),
	@UserName		nvarchar(256)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @UserDetailId int
	EXEC dbo.rad_UserDetails_GetOrCreate @WorldName, @UserName, @UserDetailId OUT

	SELECT * FROM dbo.rad_UserDetails WHERE UserDetailId = @UserDetailId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_GetCount]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetCount]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	SELECT COUNT(*) FROM rad_Players WHERE WorldId = @WorldId
END
GO
/****** Object:  View [dbo].[rad_vw_Players]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Alter Players View
CREATE VIEW [dbo].[rad_vw_Players]
AS
    SELECT
        o.PlayerId AS ObjectId, 
        o.UserDetailId, 
        o.WorldId, 
        o.TypeId, 
        t.TypeName, 
        NULL AS TemplateId, 
        NULL AS OwnerObjectId, 
        NULL AS OwnerPlayerId, 
        o.FirstName,
        o.LastName,
        (
            CASE 
            WHEN LastName IS NULL THEN FirstName 
            ELSE FirstName + ' ' + LastName 
            END
        ) AS ObjectName, 
        o.Properties, 
        NULL AS TemplateProperties,
        o.HouseholdId,
        h.HouseholdName,
        h.ImageUri AS HouseholdImageUri,
        o.RankId,
        r.RankName,
        r.ImageUri AS RankImageUri,
        r.RankOrder,
        (
        CASE o.Properties.value('(/properties/property[@name = "Gender"])[1]', 'nvarchar(10)')
            WHEN 'Male' THEN r.TitleMale
            WHEN 'Female' THEN r.TitleFemale
            ELSE NULL
        END
        ) AS Title
    FROM         
        dbo.rad_Players AS o 
        INNER JOIN  dbo.rad_Types AS t ON o.TypeId = t.TypeId
        LEFT OUTER JOIN dbo.rad_Households h ON o.HouseholdId = h.HouseholdId
        LEFT OUTER JOIN dbo.rad_HouseholdRanks r ON o.RankId = r.RankId
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_ValidateUserObject]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_ValidateUserObject]
(
	@ObjectId			int,
	@UserName			nvarchar(256)
)
AS
BEGIN
	SELECT 
		o.PlayerId 
	FROM 
		dbo.rad_Players o
		INNER JOIN dbo.rad_UserDetails u ON o.UserDetailId = u.UserDetailId
	WHERE
		o.PlayerId = @ObjectId
		AND u.UserName = @UserName
END
GO
/****** Object:  Table [dbo].[rad_Sessions]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Sessions](
	[SessionId] [uniqueidentifier] NOT NULL,
	[IPAddress] [nvarchar](32) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[PlayerId] [int] NULL,
	[LastHeartbeat] [datetime] NOT NULL,
	[LastCommand] [nvarchar](max) NULL,
 CONSTRAINT [PK_rad_Sessions] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_Save]
(
	@WorldName			nvarchar(64),
	@UserName			nvarchar(256),
	@FirstName			nvarchar(32),
	@LastName			nvarchar(32),
	@TypeName			nvarchar(256),
	@Properties			xml,
	@HouseholdId        int = null,
	@RankId             int = null,
	@PlayerId			int output
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeId int
	EXEC dbo.rad_Types_GetTypeId @TypeName = @TypeName, @TypeId = @TypeId OUT

	DECLARE @UserDetailId	int
	EXEC dbo.rad_UserDetails_GetOrCreate @WorldName, @UserName, @UserDetailId OUT
	
	IF @HouseholdId = 0
	    SET @HouseholdId = NULL
	    
	IF @RankId = 0
	    SET @RankId = NULL

	DECLARE @Err int
	SET @Err = 0
	BEGIN TRAN

	IF @PlayerId > 0
	BEGIN
		UPDATE
			dbo.rad_Players
		SET
			FirstName		= @FirstName,
			LastName		= @LastName,
			TypeId			= @TypeId,
			Properties		= @Properties,
			HouseholdId     = @HouseholdId,
			RankId          = @RankId
		WHERE
			PlayerId = @PlayerId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_Players
		(
			WorldId,
			UserDetailId,
			TypeId,
			FirstName,
			LastName,
			Properties,
			HouseholdId,
			RankId
		)
		VALUES
		(
			@WorldId,
			@UserDetailId,
			@TypeId,
			@FirstName,
			@LastName,
			@Properties,
			@HouseholdId,
			@RankId
		)
		SET @PlayerId = SCOPE_IDENTITY()
	END	

	SET @Err = @@ERROR
	IF @Err <> 0
		GOTO FINISHED

	FINISHED:
		IF @Err = 0
		BEGIN
			COMMIT TRAN
		END
		ELSE
		BEGIN
			RAISERROR ('Object failed to save: World = %d, Name = %d, UserName = %d', 16, 1, @WorldName, @FirstName, @UserName)
			ROLLBACK TRAN
		END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_RemoveProperty]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_RemoveProperty]
(
	@PlayerId		int,
	@PropertyName	nvarchar(64)
)
AS
BEGIN
	UPDATE dbo.rad_Players SET Properties.modify('delete /properties/property[@name = sql:variable("@PropertyName")]') WHERE PlayerId = @PlayerId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_GetRandomPlayer]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetRandomPlayer]
AS
BEGIN
	SELECT TOP 1 PlayerId FROM dbo.rad_Players ORDER BY NEWID()
END
GO
/****** Object:  Table [dbo].[rad_Objects]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Objects](
	[ObjectId] [int] IDENTITY(1,1) NOT NULL,
	[WorldId] [int] NOT NULL,
	[TypeId] [int] NOT NULL,
	[TemplateId] [int] NULL,
	[OwnerObjectId] [int] NULL,
	[OwnerPlayerId] [int] NULL,
	[ObjectName] [nvarchar](64) NOT NULL,
	[Properties] [xml] NOT NULL,
 CONSTRAINT [PK_rad_Objects] PRIMARY KEY CLUSTERED 
(
	[ObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetAll]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetAll]
(
    @WorldName       nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    DECLARE @table TABLE
    (
		HouseholdId int not null primary key
    )

	INSERT INTO @table (HouseholdId)
    SELECT 
		HouseholdId
    FROM 
        dbo.rad_Households 
    WHERE 
        WorldId = @WorldId
        
        
    SELECT 
        *,
        (SELECT COUNT(*) FROM rad_Players WHERE HouseholdId IN (SELECT HouseholdId FROM @table)) AS MemberCount  
    FROM 
        dbo.rad_Households 
    WHERE 
        HouseholdId IN (SELECT HouseholdId FROM @table)
        
    -- Relations
    SELECT
        *
    FROM
        dbo.rad_HouseholdRelations
    WHERE
		(
			PrimaryHouseholdId IN (SELECT HouseholdId FROM @table) 
			OR 
			SecondaryHouseholdId IN (SELECT HouseholdId FROM @table)
		)
        
    -- Ranks
    SELECT
		*
	FROM
		dbo.rad_HouseholdRanks
	WHERE
        HouseholdId IN (SELECT HouseholdId FROM @table)
	ORDER BY
		RankOrder ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_Get]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_Get]
(
    @HouseholdId        int
)
AS
BEGIN
    -- Household
    SELECT 
        *,
        (SELECT COUNT(*) FROM rad_Players WHERE HouseholdId = @HouseholdId) AS MemberCount 
    FROM 
        dbo.rad_Households
    WHERE 
        HouseholdId = @HouseholdId
        
    -- Relations
    SELECT
        *
    FROM
        dbo.rad_HouseholdRelations
    WHERE
        PrimaryHouseholdId = @HouseholdId
        OR SecondaryHouseholdId = @HouseholdId
        
    -- Ranks
    SELECT
		*
	FROM
		dbo.rad_HouseholdRanks
	WHERE
		HouseholdId = @HouseholdId
	ORDER BY
		RankOrder ASC
END
GO
/****** Object:  StoredProcedure [dbo].[rad_FeaturedPlayer_Get]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_FeaturedPlayer_Get]
AS
BEGIN
--    DECLARE @dt datetime
--    SET @dt = GETDATE()
--	SELECT TOP 1 * FROM dbo.rad_FeaturedPlayer WHERE StartDate >= @dt ORDER BY StartDate ASC

	---- Get the highest level player in the game for now.
	--SELECT TOP 1
	--	-1 as FeaturedPlayerId,
	--	PlayerId,
	--	GETDATE() as StartDate,
	--	DateAdd(day,1,GETDATE()) as EndDate
	--FROM
	--	dbo.rad_Players
	--ORDER BY
	--	Properties.value('(/properties/property[@name="Level"]/text())[1]', 'int') DESC
		
	SELECT TOP 1
		-1 as FeaturedPlayerId,
		PlayerId,
		GETDATE() as StartDate,
		DateAdd(day,1,GETDATE()) as EndDate
	FROM
		dbo.rad_Players
	ORDER BY NEWID()
END
GO
/****** Object:  Table [dbo].[rad_FeaturedPlayer]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_FeaturedPlayer](
	[FeaturedPlayerId] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
 CONSTRAINT [PK__rad_FeaturedPlay__53B8409C] PRIMARY KEY CLUSTERED 
(
	[FeaturedPlayerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[rad_HouseholdArmory]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_HouseholdArmory](
	[HouseholdArmoryId] [int] IDENTITY(1,1) NOT NULL,
	[HouseholdId] [int] NOT NULL,
	[ObjectId] [int] NOT NULL,
	[AddedByPlayerId] [int] NOT NULL,
	[DateAdded] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[HouseholdArmoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_SaveMember]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_SaveMember]
(
    @HouseholdId            int,
    @PlayerId               int,
    @RankId                 int
)
AS
BEGIN
    -- Get the member count before and after the update.
    DECLARE @MemberCountPreUpdate int,
            @MemberCountPostUpdate int
            
    SELECT @MemberCountPreUpdate = COUNT(*) FROM dbo.rad_Players WHERE HouseholdId = @HouseholdId

    -- Update the player's rank in the household and add the player to the household if they are not already
    -- a member.
    UPDATE rad_Players SET HouseholdId = @HouseholdId, RankId = @RankId WHERE PlayerId = @PlayerId
    
    SELECT @MemberCountPostUpdate = COUNT(*) FROM dbo.rad_Players WHERE HouseholdId = @HouseholdId
    
    -- If the member count has changed then do a check to see if the head of household title has increased.
    IF @MemberCountPostUpdate > @MemberCountPreUpdate
    BEGIN
        -- Get the current head of household rank and player ids.
        DECLARE @HeadOfHouseId int,
                @CurrentRankId int
        
        SELECT TOP 1
            @HeadOfHouseId = p.ObjectId, 
            @CurrentRankId = p.RankId
        FROM
            dbo.rad_vw_Players p
            INNER JOIN dbo.rad_HouseholdRanks r ON p.RankId = r.RankId
        WHERE
            p.HouseholdId = @HouseholdId
            AND r.HouseholdId IS NULL
        ORDER BY 
            p.RankId
                
        -- Check to see if a rank exists for the current count.
        DECLARE @NextRankId int
        
        SELECT TOP 1 
            @NextRankId = RankId 
        FROM 
            dbo.rad_HouseholdRanks 
        WHERE 
            HouseholdId IS NULL 
            AND @MemberCountPostUpdate >= RequiredMemberCount
        ORDER BY 
            RankOrder DESC -- Greatest first
            
        IF @NextRankId <> @CurrentRankId
        BEGIN
            -- Update the head of household rank.
            UPDATE dbo.rad_Players SET RankId = @NextRankId WHERE PlayerId = @HeadOfHouseId
        END
    END
    
    -- Return the details of the household and rank.
    SELECT
        HouseholdId,
        HouseholdImageUri,
        HouseholdName,
        RankId,
        RankImageUri,
        RankName,
        Title
    FROM
        dbo.rad_vw_Players
    WHERE
        ObjectId = @PlayerId

END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetMembers]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetMembers]
(
    @HouseholdId        int,
    @StartRowIndex		int,
    @MaxRows			int
)
AS
BEGIN
    SET @StartRowIndex = @StartRowIndex + 1
    
    SELECT
		*
	FROM
		(
		SELECT
			ROW_NUMBER() OVER(ORDER BY r.RankOrder DESC) AS RowNum,
			p.*
		FROM
			dbo.rad_vw_Players p
			INNER JOIN dbo.rad_HouseholdRanks r ON (p.HouseholdId = r.HouseholdId AND p.RankId = r.RankId)
		WHERE
			p.HouseholdId = @HouseholdId
		) as Characters
	WHERE
		RowNum BETWEEN @StartRowIndex AND ((@StartRowIndex + @MaxRows) - 1)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetByName]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetByName]
(
    @HouseholdName        nvarchar(20)
)
AS
BEGIN
    DECLARE @HouseholdId int
    SELECT @HouseholdId = HouseholdId FROM dbo.rad_Households WHERE HouseholdName = @HouseholdName
    
    EXEC dbo.rad_Households_Get @HouseholdId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_FeaturedPlayer_Save]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_FeaturedPlayer_Save]
(
	@PlayerId		int
)
AS
BEGIN
    DECLARE @StartDate datetime,
            @EndDate datetime
    SELECT TOP 1 @StartDate = EndDate FROM dbo.rad_FeaturedPlayer ORDER BY EndDate DESC
    
    IF @StartDate IS NULL
        SET @StartDate = GETDATE()
    
    SET @EndDate = DATEADD(day,1,@StartDate)
    
	INSERT INTO dbo.rad_FeaturedPlayer (PlayerId, StartDate, EndDate) VALUES (@PlayerId, @StartDate, @EndDate)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_Save]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_Save]
(
	@WorldName			nvarchar(64),
	@ObjectName			nvarchar(64),
	@TypeName			nvarchar(256),
	@OwnerObjectId		int = null,
	@OwnerPlayerId		int = null,
	@TemplateId			int = null,
	@Properties			xml,
	@ObjectId			int output
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeId int
	EXEC dbo.rad_Types_GetTypeId @TypeName = @TypeName, @TypeId = @TypeId OUT

	IF @OwnerObjectId = @OwnerPlayerId
		SET @OwnerObjectId = NULL

	IF @OwnerObjectId = 0
		SET @OwnerObjectId = NULL

	IF @OwnerPlayerId = 0
		SET @OwnerPlayerId = NULL

	IF @TemplateId = 0
		SET @TemplateId = NULL

	DECLARE @Err int
	SET @Err = 0
	BEGIN TRAN

	IF @ObjectId > 0
	BEGIN
		UPDATE
			dbo.rad_Objects
		SET
			ObjectName		= @ObjectName,
			TypeId			= @TypeId,
			OwnerObjectId	= @OwnerObjectId,
			OwnerPlayerId	= @OwnerPlayerId,
			TemplateId		= @TemplateId,
			Properties		= @Properties
		WHERE
			ObjectId = @ObjectId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.rad_Objects
		(
			WorldId,
			TypeId,
			ObjectName,
			OwnerObjectId,
			OwnerPlayerId,
			TemplateId,
			Properties
		)
		VALUES
		(
			@WorldId,
			@TypeId,
			@ObjectName,
			@OwnerObjectId,
			@OwnerPlayerId,
			@TemplateId,
			@Properties
		)
		SET @ObjectId = SCOPE_IDENTITY()
	END	

	SET @Err = @@ERROR
	IF @Err <> 0
		GOTO FINISHED

	FINISHED:
		IF @Err = 0
		BEGIN
			COMMIT TRAN
		END
		ELSE
		BEGIN
			RAISERROR ('Object failed to save: World = %d, Name = %d', 16, 1, @WorldName, @ObjectName)
			ROLLBACK TRAN
		END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_RemoveProperty]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_RemoveProperty]
(
	@ObjectId		int,
	@PropertyName	nvarchar(64)
)
AS
BEGIN
	UPDATE dbo.rad_Objects SET Properties.modify('delete /properties/property[@name = sql:variable("@PropertyName")]') WHERE ObjectId = @ObjectId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_NameExists]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_NameExists]
(
	@Name			nvarchar(64),
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @Index int
	SELECT @Index = CHARINDEX(' ', @Name)

	DECLARE @Result int
	SET @Result = 0

	-- Players
	SELECT @Result = PlayerId FROM dbo.rad_Players WHERE WorldId = @WorldId AND LOWER(FirstName) = LOWER(@Name)

	-- Objects
	IF @Result = 0
	BEGIN
		IF @Index > 0
		BEGIN
			SELECT @Result = ObjectId FROM dbo.rad_Objects WHERE WorldId = @WorldId AND LOWER(LEFT(ObjectName, CHARINDEX(' ', ObjectName) - 1)) = LOWER(@Name)
		END
		ELSE
		BEGIN
			SELECT @Result = ObjectId FROM dbo.rad_Objects WHERE WorldId = @WorldId AND LOWER(ObjectName) = LOWER(@Name)
		END
	END

	IF @Result = 0
		SET @Result = NULL

	SELECT @Result
END
GO
/****** Object:  Table [dbo].[rad_Places]    Script Date: 04/20/2016 18:17:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rad_Places](
	[ObjectId] [int] NOT NULL,
	[WorldId] [int] NOT NULL,
	[X] [int] NOT NULL,
	[Y] [int] NOT NULL,
	[Z] [int] NOT NULL,
 CONSTRAINT [PK_rad_Places] PRIMARY KEY CLUSTERED 
(
	[ObjectId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_GetPlayers]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetPlayers]
(
	@UserName		nvarchar(256),
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	-- Player
	SELECT
		o.*
	FROM
		dbo.rad_vw_Players o
		INNER JOIN dbo.rad_UserDetails u ON o.UserDetailId = u.UserDetailId
	WHERE	
		u.WorldId = @WorldId
		AND u.UserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Sessions_Update]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[rad_Sessions_Update]
(
	@SessionId		uniqueidentifier,
	@UserName		nvarchar(256) = null,
	@PlayerId		int = null,
	@LastHeartbeat	datetime,
	@LastCommand	nvarchar(max) = null
	
)
as
begin
	update
		dbo.rad_Sessions
	set
		UserName		= @UserName,
		PlayerId		= @PlayerId,
		LastHeartbeat	= @LastHeartbeat,
		LastCommand		= @LastCommand
	where
		SessionId = @SessionId
end
GO
/****** Object:  StoredProcedure [dbo].[rad_Sessions_Remove]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[rad_Sessions_Remove]
(
	@SessionId		uniqueidentifier
)
as
begin
	delete from dbo.rad_Sessions where SessionId = @SessionId
end
GO
/****** Object:  StoredProcedure [dbo].[rad_Sessions_Create]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[rad_Sessions_Create]
(
	@SessionId		uniqueidentifier,
	@IPAddress		nvarchar(32)
)
as
begin
	insert into dbo.rad_Sessions (SessionId, IPAddress, LastHeartbeat) values (@SessionId, @IPAddress, getdate())
end
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_Delete]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_Delete]
(
	@PlayerId		int
)
AS
BEGIN
	BEGIN TRAN

	-- Get owned objects
	DECLARE @OwnedObjects TABLE
	(
		ObjectId		int primary key
	)
	INSERT INTO @OwnedObjects (ObjectId)
	SELECT ObjectId FROM dbo.rad_Objects WHERE OwnerPlayerId = @PlayerId

	-- Clear the owner player id from the owned objects
	UPDATE dbo.rad_Objects SET OwnerPlayerId = NULL WHERE OwnerPlayerId = @PlayerId

	-- Create an xml list of owned objects
	DECLARE @Objects XML
	SET @Objects = (SELECT ObjectId as id FROM @OwnedObjects as object FOR XML AUTO, ROOT('objects'))

	-- Inser the current player record into the deleted table
	INSERT INTO dbo.rad_Players_Deleted
	(
		PlayerId, UserDetailId, WorldId, TypeId, FirstName, LastName, Properties, OwnedObjects
	)
	SELECT
		PlayerId, UserDetailId, WorldId, TypeId, FirstName, LastName, Properties, @Objects
	FROM
		dbo.rad_Players
	WHERE
		PlayerId = @PlayerId
		
	-- Delete the player record from the feartured players table
	DELETE FROM dbo.rad_FeaturedPlayer WHERE PlayerId = @PlayerId

	-- Delete the player record from the players table
	DELETE FROM dbo.rad_Players WHERE PlayerId = @PlayerId

	IF @@ERROR = 0
	BEGIN
		COMMIT TRAN
	END
	ELSE
	BEGIN
		ROLLBACK TRAN
	END
END
GO
/****** Object:  View [dbo].[rad_vw_Templates]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[rad_vw_Templates]
AS
SELECT
	o.ObjectId, 
	o.WorldId, 
	o.TypeId, 
	t.TypeName,
	o.TemplateId,
	o.OwnerObjectId, 
	o.OwnerPlayerId,
	o.ObjectName, 
	o.Properties,
	NULL AS TemplateProperties
FROM
	dbo.rad_Objects o
	INNER JOIN dbo.rad_Types t ON o.TypeId = t.TypeId
WHERE
	TemplateId IS NULL
GO
/****** Object:  View [dbo].[rad_vw_Sessions]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[rad_vw_Sessions]
as
	select
		s.SessionId,
		s.IPAddress,
		s.UserName,
		s.PlayerId,
		p.FirstName,
		s.LastHeartbeat,
		s.LastCommand
	from
		dbo.rad_Sessions s
		left outer join dbo.rad_Players p on s.PlayerId = p.PlayerId
GO
/****** Object:  View [dbo].[rad_vw_Objects]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[rad_vw_Objects]
AS
SELECT
	o.ObjectId, 
	o.WorldId, 
	o.TypeId, 
	t.TypeName,
	o.TemplateId,
	o.OwnerObjectId, 
	o.OwnerPlayerId,
	o.ObjectName, 
	o.Properties,
	tm.Properties as TemplateProperties
FROM
	dbo.rad_Objects o
	INNER JOIN dbo.rad_Types t ON o.TypeId = t.TypeId
	INNER JOIN dbo.rad_Objects tm ON o.TemplateId = tm.ObjectId
GO
/****** Object:  View [dbo].[rad_vw_Places]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[rad_vw_Places]
AS
SELECT
	o.ObjectId, 
	o.WorldId, 
	o.TypeId, 
	t.TypeName,
	o.OwnerObjectId, 
	o.OwnerPlayerId,
	o.ObjectName, 
	o.Properties,
	p.X,
	p.Y,
	p.Z
FROM
	dbo.rad_Objects o
	INNER JOIN dbo.rad_Places p ON o.ObjectId = p.ObjectId
	INNER JOIN dbo.rad_Types t ON o.TypeId = t.TypeId
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_SavePlace]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_SavePlace]
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
GO
/****** Object:  StoredProcedure [dbo].[rad_Players_GetPlayerByName]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetPlayerByName]
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
/****** Object:  StoredProcedure [dbo].[rad_Players_GetPlayer]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Players_GetPlayer]
(
	@PlayerId		int
)
AS
BEGIN
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
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetTemplates]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetTemplates]
(
	@WorldName			nvarchar(64),
	@Types				xml
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeIds TABLE
	(
		TypeId			int primary key
	)

	INSERT INTO @TypeIds (TypeId)
	SELECT
		TypeId
	FROM
		dbo.rad_Types
	WHERE
		TypeName IN (SELECT t.n.value('(text())[1]', 'nvarchar(256)') FROM @Types.nodes('/ArrayOfString/string') as t(n))

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
		dbo.rad_vw_Templates
	WHERE
		WorldId = @WorldId
		AND TypeId IN (SELECT TypeId FROM @TypeIds)
		AND TemplateId IS NULL
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetTemplateById]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetTemplateById]
(
	@WorldName			nvarchar(64),
	@ObjectId			int
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

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
		dbo.rad_vw_Templates
	WHERE
		WorldId = @WorldId
		AND ObjectId = @ObjectId
		AND TemplateId IS NULL
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetTemplate]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetTemplate]
(
	@WorldName			nvarchar(64),
	@Name				nvarchar(64),
	@Types				xml
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeIds TABLE
	(
		TypeId			int primary key
	)

	INSERT INTO @TypeIds (TypeId)
	SELECT
		TypeId
	FROM
		dbo.rad_Types
	WHERE
		TypeName IN (SELECT t.n.value('(text())[1]', 'nvarchar(256)') FROM @Types.nodes('/ArrayOfString/string') as t(n))

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
		dbo.rad_vw_Templates
	WHERE
		WorldId = @WorldId
		AND ObjectName = @Name
		AND TypeId IN (SELECT TypeId FROM @TypeIds)
		AND TemplateId IS NULL
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetQuestsForActor]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetQuestsForActor]
(
	@WorldName		nvarchar(64),
	@ObjectId		int,
	@Types			xml
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeIds TABLE
	(
		TypeId			int primary key
	)

	INSERT INTO @TypeIds (TypeId)
	SELECT
		TypeId
	FROM
		dbo.rad_Types
	WHERE
		TypeName IN (SELECT t.n.value('(text())[1]', 'nvarchar(256)') FROM @Types.nodes('/ArrayOfString/string') as t(n))
	
	DECLARE @ids TABLE
	(
		ObjectId		int primary key
	)

	DECLARE @StartsWith nvarchar(128),
			@EndsWith	nvarchar(128)

	SET @StartsWith = 'StartsWith_' + CONVERT(nvarchar, @ObjectId)
	SET @EndsWith = 'EndsWith_' + CONVERT(nvarchar, @ObjectId)

	INSERT INTO @ids (ObjectId)
	SELECT
		ObjectId
	FROM
		dbo.rad_Objects
	WHERE
		WorldId = @WorldId
		AND TypeId IN (SELECT TypeId FROM @TypeIds)
		AND TemplateId IS NULL
		AND 
		(
			Properties.exist('(//property[@name = sql:variable("@StartsWith")])[1]') = 1
			OR
			Properties.exist('(//property[@name = sql:variable("@EndsWith")])') = 1
		)

	--select * from @ids

	-- Quest Templates
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
		dbo.rad_vw_Templates
	WHERE
		ObjectId IN (SELECT ObjectId FROM @ids)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetAllTemplates]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetAllTemplates]
(
	@WorldName			nvarchar(64),
	@ObjectType			nvarchar(32)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

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
		dbo.rad_vw_Templates
	WHERE
		WorldId = @WorldId
		AND TemplateId IS NULL
		AND ((@ObjectType IS NULL) OR (Properties.value('(//property[@name="ObjectType"]/text())[1]', 'nvarchar(32)') = @ObjectType))
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetAllObjectsByType]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetAllObjectsByType]
(
	@WorldName		nvarchar(64),
	@ObjectType		nvarchar(32)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @tbl TABLE
	(
		ObjectId		int primary key
	)

	INSERT INTO @tbl (ObjectId)
	SELECT 
		ObjectId
	FROM
		dbo.rad_Objects
	WHERE
		WorldId = @WorldId
		AND Properties.value('(//property[@name="ObjectType"]/text())[1]', 'nvarchar(32)') = @ObjectType

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
		ObjectId IN (SELECT ObjectId FROM @tbl)

	-- Owned Objects
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
		OwnerObjectId IN (SELECT ObjectId FROM @tbl)

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
		p.OwnerObjectId IN (SELECT ObjectId FROM @tbl)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetAllObjectsByRuntimeType]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetAllObjectsByRuntimeType]
(
	@WorldName		nvarchar(64),
	@TypeName		nvarchar(256)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @TypeId int
	EXEC dbo.rad_Types_GetTypeId @TypeName = @TypeName, @TypeId = @TypeId OUT

	DECLARE @tbl TABLE
	(
		ObjectId		int primary key
	)

	INSERT INTO @tbl (ObjectId)
	SELECT 
		ObjectId
	FROM
		dbo.rad_Objects
	WHERE
		WorldId = @WorldId
		AND TypeId = @TypeId

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
		ObjectId IN (SELECT ObjectId FROM @tbl)

	-- Owned Objects
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
		OwnerObjectId IN (SELECT ObjectId FROM @tbl)

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
		p.OwnerObjectId IN (SELECT ObjectId FROM @tbl)
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetObject]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetObject]
(
	@ObjectId		int
)
AS
BEGIN
	-- Object
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
		ObjectId = @ObjectId

	-- Owned Objects
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
		OwnerObjectId = @ObjectId

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
		p.OwnerObjectId = @ObjectId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetObjectsForUserName]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetObjectsForUserName]
(
	@UserName		nvarchar(256),
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	-- Object
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
		INNER JOIN dbo.rad_UserObjects uo ON o.ObjectId = uo.ObjectId
		INNER JOIN dbo.rad_UserDetails ud ON uo.UserDetailId = ud.UserDetailId
	WHERE	
		ud.WorldId = @WorldId
		AND ud.UserName = @UserName
END
GO
/****** Object:  StoredProcedure [dbo].[rad_FeaturedPlayer_Seed]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_FeaturedPlayer_Seed]
AS
BEGIN
    DECLARE @tbl TABLE
    (
        ID          int not null identity(1,1) primary key,
        PlayerId    int not null
    )
    
    INSERT INTO @tbl (PlayerId)
    SELECT PlayerId FROM dbo.rad_Players ORDER BY NEWID()
    
    DECLARE @index int,
            @count int
    SET @index = 1
    SELECT @count = COUNT(*) FROM @tbl
    
    WHILE (@index < (@count + 1))
    BEGIN   
        DECLARE @PlayerId int
        SELECT @PlayerId = PlayerId FROM @tbl WHERE ID = @index
        
        EXEC dbo.rad_FeaturedPlayer_Save @PlayerId
        
        SET @index = @index + 1
    END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Households_GetArmory]    Script Date: 04/20/2016 18:17:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--=================================================================================================
CREATE PROCEDURE [dbo].[rad_Households_GetArmory]
(
    @HouseholdId        int
)
AS
BEGIN
    SELECT
        o.*
    FROM
        dbo.rad_vw_Objects o
        INNER JOIN dbo.rad_HouseholdArmory a ON o.Objectid = a.ObjectId
    WHERE
        a.HouseholdId = @HouseholdId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Maps_GetAll]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Maps_GetAll]
(
	@WorldName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId out
	
	-- Maps
	SELECT 
		m.*, 
		t.TypeName 
	FROM 
		dbo.rad_Maps m 
		LEFT OUTER JOIN dbo.rad_Types t ON m.TypeId = t.TypeId 
	WHERE 
		m.WorldId = @WorldId
		
	-- Places
	SELECT
		p.X,
		p.Y,
		p.Z,
		p.Terrain,
		tr.TerrainName,
		tr.WalkType,
		tr.Color,
		tr.ImageUrl
	FROM
		(
			SELECT
				X, 
				Y, 
				Z, 
				Properties.value('(/properties/property[@name = "Terrain"]/text())[1]', 'int') as Terrain,
				WorldId
			FROM
				dbo.rad_vw_Places
			WHERE
				WorldId = @WorldId
		) as p
		INNER JOIN dbo.rad_Terrain tr ON p.Terrain = tr.TerrainId
	WHERE
		p.WorldId = @WorldId
	--SELECT
	--	ObjectId,
	--	WorldId,
	--	OwnerObjectId,
	--	NULL AS TemplateId,
	--	TypeName,
	--	ObjectName,
	--	Properties,
	--	NULL AS TemplateProperties,
	--	X,
	--	Y,
	--	Z
	--FROM
	--    dbo.rad_vw_Places
	--WHERE
	--	WorldId = @WorldId
	--	AND X BETWEEN @startX AND @endX
	--	AND Y BETWEEN @startY AND @endY
		
	-- Tiles
	--SELECT
	--	t.X,
	--	t.Y,
	--	t.Z,
	--	t.TerrainId,
	--	tr.TerrainName,
	--	tr.WalkType,
	--	tr.Color,
	--	tr.ImageUrl
	--FROM
	--	dbo.rad_Tiles t
	--	INNER JOIN dbo.rad_Terrain tr ON t.TerrainId = tr.TerrainId
	--WHERE
	--	t.WorldId = @WorldId
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetObjectByName]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetObjectByName]
(
	@WorldName		nvarchar(64),
	@ObjectName		nvarchar(64)
)
AS
BEGIN
	DECLARE @WorldId int
	EXEC dbo.rad_Worlds_GetWorldId @WorldName = @WorldName, @WorldId = @WorldId OUT

	DECLARE @ObjectId int
	SELECT 
		@ObjectId = ObjectId
	FROM
		dbo.rad_Objects
	WHERE	
		WorldId = @WorldId
		AND (LOWER(ObjectName) = LOWER(@ObjectName) OR LOWER(REPLACE(REPLACE(ObjectName, ' ', ''), '''', '')) = REPLACE(LOWER(@ObjectName), '''', ''))

	DECLARE @PlayerId int
	IF @ObjectId IS NULL
	BEGIN
		SELECT
			@PlayerId = PlayerId
		FROM
			dbo.rad_Players
		WHERE	
			WorldId = @WorldId
			AND (LOWER(FirstName) = LOWER(@ObjectName) OR LOWER(FirstName + ' ' + LastName) = LOWER(@ObjectName))
	END

	IF @ObjectId IS NOT NULL
	BEGIN
		EXEC dbo.rad_Objects_GetObject @ObjectId
	END

	IF @PlayerId IS NOT NULL
	BEGIN
		EXEC dbo.rad_Players_GetPlayer @PlayerId
	END
END
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetMap]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetMap]
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
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetPlaceById]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Object:  StoredProcedure [dbo].[rad_Objects_GetPlace]    Script Date: 04/20/2016 18:17:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[rad_Objects_GetPlace]
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
GO
/****** Object:  Default [DF_rad_Commands_IsVisible]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Commands] ADD  CONSTRAINT [DF_rad_Commands_IsVisible]  DEFAULT ((1)) FOR [IsVisible]
GO
/****** Object:  Default [DF__rad_House__DateA__0737E4A2]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdArmory] ADD  DEFAULT (getdate()) FOR [DateAdded]
GO
/****** Object:  Default [DF__rad_House__Requi__77017CD9]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRanks] ADD  DEFAULT ((0)) FOR [RequiredMemberCount]
GO
/****** Object:  Default [DF__rad_House__Emble__77F5A112]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRanks] ADD  DEFAULT ((0)) FOR [EmblemCost]
GO
/****** Object:  Default [DF__rad_House__DateC__70547F4A]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Households] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF_rad_Logs_LogDate]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Logs] ADD  CONSTRAINT [DF_rad_Logs_LogDate]  DEFAULT (getdate()) FOR [LogDate]
GO
/****** Object:  Default [DF_rad_Players_IsDeleted]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players] ADD  CONSTRAINT [DF_rad_Players_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]
GO
/****** Object:  Default [DF__rad_Playe__DateD__6FB49575]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players_Deleted] ADD  CONSTRAINT [DF__rad_Playe__DateD__6FB49575]  DEFAULT (getdate()) FOR [DateDeleted]
GO
/****** Object:  ForeignKey [FK_rad_Commands_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Commands]  WITH CHECK ADD  CONSTRAINT [FK_rad_Commands_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Commands] CHECK CONSTRAINT [FK_rad_Commands_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_FeaturedPlayer_rad_Players]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_FeaturedPlayer]  WITH CHECK ADD  CONSTRAINT [FK_rad_FeaturedPlayer_rad_Players] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[rad_Players] ([PlayerId])
GO
ALTER TABLE [dbo].[rad_FeaturedPlayer] CHECK CONSTRAINT [FK_rad_FeaturedPlayer_rad_Players]
GO
/****** Object:  ForeignKey [fk_FileUpdates_WorldId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_FileUpdates]  WITH CHECK ADD  CONSTRAINT [fk_FileUpdates_WorldId] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_FileUpdates] CHECK CONSTRAINT [fk_FileUpdates_WorldId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdArmory_AddedByPlayerId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdArmory]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdArmory_AddedByPlayerId] FOREIGN KEY([AddedByPlayerId])
REFERENCES [dbo].[rad_Players] ([PlayerId])
GO
ALTER TABLE [dbo].[rad_HouseholdArmory] CHECK CONSTRAINT [FK_rad_HouseholdArmory_AddedByPlayerId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdArmory_HouseholdId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdArmory]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdArmory_HouseholdId] FOREIGN KEY([HouseholdId])
REFERENCES [dbo].[rad_Households] ([HouseholdId])
GO
ALTER TABLE [dbo].[rad_HouseholdArmory] CHECK CONSTRAINT [FK_rad_HouseholdArmory_HouseholdId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdArmory_ObjectId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdArmory]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdArmory_ObjectId] FOREIGN KEY([ObjectId])
REFERENCES [dbo].[rad_Objects] ([ObjectId])
GO
ALTER TABLE [dbo].[rad_HouseholdArmory] CHECK CONSTRAINT [FK_rad_HouseholdArmory_ObjectId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdRanks_HouseholdId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRanks]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdRanks_HouseholdId] FOREIGN KEY([HouseholdId])
REFERENCES [dbo].[rad_Households] ([HouseholdId])
GO
ALTER TABLE [dbo].[rad_HouseholdRanks] CHECK CONSTRAINT [FK_rad_HouseholdRanks_HouseholdId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdRanks_WorldId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRanks]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdRanks_WorldId] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_HouseholdRanks] CHECK CONSTRAINT [FK_rad_HouseholdRanks_WorldId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdRelations_PrimaryHouseholdId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRelations]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdRelations_PrimaryHouseholdId] FOREIGN KEY([PrimaryHouseholdId])
REFERENCES [dbo].[rad_Households] ([HouseholdId])
GO
ALTER TABLE [dbo].[rad_HouseholdRelations] CHECK CONSTRAINT [FK_rad_HouseholdRelations_PrimaryHouseholdId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdRelations_RelationTypeId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRelations]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdRelations_RelationTypeId] FOREIGN KEY([RelationTypeId])
REFERENCES [dbo].[rad_HouseholdRelationTypes] ([RelationTypeId])
GO
ALTER TABLE [dbo].[rad_HouseholdRelations] CHECK CONSTRAINT [FK_rad_HouseholdRelations_RelationTypeId]
GO
/****** Object:  ForeignKey [FK_rad_HouseholdRelations_SecondaryHouseholdId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_HouseholdRelations]  WITH CHECK ADD  CONSTRAINT [FK_rad_HouseholdRelations_SecondaryHouseholdId] FOREIGN KEY([SecondaryHouseholdId])
REFERENCES [dbo].[rad_Households] ([HouseholdId])
GO
ALTER TABLE [dbo].[rad_HouseholdRelations] CHECK CONSTRAINT [FK_rad_HouseholdRelations_SecondaryHouseholdId]
GO
/****** Object:  ForeignKey [FK_rad_Households_WorldId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Households]  WITH CHECK ADD  CONSTRAINT [FK_rad_Households_WorldId] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Households] CHECK CONSTRAINT [FK_rad_Households_WorldId]
GO
/****** Object:  ForeignKey [FK_rad_Logs_rad_LogType]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Logs]  WITH CHECK ADD  CONSTRAINT [FK_rad_Logs_rad_LogType] FOREIGN KEY([LogTypeId])
REFERENCES [dbo].[rad_LogType] ([LogTypeId])
GO
ALTER TABLE [dbo].[rad_Logs] CHECK CONSTRAINT [FK_rad_Logs_rad_LogType]
GO
/****** Object:  ForeignKey [FK_rad_Logs_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Logs]  WITH CHECK ADD  CONSTRAINT [FK_rad_Logs_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Logs] CHECK CONSTRAINT [FK_rad_Logs_rad_Worlds]
GO
/****** Object:  ForeignKey [fk_rad_Maps_rad_Terrain]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Maps]  WITH CHECK ADD  CONSTRAINT [fk_rad_Maps_rad_Terrain] FOREIGN KEY([DefaultTerrainId])
REFERENCES [dbo].[rad_Terrain] ([TerrainId])
GO
ALTER TABLE [dbo].[rad_Maps] CHECK CONSTRAINT [fk_rad_Maps_rad_Terrain]
GO
/****** Object:  ForeignKey [FK_rad_Maps_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Maps]  WITH CHECK ADD  CONSTRAINT [FK_rad_Maps_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Maps] CHECK CONSTRAINT [FK_rad_Maps_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_Maps_TypeId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Maps]  WITH CHECK ADD  CONSTRAINT [FK_rad_Maps_TypeId] FOREIGN KEY([TypeId])
REFERENCES [dbo].[rad_Types] ([TypeId])
GO
ALTER TABLE [dbo].[rad_Maps] CHECK CONSTRAINT [FK_rad_Maps_TypeId]
GO
/****** Object:  ForeignKey [FK_rad_Objects_rad_Objects]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Objects]  WITH CHECK ADD  CONSTRAINT [FK_rad_Objects_rad_Objects] FOREIGN KEY([OwnerObjectId])
REFERENCES [dbo].[rad_Objects] ([ObjectId])
GO
ALTER TABLE [dbo].[rad_Objects] CHECK CONSTRAINT [FK_rad_Objects_rad_Objects]
GO
/****** Object:  ForeignKey [FK_rad_Objects_rad_Players]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Objects]  WITH CHECK ADD  CONSTRAINT [FK_rad_Objects_rad_Players] FOREIGN KEY([OwnerPlayerId])
REFERENCES [dbo].[rad_Players] ([PlayerId])
GO
ALTER TABLE [dbo].[rad_Objects] CHECK CONSTRAINT [FK_rad_Objects_rad_Players]
GO
/****** Object:  ForeignKey [FK_rad_Objects_rad_Types]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Objects]  WITH CHECK ADD  CONSTRAINT [FK_rad_Objects_rad_Types] FOREIGN KEY([TypeId])
REFERENCES [dbo].[rad_Types] ([TypeId])
GO
ALTER TABLE [dbo].[rad_Objects] CHECK CONSTRAINT [FK_rad_Objects_rad_Types]
GO
/****** Object:  ForeignKey [FK_rad_Objects_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Objects]  WITH CHECK ADD  CONSTRAINT [FK_rad_Objects_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Objects] CHECK CONSTRAINT [FK_rad_Objects_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_Objects_Templates]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Objects]  WITH CHECK ADD  CONSTRAINT [FK_rad_Objects_Templates] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[rad_Objects] ([ObjectId])
GO
ALTER TABLE [dbo].[rad_Objects] CHECK CONSTRAINT [FK_rad_Objects_Templates]
GO
/****** Object:  ForeignKey [FK_rad_Places_rad_Objects]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Places]  WITH CHECK ADD  CONSTRAINT [FK_rad_Places_rad_Objects] FOREIGN KEY([ObjectId])
REFERENCES [dbo].[rad_Objects] ([ObjectId])
GO
ALTER TABLE [dbo].[rad_Places] CHECK CONSTRAINT [FK_rad_Places_rad_Objects]
GO
/****** Object:  ForeignKey [FK_rad_Places_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Places]  WITH CHECK ADD  CONSTRAINT [FK_rad_Places_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Places] CHECK CONSTRAINT [FK_rad_Places_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_Players_HouseholdId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players]  WITH CHECK ADD  CONSTRAINT [FK_rad_Players_HouseholdId] FOREIGN KEY([HouseholdId])
REFERENCES [dbo].[rad_Households] ([HouseholdId])
GO
ALTER TABLE [dbo].[rad_Players] CHECK CONSTRAINT [FK_rad_Players_HouseholdId]
GO
/****** Object:  ForeignKey [FK_rad_Players_rad_Types]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players]  WITH CHECK ADD  CONSTRAINT [FK_rad_Players_rad_Types] FOREIGN KEY([TypeId])
REFERENCES [dbo].[rad_Types] ([TypeId])
GO
ALTER TABLE [dbo].[rad_Players] CHECK CONSTRAINT [FK_rad_Players_rad_Types]
GO
/****** Object:  ForeignKey [FK_rad_Players_rad_UserDetails]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players]  WITH CHECK ADD  CONSTRAINT [FK_rad_Players_rad_UserDetails] FOREIGN KEY([UserDetailId])
REFERENCES [dbo].[rad_UserDetails] ([UserDetailId])
GO
ALTER TABLE [dbo].[rad_Players] CHECK CONSTRAINT [FK_rad_Players_rad_UserDetails]
GO
/****** Object:  ForeignKey [FK_rad_Players_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players]  WITH CHECK ADD  CONSTRAINT [FK_rad_Players_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Players] CHECK CONSTRAINT [FK_rad_Players_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_Players_RankId]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Players]  WITH CHECK ADD  CONSTRAINT [FK_rad_Players_RankId] FOREIGN KEY([RankId])
REFERENCES [dbo].[rad_HouseholdRanks] ([RankId])
GO
ALTER TABLE [dbo].[rad_Players] CHECK CONSTRAINT [FK_rad_Players_RankId]
GO
/****** Object:  ForeignKey [FK_rad_Sessions_rad_Players]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Sessions]  WITH CHECK ADD  CONSTRAINT [FK_rad_Sessions_rad_Players] FOREIGN KEY([PlayerId])
REFERENCES [dbo].[rad_Players] ([PlayerId])
GO
ALTER TABLE [dbo].[rad_Sessions] CHECK CONSTRAINT [FK_rad_Sessions_rad_Players]
GO
/****** Object:  ForeignKey [FK_rad_Skills_rad_SkillTypes]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Skills]  WITH CHECK ADD  CONSTRAINT [FK_rad_Skills_rad_SkillTypes] FOREIGN KEY([SkillTypeId])
REFERENCES [dbo].[rad_SkillTypes] ([SkillTypeId])
GO
ALTER TABLE [dbo].[rad_Skills] CHECK CONSTRAINT [FK_rad_Skills_rad_SkillTypes]
GO
/****** Object:  ForeignKey [FK_rad_Skills_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Skills]  WITH CHECK ADD  CONSTRAINT [FK_rad_Skills_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Skills] CHECK CONSTRAINT [FK_rad_Skills_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_SkillTypes_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_SkillTypes]  WITH CHECK ADD  CONSTRAINT [FK_rad_SkillTypes_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_SkillTypes] CHECK CONSTRAINT [FK_rad_SkillTypes_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_Terrain_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_Terrain]  WITH CHECK ADD  CONSTRAINT [FK_rad_Terrain_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_Terrain] CHECK CONSTRAINT [FK_rad_Terrain_rad_Worlds]
GO
/****** Object:  ForeignKey [FK_rad_UserDetails_rad_Worlds]    Script Date: 04/20/2016 18:17:46 ******/
ALTER TABLE [dbo].[rad_UserDetails]  WITH CHECK ADD  CONSTRAINT [FK_rad_UserDetails_rad_Worlds] FOREIGN KEY([WorldId])
REFERENCES [dbo].[rad_Worlds] ([WorldId])
GO
ALTER TABLE [dbo].[rad_UserDetails] CHECK CONSTRAINT [FK_rad_UserDetails_rad_Worlds]
GO

-- Alter Players Table
alter table dbo.rad_Players add HouseholdId int null
go
alter table dbo.rad_Players add RankId int null
go
alter table dbo.rad_Players add constraint FK_rad_Players_HouseholdId foreign key (HouseholdId) references dbo.rad_Households(HouseholdId)
go
alter table dbo.rad_Players add constraint FK_rad_Players_RankId foreign key (RankId) references dbo.rad_HouseholdRanks(RankId)
go

-- Alter Players View
ALTER VIEW dbo.rad_vw_Players
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
go

ALTER PROCEDURE [dbo].[rad_Players_GetPlayer]
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
go

ALTER PROCEDURE [dbo].[rad_Players_GetPlayers]
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
go

ALTER PROCEDURE [dbo].[rad_Players_Save]
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
go
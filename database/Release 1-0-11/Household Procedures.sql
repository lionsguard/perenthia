


--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_Get
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetByName
(
    @HouseholdName        nvarchar(20)
)
AS
BEGIN
    DECLARE @HouseholdId int
    SELECT @HouseholdId = HouseholdId FROM dbo.rad_Households WHERE HouseholdName = @HouseholdName
    
    EXEC dbo.rad_Households_Get @HouseholdId
END
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetAll
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetMembers
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetRanks
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetHeadOfHouseholdRank
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_GetArmory
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_Save
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_SaveMember
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
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_Households_SaveRank
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
go
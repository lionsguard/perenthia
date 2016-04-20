IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rad_Players_GetPaged]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[rad_Players_GetPaged]
GO


CREATE PROCEDURE dbo.rad_Players_GetPaged
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

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rad_Players_GetCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[rad_Players_GetCount]
GO

CREATE PROCEDURE dbo.rad_Players_GetCount
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

exec dbo.rad_Players_GetPaged 'Perenthia', 0, 10, 'Name', 'ASC'
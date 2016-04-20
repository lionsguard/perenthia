-- objects
-- places
-- maps
CREATE PROCEDURE dbo.rad_Maps_ReAdjustXYValues
AS
BEGIN
    declare @x int,
		    @y int

    set @x = 444
    set @y = 485

    update rad_maps set x = x + @x, y = y + @y
    update rad_places set x = x + @x, y = y + @y

    DECLARE cur CURSOR
    READ_ONLY
    FOR select objectid from rad_objects
	    where Properties.exist('(/properties/property[@name = "X"])') = 1
	    or Properties.exist('(/properties/property[@name = "Y"])') = 1

    DECLARE @objectid int
    OPEN cur

    FETCH NEXT FROM cur INTO @objectid
    WHILE (@@fetch_status <> -1)
    BEGIN
	    IF (@@fetch_status <> -2)
	    BEGIN
		    declare @curX int,
				    @curY int

		    select 
			    @curX = properties.value('(//property[@name = "X"]/text())[1]', 'int'), 
			    @curY = properties.value('(//property[@name = "Y"]/text())[1]', 'int')
		    from rad_objects where objectid = @objectid

		    print '--------------------------------'
		    print @objectid

		    print @curX
		    print @curY

		    set @curX = @curX + @x
		    set @curY = @curY + @y

		    print @curX
		    print @curY
    		
		    update rad_objects
		    set Properties.modify('replace value of (//property[@name = "X"]/text())[1] with sql:variable("@curX")')
		    where objectid = @objectid
    		
		    update rad_objects
		    set	Properties.modify('replace value of (//property[@name = "Y"]/text())[1] with sql:variable("@curY")')
		    where objectid = @objectid
	    END
	    FETCH NEXT FROM cur INTO @objectid
    END

    CLOSE cur
    DEALLOCATE cur

    DECLARE cur1 CURSOR
    READ_ONLY
    FOR select playerid from rad_players
	    where Properties.exist('(/properties/property[@name = "X"])') = 1
	    or Properties.exist('(/properties/property[@name = "Y"])') = 1

    DECLARE @playerid int
    OPEN cur1

    FETCH NEXT FROM cur1 INTO @playerid
    WHILE (@@fetch_status <> -1)
    BEGIN
	    IF (@@fetch_status <> -2)
	    BEGIN
		    select 
			    @curX = properties.value('(//property[@name = "X"]/text())[1]', 'int'), 
			    @curY = properties.value('(//property[@name = "Y"]/text())[1]', 'int')
		    from rad_players where playerid = @playerid

		    set @curX = @curX + @x
		    set @curY = @curY + @y
    		
		    update rad_players
		    set Properties.modify('replace value of (//property[@name = "X"]/text())[1] with sql:variable("@curX")')
		    where playerid = @playerid
    		
		    update rad_players
		    set	Properties.modify('replace value of (//property[@name = "Y"]/text())[1] with sql:variable("@curY")')
		    where playerid = @playerid
	    END
	    FETCH NEXT FROM cur1 INTO @playerid
    END

    CLOSE cur1
    DEALLOCATE cur1
    GO
END
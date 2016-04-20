set nocount on

declare @startid int
set @startid = 9295

declare @ObjectId int,
		@OwnerObjectId int,
		@sql nvarchar(max)

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[rad_temp_objects]') AND type in (N'U'))
DROP TABLE [dbo].[rad_temp_objects]

create table dbo.rad_temp_objects
(
	RowNum			int identity(1,1) not null primary key,
	ObjectId		int not null,
	NewObjectId		int null,
	ObjectName		nvarchar(64) not null,
	TypeName		nvarchar(256) not null,
	OwnerObjectId	int null,
	TemplateId		int null,
	X				int null,
	Y				int null,
	Z				int null,
	Properties		xml null
)

--==================================================================================================
-- Parent Objects
--==================================================================================================
insert into dbo.rad_temp_objects (ObjectId, ObjectName, TypeName, OwnerObjectId, x, y, z, Properties) 
select objectid, objectname, typename, ownerobjectid, x, y, z, properties
from dbo.rad_vw_places where objectid > @startid and ownerobjectid is null

insert into dbo.rad_temp_objects (ObjectId, ObjectName, TypeName, OwnerObjectId, TemplateId, Properties) 
select objectid, objectname, typename, ownerobjectid, templateid, properties
from dbo.rad_vw_objects where objectid > @startid and ownerobjectid is null
and objectid not in (select objectid from rad_places where objectid > @startid)
and typeid not in (2,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,28,29,30,37,38,47,48,49,50,51,52,53,54,55,56,57,58,59,60,62,63,70,71)

insert into dbo.rad_temp_objects (ObjectId, ObjectName, TypeName, OwnerObjectId, TemplateId, Properties) 
select objectid, objectname, typename, ownerobjectid, templateid, properties
from dbo.rad_vw_objects where objectid > @startid and ownerobjectid is not null

declare @count int,
		@index int

select * from dbo.rad_temp_objects


--select @count = count(*) from @objects
--set @index = 1
--
----select * from @objects --where x is not null and y is not null and z is not null
--
--while @index < (@count + 1)
--begin
--	-- Places
--	select @sql = N'
--		exec dbo.rad_Objects_SavePlace
--			@WorldName			=''Perenthia'',
--			@ObjectName			=''' + replace(objectname, '''', '''''') + ''',
--			@TypeName			=''' + typename + ''',
--			@OwnerObjectId		= ' + isnull(convert(nvarchar, ownerobjectid), 'NULL') + ',
--			@Properties			= ''' + convert(nvarchar(max), properties) + ''',
--			@X					= ' + convert(nvarchar, x) + ',
--			@Y					= ' + convert(nvarchar, y) + ',
--			@Z					= ' + convert(nvarchar, z) + ',
--			@ObjectId			= @ObjectId out
--
--		insert into @objects (objectid, oldobjectid) values (@objectid, ' + convert(nvarchar, objectid) + ')
--		'
--	from @objects where rownum = @index and x is not null and y is not null and z is not null
--
--	print @sql
--
--	-- Owner Objects
--	select @sql = N'
--        exec dbo.rad_Objects_Save
--	        @WorldName			=''Perenthia'',
--	        @ObjectName			=''' + replace(objectname, '''', '''''') + ''',
--	        @TypeName			=''' + typename + ''',
--	        @OwnerObjectId		= ' + isnull(convert(nvarchar, ownerobjectid), 'NULL') + ',
--	        @TemplateId		    = ' + isnull(convert(nvarchar, templateid), 'NULL') + ',
--	        @OwnerPlayerId		= NULL,
--	        @Properties			= ''' + convert(nvarchar(max), properties) + ''',
--	        @ObjectId			= @objectId out
--	        
--	    insert into @objects (objectid, oldobjectid) values (@objectid, ' + convert(nvarchar, objectid) + ')
--	    '
--	from @objects where rownum = @index and x is null and y is null and z is null and ownerobjectid is null
--
--	print @sql
--
--    -- Owner Owned Objects
--	select @sql = N'
--    -- Find the new object id from the @objects table.
--    select @ownerobjectid = objectid from @objects where oldobjectid = ' + isnull(convert(nvarchar, ownerobjectid), 'NULL') + '
--    
--    exec dbo.rad_Objects_Save
--        @WorldName			=''Perenthia'',
--        @ObjectName			=''' + replace(objectname, '''', '''''') + ''',
--        @TypeName			=''' + typename + ''',
--        @OwnerObjectId		= @ownerobjectid,
--        @TemplateId		    = ' + isnull(convert(nvarchar, templateid), 'NULL') + ',
--        @OwnerPlayerId		= NULL,
--        @Properties			= ''' + convert(nvarchar(max), properties) + ''',
--        @ObjectId			= @objectId out
--    '
--    from @objects where rownum = @index and x is null and y is null and z is null and ownerobjectid is not null
--	
--	print @sql
--	set @index = @index + 1
--end
go
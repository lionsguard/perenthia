set nocount on

declare @count int,
		@index int,
		@sql nvarchar(max),
		@objectname nvarchar(64),
		@typename nvarchar(256),
		@objectid int,
		@newobjectid int,
		@ownerobjectid int,
		@properties xml,
		@x int,
		@y int,
		@z int,
		@templateid int

declare @objects table
(
	RowNum			int identity(1,1) not null primary key,
	ObjectId		int not null,
	ObjectName		nvarchar(64) not null,
	TypeName		nvarchar(256) not null,
	X				int null,
	Y				int null,
	Z				int null,
	Properties		xml null
)

insert into @objects
select
	objectid,objectname,typename,x, y, z, properties
from rad_temp_objects

select @count = count(*) from @objects
set @index = 1


while @index < (@count + 1)
begin
	-- Places
	select 
		@objectid = objectid,
		@objectname = objectname,
		@typename = typename,
		@properties = properties,
		@x = x,
		@y = y,
		@z = z
	from @objects where rownum = @index

	if @objectid is not null
	begin
		set @sql = N'
declare @newobjectid int
		exec dbo.rad_Objects_SavePlace
			@WorldName			=''Perenthia'',
			@ObjectName			=''' + @objectname + ''', 
			@TypeName			=''' + @typename + ''',
			@OwnerObjectId		=null,
			@Properties			=''' + convert(nvarchar(max), @properties) + ''',
			@X					=' + convert(nvarchar, @x) + ',
			@Y					=' + convert(nvarchar, @y) + ',
			@Z					=' + convert(nvarchar, @z) + ',
			@ObjectId			= @newobjectid out
go
		'
		print @sql

		--update dbo.rad_temp_objects set newobjectid = @newobjectid where objectid = @objectid
	end
	set @index = @index + 1
end
go

--select @count = count(*) from dbo.rad_temp_objects
--set @index = 1
--
--while @index < (@count + 1)
--begin
--
--	-- Owner Objects
--	select 
--		@objectid = objectid,
--		@templateid = templateid,
--		@objectname = objectname,
--		@typename = typename,
--		@ownerobjectid = ownerobjectid,
--		@properties = properties
--	from dbo.rad_temp_objects where rownum = @index and x is null and y is null and z is null and ownerobjectid is null
--
--	if @objectid is not null
--	begin
--		exec dbo.rad_Objects_Save
--				@WorldName			='Perenthia',
--				@ObjectName			=@objectname,
--				@TypeName			=@typename,
--				@OwnerObjectId		=@ownerobjectid,
--				@TemplateId		    =@templateid,
--				@OwnerPlayerId		= NULL,
--				@Properties			=@properties,
--				@ObjectId			= @newobjectId out
--		
--		update dbo.rad_temp_objects set newobjectid = @newobjectid where objectid = @objectid
--	end
--	set @index = @index + 1
--end
--
--select @count = count(*) from dbo.rad_temp_objects
--set @index = 1
--
--while @index < (@count + 1)
--begin
--	
--	select 
--		@objectid = objectid,
--		@templateid = templateid,
--		@objectname = objectname,
--		@typename = typename,
--		@ownerobjectid = ownerobjectid,
--		@properties = properties
--	from dbo.rad_temp_objects where rownum = @index and x is null and y is null and z is null and ownerobjectid is not null
--
--	if @objectid is not null
--	begin
--		select @ownerobjectid = newobjectid from dbo.rad_temp_objects where objectid = @objectid
--	    
--		exec dbo.rad_Objects_Save
--			@WorldName			='Perenthia',
--			@ObjectName			=@objectname,
--			@TypeName			=@typename,
--			@OwnerObjectId		=@ownerobjectid,
--			@TemplateId		    =@templateid,
--			@OwnerPlayerId		= NULL,
--			@Properties			=@properties,
--			@ObjectId			= @newobjectId out
--	end
--
--	set @index = @index + 1
--end

-- Update Commands Table
update rad_commands set isvisible = 0 where commandname = 'ATTACK' or commandname = 'CAST'
update rad_commands set syntax = 'HELP command(optional)', arguments='"COMMAND: The command in which to provide help information."', help='Provides help information on the specified COMMAND.' where commandname = 'HELP'
go
-- Household Commands
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDCREATE', 'Mortal', 'HOUSEHOLDCREATE name, closedMembership', '"NAME: The name of the Household to create.","CLOSEDMEMBERSHIP: Indicates that membership in the Household requires approval. (true or false)"', 'Creates a new Household.', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDDELETE', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDVIEW', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDIMAGE', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDMOTTO', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDDESC', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDMEMBERS', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDADDMEMBER', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDREMOVEMEMBER', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDADVANCEMEMBER', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDSTATUS', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDUPDATESTATUS', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDSAVERANK', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDREMOVERANK', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDSAVEHONOR', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDREMOVEHONOR', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDARMORY', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDADDITEM', 'Mortal', '', '', '', 0)
insert into rad_commands (WorldId,CommandName,RequiredRole,Syntax,Arguments,Help,IsVisible) values (1, 'HOUSEHOLDREMOVEITEM', 'Mortal', '', '', '', 0)
go
-- Map Table Changes
alter table dbo.rad_maps add DefaultTerrainId int null
alter table dbo.rad_maps add constraint fk_rad_Maps_rad_Terrain foreign key (DefaultTerrainId) references dbo.rad_Terrain(TerrainId)
go
update dbo.rad_maps set DefaultTerrainId = 1
go

-- Terrain Changes
update dbo.rad_terrain set imageurl = 'terrain/grass.png' where terrainid = 1
update dbo.rad_terrain set TerrainName = 'Woods', imageurl = 'terrain/woods.png' where terrainid = 2
update dbo.rad_terrain set imageurl = 'terrain/water.png' where terrainid = 3
update dbo.rad_terrain set imageurl = 'terrain/stone.png' where terrainid = 4
update dbo.rad_terrain set imageurl = 'terrain/rock.png' where terrainid = 5
update dbo.rad_terrain set imageurl = 'terrain/sand.png' where terrainid = 6
update dbo.rad_terrain set imageurl = 'terrain/tile.png' where terrainid = 7
update dbo.rad_terrain set TerrainName = 'Wooden Bridge', imageurl = 'terrain/wooden-bridge.png' where terrainid = 8
update dbo.rad_terrain set imageurl = 'terrain/road.png' where terrainid = 9
go

insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Desert Mountain Range', 5, 0, 'terrain/desert-range.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Dirt', 5, 0, 'terrain/dirt.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Forest', 5, 0, 'terrain/forest.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Spine Mountain', 5, 0, 'terrain/spine-mountain.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Mountain Range', 5, 0, 'terrain/range.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Sea', 5, 0, 'terrain/sea.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Snow Capped Mountain Range', 5, 0, 'terrain/snow-range.png')
insert into dbo.rad_terrain (WorldId, TerrainName, WalkType, Color, ImageUrl)  values (1, 'Stone Bridge', 5, 0, 'terrain/stone-bridge.png')
go

update dbo.rad_FileUpdates set LastUpdateDate = getdate() where FileUpdateName='terrain'
go



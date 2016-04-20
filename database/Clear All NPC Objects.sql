-- NPC Objects
delete from rad_objects where ownerobjectid in (select objectid from rad_objects where ownerobjectid in (select objectid from rad_objects where typeid in (43,44,45,46,61,64)))
delete from rad_objects where ownerobjectid in (select objectid from rad_objects where typeid in (43,44,45,46,61,64))



delete from rad_Objects where OwnerObjectId in (
	select ObjectId from rad_Objects where OwnerObjectId in (
	select ObjectId from rad_Objects where TypeId in (39,40,41,42)
	)
	)

delete from rad_Objects where OwnerObjectId in (
	select ObjectId from rad_Objects where TypeId in (39,40,41,42)
	)
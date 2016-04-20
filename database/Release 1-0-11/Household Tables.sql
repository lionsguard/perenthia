--=================================================================================================
-- Households
--=================================================================================================
create table dbo.rad_Households
(
    HouseholdId         int not null identity(2580,1) primary key clustered,
    WorldId             int not null,
    HouseholdName       nvarchar(20) not null,
    ImageUri            nvarchar(256) null,
    Properties          xml not null, -- Motto, Description, RequiresDues, DuesTerm
    DateCreated         datetime null default(getdate())
)
go
alter table dbo.rad_Households add constraint FK_rad_Households_WorldId foreign key (WorldId) references dbo.rad_Worlds(WorldId)
go
create primary xml index ix_rad_Households_Properties on dbo.rad_Households(Properties)
go
create xml index ix_rad_Households_Properties_Path on dbo.rad_Households(Properties) using xml index ix_rad_Households_Properties for value
go
create xml index ix_rad_Households_Properties_Value on dbo.rad_Households(Properties) using xml index ix_rad_Households_Properties for path
go
create unique index ix_rad_Households_HouseholdName on dbo.rad_Households(WorldId, HouseholdName)
go



--=================================================================================================
-- HouseholdRanks
--=================================================================================================
create table dbo.rad_HouseholdRanks
(
    RankId                  int not null identity(1,1) primary key clustered,
    WorldId                 int not null,
    HouseholdId             int null, -- Allow null for heads of household ranks
    RankName                nvarchar(20) not null,
    RankOrder               int not null,
    ImageUri                nvarchar(256) null,
    [Permissions]           int not null, -- Bit Mask of permission values
    TitleMale               nvarchar(20) null,
    TitleFemale             nvarchar(20) null,
    RequiredMemberCount     int null default(0), -- Required for head of household title
    EmblemCost              int null default(0) -- Required for head of household title
)
go
alter table dbo.rad_HouseholdRanks add constraint FK_rad_HouseholdRanks_WorldId foreign key (WorldId) references dbo.rad_Worlds(WorldId)
go
alter table dbo.rad_HouseholdRanks add constraint FK_rad_HouseholdRanks_HouseholdId foreign key (HouseholdId) references dbo.rad_Households(HouseholdId)
go
create unique index ix_rad_HouseholdRanks_RankName on dbo.rad_HouseholdRanks(HouseholdId, RankName)
go


insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Household',1,'',0,'','',5,2)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Club',2,'',0,'','',15,5)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Clan',2,'',0,'Chieftain','Chieftain',25,10)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Guild',3,'',0,'Master','Mistress',50,15)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Manor',4,'',0,'Lord','Lady',100,20)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Shire',5,'',0,'Reeve','Reeve',150,30)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Township',6,'',0,'Mayor','Mayor',300,40)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'City',7,'',0,'Governor','Governess',800,50)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Barony',8,'',0,'Baron','Baroness',1500,65)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Viscounty',9,'',0,'Viscount','Viscountess',2500,80)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'County',10,'',0,'Count','Countess',5000,100)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'March',11,'',0,'Marquis','Marchioness',8000,125)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Duchy',12,'',0,'Duke','Duchess',12000,250)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Principality',13,'',0,'Prince','Princess',25000,500)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Realm',14,'',0,'King','Queen',50000,1000)
insert into dbo.rad_HouseholdRanks (WorldId,HouseholdId,RankName,RankOrder,ImageUri,[Permissions],TitleMale,TitleFemale,RequiredMemberCount,EmblemCost) values (1,NULL,'Empire',15,'',0,'Emperor','Empress',100000,2000)
go



--=================================================================================================
-- HouseholdRelationTypes
--=================================================================================================
create table dbo.rad_HouseholdRelationTypes
(
    RelationTypeId          int not null primary key clustered,
    RelationTypeName        nvarchar(32)
)
go
insert into dbo.rad_HouseholdRelationTypes (RelationTypeId, RelationTypeName) values (1, 'Neurtal')
insert into dbo.rad_HouseholdRelationTypes (RelationTypeId, RelationTypeName) values (2, 'Alliance')
insert into dbo.rad_HouseholdRelationTypes (RelationTypeId, RelationTypeName) values (3, 'Hostile')
go



--=================================================================================================
-- HouseholdRelations
--=================================================================================================
create table dbo.rad_HouseholdRelations
(
    PrimaryHouseholdId      int not null,
    SecondaryHouseholdId    int not null,
    RelationTypeId          int not null
)
go
alter table dbo.rad_HouseholdRelations add constraint PK_rad_HouseholdRelations primary key (PrimaryHouseholdId, SecondaryHouseholdId)
go
alter table dbo.rad_HouseholdRelations add constraint FK_rad_HouseholdRelations_PrimaryHouseholdId foreign key (PrimaryHouseholdId) references dbo.rad_Households(HouseholdId)
go
alter table dbo.rad_HouseholdRelations add constraint FK_rad_HouseholdRelations_SecondaryHouseholdId foreign key (SecondaryHouseholdId) references dbo.rad_Households(HouseholdId)
go
alter table dbo.rad_HouseholdRelations add constraint FK_rad_HouseholdRelations_RelationTypeId foreign key (RelationTypeId) references dbo.rad_HouseholdRelationTypes(RelationTypeId)
go



--=================================================================================================
-- Household Armory
--=================================================================================================
create table dbo.rad_HouseholdArmory
(
    HouseholdArmoryId       int not null identity(1,1) primary key clustered,
    HouseholdId             int not null,
    ObjectId                int not null,
    AddedByPlayerId         int not null,
    DateAdded               datetime null default(getdate())
)
go
alter table dbo.rad_HouseholdArmory add constraint FK_rad_HouseholdArmory_HouseholdId foreign key (HouseholdId) references dbo.rad_Households(HouseholdId)
go
alter table dbo.rad_HouseholdArmory add constraint FK_rad_HouseholdArmory_ObjectId foreign key (ObjectId) references dbo.rad_Objects(ObjectId)
go
alter table dbo.rad_HouseholdArmory add constraint FK_rad_HouseholdArmory_AddedByPlayerId foreign key (AddedByPlayerId) references dbo.rad_Players(PlayerId)
go


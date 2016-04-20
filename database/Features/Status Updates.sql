--=================================================================================================
-- Status Updates
--=================================================================================================
create table dbo.rad_StatusUpdates
(
    StatusUpdateId      bigint not null identity(1,1) primary key clustered,
    PlayerId            int not null,
    StatusUpdateText    nvarchar(140) not null,
    DateCreated         datetime null default(getdate()),
    DateDeleted         datetime null
)
go
alter table dbo.rad_StatusUpdates add constraint FK_rad_StatusUpdates_PlayerId foreign key (PlayerId) references dbo.rad_Players(PlayerId)
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_StatusUpdates_Add
(
    @PlayerId           int,
    @StatusUpdateText   nvarchar(140)
)
AS
BEGIN
    INSERT INTO dbo.rad_StatusUpdates (PlayerId, StatusUpdateText) VALUES (@PlayerId, @StatusUpdateText)
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_StatusUpdates_Delete
(
    @StatusUpdateId     bigint
)
AS
BEGIN
    UPDATE dbo.rad_StatusUpdates SET DateDeleted = GETDATE() WHERE StatusUpdateId = @StatusUpdateId
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_StatusUpdates_Get
(
    @PlayerId           int,
    @StartIndex         int,
    @MaxRows            int
)
AS
BEGIN
    SET @StartIndex = @StartIndex + 1
    
    SELECT
        ROW_NUMBER() as RowNum,
        StatusUpdateId,
        StatusUpdateText,
        DateCreated
    FROM
        dbo.rad_StatusUpdates
    WHERE
        PlayerId = @PlayerId
        AND DateDeleted IS NULL
END
go
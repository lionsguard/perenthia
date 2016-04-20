--=================================================================================================
-- In Game Messaging
--=================================================================================================
create table dbo.rad_Messages
(
    MessageId           bigint not null identity(1,1) primary key clustered,
    ReplyToMessageId    bigint null,
    SenderPlayerId      int not null,
    RecipientPlayerId   int not null,
    Subject             nvarchar(128) not null,
    MessageText         nvarchar(max) not null,
    DateAdded           datetime null default(getdate()),
    DateDeleted         datetime null,
    IsRead              bit null default(0),
    IsDeleted           bit null default(0),
)
go
alter table dbo.rad_Messages add constraint FK_rad_Messages_ReplyToMessageId foreign key (ReplyToMessageId) references dbo.rad_Messages(MessageId)
go
alter table dbo.rad_Messages add constraint FK_rad_Messages_SenderPlayerId foreign key (SenderPlayerId) references dbo.rad_Players(PlayerId)
go
alter table dbo.rad_Messages add constraint FK_rad_Messages_RecipientPlayerId foreign key (RecipientPlayerId) references dbo.rad_Players(PlayerId)
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_Add
(
    @SenderPlayerId         int,
    @RecipientPlayerId      int,
    @Subject                nvarchar(128),
    @MessageText            nvarchar(max),
    @ReplyToMessageId       bigint = null,
    @MessageId              bigint output
)
AS
BEGIN
    IF @ReplyToMessageId = 0
        SET @ReplyToMessageId = NULL

    INSERT INTO dbo.rad_Messages
    (
        ReplyToMessageId,
        SenderPlayerId,
        RecipientPlayerId,
        Subject,
        MessageText
    )
    VALUES
    (
        @ReplyToMessageId,
        @SenderPlayerId,
        @RecipientPlayerId,
        @Subject,
        @MessageText
    )
    SET @MessageId = SCOPE_IDENTITY()
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_Read
(
    @MessageId              bigint
)
AS
BEGIN
    UPDATE dbo.rad_Messages SET IsRead = 1 WHERE MessageId = @MessageId
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_Delete
(
    @MessageId              bigint
)
AS
BEGIN
    UPDATE dbo.rad_Messages SET IsDeleted = 1, DateDeleted = GETDATE() WHERE MessageId = @MessageId
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_Get
(
    @RecipientPlayerId      int
)
AS
BEGIN
    SELECT * FROM dbo.rad_Messages WHERE RecipientPlayerId = @RecipientPlayerId
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_GetSent
(
    @SenderPlayerId         int
)
AS
BEGIN
    SELECT * FROM dbo.rad_Messages WHERE SenderPlayerId = @SenderPlayerId
END
go


--=================================================================================================
CREATE PROCEDURE dbo.rad_Messages_GetDeleted
(
    @RecipientPlayerId      int
)
AS
BEGIN
    SELECT * FROM dbo.rad_Messages WHERE RecipientPlayerId = @RecipientPlayerId AND IsDeleted = 1
END
go
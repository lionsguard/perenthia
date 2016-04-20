--=================================================================================================
-- Player Friends
--=================================================================================================
create table dbo.rad_PlayerFriends
(
    PlayerId            int not null,
    FriendPlayerId      int not null,
    DateAdded           datetime null default(getdate()),
    FriendRank          int not null
)
go
alter table dbo.rad_PlayerFriends add constraint PK_rad_PlayerFriends primary key (PlayerId,FriendPlayerId)
go
alter table dbo.rad_PlayerFriends add constraint FK_rad_PlayerFriends_PlayerId foreign key (PlayerId) references dbo.rad_Playerss(PlayerId)
go
alter table dbo.rad_PlayerFriends add constraint FK_rad_PlayerFriends_FriendPlayerId foreign key (FriendPlayerId) references dbo.rad_Playerss(PlayerId)
go



--=================================================================================================
CREATE PROCEDURE dbo.rad_PlayerFriends_Add
(
    @PlayerId           int,
    @FriendPlayerId     int
)
AS
BEGIN
    IF NOT EXISTS(SELECT 1 FROM dbo.rad_PlayerFriends WHERE PlayerId = @PlayerId AND FriendPlayerId = @FriendPlayerId)
    BEGIN
        DECLARE @FriendRank int
        SELECT @FriendRank = FriendRank FROM dbo.rad_PlayerFriends WHERE PlayerId = @PlayerId AND FriendPlayerId = @FriendPlayerId
        IF @FriendRank IS NULL
            SET @FriendRank = 0
        ELSE
            SET @FriendRank = @FriendRank + 1
            
        INSERT INTO dbo.rad_PlayerFriends (PlayerId, FriendPlayerId, FriendRank) VALUES (@PlayerId, @FriendPlayerId, @FriendRank)
    END
END
go

--=================================================================================================
CREATE PROCEDURE dbo.rad_PlayerFriends_Delete
(
    @PlayerId           int,
    @FriendPlayerId     int
)
AS
BEGIN
    DELETE FROM dbo.rad_PlayerFriends WHERE PlayerId = @PlayerId AND FriendPlayerId = @FriendPlayerId 
END
go

--=================================================================================================
CREATE PROCEDURE dbo.rad_PlayerFriends_Rank
(
    @FriendPlayerId     int,
    @FriendRank         int
)
AS
BEGIN
    UPDATE dbo.rad_PlayerFriends SET FriendRank = FriendRank + 1 WHERE PlayerId = @PlayerId AND FriendPlayerId <> @FriendPlayerId AND FriendRank >= @FriendRank
    
    UPDATE dbo.rad_PlayerFriends SET FriendRank = @FriendRank WHERE PlayerId = @PlayerId AND FriendPlayerId = @FriendPlayerId
END
go

--=================================================================================================
CREATE PROCEDURE dbo.rad_PlayerFriends_Get
(
    @PlayerId           int
)
AS
BEGIN
    SELECT
        p.*,
        pf.FriendRank
    FROM 
        dbo.rad_PlayerFriends pf
        INNER JOIN dbo.rad_vw_Players p ON pf.FriendPlayerId = p.PlayerId
    WHERE 
        pf.PlayerId = @PlayerId 
    ORDER BY pf.FriendRank
END
go
--=================================================================================================
-- MarketItems
--=================================================================================================
create table dbo.rad_MarketItems
(
    MarketItemId            bigint not null identity(1,1) primary key clustered,
    WorldId                 int not null,
    ObjectId                int not null,
    SellerPlayerId          int not null,
    BuyerPlayerId           int null,
    TradeTemplateId         int null,
    GoldCost                int not null,
    EmblemCost              int not null,
    DateAdded               datetime null default(getdate())
    DateExpires             datetime not null,
    DateSold                datetime null,
    ViewCount               int not null
)
go
alter table dbo.rad_MarketItems add constraint FK_rad_MarketItems_WorldId foreign key (WorldId) references dbo.rad_Worlds(WorldId)
go
alter table dbo.rad_MarketItems add constraint FK_rad_MarketItems_ObjectId foreign key (ObjectId) references dbo.rad_Objects(ObjectId)
go
alter table dbo.rad_MarketItems add constraint FK_rad_MarketItems_SellerPlayerId foreign key (SellerPlayerId) references dbo.rad_Players(PlayerId)
go
alter table dbo.rad_MarketItems add constraint FK_rad_MarketItems_TradeTemplateId foreign key (ObjectId) references dbo.rad_Objects(ObjectId)
go
alter table dbo.rad_MarketItems add constraint FK_rad_MarketItems_BuyerPlayerId foreign key (BuyerPlayerId) references dbo.rad_Players(PlayerId)
go


-- FindItems
CREATE PROCEDURE dbo.rad_MarketItems_FindItems
(
    @WorldName      nvarchar(64),
    @Query          nvarchar(256),
    @TypeList       xml
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds @WorldName = @WorldName, @WorldId = @WorldId OUT
    -- Use the object by type procedure to build this out...
END
go


-- GetItems
CREATE PROCEDURE dbo.rad_MarketItems_GetItems
(
    @WorldName      nvarchar(64),
    @TypeList       xml
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds @WorldName = @WorldName, @WorldId = @WorldId OUT
    -- Use the object by type procedure to build this out...
END
go


-- GetRecentItems
CREATE PROCEDURE dbo.rad_MarketItems_GetRecentItems
(
    @WorldName      nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds @WorldName = @WorldName, @WorldId = @WorldId OUT

    SELECT TOP 10 * FROM dbo.rad_MarketItems WHERE DateSold IS NULL ORDER BY DateAdded DESC
END
go


-- GetHotItems
CREATE PROCEDURE dbo.rad_MarketItems_GetHotItems
(
    @WorldName      nvarchar(64)
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds @WorldName = @WorldName, @WorldId = @WorldId OUT

    SELECT TOP 10 * FROM dbo.rad_MarketItems WHERE DateSold IS NULL ORDER BY ViewCount ASC
END
go


-- GetItem
CREATE PROCEDURE dbo.rad_MarketItems_GetItem
(
    @MarketItemId   bigint
)
AS
BEGIN
    SELECT * FROM dbo.rad_MarketItems WHERE MarketItemId = @MarketItemId
END
go


-- BuyItem (Maybe this needs to be app specific instead of db specific??)
CREATE PROCEDURE dbo.rad_MarketItems_BuyItem
(
    @MarketItemId       bigint,
    @BuyerPlayerId      int,
    @TradeObjectId      int = null,
    @Success            bit output
)
AS
BEGIN
    SET @Success = 0
    
    DECLARE @ObjectId int
    SELECT @ObjectId = ObjectId FROM dbo.rad_MarketItems WHERE MarketItemId = @MarketItemId AND DateSold IS NULL
    
    IF @ObjectId > 0
    BEGIN
        -- Update MarketItem to SOLD.
        UPDATE 
            dbo.rad_MarketItems
        SET
            DateSold        = GETDATE(),
            BuyerPlayerId   = @BuyerPlayerId
        WHERE
            MarketItemId = @MarketItemId
            
        -- Update the object owner to the buyer.
        UPDATE 
            dbo.rad_Objects
        SET
            OwnerPlayerId   = @BuyerPlayerId
        WHERE
            ObjectId = @ObjectId
            
        IF @@ERROR = 0
        BEGIN
            COMMIT TRAN
            @Success = 1
        END
        ELSE
        BEGIN
            ROLLBACK TRAN
        END
    END
END
go


-- SaveItem
CREATE PROCEDURE dbo.rad_MarketItems_SaveItem
(
    @WorldName          nvarchar(64),
    @ObjectId           int,
    @OwnerPlayerId      int,
    @TradeTemplateId    int = null,
    @GoldCost           int = 0,
    @EmblemCost         int = 0,
    @DateExpires        datetime,
    @MarketItemId       bigint output
)
AS
BEGIN
    DECLARE @WorldId int
    EXEC dbo.rad_Worlds @WorldName = @WorldName, @WorldId = @WorldId OUT
    
    IF @MarketItemId > 0
    BEGIN
        UPDATE
            dbo.rad_MarketItems
        SET
            TradeTemplateId     = @TradeTemplateId,
            GoldCost            = @GoldCost,
            EmblemCost          = @EmblemCost,
            DateExpires         = @DateExpires
        WHERE
            MarketItemId = @MarketItemId
    END
    ELSE
    BEGIN
        BEGIN TRAN
        
        INSERT INTO dbo.rad_MarketItems
        (
            WorldId,
            ObjectId,
            OwnerPlayerId,
            TradeTemplateId,
            GoldCost,
            EmblemCost,
            DateExpires,
            ViewCount
        )
        VALUES
        (
            @WorldId,
            @ObjectId,
            @OwnerPlayerId,
            @TradeTemplateId,
            @GoldCost,
            @EmblemCost,
            @DateExpires,
            0
        )
        SET @MarketItemId = SCOPE_IDENTITY()
        
        UPDATE dbo.rad_Objects SET OwnerPlayerId = NULL WHERE ObjectId = @ObjectId
            
        IF @@ERROR = 0
            COMMIT TRAN
        ELSE
            ROLLBACK TRAN
    END
END
go


-- RemoveItem
CREATE PROCEDURE dbo.rad_MarketItems_RemoveItem
(
    @MarketItemId       bigint
)
AS
BEGIN
    DECLARE @ObjectId int,
            @SellerPlayerId int
    SELECT @ObjectId = ObjectId, @SellerPlayerId = SellerPlayerId FROM dbo.rad_MarketItems WHERE MarketItemId = @MarketItemId AND DateSold IS NULl

    IF @ObjectId > 0
    BEGIN
        BEGIN TRAN
        
        DELETE FROM dbo.rad_MarketItems WHERE MarketItemId = @MarketItemId
        
        UPDATE dbo.rad_Objects SET OwnerPlayerId = @SellerPlayerId WHERE ObjectId = @ObjectId
        
        IF @@ERROR = 0
            COMMIT TRAN
        ELSE
            ROLLBACK TRAN
    END
END
go


-- GetMerchandise
CREATE PROCEDURE dbo.rad_MarketItems_GetMerchandise
(
    @SellerPlayerId      int
)
AS
BEGIN
    SELECT * FROM dbo.rad_MarketItems WHERE SellerPlayerId = @SellerPlayerId AND DateSold IS NULL
END
go


-- GetHistory
CREATE PROCEDURE dbo.rad_MarketItems_GetHistory
(
    @SellerPlayerId     int
)
AS
BEGIN
    SELECT * FROM dbo.rad_MarketItems WHERE SellerPlayerId = @SellerPlayerId AND DateSold IS NOT NULL
END
go


-- PlaceBid
-- CREATE PROCEDURE dbo.rad_MarketItems_PlaceBid
-- (
-- 
-- )
-- AS
-- BEGIN
-- 
-- END
-- go
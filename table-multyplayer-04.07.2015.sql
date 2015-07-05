CREATE TABLE [dbo].[MultiplayerGames] (
    [Id]     INT           IDENTITY (1, 1) NOT NULL,
    [gameId] VARCHAR (250)           NOT NULL,
    [userId1] VARCHAR (150) NOT NULL,
    [userId2] VARCHAR (150) NULL,
    [winner] VARCHAR (150) NULL,
    [startPage] TEXT NULL,
    [points] INT           NULL ,
    [timeElapsed] INT           DEFAULT 0 NOT NULL,
    [category]    VARCHAR (255) DEFAULT 'Sport' NOT NULL,
    [dateOfGame]  DATETIME    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
ALTER TABLE [dbo].[Points]
    ADD [timeElapsed] INT           DEFAULT 0 NOT NULL,
        [category]    VARCHAR (255) DEFAULT 'Sport' NOT NULL,
        [dateOfGame]  DATETIME    NULL;
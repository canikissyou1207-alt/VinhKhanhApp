CREATE TABLE [dbo].[UserPositions] (
    [DeviceId]        NVARCHAR (450) NOT NULL,
    [Latitude]        FLOAT          NOT NULL,
    [Longitude]       FLOAT          NOT NULL,
    [DeviceModel]     NVARCHAR (MAX) NULL,
    [CurrentLanguage] NVARCHAR (MAX) NULL,
    [LastUpdate]      DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_UserPositions] PRIMARY KEY ([DeviceId])
);
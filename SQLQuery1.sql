CREATE TABLE UserPositions (
    DeviceId NVARCHAR(450) NOT NULL PRIMARY KEY,
    Latitude FLOAT NOT NULL,
    Longitude FLOAT NOT NULL,
    DeviceModel NVARCHAR(MAX) NULL,
    CurrentLanguage NVARCHAR(MAX) NULL,
    LastUpdate DATETIME2 NOT NULL
);
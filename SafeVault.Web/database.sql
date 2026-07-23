IF DB_ID('SafeVaultDb') IS NULL
BEGIN
    CREATE DATABASE SafeVaultDb;
END;
GO

USE SafeVaultDb;
GO

IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserID INT IDENTITY(1,1) PRIMARY KEY,
        Username VARCHAR(100) NOT NULL,
        Email VARCHAR(100) NOT NULL,

        CONSTRAINT UQ_Users_Username UNIQUE (Username),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END;
GO
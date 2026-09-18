/* ==========================================================================
   Enhanzer Full Stack Assignment - Database Script
   Database : PurchaseBillDb
   Table    : Location_Details
   ========================================================================== */

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'PurchaseBillDb')
BEGIN
    CREATE DATABASE PurchaseBillDb;
END
GO

USE PurchaseBillDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Location_Details')
BEGIN
    CREATE TABLE dbo.Location_Details
    (
        Id            INT IDENTITY(1,1)  NOT NULL PRIMARY KEY,
        Location_Code NVARCHAR(50)       NOT NULL,
        Location_Name NVARCHAR(200)      NOT NULL,
        Username      NVARCHAR(200)      NULL,       -- which user session this location came from
        Created_At    DATETIME2          NOT NULL DEFAULT (SYSUTCDATETIME())
    );

    CREATE UNIQUE INDEX UX_Location_Details_Code_Username
        ON dbo.Location_Details (Location_Code, Username);
END
GO

/* Optional: table to persist submitted purchase bills (kept simple, in-memory
   on the frontend also works for the assignment, but this gives a full
   backend round trip if you want to demo it). */
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Purchase_Bill_Item')
BEGIN
    CREATE TABLE dbo.Purchase_Bill_Item
    (
        Id             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Item_Name      NVARCHAR(100)     NOT NULL,
        Batch          NVARCHAR(200)     NOT NULL,   -- Location_Name selected
        Standard_Cost  DECIMAL(18,2)     NOT NULL,
        Standard_Price DECIMAL(18,2)     NOT NULL,
        Margin         DECIMAL(18,2)     NOT NULL,
        Qty            DECIMAL(18,2)     NOT NULL,
        Free_Qty       DECIMAL(18,2)     NOT NULL DEFAULT 0,
        Discount       DECIMAL(18,2)     NOT NULL DEFAULT 0,
        Total_Cost     DECIMAL(18,2)     NOT NULL,
        Total_Selling  DECIMAL(18,2)     NOT NULL,
        Created_At     DATETIME2         NOT NULL DEFAULT (SYSUTCDATETIME())
    );
END
GO

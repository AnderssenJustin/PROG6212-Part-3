IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Username] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [Role] nvarchar(50) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [HourlyRate] float NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [LastLogin] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);

CREATE TABLE [Claims] (
    [ClaimId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [HoursWorked] float NOT NULL,
    [Notes] nvarchar(500) NULL,
    [DocumentPath] nvarchar(max) NULL,
    [Status] nvarchar(50) NOT NULL,
    [SubmittedDate] datetime2 NOT NULL,
    [CoordinatorApprovedDate] datetime2 NULL,
    [ManagerApprovedDate] datetime2 NULL,
    [RejectionReason] nvarchar(500) NULL,
    [IsDocumentValid] bit NOT NULL,
    [IsAmountValid] bit NOT NULL,
    [IsHoursValid] bit NOT NULL,
    [ApprovedByCoordinator] nvarchar(100) NULL,
    [ApprovedByManager] nvarchar(100) NULL,
    CONSTRAINT [PK_Claims] PRIMARY KEY ([ClaimId]),
    CONSTRAINT [FK_Claims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'CreatedDate', N'Email', N'FirstName', N'HourlyRate', N'IsActive', N'LastLogin', N'LastName', N'PasswordHash', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [CreatedDate], [Email], [FirstName], [HourlyRate], [IsActive], [LastLogin], [LastName], [PasswordHash], [Role], [Username])
VALUES (1, '2025-11-16T00:00:00.0000000', N'hr@university.edu', N'HR', 0.0E0, CAST(1 AS bit), NULL, N'Administrator', N'HR@2025', N'HR', N'hradmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'CreatedDate', N'Email', N'FirstName', N'HourlyRate', N'IsActive', N'LastLogin', N'LastName', N'PasswordHash', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;

CREATE INDEX [IX_Claims_UserId] ON [Claims] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251121101706_InitialSqlMigration', N'9.0.11');

COMMIT;
GO


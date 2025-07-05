-- Script de création de la base de données EducProject
-- Exécutez ce script dans SQL Server Management Studio ou via sqlcmd

-- Créer la base de données si elle n'existe pas
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EducProjectDB')
BEGIN
    CREATE DATABASE EducProjectDB;
END
GO

USE EducProjectDB;
GO

-- Créer la table Users si elle n'existe pas
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Username] [nvarchar](50) NOT NULL,
        [Email] [nvarchar](100) NOT NULL,
        [PasswordHash] [nvarchar](max) NOT NULL,
        [FirstName] [nvarchar](50) NOT NULL,
        [LastName] [nvarchar](50) NOT NULL,
        [DateOfBirth] [datetime2](7) NOT NULL,
        [Age] [int] NOT NULL,
        [Role] [int] NOT NULL,
        [Level] [int] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [LastLoginAt] [datetime2](7) NULL,
        [ParentId] [int] NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Créer les index uniques
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
END
GO

-- Créer la contrainte de clé étrangère pour ParentId
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Users_ParentId')
BEGIN
    ALTER TABLE [dbo].[Users] 
    ADD CONSTRAINT [FK_Users_Users_ParentId] 
    FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Users] ([Id]);
END
GO

-- Insérer l'utilisateur admin par défaut
IF NOT EXISTS (SELECT * FROM [dbo].[Users] WHERE Username = 'admin')
BEGIN
    INSERT INTO [dbo].[Users] (
        [Username], 
        [Email], 
        [PasswordHash], 
        [FirstName], 
        [LastName], 
        [DateOfBirth], 
        [Age], 
        [Role], 
        [Level], 
        [CreatedAt]
    ) VALUES (
        'admin',
        'admin@educproject.com',
        '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- 'admin123' hashé avec BCrypt
        'Admin',
        'User',
        '1990-01-01',
        33,
        4, -- Admin role
        3, -- Advanced level
        GETUTCDATE()
    );
END
GO

PRINT 'Base de données EducProjectDB créée avec succès!';
PRINT 'Utilisateur admin créé avec les identifiants:';
PRINT 'Username: admin';
PRINT 'Password: admin123';
GO 
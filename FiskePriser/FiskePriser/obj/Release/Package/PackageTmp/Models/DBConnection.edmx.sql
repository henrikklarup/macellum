
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/08/2014 19:50:38
-- Generated from EDMX file: C:\Users\henri_000\Documents\Visual Studio 2013\Projects\FiskePriser\FiskePriser\Models\DBConnection.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_FiskArter]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fisks] DROP CONSTRAINT [FK_FiskArter];
GO
IF OBJECT_ID(N'[dbo].[FK_FiskHavne]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Fisks] DROP CONSTRAINT [FK_FiskHavne];
GO
IF OBJECT_ID(N'[dbo].[FK_BenzinPrisHavne]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BenzinPris] DROP CONSTRAINT [FK_BenzinPrisHavne];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Fisks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Fisks];
GO
IF OBJECT_ID(N'[dbo].[Arters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Arters];
GO
IF OBJECT_ID(N'[dbo].[Havns]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Havns];
GO
IF OBJECT_ID(N'[dbo].[BenzinPris]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BenzinPris];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Fisks'
CREATE TABLE [dbo].[Fisks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AvgPrice] nvarchar(max)  NOT NULL,
    [MaxPrice] nvarchar(max)  NOT NULL,
    [Sort] nvarchar(max)  NOT NULL,
    [Amount] nvarchar(max)  NOT NULL,
    [Dato] nvarchar(max)  NOT NULL,
    [Arters_Id] int  NOT NULL,
    [Havne_Id] int  NOT NULL
);
GO

-- Creating table 'Arters'
CREATE TABLE [dbo].[Arters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Navn] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Havns'
CREATE TABLE [dbo].[Havns] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Navn] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BenzinPris'
CREATE TABLE [dbo].[BenzinPris] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] nvarchar(max)  NOT NULL,
    [Dato] nvarchar(max)  NOT NULL,
    [Havne_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Fisks'
ALTER TABLE [dbo].[Fisks]
ADD CONSTRAINT [PK_Fisks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Arters'
ALTER TABLE [dbo].[Arters]
ADD CONSTRAINT [PK_Arters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Havns'
ALTER TABLE [dbo].[Havns]
ADD CONSTRAINT [PK_Havns]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BenzinPris'
ALTER TABLE [dbo].[BenzinPris]
ADD CONSTRAINT [PK_BenzinPris]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Arters_Id] in table 'Fisks'
ALTER TABLE [dbo].[Fisks]
ADD CONSTRAINT [FK_FiskArter]
    FOREIGN KEY ([Arters_Id])
    REFERENCES [dbo].[Arters]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FiskArter'
CREATE INDEX [IX_FK_FiskArter]
ON [dbo].[Fisks]
    ([Arters_Id]);
GO

-- Creating foreign key on [Havne_Id] in table 'Fisks'
ALTER TABLE [dbo].[Fisks]
ADD CONSTRAINT [FK_FiskHavne]
    FOREIGN KEY ([Havne_Id])
    REFERENCES [dbo].[Havns]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FiskHavne'
CREATE INDEX [IX_FK_FiskHavne]
ON [dbo].[Fisks]
    ([Havne_Id]);
GO

-- Creating foreign key on [Havne_Id] in table 'BenzinPris'
ALTER TABLE [dbo].[BenzinPris]
ADD CONSTRAINT [FK_BenzinPrisHavne]
    FOREIGN KEY ([Havne_Id])
    REFERENCES [dbo].[Havns]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_BenzinPrisHavne'
CREATE INDEX [IX_FK_BenzinPrisHavne]
ON [dbo].[BenzinPris]
    ([Havne_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
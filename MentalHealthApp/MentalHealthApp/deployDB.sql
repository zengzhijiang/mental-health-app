USE [master]
CREATE DATABASE MentalHealthDB;
GO
CREATE TABLE [MentalHealthDB].[dbo].[Users] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Username]    NVARCHAR (50) NULL,
    [Password]    NVARCHAR (50) NULL,
    [First]       NVARCHAR (50) NULL,
    [Last]        NVARCHAR (50) NULL,
    [Email]       NVARCHAR (50) NULL,
    [AccountType] INT           DEFAULT ((-1)) NOT NULL,
    [Avatar]      VARCHAR (50)  DEFAULT ('') NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

INSERT INTO [MentalHealthDB].[dbo].[Users] (Username ,Password,First,Last,Email,AccountType,Avatar) VALUES ('teacher', 'password','teacher','teacher','teacher@teacher.com', 2, ''); 
INSERT INTO [MentalHealthDB].[dbo].[Users] (Username ,Password,First,Last,Email,AccountType,Avatar) VALUES ('child', 'password','child','child','child@child.com', 0, 'hippo'); 


CREATE TABLE [MentalHealthDB].[dbo].[Relations] (
    [OwnerId]     INT NOT NULL,
    [AssociateId] INT NOT NULL
);

CREATE TABLE [MentalHealthDB].[dbo].[Invites] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [LinkCode] VARCHAR (50) NOT NULL,
    [Inviter]  INT          NOT NULL,
    [Date]     DATETIME     DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [MentalHealthDB].[dbo].[Data] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [OwnerId]  INT          DEFAULT ((-1)) NULL,
    [Emotion]  VARCHAR (50) DEFAULT ('None') NULL,
    [Type]     VARCHAR (50) DEFAULT ('None') NULL,
    [Response] VARCHAR (50) DEFAULT ('None') NULL,
    [Date]     DATETIME     DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
USE [MentalHealthDB]
GO
CREATE PROCEDURE [dbo].[spRegisterUser]
	@userName varchar(50),
	@password varchar(50),
	@firstName varchar(50),
	@lastName varchar(50),
	@email nvarchar(50),
	@accountType int,
	@avatar varchar(50)

AS
	/*if !exists register and send confirmation email*/
	INSERT INTO [dbo].[Users]
		(Username ,Password,First,Last,Email,AccountType,Avatar) 
		VALUES (@userName, @password, @firstName, @lastName, @email, @accountType, @avatar);
	/*if already exists let it be known in the interface*/
RETURN 0
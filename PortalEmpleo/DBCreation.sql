CREATE DATABASE PortalEmpleo
GO

USE PortalEmpleo

CREATE TABLE [dbo].[Category] (
    [category_id]   INT            IDENTITY (1, 1) NOT NULL,
    [category_name] NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([category_id] ASC),
    UNIQUE NONCLUSTERED ([category_name] ASC)
);

CREATE TABLE [dbo].[Role] (
    [role_description] VARCHAR (30) NOT NULL,
    PRIMARY KEY CLUSTERED ([role_description] ASC)
);

CREATE TABLE [dbo].[Skills] (
    [skill_name] VARCHAR (40) NOT NULL,
    PRIMARY KEY CLUSTERED ([skill_name] ASC)
);

CREATE TABLE [dbo].[Users] (
    [user_id]                INT             IDENTITY (1, 1) NOT NULL,
    [user_name]              VARCHAR (30)    NOT NULL,
    [user_surname]           VARCHAR (30)    NOT NULL,
    [user_email]             NVARCHAR (150)  NOT NULL,
    [user_password]          VARCHAR (255)   NULL,
    [user_birth_date]        DATE            NOT NULL,
    [user_profile_img]       VARBINARY (MAX) NULL,
    [user_age]               INT             NULL,
    [role_description]       VARCHAR (30)    NOT NULL,
    [user_password_salt]     VARCHAR (255)   NOT NULL,
    [user_title_description] VARCHAR (200)   NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC),
    FOREIGN KEY ([role_description]) REFERENCES [dbo].[Role] ([role_description])
);

CREATE TABLE [dbo].[Companies] (
    [company_id]    INT            IDENTITY (1, 1) NOT NULL,
    [company_name]  VARCHAR (100)  NOT NULL,
    [company_email] NVARCHAR (150) NOT NULL,
    [description]   TEXT           NULL,
    PRIMARY KEY CLUSTERED ([company_id] ASC)
);

CREATE TABLE [dbo].[Posts] (
    [post_id]          INT             IDENTITY (1, 1) NOT NULL,
    [post_title]       VARCHAR (40)    NOT NULL,
    [post_description] TEXT            NOT NULL,
    [user_id]          INT             NOT NULL,
    [category_id]      INT             NULL,
    [post_image]       VARBINARY (MAX) NULL,
    [post_date]        DATE            NULL,
    PRIMARY KEY CLUSTERED ([post_id] ASC),
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([category_id]) REFERENCES [dbo].[Category] ([category_id])
);

CREATE TABLE [dbo].[users_skills] (
    [user_id]    INT          NULL,
    [skill_name] VARCHAR (40) NULL,
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([skill_name]) REFERENCES [dbo].[Skills] ([skill_name])
);

CREATE TABLE [dbo].[users_companies] (
    [user_id]    INT NULL,
    [company_id] INT NULL,
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([company_id]) REFERENCES [dbo].[Companies] ([company_id])
);


/*Crear procedimientos en una secuencia a parte*/

CREATE PROCEDURE [dbo].[LoginUser]
	@user_email nvarchar(150),
	@user_password nvarchar(16)
AS
	SELECT * FROM dbo.Users WHERE user_email = @user_email and user_password = @user_password
GO

CREATE PROCEDURE [dbo].[GetUserByEmail]
	@user_email nvarchar(150)
AS
	SELECT * FROM dbo.Users WHERE user_email = @user_email

INSERT INTO dbo.Role VALUES('Desempleado'),('Empresa')
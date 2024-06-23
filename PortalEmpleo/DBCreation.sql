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
    [skill_id]   INT IDENTITY,
    [skill_name] VARCHAR (40) NOT NULL,
    PRIMARY KEY CLUSTERED ([skill_id] ASC)
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
    [user_about_me]          TEXT            NULL,
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
    [post_id]            INT             IDENTITY (1, 1) NOT NULL,
    [post_title]         VARCHAR (40)    NOT NULL,
    [post_description]   TEXT            NOT NULL,
    [user_id]            INT             NOT NULL,
    [category_id]        INT             NULL,
    [post_image]         VARBINARY (MAX) NULL,
    [post_modified_date] DATETIME        NULL,
    [post_date]          DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([post_id] ASC),
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([category_id]) REFERENCES [dbo].[Category] ([category_id])
);

CREATE TABLE [dbo].[users_skills] (
    [user_id]  INT NULL,
    [skill_id] INT NULL,
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([skill_id]) REFERENCES [dbo].[Skills] ([skill_id])
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

INSERT INTO dbo.Category VALUES ('Tecnología');
INSERT INTO dbo.Category VALUES ('Deportes');
INSERT INTO dbo.Category VALUES ('Entretenimiento');
INSERT INTO dbo.Category VALUES ('Salud');
INSERT INTO dbo.Category VALUES ('Educación');
INSERT INTO dbo.Category VALUES ('Negocios');
INSERT INTO dbo.Category VALUES ('Moda');
INSERT INTO dbo.Category VALUES ('Viajes');
INSERT INTO dbo.Category VALUES ('Cocina');
INSERT INTO dbo.Category VALUES ('Arte');

INSERT INTO dbo.Skills VALUES ('Programación');
INSERT INTO dbo.Skills VALUES ('Diseño gráfico');
INSERT INTO dbo.Skills VALUES ('Marketing digital');
INSERT INTO dbo.Skills VALUES ('Gestión de proyectos');
INSERT INTO dbo.Skills VALUES ('Redes sociales');
INSERT INTO dbo.Skills VALUES ('Desarrollo web');
INSERT INTO dbo.Skills VALUES ('Inglés avanzado');
INSERT INTO dbo.Skills VALUES ('Finanzas');
INSERT INTO dbo.Skills VALUES ('Cocina gourmet');
INSERT INTO dbo.Skills VALUES ('Fotografía');
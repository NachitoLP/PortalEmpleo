CREATE DATABASE PortalEmpleo
GO

USE PortalEmpleo

CREATE TABLE Role (
	role_description varchar(30) PRIMARY KEY
)

CREATE TABLE Skills (
	skill_name varchar(40) PRIMARY KEY
)

CREATE TABLE Users (
	user_id int PRIMARY KEY IDENTITY,
	user_name varchar(30) NOT NULL,
	user_surname varchar(30) NOT NULL,
	user_email nvarchar(150) NOT NULL,
	user_password nvarchar(16) NOT NULL,
	user_birth_date date NOT NULL,
	user_profile_img VARBINARY(MAX),
	user_age int,
	role_description varchar(30) NOT NULL,
	FOREIGN KEY (role_description) REFERENCES Role(role_description)
)

CREATE TABLE Companies (
	company_id int PRIMARY KEY IDENTITY,
	company_name varchar(100) NOT NULL,
	company_email nvarchar(150) NOT NULL,
	description text
)

CREATE TABLE Posts (
	post_id INT PRIMARY KEY IDENTITY,
	post_title varchar(40) NOT NULL,
	post_description text NOT NULL,
	post_image varchar(255),
	user_id int NOT NULL,
	FOREIGN KEY (user_id) REFERENCES Users(user_id)
)

CREATE TABLE users_skills (
	user_id int,
	skill_name varchar(40),
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (skill_name) REFERENCES Skills(skill_name)
)

CREATE TABLE users_companies (
	user_id int,
	company_id int,
	FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (company_id) REFERENCES Companies(company_id)
)

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
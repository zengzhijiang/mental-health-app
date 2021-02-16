CREATE PROCEDURE [dbo].[spUpdateUser]
	@id int,
	@userName varchar(50),
	@password varchar(50),
	@firstName varchar(50),
	@lastName varchar(50),
	@email nvarchar(50),
	@accountType int,
	@avatar varchar(50)

AS

UPDATE  [dbo].[Users] SET 
	Username =  @userName,
	[Password] = @password,
	[First] = @firstName,
	[Last] = @lastName,
	Email = @email,
	AccountType = @accountType,
	Avatar = @avatar 
		WHERE 
	Id= @id
RETURN 0
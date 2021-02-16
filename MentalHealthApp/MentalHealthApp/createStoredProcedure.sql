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


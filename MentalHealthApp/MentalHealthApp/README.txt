Chase Guggenheim, Alan Kocher, Andrew Stone, Derek Spangler

To whom it may concern.
To set up the app database. Run the queries within 'deployDB.sql'.
This will create the 4 tables required for the app to work.

This will also create the users
	Username: teacher  ,   Password: password       --> teacher privilages
	Username: child    ,   Password: password		--> child privilages

The AccountType column on the table [dbo].[Users] represents their privilage type.
If you would like to change the users privilage type, it must be done manually on the database.

AccountType 0 = child
AccountType 2 = parent

You must also add the stored procedures to the DB.
To do this, run the query 'createStoredProcedure.sql'


TFS source code at:
chase-guggenheim.visualstudio.com/MentalHealth/MentalHealth
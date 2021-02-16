using System;
using System.Collections.Generic;      
using MentalHealthApp.Data;
using MentalHealthApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthApp.Controllers
{
    public class LoginController : Controller
    {

        private ApplicationDbContext _db;
        //https://msdn.microsoft.com/en-us/library/jj592907%28v=vs.113%29.aspx?f=255&MSPPError=-2147217396

        public LoginController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string u, string p)
        {
            // var str = "INSERT INTO [dbo].[Users] (Username ,Password,First,Last,Email,AccountType,Avatar) VALUES ('" + model.UserName + "', '" + model.Password + "','" + model.FirstName + "','" + model.LastName + "',NULL,0,NULL  );";
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Users WHERE Username Like '" + u + "' AND Password Like'" + p + "';"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    if (reader[2].Equals(p))
                    {

                        HttpContext.Session.SetSerializedObject("User",
                            value: new UserViewModel
                            {
                                Id = (int)reader[0],
                                UserName = (string)reader[1],
                                Password = null,
                                FirstName = (string)reader[3],
                                LastName = (string)reader[4],
                                Email = null,
                                AccountType = (int)reader[6],
                                Avatar = (string)reader[7]
                            });

                        return RedirectToAction("Index", "Diagnostic");
                    }
                }
                catch (Exception e)
                {

                }


            }
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }




        /*
         * account type
         *          -1 = unassigned
         *           1 = child/unattached
         *           2 = parent
         */
        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            if (!ModelState.IsValid || model.Password != model.RepeatPassword)
                return View(model);
            if (!model.Email.Contains("@") || !model.Email.Contains("."))
            {
                ModelState.AddModelError("Email", "Email does not contain '@' or '.'.");
                View(model);
            }

            try
            {
                //checks to see if the user exists
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Users WHERE Username Like '" + model.UserName + "';"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    reader.Read();

                    if (reader[1].Equals(model.UserName))
                    {
                        ModelState.AddModelError("Username", "Username already in use.");
                        return View(model);
                    }
                }
            }
            catch (Exception e)
            {

            }

            //Try-catch block is when elevated user creates an account it is parent to
            try
            {
                var user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
                if (!user.Equals(null) && user.AccountType == 2)
                {
                    //insert the user
                    _db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Users] (Username ,Password,First,Last,Email,AccountType,Avatar) VALUES ('" +
                        model.UserName + "', '" + model.Password + "','" + model.FirstName + "','" + model.LastName + "','" + model.Email + "', 0, '"+ model.Avatar +"'  );");

                    int userId = -1;
                    //reclaims the user using select statement
                    using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Users WHERE Username Like '" + model.UserName + "' AND Password Like'" + model.Password + "';"))
                    {
                        // Output rows.
                        var reader = datareader.DbDataReader;
                        reader.Read();

                        if (reader[2].Equals(model.Password))
                            userId = (int)reader[0];
                    }
                    //insert the relations
                    var relationString = "INSERT INTO [dbo].[Relations] (OwnerId , AssociateId) VALUES (" + user.Id + ", " + userId + ");";
                    _db.Database.ExecuteSqlCommand(relationString);

                }
            }
            catch (Exception e)
            {

            }

            //insert the user
            if (HttpContext.Session.GetDeserialized<UserViewModel>("User") == null)
            {
                var str = "EXEC [dbo].spRegisterUser  '" + model.UserName + "', '" + model.Password + "', '" + model.FirstName + "', '" + model.LastName + "', '" + model.Email + "', 0, '"+model.Avatar+"';";
                // var str = "INSERT INTO [dbo].[Users] (Username ,Password,First,Last,Email,AccountType,Avatar) VALUES ('" + model.UserName + "', '" + model.Password + "','" + model.FirstName + "','" + model.LastName + "',NULL,0,NULL  );";
                _db.Database.ExecuteSqlCommand(str);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");

        }

        public IActionResult DependentList()
        {

            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            //fetch list of user ids associated with the current user.
            List<int> userIdList = new List<int>();
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Relations] WHERE OwnerId=" + user.Id + ";"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                while (reader.Read())
                    userIdList.Add((int)reader[1]);
            }

            //fetch list of the users from the user table
            List<UserViewModel> users = new List<UserViewModel>();
            foreach (var id in userIdList)
            {
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Users] WHERE Id=" + id + ";"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    while (reader.Read())
                    {
                        users.Add(new UserViewModel
                        {
                            Id = (reader[0] != null && !reader[0].GetType().Equals(typeof(System.DBNull))) ? (int)reader[0] : -1,
                            UserName = (reader[1] != null && !reader[1].GetType().Equals(typeof(System.DBNull))) ? (string)reader[1] : "",
                            Password = (reader[2] != null && !reader[2].GetType().Equals(typeof(System.DBNull))) ? (string)reader[2] : "",
                            FirstName = (reader[3] != null && !reader[3].GetType().Equals(typeof(System.DBNull))) ? (string)reader[3] : "",
                            LastName = (reader[4] != null && !reader[4].GetType().Equals(typeof(System.DBNull))) ? (string)reader[4] : "",
                            Email = (reader[5] != null && !reader[5].GetType().Equals(typeof(System.DBNull))) ? (string)reader[5] : "",
                            AccountType = (reader[6] != null && !reader[6].GetType().Equals(typeof(System.DBNull))) ? (int)reader[6] : -1,
                            Avatar = null
                        });
                    }


                }
            }


            return View(users);
        }

        public IActionResult Edit(int id)
        {
            var user = new UserViewModel();
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Users] WHERE Id=" + id + ";"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                reader.Read();

                user = new UserViewModel
                {
                    Id = (int)reader[0],
                    UserName = (string)reader[1],
                    Password = (string)reader[2],
                    FirstName = (string)reader[3],
                    LastName = (string)reader[4],
                    Email = (string)reader[5],
                    AccountType = (int)reader[6],
                    Avatar = null
                };
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            if (!ModelState.IsValid || model.Password != model.RepeatPassword)
            {
                return View(model);
            }

            var str = "EXEC [dbo].spUpdateUser '" + model.Id + "', '" + model.UserName + "', '" + model.Password + "', '" + model.FirstName + "', '" + model.LastName + "', '" + model.Email + "', 0, '"+model.Avatar+"';";
            // var str = "UPDATE  [dbo].[Users] SET Username ='" + model.UserName + "',Password ='" + model.Password + "',First='" + model.FirstName + "',Last = '" + model.LastName + "',Email='" + model.Email + "',AccountType='" + model.AccountType + "',Avatar='" + model.avatar + "' WHERE Id=" + model.Id + ";";
            _db.Database.ExecuteSqlCommand(str);

            return RedirectToAction("DependentList");
        }
    }
}
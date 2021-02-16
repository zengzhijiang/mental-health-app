using System;
using System.Collections.Generic;                  
using MentalHealthApp.Data;
using MentalHealthApp.ViewModels;   
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthApp.Controllers
{
    public class AdminController : Controller
    {


        private ApplicationDbContext _db;
        //https://msdn.microsoft.com/en-us/library/jj592907%28v=vs.113%29.aspx?f=255&MSPPError=-2147217396

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: Admin
        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/Details/5
        public IActionResult UserList()
        {

            var userList = new List<UserViewModel>();
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Users"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
               // reader.Read();


                while (reader.Read())
                {
                    userList.Add(new UserViewModel
                    {
                        Id = (int)reader[0],
                        UserName = (string)reader[1],
                        Password = null,
                        FirstName = (string)reader[3],
                        LastName = (string)reader[4],
                        Email = null,
                        AccountType = (int)reader[6],
                        Avatar = null
                    });
                }

                return View(userList);
            }

        }

    }
}

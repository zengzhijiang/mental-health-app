using System;
using System.Collections.Generic;
using System.Linq;              
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Http;
using MentalHealthApp.Data;
using Microsoft.EntityFrameworkCore;
using MentalHealthApp.ViewModels;

namespace MentalHealthApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        private ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult CreateAcct()
        {
            return View();
        }

        public IActionResult UserReports()
        {
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            List<UserResponseViewModel> users = new List<UserResponseViewModel>();
            List<UserResponseViewModel> tempUsers = new List<UserResponseViewModel>();
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Relations] WHERE OwnerId=" + user.Id + ";"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                while (reader.Read())
                    users.Add(new UserResponseViewModel() { Id = (int)reader[1] });
            }


            foreach (var response in users)
            {
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Data] WHERE Date=(SELECT MAX(date) FROM [dbo].[Data] WHERE OwnerId = " + response.Id + ");"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    while (reader.Read())
                    {
                        tempUsers.Add(new UserResponseViewModel() { Id = response.Id, Emotion = (string)reader[2], Type = (string)reader[3], Response = (string)reader[4], Date = (DateTime)reader[5] });
                    }

                }
            }

            foreach (var response in tempUsers)
            {
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Users] WHERE Id=" + response.Id + ";"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    while (reader.Read())
                    {
                        tempUsers[tempUsers.FindIndex(u => u.Id == response.Id)].Name = (string)reader[3] + ", " + (string)reader[3];
                    }

                }
            }

            return View(tempUsers);
        }
        public IActionResult UserReport(int id)
        {
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            List<UserResponseViewModel> users = new List<UserResponseViewModel>();
            List<UserResponseViewModel> tempUsers = new List<UserResponseViewModel>();
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Relations] WHERE OwnerId=" + user.Id + ";"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                while (reader.Read())
                    users.Add(new UserResponseViewModel() { Id = (int)reader[1] });
            }

            //prevent someone from looking at someone thats not their dependent
            if (users.Where(u => u.Id == id) == null)
            {
                return RedirectToAction("Menu");
            }

            foreach (var response in users)
            {
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Data] WHERE OwnerId = " + response.Id + ";"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    while (reader.Read())
                    {
                        tempUsers.Add(new UserResponseViewModel() { Id = response.Id, Emotion = (string)reader[2], Type = (string)reader[3], Response = (string)reader[4], Date = (DateTime)reader[5] });
                    }

                }
            }

            foreach (var response in tempUsers)
            {
                using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM [dbo].[Users] WHERE Id=" + response.Id + ";"))
                {
                    // Output rows.
                    var reader = datareader.DbDataReader;
                    while (reader.Read())
                    {
                        tempUsers[tempUsers.FindIndex(u => u.Id == response.Id)].Name = (string)reader[3] + ", " + (string)reader[3];
                    }

                }
            }
            ViewBag.Name = tempUsers.FirstOrDefault().Name;
            return View(tempUsers);
        }
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PasswordChange(string previousPassword, string newPassword, string newPasswordRepeat)
        {
            if (newPassword != newPasswordRepeat)
            {
                ViewBag.Validation = "New password does not match";
                return View();
            }

            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Users WHERE Username Like '" + user.UserName + "' AND Password Like'" + previousPassword + "';"))
            {
                // Output rows.
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    if (reader[2].Equals(previousPassword))
                    {
                        _db.Database.ExecuteSqlCommand("UPDATE [dbo].[Users] SET Password='" + newPassword + "' WHERE Id = " + user.Id + ";");
                        return RedirectToAction("Menu");
                    }
                }
                catch (Exception e)
                {

                }


            }

            return View();
        }
        public IActionResult Linkuser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ConfirmLinkCode(string code)
        {
            var user = new UserViewModel();
            int inviterId = -1;
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            //check to see if there is a code out there waiting to be accepted
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Invites WHERE LinkCode Like '" + code + "';"))
            {
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    //if there is a invite waiting with the matching code. And add he relation.
                    if (reader[1].Equals(code))
                    {
                        //get the inviter id
                        inviterId = (int)reader[2];
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    //if the insert attempt fails
                    return RedirectToAction("Linkuser");
                }
                reader.Close();
            }

            if (inviterId != -1)
            {
                var relationString = "";
                //establish the relation 
                if (user.AccountType < 2)
                {
                    relationString = "INSERT INTO [dbo].[Relations] (OwnerId , AssociateId) VALUES (" + inviterId + ", " + user.Id + ");";
                }
                else
                {
                    relationString = "INSERT INTO [dbo].[Relations] (OwnerId , AssociateId) VALUES (" + user.Id + ", " + inviterId + ");";
                }

                _db.Database.ExecuteSqlCommand(relationString);
            }



            return RedirectToAction("UserList", "Admin");
        }

        public IActionResult Linkcode()
        {
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }


            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Invites WHERE Inviter = " + user.Id + ";"))
            {
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    if (!reader[2].Equals(null))
                    {
                        ViewBag.LinkCode = reader[1];
                    }
                    reader.Close();
                }
                catch (Exception e) { }
            }
            return View();
        }

        public IActionResult GenerateLinkcode()
        {
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            var rand = new Random();
            var arr = new char[] {
                (char)rand.Next('A', 'Z'),
                (char)rand.Next('A', 'Z'),
                (char)rand.Next('A', 'Z'),
                (char)rand.Next('A', 'Z'),
                (char)rand.Next('A', 'Z'),
                (char)rand.Next('A', 'Z')};
            var randStr = new string(arr);

            //chekc if the code already exists
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Invites WHERE LinkCode LIKE '" + randStr + "';"))
            {
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    if (!reader[2].Equals(null))
                    {
                        return RedirectToAction("GenerateLinkcode");
                    }
                }
                catch (Exception e) { }
                reader.Close();
            }

            //check if the user linke already exists
            using (var datareader = _db.Database.ExecuteSqlQuery("SELECT * FROM Invites WHERE Inviter = " + user.Id + ";"))
            {
                var reader = datareader.DbDataReader;
                try
                {
                    reader.Read();
                    if (!reader[2].Equals(null))
                    {
                        reader.Close();
                        _db.Database.ExecuteSqlCommand("UPDATE [dbo].[Invites] SET LinkCode='" + randStr + "' WHERE Inviter = " + user.Id + ";");
                        return RedirectToAction("Linkcode");
                    }
                    reader.Close();
                }
                catch (Exception e) { }
            }

            var relationString = "INSERT INTO [dbo].[Invites] (LinkCode , Inviter) VALUES ('" + randStr + "', " + user.Id + ");";
            _db.Database.ExecuteSqlCommand(relationString);

            return RedirectToAction("Linkcode");
        }
    }
}

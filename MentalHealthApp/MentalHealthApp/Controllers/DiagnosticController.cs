using System;                           
using MentalHealthApp.Data;
using MentalHealthApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthApp.Controllers
{

    public class DiagnosticController : Controller
    {   

        private ApplicationDbContext _db;
        public DiagnosticController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }

            return View(user);
        }

        public IActionResult FirstQuestion(String emotion, String color)
        {
            ViewBag.Emotion = emotion;
            ViewBag.Color = color;
            return View();
        }

        public IActionResult SecondQuestion(String emotion, String color)
        {
            ViewBag.Emotion = emotion;
            ViewBag.Color = color;
            return View();
        }
        public IActionResult ThirdQuestion(String emotion, String color)
        {
            ViewBag.Emotion = emotion;
            ViewBag.Color = color;
            return View();
        }
        public IActionResult CustomInput(String emotion, String color)
        {
            ViewBag.Emotion = emotion;
            ViewBag.Color = color;
            return View();
        }

        [HttpPost]
        public IActionResult PostQuestions(string emotion, string answer, string type)
        {
            if (string.IsNullOrEmpty(answer))
                answer = "";
            var user = new UserViewModel();
            try
            {
                user = HttpContext.Session.GetDeserialized<UserViewModel>("User");
            }
            catch (Exception e) { }
            if (user == null)
                return RedirectToAction("Index", "Diagnostic");

            if (!string.IsNullOrEmpty(emotion) && !string.IsNullOrEmpty(type) && !answer.Replace(" ", "").ToLower().Contains("droptable"))
            {
                _db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Data] (OwnerId,Emotion,Type,Response) VALUES ('" + user.Id + "', '" + emotion + "','" + type + "','" + answer + "');");
            }
            //Request.Form["answer"].ToString();
            //data will be recorded here.

            //commented code below is for when the other games are complete.
            if (emotion == "happy") { return RedirectToAction("HappyGame", "Game"); }
            else if (emotion == "sad") { return RedirectToAction("SadGame", "Game"); }
            else if (emotion == "angry") { return RedirectToAction("AngryGame", "Game"); }
            else if (emotion == "worried") { return RedirectToAction("WorriedGame", "Game"); }
            else if (emotion == "proud") { return RedirectToAction("ProudGame", "Game"); }
            else if (emotion == "relaxed") { return RedirectToAction("RelaxedGame", "Game"); }
            return RedirectToAction("Index");
        }
    }
}

//@Html.ActionLink("act", "Contoller") 

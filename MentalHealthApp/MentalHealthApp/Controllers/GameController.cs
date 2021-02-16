using System;
using MentalHealthApp.Data;
using MentalHealthApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MentalHealthApp.Controllers
{
    public class GameController : Controller {
        public IActionResult HappyGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Polar bear";
            return View(user);
        }

        public IActionResult SadGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Mr. Bear";
            return View(user);
        }

        public IActionResult WorriedGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Ms. Hippo";
            return View(user);
        }

        public IActionResult ProudGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Monkey";
            Random r = new Random();
            ViewBag.numStars = r.Next(1, 10);
            return View(user);
        }

        public IActionResult RelaxedGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            //ViewBag.Avatar = (user.Avatar !=null ? user.Avatar : "animal");
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Mr. Zebra";
            Random r = new Random();
            ViewBag.numStars = r.Next(1, 4);
            return View(user);
        }

        public IActionResult AngryGame() {
            var user = new UserViewModel();
            try { user = HttpContext.Session.GetDeserialized<UserViewModel>("User"); }
            catch (Exception e) { }
            if (string.IsNullOrEmpty(user.Avatar) || string.IsNullOrWhiteSpace(user.Avatar))
                user.Avatar = "Mr. Dog";
            return View(user);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.Models;

namespace MultiFive.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "MultiFive description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "MultiFive contacts.";

            return View();
        }

        [Authorize]
        public ActionResult CreateGame()
        {
            string userId = User.Identity.GetUserId();
            Player player = _repository.FindPlayer(userId);
            return RedirectToAction("Create", "Game", new { creatingPlayerId = player.Id });
        }
    }
}
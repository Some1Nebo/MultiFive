using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MultiFive.Web.Controllers
{
    public class HomeController : Controller
    {
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
    }
}
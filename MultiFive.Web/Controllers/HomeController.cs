using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.DataAccess;

namespace MultiFive.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPrincipal _user; 
        private readonly IRepository _repository;

        public HomeController(IPrincipal user, IRepository repository)
        {
            _user = user; 
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
            string userId = _user.Identity.GetUserId();
            Player player = _repository.FindPlayer(userId);

            return RedirectToAction("Create", "Game", new { creatingPlayerId = player.Id });
        }
    }
}
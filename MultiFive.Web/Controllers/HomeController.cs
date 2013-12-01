using System.Security.Principal;
using System.Web.Mvc;
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
            var player = _repository.FindPlayer(_user);

            return RedirectToAction("Create", "Game", new { creatingPlayerId = player.Id });
        }
    }
}
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
    public class GameController : Controller
    {
        private readonly IRepository _repository;

        public GameController(IRepository repository)
        {
            _repository = repository;
        }
        
        [Authorize]
        public ActionResult Create(int creatingPlayerId)
        {
            string userId = User.Identity.GetUserId();
            Player player = _repository.FindPlayer(userId);

            var game = _repository.CreateGame(player);
            _repository.Save();

            return RedirectToRoute("Game", new {gameId = game.Id});
        }

        //[Authorize]
        public ActionResult Show(Guid gameId)
        {
            // TODO: add non authorized behavior (watching game?)
            // TODO: uncomment following lines and Authorize attribute to get info for authenticated user

            //string userId = User.Identity.GetUserId();
            //Player player = _repository.FindPlayer(userId);

            var game = _repository.Games
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .FirstOrDefault(g => g.Id == gameId);

            if (game == null)
            {
                string msg = string.Format("Game with id {0} is not found.", gameId);
                throw new ApplicationException(msg);
            }
            
            return View(game);
        }
	}
}
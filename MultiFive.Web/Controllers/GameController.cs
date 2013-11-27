using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Http.Results;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.Controllers.Attributes;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Infrastructure;
using MultiFive.Web.Models.Messaging;
using System.Web.Helpers;

namespace MultiFive.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly IPrincipal _user;
        private readonly Player _player;
        private readonly IRepository _repository;
        private readonly IMessageFactory _messageFactory; 

        public GameController(IPrincipal user, IRepository repository, IMessageFactory messageFactory)
        {
            _user = user;
            _repository = repository;
            _messageFactory = messageFactory;
            _player = _repository.FindPlayer(_user);
        }
        
        [Authorize]
        public ActionResult Create(int creatingPlayerId)
        {
            var game = _repository.CreateGame(_player);
            _repository.Save();

            return RedirectToRoute("Game", new {gameId = game.Id});
        }

        [Authorize]
        public ActionResult Show(Guid gameId)
        {
            var game = GetGame(gameId);

            if (game.Player2 == null)
            {
                if (_player.Id != game.Player1.Id)
                {
                    game.Lock(_player);

                    var message = _messageFactory.CreateJoinedMessage(gameId, game.Player1.Id);
                    _repository.AddMessage(message);

                    _repository.Save(); 
                }

                return View(game);
            }

            if (_player.Id != game.Player1.Id && _player.Id != game.Player2.Id)
            {
                return View("AlreadyFull", gameId);
            }

            return View(game);
        }

        [Authorize]
        public JsonResult Poll(Guid gameId)
        {
            // TODO: add non-authorized behavior for Poll action,
            // i.e. messages sent to all subscribers (playerId = -1?)

            var messages = _repository.Messages
                .Where(m => m.ReceiverId == _player.Id
                            && m.GameId == gameId
                            && m.Status != Status.Fulfilled)
                .OrderBy(m => m.CreationTime)
                .ToList();

            foreach (var msg in messages)
            {
                msg.Status = Status.Fulfilled;
            }

            _repository.Save();

            var jsonMessages = messages.Select(m => m.JsonContent).ToList();
            
            return Json(jsonMessages, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult Move()
        {
            throw new NotImplementedException(); 
        }

        private Game GetGame(Guid gameId)
        {
            var game = _repository.Games
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .FirstOrDefault(g => g.Id == gameId);

            if (game == null)
            {
                string msg = string.Format("Game with id {0} is not found.", gameId);
                throw new ApplicationException(msg);
            }

            return game; 
        }
	}
}
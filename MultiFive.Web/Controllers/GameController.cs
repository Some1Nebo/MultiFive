using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using MultiFive.Web.Models;
using WebHelpers = System.Web.Helpers;
using MultiFive.Domain;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly Player _player;
        private readonly IRepository _repository;
        private readonly IMessageFactory _messageFactory;

        private const int DefaultWidth = 10;
        private const int DefaultHeight = 10; 

        public GameController(IPrincipal user, IRepository repository, IMessageFactory messageFactory)
        {
            _repository = repository;
            _messageFactory = messageFactory;
            _player = _repository.FindPlayer(user);
        }
        
        [Authorize]
        public ActionResult Create(int creatingPlayerId)
        {
            var game = _repository.CreateGame(DefaultWidth, DefaultHeight, _player);
            _repository.Save();

            return RedirectToRoute("ShowGame", new { gameId = game.Id });
        }

        [Authorize]
        public ActionResult Show(Guid gameId)
        {
            var game = GetGame(gameId);

            GameSnapshot snapshot = _repository.GameSnapshots
                                        .Include(s => s.Game)
                                        .Include(s => s.LastMessage)
                                        .FirstOrDefault(s => s.Game.Id == game.Id)
                                    ?? new GameSnapshot(game);

            ViewBag.PlayerRole = PlayerRole.Spectator;

            if (_player.Id == game.Player1.Id)
                ViewBag.PlayerRole = PlayerRole.Player1;

            if (game.Player2 == null)
            {
                if (_player.Id != game.Player1.Id)
                {
                    ViewBag.PlayerRole = PlayerRole.Player2;

                    game.Lock(_player); // player1 chosen as first

                    var playerName = string.Format("Player {0}", _player.Id);

                    var message = _messageFactory.CreateJoinedMessage(gameId, playerName, game.CurrentState.ToString());
                    _repository.AddMessage(message);

                    snapshot = new GameSnapshot(game, message);
                    _repository.UpdateGameSnapshot(snapshot);

                    _repository.Save();

                    // allow player2 to receive the message too
                    return View(new GameSnapshot(game));
                }

                return View(snapshot);
            }

            if (_player.Id != game.Player1.Id && _player.Id != game.Player2.Id)
            {
                return View("AlreadyFull", gameId);
            }

            return View(snapshot);
        }

        [Authorize]
        public JsonResult Move()
        {
            throw new NotImplementedException("Implement Move action and do not forget to set game.StateNumber"); 
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
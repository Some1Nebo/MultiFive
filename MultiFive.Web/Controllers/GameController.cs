using System;
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
            var game = _repository.GetGame(gameId);

            var snapshot = _repository.GetGameSnapshot(game);

            var playerRole = GetPlayerRole(game, _player);

            ViewBag.PlayerRole = playerRole;

            if (game.Player2 == null)
            {
                if (playerRole == PlayerRole.Spectator)
                {
                    ViewBag.PlayerRole = playerRole = PlayerRole.Player2;

                    game.Lock(_player); // player1 chosen as first

                    var playerName = string.Format("Player {0}", _player.Id);

                    var message = _messageFactory.CreateJoinedMessage(gameId, playerName, game.CurrentState);
                    _repository.AddMessage(message);

                    snapshot = new GameSnapshot(game, message);
                    _repository.AddGameSnapshot(snapshot);

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

        static readonly object MoveLock = new object();

        [Authorize]
        public JsonResult Move(Guid gameId, int row, int col)
        {
            lock (MoveLock)
            {
                var game = _repository.GetGame(gameId);
                var snapshot = _repository.GetGameSnapshot(game);

                game.Move(_player, row, col);

                PlayerRole playerRole = GetPlayerRole(game, _player);

                var message = _messageFactory.CreateMovedMessage(gameId, playerRole, row, col, game.CurrentState);
                _repository.AddMessage(message);

                snapshot.LastMessage = message;

                // uncomment the following line to reproduce delay behavior locally
                //Thread.Sleep(TimeSpan.FromSeconds(2));
                
                _repository.Save();

                return Json(game.CurrentState.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        private static PlayerRole GetPlayerRole(Game game, Player player)
        {
            if (game.Player1 != null && player.Id == game.Player1.Id)
                return PlayerRole.Player1;
            else if (game.Player2 != null && player.Id == game.Player2.Id)
                return PlayerRole.Player2;
            else
                return PlayerRole.Spectator;
        }
	}
}
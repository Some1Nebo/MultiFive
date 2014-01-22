using System;
using System.Collections.Concurrent;
using System.Security.Principal;
using System.Threading.Tasks;
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

            var playerRole = game.GetPlayerRole(_player);

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

        // Below is proof of concept of delayed synchronized Move execution per game

        // TODO: remove inactive games from collection to avoid eventual memory overflow
        static readonly ConcurrentDictionary<Guid, Task> MoveRequests = new ConcurrentDictionary<Guid, Task>();

        [Authorize]
        public void Move(Guid gameId, int row, int col)
        {
            // TODO: exceptions in following invocation are silently lost, so we need a way to log and show them.

            Action move = () =>
            {
                /* 
                 * Here instance members are out of lifetime scope setup by Autofac because of delayed invocation
                 * TODO: use ServiceLocator to instantiate them?
                 */
                using (var repository = new EFRepository(new ApplicationDbContext()))
                {
                    var messageFactory = new JsonMessageFactory();

                    var game = repository.GetGame(gameId);
                    var snapshot = repository.GetGameSnapshot(game);

                    game.Move(_player, row, col);

                    PlayerRole playerRole = game.GetPlayerRole(_player);

                    var message = messageFactory.CreateMovedMessage(gameId,
                        playerRole, row, col, game.CurrentState);
                    repository.AddMessage(message);

                    snapshot.LastMessage = message;

                    repository.Save();
                }
            };

            MoveRequests.AddOrUpdate(gameId,
                id => Task.Factory.StartNew(move), // on first call run for this gameId
                (id, currentTask) => currentTask.ContinueWith(t => move())); // queue invocation for this gameId
        }
	}
}
﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.Controllers.Attributes;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.Controllers
{
    public class GameController : Controller
    {
        private readonly IPrincipal _user;
        private readonly IRepository _repository;
        private readonly IMessageFactory _messageFactory; 

        public GameController(IPrincipal user, IRepository repository, IMessageFactory messageFactory)
        {
            _user = user;
            _repository = repository;
            _messageFactory = messageFactory; 
        }
        
        [Authorize]
        public ActionResult Create(int creatingPlayerId)
        {
            string userId = _user.Identity.GetUserId();
            Player player = _repository.FindPlayer(userId);

            var game = _repository.CreateGame(player);
            _repository.Save();

            return RedirectToRoute("Game", new {gameId = game.Id});
        }

        [Authorize]
        public ActionResult Show(Guid gameId)
        {
            string userId = _user.Identity.GetUserId();
            Player player = _repository.FindPlayer(userId);

            var game = GetGame(gameId);

            if (game.Player2 == null)
            {
                if (player.Id != game.Player1.Id)
                {
                    game.Lock(player);

                    var message = _messageFactory.CreateJoinedMessage(gameId, player.Id);
                    _repository.AddMessage(message);

                    _repository.Save(); 
                }

                return View(game);
            }

            if (player.Id != game.Player1.Id && player.Id != game.Player2.Id)
            {
                return View("AlreadyFull", gameId);
            }

            return View(game);
        }

        [AjaxOnly]
        public JsonResult Ping(Guid gameId)
        {
            // todo: returns json array of messages

            throw new NotImplementedException(); 
        }

        [AjaxOnly]
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
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using MultiFive.Domain;
using MultiFive.Web.Models;

namespace MultiFive.Web.DataAccess
{
    public static class DataAccessExtensions
    {
        public static Player FindPlayer(this IRepository repository, IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                string userId = user.Identity.GetUserId();
                return repository.FindPlayer(userId);
            }

            return null;
        }

        public static Game GetGame(this IRepository repository, Guid gameId)
        {
            var game = repository.Games
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

        public static GameSnapshot GetGameSnapshot(this IRepository repository, Game game)
        {
            return repository.GameSnapshots
                .Include(s => s.Game)
                .Include(s => s.LastMessage)
                .FirstOrDefault(s => s.Game.Id == game.Id)
                   ?? new GameSnapshot(game);
        }
    }
}
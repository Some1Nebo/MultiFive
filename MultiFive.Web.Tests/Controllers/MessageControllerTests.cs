using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using MultiFive.Domain;
using MultiFive.Web.Controllers;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Models.Messaging;
using NSubstitute;
using NUnit.Framework;

namespace MultiFive.Web.Tests.Controllers
{
    [TestFixture]
    public class MessageControllerTests
    {
        [Test]
        public void Poll_for_creating_player_should_return_join_message_when_game_is_locked()
        {
            // Arrange
            const int width = 100;
            const int height = 50; 
            var user = Substitute.For<IPrincipal>();
            user.Identity.IsAuthenticated.Returns(true);
            var player1 = new Player(1);
            var player2 = new Player(2);
            var game = new Game(width, height, player1);
            var games = new List<Game> { game }.AsQueryable();

            var repository = Substitute.For<IRepository>(); 
            repository.Games.Returns(games);

            var jsonMessageFactory = new JsonMessageFactory();

            var messages = new List<Message>();
            repository.AddMessage(Arg.Do<Message>(messages.Add));
            repository.PollMessages(Arg.Any<Guid?>(), Arg.Any<int?>(), Arg.Any<int>()).Returns(messages);

            // Act (locking player)
            repository.FindPlayer(Arg.Any<string>()).Returns(player2);
            var controller2 = new GameController(user, repository, jsonMessageFactory);
            controller2.Show(game.Id); // locks game to player2

            // Act (polling player)
            repository.FindPlayer(Arg.Any<string>()).Returns(player1);
            var controller1 = new MessageController(user, repository);
            var pollResult = controller1.Poll(game.Id);

            // Assert
            dynamic jsonMessages = pollResult.Data;
            var firstMessage = messages.First();

            Assert.AreEqual(new { id = firstMessage.Id, name = firstMessage.Name, content = firstMessage.Content }.ToString(), jsonMessages[0].ToString());
        }
    }
}

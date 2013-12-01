using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using MultiFive.Domain;
using MultiFive.Web.Controllers;
using MultiFive.Web.DataAccess;
using MultiFive.Web.Models.Messaging;
using NSubstitute;
using NUnit.Framework;

using Should.Fluent;

namespace MultiFive.Web.Tests.Controllers
{
    [TestFixture]
    public class GameControllerTests
    {
        private IRepository repository;
        private IPrincipal user;
        private IMessageFactory messageFactory; 

        private Player player1;
        private Player player2;
        private Player player3;
        private Game lockedGame;
        private Guid gameId; 

        [SetUp]
        public void Setup()
        {
            user = Substitute.For<IPrincipal>();
            user.Identity.IsAuthenticated.Returns(true);

            repository = Substitute.For<IRepository>();
            repository.Games.Returns(new List<Game>().AsQueryable());
            messageFactory = Substitute.For<IMessageFactory>(); 
            
            player1 = new Player(1);
            player2 = new Player(2);
            player3 = new Player(3);

            lockedGame = new Game(player1);
            lockedGame.Lock(player2);

            gameId = lockedGame.Id; 
        }

        [Test]
        public void Show_unknown_gameId_throws_ApplicationException()
        {
            // Arrange
            var controller = new GameController(user, repository, messageFactory);

            // Act 
            Assert.Throws<ApplicationException>(() => controller.Show(Guid.NewGuid())); 
        }

        [Test]
        public void Show_if_game_locked_returns_game_already_full_view_for_a_new_player()
        {
            // Arrange 
            var games = new List<Game> { lockedGame }.AsQueryable(); 

            repository.Games.Returns(games);
            repository.FindPlayer(Arg.Any<string>()).Returns(player3);

            var controller = new GameController(user, repository, messageFactory);

            // Act 
            var result = (ViewResult)controller.Show(gameId);

            // Assert 
            result.ViewName.Should().Equal("AlreadyFull");
            result.Model.Should().Equal(gameId); 
        }

        [Test]
        public void Show_for_first_game_player_returns_game_view()
        {
            // Arrange 
            var games = new List<Game> { lockedGame }.AsQueryable();

            repository.Games.Returns(games);
            repository.FindPlayer(Arg.Any<string>()).Returns(player1);

            var controller = new GameController(user, repository, messageFactory);

            // Act
            var result = (ViewResult)controller.Show(gameId); 

            //Assert
            result.ViewName.Should().Equal(string.Empty);
            ((Game)result.Model).Id.Should().Equal(gameId); 
        }

        [Test]
        public void Show_for_second_game_player_returns_game_view()
        {
            // Arrange 
            var games = new List<Game> { lockedGame }.AsQueryable();

            repository.Games.Returns(games);
            repository.FindPlayer(Arg.Any<string>()).Returns(player2);

            var controller = new GameController(user, repository, messageFactory);

            // Act
            var result = (ViewResult)controller.Show(gameId);

            //Assert
            result.ViewName.Should().Equal(string.Empty);
            ((Game)result.Model).Id.Should().Equal(gameId); 
        }

        [Test]
        public void Show_if_game_unlocked_locks_the_game_creates_joined_message_and_returns_game_view()
        {
            // Arrange
            var unlockedGame = new Game(player1);
            var games = new List<Game> { unlockedGame }.AsQueryable();

            repository.Games.Returns(games);
            repository.FindPlayer(Arg.Any<string>()).Returns(player2);

            var controller = new GameController(user, repository, messageFactory);

            // Act
            var result = (ViewResult)controller.Show(unlockedGame.Id);

            // Assert
            result.ViewName.Should().Equal(string.Empty);
            ((Game)result.Model).Id.Should().Equal(unlockedGame.Id);
            unlockedGame.Player2.Id.Should().Equal(player2.Id);

            repository.Received().AddMessage(Arg.Any<Message>());
            repository.Received().Save(); 
        }
    }
}

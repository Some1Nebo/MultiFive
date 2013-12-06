using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Should.Fluent;

namespace MultiFive.Domain.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void Move_Should_Be_Correctly_Stored()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            game.Move(p1, 5, 5);

            game[5, 5].Should().Equal(Cell.Player1);
        }

        [Test]
        public void Unlocked_Game_Should_Throw_If_Player_Makes_Move()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);

            Assert.Throws<InvalidOperationException>(() => game.Move(p1, 5, 5));
        }

        [Test]
        public void Game_Should_Throw_When_Invalid_Player_Makes_Move()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            Assert.Throws<InvalidOperationException>(() => game.Move(p2, 5, 5));
        }
    }
}

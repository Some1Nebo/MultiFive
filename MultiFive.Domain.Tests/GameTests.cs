using System;
using NUnit.Framework;
using Should.Fluent;

namespace MultiFive.Domain.Tests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void Constructor_Throws_ArgumentNullException_If_Player1_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new Game(10, 10, null));
        }

        [Test]
        [TestCase(0, 30)]
        [TestCase(30, 0)]
        [TestCase(-5, 30)]
        [TestCase(30, -5)]
        public void Constructor_Throws_ArgumentException_If_Invalid_GameField_Size(int width, int height)
        {
            Assert.Throws<ArgumentException>(() => new Game(width, height, new Player(1)));
        }

        [Test]
        public void Has_NotStarted_State_When_Just_Created()
        {
            var p1 = new Player(1); 

            var game = new Game(10, 10, p1);

            game.CurrentState.Should().Equal(Game.State.NotStarted); 
        }

        [Test]
        public void StartingPlayer_Moves_Game_To_Correct_State()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            var game1 = new Game(10, 10, p1);
            game.Lock(p2, p1);

            var game2 = new Game(10, 10, p2);
            game2.Lock(p2, p2);

            game.CurrentState.Should().Equal(Game.State.Player1Move);
            game1.CurrentState.Should().Equal(Game.State.Player1Move);
            game2.CurrentState.Should().Equal(Game.State.Player2Move);
        }

        [Test]
        public void Throws_If_Locking_Already_Locked_Game()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);
            var p3 = new Player(3); 

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            var game1 = new Game(10, 10, p1);
            game.Lock(p2, p1);

            var game2 = new Game(10, 10, p2);
            game2.Lock(p2, p2);

            Assert.Throws<InvalidOperationException>(() => game.Lock(p3));
            Assert.Throws<InvalidOperationException>(() => game1.Lock(p3));
            Assert.Throws<InvalidOperationException>(() => game2.Lock(p3));
        }

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
        public void Throws_If_Player_Makes_Move_In_Unlocked_Game()
        {
            var p1 = new Player(1); 

            var game = new Game(10, 10, p1);

            Assert.Throws<InvalidOperationException>(() => game.Move(p1, 5, 5));
        }

        [Test]
        public void Throws_When_Invalid_Player_Makes_Move()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            Assert.Throws<InvalidOperationException>(() => game.Move(p2, 5, 5));
        }

        [Test]
        public void Throws_If_Player_Makes_Move_In_Game_That_Is_Over()
        {
            Assert.Ignore(); 
        }

        [Test]
        public void Throws_IfPlayer_Makes_Move_In_Game_That_Is_Draw()
        {
            Assert.Ignore(); 
        }

        [Test]
        public void Has_Player2Move_State_When_Player1_Made_A_Move_()
        {
            Assert.Ignore(); 
        }

        [Test]
        public void Has_Player1Move_State_When_Player2_Made_A_Move_()
        {
            Assert.Ignore();
        }

        [Test]
        public void Has_Player1Win_State_When_Player1_Wins()
        {
            Assert.Ignore(); 
        }

        [Test]
        public void Has_Player2Win_State_When_Player2_Wins()
        {
            Assert.Ignore();
        }

        [Test]
        public void Has_Draw_State_When_Nobody_Can_Win_At_This_Stage()
        {
            Assert.Ignore(); 
        }

        [Test]
        public void Has_Draw_State_When_Players_Agreed_To_Truce_In_Game_That_Is_In_Progress()
        {
            var p1 = new Player(1);
            var p2 = new Player(2);

            var game = new Game(10, 10, p1);
            game.Lock(p2);

            game.Truce();

            game.CurrentState.Should().Equal(Game.State.Draw); 
        }

        [Test]
        public void Throws_When_Players_Attempt_To_Truce_In_Game_That_Is_Over()
        {
            // test for different states: player 1 won, player 2 won, draw 

            Assert.Ignore(); 
        }

        [Test]
        public void Throws_When_Players_Attempt_To_Truce_In_Game_That_Has_Not_Started()
        {
            var p1 = new Player(1); 

            var game = new Game(10, 10, p1);

            Assert.Throws<InvalidOperationException>(game.Truce);
        }
    }
}

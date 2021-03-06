﻿using System;
using System.Linq;
using Stateless;

namespace MultiFive.Domain
{
    public enum Cell
    {
        Empty = 0,
        Player1, 
        Player2
    }

    public class Game
    {
        public enum State
        {
            NotStarted,
            Player1Move,
            Player2Move,
            Player1Win,
            Player2Win,
            Draw
        }

        enum Event
        {
            Locking,
            PlayerMove,
            Truce
        }

        enum Outcome
        {
            Win,
            Draw
        }

        private const int WinnerChain = 5; 

        private StateMachine<State, Event> _stateMachine;
        private StateMachine<State, Event>.TriggerWithParameters<Player, int, int> _moveEvent;
        private StateMachine<State, Event>.TriggerWithParameters<Player, Player> _lockingEvent;

        private Game()
        {
            SetupStateMachine(); 
        }

        public Guid Id { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public State CurrentState { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] FieldData { get; private set; }

        public Game(int width, int height, Player player1)
        {
            if (player1 == null) 
                throw new ArgumentNullException("player1");

            if (width < 0 || height < 0)
                throw new ArgumentException("Invalid size of the game field.");

            Id = Guid.NewGuid();
            Player1 = player1;
            Player2 = null;
            CurrentState = State.NotStarted;

            Width = width;
            Height = height;
            FieldData = new byte[Width * Height];

            SetupStateMachine(); 
        }

        public void Lock(Player player2, Player startingPlayer = null)
        {
            _stateMachine.Fire(_lockingEvent, player2, startingPlayer);
        }

        public void Move(Player player, int row, int column)
        {
            _stateMachine.Fire(_moveEvent, player, row, column);
        }

        public void Truce()
        {
            _stateMachine.Fire(Event.Truce);
        }

        public Cell this[int row, int column]
        {
            get
            {
                var idx = LinearIndex(row, column);
                return (Cell)FieldData[idx];
            }

            private set
            {
                if (this[row, column] != Cell.Empty)
                    throw new InvalidOperationException("Cannot mark a cell that is already marked.");

                var idx = LinearIndex(row, column);
                FieldData[idx] = (byte)value;

                // TODO: refactor this hack, done for entityframework to pick-up changes before saving
                FieldData = FieldData.ToArray();
            }
        }

        private int LinearIndex(int row, int column)
        {
            return row * Width + column;
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine<State, Event>(() => CurrentState, s => CurrentState = s);

            _lockingEvent = _stateMachine.SetTriggerParameters<Player, Player>(Event.Locking);
            _moveEvent = _stateMachine.SetTriggerParameters<Player, int, int>(Event.PlayerMove);

            _stateMachine
                .Configure(State.NotStarted)
                .PermitDynamic(_lockingEvent, NotStartedOnLocking);
            
            _stateMachine
                .Configure(State.Player1Move)
                .Permit(Event.Truce, State.Draw)
                .PermitDynamic(_moveEvent, Player1MoveOnPlayerMoved);

            _stateMachine
                .Configure(State.Player2Move)
                .Permit(Event.Truce, State.Draw)
                .PermitDynamic(_moveEvent, Player2MoveOnPlayerMoved);
        }

        private State NotStartedOnLocking(Player player2, Player startingPlayer)
        {
            if (Player2 != null)
                throw new InvalidOperationException("Game is already locked to two players");

            Player2 = player2;

            if (startingPlayer == null)
                startingPlayer = Player1;

            if (startingPlayer.Id != Player1.Id && startingPlayer.Id != Player2.Id)
                throw new ArgumentException(string.Format("StartingPlayer must be either player ({0}) or player ({1}), but was player ({2}).",
                    Player1.Id, Player2.Id, startingPlayer.Id));

            return startingPlayer.Id == Player1.Id ? 
                State.Player1Move : 
                State.Player2Move;
        }

        private State Player1MoveOnPlayerMoved(Player player, int row, int column)
        {
            if (player.Id != Player1.Id)
            {
                var msg = string.Format("Player must be ({0}), but was player ({1}).",
                    Player1.Id, player.Id);

                throw new InvalidOperationException(msg);
            }

            this[row, column] = Cell.Player1;

            var outcome = GetGameOutcome(row, column);

            if (outcome.HasValue)
                return (outcome.Value == Outcome.Win) ?
                    State.Player1Win :
                    State.Draw;

            return State.Player2Move;
        }

        private State Player2MoveOnPlayerMoved(Player player, int row, int column)
        {
            if (player.Id != Player2.Id)
            {
                var msg = string.Format("Player must be ({0}), but was player ({1}).",
                    Player2.Id, player.Id);

                throw new InvalidOperationException(msg);
            }

            this[row, column] = Cell.Player2;

            var outcome = GetGameOutcome(row, column);

            if (outcome.HasValue)
                return (outcome.Value == Outcome.Win) ?
                    State.Player2Win :
                    State.Draw;

            return State.Player1Move;
        }

        private Outcome? GetGameOutcome(int row, int column)
        {
            if (this[row, column] == Cell.Empty)
                throw new ArgumentException("row & col");

            bool isWin = IsChainLongEnough(row, column, 0, 1)
                         || IsChainLongEnough(row, column, 1, 0)
                         || IsChainLongEnough(row, column, 1, 1)
                         || IsChainLongEnough(row, column, 1, -1);

            // todo: check if the game is draw

            return isWin ? Outcome.Win : (Outcome?)null;
        }

        private bool IsChainLongEnough(int row, int column, int dr, int dc)
        {
            int currLength = 1;
            currLength = GrowChain(row, column, currLength, dr, dc);
            currLength = GrowChain(row, column, currLength, -dr, -dc); 

            return (currLength == WinnerChain); 
        }

        private int GrowChain(int row, int column, int currLength, int dr, int dc)
        {
            var cellOwner = this[row, column];
            int rowIdx = row;
            int colIdx = column;

            while (currLength < WinnerChain)
            {
                rowIdx += dr;
                colIdx += dc;

                if (OutOfField(rowIdx, colIdx))
                    break; 

                if (this[rowIdx, colIdx] == cellOwner)
                    currLength += 1;
                else
                    break;
            }

            return currLength; 
        }

        private bool OutOfField(int rowIdx, int colIdx)
        {
            return (rowIdx < 0 || rowIdx > Width - 1 || colIdx < 0  || colIdx > Width - 1);
        }
    }
}
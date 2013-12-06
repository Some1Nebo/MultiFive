using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using MultiFive.Domain;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.Models
{
    public class GameSnapshot
    {
        private GameSnapshot()
        {
        }

        public GameSnapshot(Game game)
        {
            Game = game;
            LastMessage = null;
            LastMessageId = 0;
        }

        public GameSnapshot(Game game, Message lastMessage)
        {
            Game = game;
            LastMessage = lastMessage;
        }

        public Game Game { get; set; }
        public Message LastMessage { get; set; }

        [Required]
        [Key]
        [ForeignKey("Game")]
        public Guid GameId { get; private set; }

        [ForeignKey("LastMessage")]
        public int LastMessageId { get; private set; }
    }
}
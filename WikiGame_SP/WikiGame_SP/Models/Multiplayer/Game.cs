namespace WikiGame_SP.Models.Multiplayer
{
    using System;

    public class Game
    {
        public DateTime StartGame { get; set; }

        public DateTime EndGame { get; set; }

        public Player PlayerOne { get; set; }

        public Player PlayerTwo { get; set; }

        public string CategoryName { get; set; }

        public string StartPage { get; set; }
    }
}
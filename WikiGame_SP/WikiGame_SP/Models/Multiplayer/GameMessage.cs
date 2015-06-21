namespace WikiGame_SP.Models.Multiplayer
{
    public class GameMessage
    {
        public enum MessageTypes {
            GameStart,
            YouLost
        }

        public MessageTypes Type { get; set; }

        public string Category { get; set; }

        public string GameId { get; set; }
    }
}
namespace WikiGame_SP.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using Models.Multiplayer;

    public class GameHub : Hub
    {
        public static Dictionary<string, List<Player>> GameRooms = new Dictionary<string, List<Player>>();
        public static Dictionary<string, Game> Games = new Dictionary<string, Game>();

        public void StartGame(string category, string username)
        {
            var newPlayer = new Player
            {
                Name = username,
                ConnectionId = Context.ConnectionId
            };

            if (GameRooms.ContainsKey(category))
            {
                if (GameRooms[category].Any())
                {
                    var playerOne = GameRooms[category].First();

                    var gameKey = string.Format("{0}_{1}", playerOne.ConnectionId, newPlayer.ConnectionId);

                    var game = new Game
                    {
                        PlayerOne = playerOne,
                        PlayerTwo = newPlayer,
                        StartGame = DateTime.Now,
                        CategoryName = category
                    };

                    Games.Add(gameKey, game);

                    var message = new GameMessage
                    {
                        Type = GameMessage.MessageTypes.GameStart,
                        Category = category,
                        GameId = gameKey
                    };

                    Clients.Caller.send(message);
                    Clients.Client(playerOne.ConnectionId).send(message);

                    GameRooms[category].RemoveAt(0);
                }
                else
                {
                    GameRooms[category].Add(newPlayer);
                }
            }
            else
            {
                GameRooms.Add(category, new List<Player> { newPlayer });
            }
        }

        public void HaveWon()
        {
            string opponentId = GetOpponentConnectionId(Context.ConnectionId);

            if (!string.IsNullOrEmpty(opponentId))
            {
                var message = new GameMessage
                {
                    Type = GameMessage.MessageTypes.YouLost,
                };

                Clients.Client(opponentId).send(message);
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            RemoveGameByPlayer(Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        private void RemoveGameByPlayer(string connectionId)
        {
            foreach (var key in Games.Keys)
            {
                var parts = key.Split('_');

                if (parts[0] == connectionId || parts[1] == connectionId)
                {
                    Games.Remove(key);
                    return;
                }
            }
        }

        private string GetOpponentConnectionId(string connectionId)
        {
            foreach (var key in Games.Keys)
            {
                var parts = key.Split('_');

                if (parts[0] == connectionId)
                {
                    return parts[1];
                }
                else if (parts[1] == connectionId)
                {
                    return parts[0];
                }
            }

            return string.Empty;
        }
    }
}
namespace WikiGame_SP.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using Models.Multiplayer;

    public class GameHub : Hub
    {
        private static Dictionary<string, List<Player>> GameRooms = new Dictionary<string, List<Player>>();
        private static Dictionary<string, Game> Games = new Dictionary<string, Game>();

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

                    Games.Add(string.Format("{0}_{1}", playerOne.ConnectionId, newPlayer.ConnectionId), new Game
                    {
                        PlayerOne = playerOne,
                        PlayerTwo = newPlayer,
                        StartGame = DateTime.Now,
                        CategoryName = category
                    });

                    var message = new GameMessage
                    {
                        Text = "Game started!"
                    };

                    Clients.Caller.send(message);
                    Clients.Client(playerOne.ConnectionId).send(message);
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
    }
}
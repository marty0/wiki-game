WikiGame = {
    BindEvents: function () {
        $(document).on("click", "#SeekGame", function () {
            WikiGame.SignalREvents();
            $("#GamePanel").html("Waiting for an opponent, game will start soon...");
        });

        $(document).on("click", "#SinglePlayer", function () {
            var category = $("#CategoryID").val();

            $.get(Site.getPathToAction("Index", "GameWindow") + "?categoryID=" + category, function (data) {
                $("#GamePanel").html(data);
            });
        });

        $(document).on("click", "#GamePanel a", function (e) {
            e.preventDefault();
            var href = $(this).attr("href");
            $.get(Site.getPathToAction("WikiPage", "GameWindow") + "?article=" + href, function (data) {
                $("#GamePanel").html(data);
                window.scrollTo(0, 0);

                if ($("#YouWon").length) {
                    $(document).trigger("has-won-event");
                }
            });
        });
    },

    SignalREvents: function () {
        var chat = $.connection.gameHub;

        chat.client.send = function (message) {
            if (message.Type == 0) { // If it is a start game
                $("#GamePanel").html("Get ready! Game will start in less than 3 seconds...");

                setTimeout(function () {
                    $.get(Site.getPathToAction("Index", "GameWindow") + "?categoryID=" + message.Category + "&gameId=" + message.GameId, function (data) {
                        $("#GamePanel").html(data);
                    });
                }, 2500);
                
            } else if (message.Type == 1) { // If it is a lost game
                alert("You fought well, but lost :(")
            }
        };

        var category = $("#CategoryID").val();
        var user = $("#Username").text();

        $.connection.hub.start().done(function () {
            chat.server.startGame(category, user);

            $(document).on("has-won-event", function () {
                chat.server.haveWon();
            })
        });
    }
}

$(document).ready(function () {
    WikiGame.BindEvents();
});
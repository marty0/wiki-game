﻿WikiGame = {
    BindEvents: function () {
        $(document).on("click", "#SeekGame", function () {
            WikiGame.SignalREvents();
            $("#GamePanel").html("Waiting for an opponent, game will start soon...");
        });
    },

    SignalREvents: function () {
        var chat = $.connection.gameHub;

        chat.client.send = function (message) {
            alert(message.Text);
            //$('#message').append(RegistryAgency.CreateMessageElement(message));
        };

        var category = $("#CategoryID").val();
        var user = $("#Username").text();

        $.connection.hub.start().done(function () {
            chat.server.startGame(category, user);

            //$('#SubmitDirectory').click(function () {
            //    $('#message').html('');
            //    $("#registry-agency-ajax-loader").show();
            //    $("#StopUpload").show();
            //    showStatus("Спрете работа по системата докато не завърши качването.", "warning");

            //    chat.server.upload($('#directory').val());
            //});

            //$('#StopUpload').click(function () {
            //    $("#registry-agency-ajax-loader").hide();
            //    $("#StopUpload").hide();
            //    chat.server.stopUpload();

            //    showStatus("След приключване на качването на последния файл, операцията ще спре...", "warning");
            //});
        });
    }
}

$(document).ready(function () {
    WikiGame.BindEvents();
});
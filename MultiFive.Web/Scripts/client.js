function ClientViewModel() {
    var self = this;

    self.opponentName = ko.observable();
}

$(document).ready(function () {
    var id = gameId;

    var clientViewModel = new ClientViewModel();
    ko.applyBindings(clientViewModel);

    var serverHooks = {
        
        joined: function (messageData) {
            clientViewModel.opponentName(messageData.senderName);
        }

    };
    
    function pollServer() {

        console.log("polling server...");

        $.ajax({
            url: "game/poll",
            data: {
                gameId: id
            }
        }).success(function (messages) {

            console.log("poll successful. returned object:");
            console.log(messages);
            dispatchMessages(messages);

        }).error(function (xhr, textStatus, errorThrown) {

            console.log(textStatus);
            console.log(errorThrown);

        });
    }

    function dispatchMessages(messages) {

        messages.forEach(function (m) {

            var serverHook = serverHooks[m.messageName];

            if (serverHook) {
                console.log("invoking hooked function for ", m.messageName, "...");
                serverHook(m.messageData);
            } else {
                console.error("hook for ", m.messageName, " not found.");
            }

        });

    }

    var interval = 5; // seconds
    setInterval(pollServer, interval * 1000);
});
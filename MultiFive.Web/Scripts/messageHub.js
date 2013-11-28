function MessageHub(gameId) {

    var self = this;
    
    self.hooks = {
    };

    function pollServer() {

        $.ajax({
            url: "game/poll",
            data: {
                gameId: gameId
            }
        }).success(function (messages) {

            if (messages.length > 0) {
                console.log("poll successful. returned object:", messages);
                dispatchMessages(messages);
            }

        }).error(function (xhr, textStatus, errorThrown) {

            console.error(xhr);
            console.error(textStatus);
            console.error(errorThrown);

        });
    }

    function dispatchMessages(messages) {

        messages.forEach(function (m) {

            var handler = self.hooks[m.messageName];

            if (handler) {
                console.log("invoking handler for", m.messageName, "...");
                handler(m.messageData);
                console.log("handler for", m.messageName, "exited successfully.");
            } else {
                console.error("handler for", m.messageName, "not found.");
            }

        });

    }

    self.listen = function (interval) {
        interval = interval || 5;  // seconds
        setInterval(pollServer, interval * 1000);
    };

}
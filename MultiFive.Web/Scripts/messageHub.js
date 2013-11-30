var MultiFive = (function(ns) {

    ns.MessageHub = function(gameId) {

        var self = this;

        // PRIVATE
        
        var nPoll = 0;

        function pollServer() {

            console.log("poll #", nPoll++, "...");

            var nTry = 0;

            $.ajax({
                url: "game/poll",
                data: {
                    gameId: gameId
                },
                cache: false
            }).success(function(messages) {

                console.log("success #", nTry++, "messages:", messages.length);

                if (messages.length > 0) {
                    console.log("poll successful. returned object:", messages);
                    dispatchMessages(messages);
                }

            }).error(function(xhr, textStatus, errorThrown) {

                console.error(xhr);
                console.error(textStatus);
                console.error(errorThrown);

            });
        }

        function dispatchMessages(messages) {

            messages.forEach(function(m) {

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

        // PUBLIC
        
        self.hooks = {
        };

        self.listen = function(interval) {
            interval = interval || 1; // seconds
            setInterval(pollServer, interval * 1000);
        };

    };

})(MultiFive || {});
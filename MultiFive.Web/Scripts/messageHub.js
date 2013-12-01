var MultiFive = (function(ns) {

    ns.MessageHub = function(gameId) {

        var self = this;

        // PRIVATE
        var pollTrigger;
        var lastMessageId = 0;
        var nPoll = 0;

        function pollServer() {

            console.log("poll #", nPoll++, "...");

            $.ajax({
                url: "message/poll",
                data: {
                    channelId: gameId,
                    lastMessageId: lastMessageId
                },
                cache: false
            }).success(function(messages) {

                console.log("poll successful, messages:", messages);
                dispatchMessages(messages);

            }).error(function(xhr, textStatus, errorThrown) {

                console.error(xhr);
                console.error(textStatus);
                console.error(errorThrown);

            }).complete(function(xhr, status) {
                pollTrigger();
            });
        }

        function dispatchMessages(messages) {

            messages.forEach(function (msg) {

                console.log("dispatching message #", msg.id);

                var handler = self.hooks[msg.name];

                if (handler) {
                    
                    console.log("handler for", msg.name, " found. invoking...");

                    var content = JSON.parse(msg.content);
                    handler(content);
                    
                    console.log("handler for", msg.name, "exited successfully.");
                    
                } else {
                    console.error("handler for", msg.name, "not found.");
                }

                // do not poll for messages already handled
                if (msg.id > lastMessageId)
                    lastMessageId = msg.id;

            });
            
        }

        // PUBLIC
        
        self.hooks = {
        };

        self.listen = function(interval) {
            interval = interval || 1; // seconds
            pollTrigger = function () { setTimeout(pollServer, interval * 1000); };
            pollTrigger();
        };

    };
    
    return ns;
    
})(MultiFive || {});
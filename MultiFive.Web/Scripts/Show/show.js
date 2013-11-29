// codebehind script for /game/show
// module pattern

var MultiFive = (function(ns) {

    // TODO: Replace with non-ctor function?
    function ViewModel(player1Name, player2Name) {

        var self = this;

        self.player1Name = ko.observable(player1Name);
        self.player2Name = ko.observable(player2Name);

        return self;
        
    }

    ns.show = function(gameId, player1Name, player2Name) {

        var viewModel = new ViewModel(player1Name, player2Name);
        ko.applyBindings(viewModel);

        var messageHub = new MessageHub(gameId);

        messageHub.hooks.joined = function(messageData) {

            viewModel.player2Name(messageData.senderName);

        };

        messageHub.listen();

    };

    return ns;
    
})(MultiFive || {});
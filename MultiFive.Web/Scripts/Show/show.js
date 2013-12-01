// codebehind script for /game/show
// module pattern

var MultiFive = (function(ns) {

    function ViewModel(player1Name, player2Name) {
        this.player1Name = ko.observable(player1Name);
        this.player2Name = ko.observable(player2Name);
    }

    ns.show = function(gameId, player1Name, player2Name) {

        var viewModel = new ViewModel(player1Name, player2Name);
        ko.applyBindings(viewModel);

        var messageHub = new ns.MessageHub(gameId);

        messageHub.hooks.joined = function(messageData) {

            viewModel.player2Name(messageData.joinedPlayerName);

        };

        messageHub.listen();

    };

    return ns;
    
})(MultiFive || {});
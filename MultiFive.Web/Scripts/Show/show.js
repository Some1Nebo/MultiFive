// codebehind script for /game/show

function ViewModel(player1Name, player2Name) {
 
    var self = this;

    self.player1Name = ko.observable(player1Name);
    self.player2Name = ko.observable(player2Name);
    
    return self;
}

function init(data) {

    var viewModel = new ViewModel(data.player1Name, data.player2Name);
    ko.applyBindings(viewModel);
    
    var messageHub = new MessageHub(data.gameId);

    messageHub.hooks.joined = function(messageData) {

        viewModel.player2Name(messageData.senderName);

    };

    messageHub.listen();

}
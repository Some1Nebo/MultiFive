// codebehind script for /game/show
// module pattern

var MultiFive = (function (ns) {

    function ViewModel(gameData, cellClicked) {
        var self = this;
        
        self.player1Name = ko.observable(gameData.player1Name);
        self.player2Name = ko.observable(gameData.player2Name);

        self.field = [];

        var cellView = {
            "0": "&nbsp;",
            "1": "X",
            "2": "O"
        };
        
        for (var i = 0; i < gameData.height; ++i) {
            var row = [];

            for (var j = 0; j < gameData.width; ++j) {
                var initValue = cellView[gameData.fieldData[i * gameData.width + j]];
                row.push(ko.observable(initValue));
            }

            self.field.push(row);
        }

        self.cellClicked = cellClicked;
        
        return self;
    }

    ns.show = function (gameData) {
        var self = this;

        var cellClicked = function (r, c) {
            viewModel.field[r][c]("X");
        };

        var viewModel = new ViewModel(gameData, cellClicked);
        ko.applyBindings(viewModel);

        var messageHub = new ns.MessageHub(gameData.gameId, gameData.lastMessageId);

        messageHub.hooks.joined = function(messageData) {

            viewModel.player2Name(messageData.joinedPlayerName);

        };
        
        messageHub.listen();

        return self;
    };

    return ns;
    
})(MultiFive || {});
// codebehind script for /game/show
// module pattern

var MultiFive = (function (ns) {
    
    // constants and enums
    var emptyCellSymbol = "&nbsp;";
    var player1Symbol = "X";
    var player2Symbol = "O";

    var GameState = {
        NotStarted: "NotStarted",
        Player1Move: "Player1Move",
        Player2Move: "Player2Move",
        Player1Win: "Player1Win",
        Player2Win: "Player2Win",
        Draw: "Draw"
    };
    
    var PlayerRole = {
        Spectator: "Spectator",
        Player1: "Player1",
        Player2: "Player2"
    };
    
    // main
    function ViewModel(gameData, cellClicked) {
        var self = this;
        
        self.player1Name = ko.observable(gameData.player1Name);
        self.player2Name = ko.observable(gameData.player2Name);

        self.field = [];

        var cellView = {
            "0": emptyCellSymbol,
            "1": player1Symbol,
            "2": player2Symbol
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

        self.gameState = ko.observable(gameData.gameState);

        function chooseCssFor(state)
        {
            return self.gameState() == state ? "playerActive" : null;
        }

        self.player1Active = ko.computed(function () {
            return chooseCssFor(GameState.Player1Move);
        });
        
        self.player2Active = ko.computed(function () {
            return chooseCssFor(GameState.Player2Move);
        });
        
        return self;
    }

    ns.show = function (gameData) {
        var self = this;

        console.log(gameData);
        
        // View model setup
        var viewModel = new ViewModel(gameData, function (r, c) {
            if (stateMachine.currentState.cellClicked)
                stateMachine.currentState.cellClicked(r, c);
        });

        ko.applyBindings(viewModel);
        
        // State machine setup
        var unlocked = new MultiFive.State({                
            onEntry: function() {
                console.log("unlocked.onEntry");
            },
            onExit: function () {
                console.log("unlocked.onExit");
            }
        });

        var myMove = new MultiFive.State({
            onEntry: function () {
                console.log("myMove.onEntry");
            },
            onExit: function () {
                console.log("myMove.onExit");
            }
        });
        
        myMove.cellClicked = function (row, col) {
            var symbol = gameData.playerRole == PlayerRole.Player1 ? player1Symbol : player2Symbol;
            viewModel.field[row][col](symbol);
            stateMachine.fire("move");
            
            // send ajax to server
        };
        
        var notMyMove = new MultiFive.State({
            onEntry: function () {
                console.log("notMyMove.onEntry");
            },
            onExit: function () {
                console.log("notMyMove.onExit");
            }
        });

        var chooseState = function () {
            // not the cleanest condition, but avoids some code-repetition
            return (
                    ((gameData.playerRole == PlayerRole.Player1) && viewModel.player1Active()) ||
                    ((gameData.playerRole == PlayerRole.Player2) && viewModel.player2Active())
                   )
                ? myMove
                : notMyMove;
        };

        unlocked.permitDynamic("lock", chooseState);
        notMyMove.permitDynamic("move", chooseState);
        myMove.permit("move", notMyMove);

        var initialState = gameData.gameState == GameState.NotStarted ? unlocked : chooseState();

        var stateMachine = new MultiFive.StateMachine(initialState);

        // Message hub setup
        var messageHub = new ns.MessageHub(gameData.gameId, gameData.lastMessageId);

        messageHub.hooks.joined = function(messageData) {

            viewModel.player2Name(messageData.joinedPlayerName);
            viewModel.gameState(messageData.gameState);
            
            stateMachine.fire("lock");
            
        };
        
        messageHub.listen();
        
        return self;
    };

    return ns;
    
})(MultiFive || {});
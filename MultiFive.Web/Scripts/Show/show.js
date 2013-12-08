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
        
        // State machine setup
        var currentSymbol = "";

        var unlockedState = new MultiFive.State({
            onEntry: function() {
                console.log("entered unlocked state");
            },
            onExit: function () {
                console.log("exited unlocked state");
            },
        });

        var p1MoveState = new MultiFive.State({
            onEntry: function() {
                console.log("entered p1MoveState");
                currentSymbol = "X";
            },
            onExit: function() {
                console.log("exited p1MoveState");
            }
        });

        var p2MoveState = new MultiFive.State({
                onEntry: function() {
                    console.log("entered p2MoveState");
                    currentSymbol = "O";
                },
                onExit: function() {
                    console.log("exited p2MoveState");
                }
            }
        );

        unlockedState.permitDynamic("lock", function() {
            if (gameData.me == "Player1")
                return p1MoveState;
            else if (gameData.me == "Player2")
                return p2MoveState;
            
            return null;
        });
        
        p1MoveState.permit("move", p2MoveState);
        p2MoveState.permit("move", p1MoveState);

        var stateMachine = new MultiFive.StateMachine(unlockedState);

        // End of state machine setup

        var cellClicked = function (r, c) {
            viewModel.field[r][c](currentSymbol);
            stateMachine.fire("move");
        };

        var viewModel = new ViewModel(gameData, cellClicked);
        ko.applyBindings(viewModel);

        // Message hub setup
        var messageHub = new ns.MessageHub(gameData.gameId, gameData.lastMessageId);

        messageHub.hooks.joined = function(messageData) {

            viewModel.player2Name(messageData.joinedPlayerName);
            stateMachine.fire("lock");

        };
        
        messageHub.listen();
        
        // End of message hub setup

        return self;
    };

    return ns;
    
})(MultiFive || {});
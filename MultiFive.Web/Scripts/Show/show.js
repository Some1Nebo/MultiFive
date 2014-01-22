// codebehind script for /game/show
// module pattern

var MultiFive = (function (ns) {
    
    // constants and enums
    var Symbol = {
        Empty: "&nbsp;",
        Player1: "X",
        Player2: "O"
    };

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

    // ctor    
    ns.showForm = function (gameData) {
        var self = this;
        
        console.log(gameData);

        // Set up data bindings
        self.player1Name = ko.observable(gameData.player1Name);
        self.player2Name = ko.observable(gameData.player2Name);

        self.field = [];

        var cellView = {
            "0": Symbol.Empty,
            "1": Symbol.Player1,
            "2": Symbol.Player2
        };

        for (var i = 0; i < gameData.height; ++i) {
            var row = [];

            for (var j = 0; j < gameData.width; ++j) {
                var initValue = cellView[gameData.fieldData[i * gameData.width + j]];
                row.push(ko.observable(initValue));
            }

            self.field.push(row);
        }

        self.cellClicked = function(r, c) {
            if (stateMachine.currentState.cellClicked)
                stateMachine.currentState.cellClicked(r, c);
        };

        self.gameState = ko.observable(gameData.gameState);

        self.player1Active = ko.computed(function () {
            return self.gameState() == GameState.Player1Move;
        });

        self.player2Active = ko.computed(function () {
            console.log(self.gameState());
            return self.gameState() == GameState.Player2Move;
        });

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

        myMove.cellClicked = function(r, c) {

            if (self.field[r][c]() == Symbol.Empty) {
                
                var newSymbol = gameData.playerRole == PlayerRole.Player1
                    ? Symbol.Player1
                    : Symbol.Player2;

                var previousSymbol = self.field[r][c]();
                
                self.field[r][c](newSymbol);
                stateMachine.fire("move");
                
                var moveUrl = "game/move/" + gameData.gameId;
                
                // send ajax to server
                $.ajax({
                    url: moveUrl,
                    data: {
                        gameId: gameData.gameId,
                        row: r,
                        col: c
                    },
                    cache: false
                }).error(function (xhr, textStatus, errorThrown) {

                    console.error("call failed, resetting move");
                    
                    self.field[r][c](previousSymbol);

                    console.error(xhr);
                    console.error(textStatus);
                    console.error(errorThrown);

                });
            }

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
            return (
                    ((gameData.playerRole == PlayerRole.Player1) && self.player1Active()) ||
                    ((gameData.playerRole == PlayerRole.Player2) && self.player2Active())
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

            self.player2Name(messageData.joinedPlayerName);
            self.gameState(messageData.gameState);
            
            stateMachine.fire("lock");
            
        };

        messageHub.hooks.moved = function (messageData) {
            
            if (messageData.playerRole == PlayerRole.Player1) {
                self.field[messageData.row][messageData.col](Symbol.Player1);
            }
            else if (messageData.playerRole == PlayerRole.Player2) {
                self.field[messageData.row][messageData.col](Symbol.Player2);
            }
            
            self.gameState(messageData.gameState);
            
            stateMachine.fire("move");
        };

        ko.applyBindings(self);
       
        messageHub.listen();
        
        return self;
    };

    return ns;
    
})(MultiFive || {});
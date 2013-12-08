test("Initial state should fire onEntry", function () {

    var initialState = new MultiFive.State({
        onEntry: function() { ok(1, "onEntry called"); }
    });
    
    var stateMachine = new MultiFive.StateMachine(initialState);
});

test("State should transfer on trigger", function () {

    var state1 = new MultiFive.State();
    
    var state2 = new MultiFive.State({
        onEntry: function() { ok(1, "State 2 entered"); }
    });

    state1.permit("move", state2);

    var stateMachine = new MultiFive.StateMachine(state1);
    stateMachine.fire("move");
});

test("State should not transfer on invalid trigger", function () {

    var state1 = new MultiFive.State();

    var state2 = new MultiFive.State({
        onEntry: function () { ok(1, "State 2 entered"); }
    });

    state1.permit("move", state2);

    var stateMachine = new MultiFive.StateMachine(state1);
    stateMachine.fire("move1");

    equal(stateMachine.currentState, state1, "current state is still state1");
});

test("State should be reentrant several times", function () {

    var n = 0;

    var state1 = new MultiFive.State({
        onEntry: function () { ++n; }
    });

    var state2 = new MultiFive.State({
        onEntry: function () { ok(1, "State 2 entered"); }
    });

    state1.permit("move", state2);
    state2.permit("move", state1);

    var stateMachine = new MultiFive.StateMachine(state1);
    stateMachine.fire("move");
    stateMachine.fire("move");

    equal(n, 2, "State 1 entered 2 times");
});

test("State should be self-reentrant", function () {

    var n = 0;

    var state = new MultiFive.State({
        onEntry: function () { ++n; }
    });

    state.permit("move", state);

    var stateMachine = new MultiFive.StateMachine(state);
    stateMachine.fire("move");
    stateMachine.fire("move");

    equal(n, 3, "State entered 3 times");
});

test("State for permitDynamic should be chosen correctly", function () {

    var stateA = new MultiFive.State();
    var stateB = new MultiFive.State();
    var stateC = new MultiFive.State();

    stateA.permitDynamic("move", function(x) {
        if (x == "B") return stateB;
        else if (x == "C") return stateC;
        else return null;
    });

    stateC.permit("move", stateA);

    var stateMachine = new MultiFive.StateMachine(stateA);
    stateMachine.fire("move", "C"); // A -> C

    equal(stateC, stateMachine.currentState, "State C entered correctly");

    stateMachine.fire("move"); // C -> A
    stateMachine.fire("move", "B"); // A -> B
    
    equal(stateB, stateMachine.currentState, "State B entered correctly");
});
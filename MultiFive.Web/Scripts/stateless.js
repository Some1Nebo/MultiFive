var MultiFive = (function(ns) {

    ns.State = function(setup) {
        var self = this;

        self.onEntry = function () { if (setup.onEntry) setup.onEntry(); };

        self.onExit = function () { if (setup.onExit) setup.onExit(); };

        self.handlers = {};

        self.permit = function(trigger, state) {
            self.handlers[trigger] = state;
        };

        self.permitDynamic = function(trigger, stateSelector) {
            self.handlers[trigger] = stateSelector;
        };

        self.on = function(trigger, params) {

            var h = self.handlers[trigger];

            if (h) {
                if (h instanceof ns.State) {
                    return h;
                } else {
                    return h(params);
                }
            }

            return null;
        };

        return self;
    };

    ns.StateMachine = function(initialState) {

        var self = this;

        initialState.onEntry();

        self.currentState = initialState;
        self.states = {};

        self.fire = function(trigger, params) {

            var newState = self.currentState.on(trigger, params);

            if (newState) {

                if (self.currentState.onExit)
                    self.currentState.onExit();

                if (newState.onEntry)
                    newState.onEntry();

                self.currentState = newState;
            }
        };

        return self;
    };

    return ns;

})(MultiFive || {});
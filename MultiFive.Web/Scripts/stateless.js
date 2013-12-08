var MultiFive = (function (ns) {
    
    ns.State = function(setup) {
        var self = this;

        self.onEntry = function() {
            if (setup && setup.onEntry)
                setup.onEntry();
        };

        self.onExit = function() {
            if (setup && setup.onExit)
                setup.onExit();
        };

        self.handlers = {};

        self.permit = function (trigger, destinationState) {
            /// <summary>
            /// Accept the specified trigger and transition to the destination state.
            /// </summary>
            /// <param name="trigger">The accepted trigger.</param>
            /// <param name="destinationState">The state that the trigger will cause a transition to.</param>
            /// <returns>The reciever.</returns>
            
            self.handlers[trigger] = destinationState;
            return self;
        };

        self.permitDynamic = function (trigger, destinationStateSelector) {
            /// <summary>
            /// Accept the specified trigger and transition to the destination state, calculated
            /// dynamically by the supplied function.
            /// </summary>
            /// <param name="trigger">The accepted trigger.</param>
            /// <param name="destinationStateSelector">Function to calculate the state that the trigger will cause a transition to.</param>
            /// <returns>The reciever.</returns>
            
            self.handlers[trigger] = destinationStateSelector;
            return self;
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

        self.fire = function(trigger, p) {
            /// <summary>
            /// Transition from the current state via the specified trigger.
            /// The target state is determined by the configuration of the current state.
            /// Actions associated with leaving the current state and entering the new one
            /// will be invoked.
            /// </summary>
            /// <param name="trigger">The trigger to fire.</param>
            /// <param name="p">The argument to be passed to state selector (only implemented for permitDynamic).</param>

            var newState = self.currentState.on(trigger, p);

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
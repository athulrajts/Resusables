using System;
using System.Collections.Generic;

namespace KEI.Infrastructure.StateMachine
{
    internal class StateInfo<TState,TTrigger>
    {
        public StateInfo(TState state)
        {
            StateIdentifier = state;
            EntryActions = new List<Action>();
            ExitActions = new List<Action>();
            Transitions = new Transitions<TState, TTrigger>();
        }
        public TState StateIdentifier { get; }
        public List<Action> EntryActions { get; }
        public List<Action> ExitActions { get; }
        internal Transitions<TState,TTrigger> Transitions { get; }
    }
}

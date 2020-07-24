using System.Collections.Generic;

namespace KEI.Infrastructure.StateMachine
{
    internal class Transition<TState, TTrigger>
    {
        public TState SourceState { get; }
        public TState TargetState { get; }
        public TTrigger Trigger { get;  }
        public Transition(TState from, TState to, TTrigger trigger)
        {
            SourceState = from;
            TargetState = to;
            Trigger = trigger;
        }
    }

    internal class Transitions<TState,TTrigger> : List<Transition<TState, TTrigger>> { }
}

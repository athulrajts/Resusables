using KEI.Infrastructure.StateMachine.Interfaces;
using System;

namespace KEI.Infrastructure.StateMachine
{
    internal class StateConfigurator<TState, TTrigger> : IStateConfigurator<TState, TTrigger>
    {
        private readonly StateInfo<TState,TTrigger> _stateInfo;

        public StateConfigurator(StateInfo<TState,TTrigger> state)
        {
            _stateInfo = state;
        }

        public IStateConfigurator<TState, TTrigger> Allow(TTrigger trigger, TState targetState)
        {
            throw new NotImplementedException();
        }

        public IStateConfigurator<TState, TTrigger> OnEntry(Action entryAction)
        {
            _stateInfo.EntryActions.Add(entryAction);
            return this;
        }

        public IStateConfigurator<TState, TTrigger> OnExit(Action exitAction)
        {
            _stateInfo.ExitActions.Add(exitAction);
            return this;
        }
    }
}

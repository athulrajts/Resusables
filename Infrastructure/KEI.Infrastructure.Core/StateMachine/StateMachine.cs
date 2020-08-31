using KEI.Infrastructure.StateMachine.Interfaces;
using System;
using System.Collections.Generic;

namespace KEI.Infrastructure.StateMachine
{
    public partial class StateMachine<TState, TTrigger> : IStateMachine<TState, TTrigger>
    {
        private readonly Func<TState> _stateAccessor;
        private readonly Action<TState> _stateMutator;
        private readonly Dictionary<TState, StateInfo<TState,TTrigger>> _states = new Dictionary<TState, StateInfo<TState, TTrigger>>();
        private readonly ILogger _logger;

        public StateMachine(Func<TState> stateAccessor, Action<TState> stateMutator, ILogger logger)
        {
            _stateAccessor = stateAccessor ?? throw new ArgumentNullException(nameof(stateAccessor));
            _stateMutator = stateMutator ?? throw new ArgumentNullException(nameof(stateMutator));
            _logger = logger;
        }

        public TState CurrentState
        {
            get => _stateAccessor();
            private set
            {
                if(EqualityComparer<TState>.Default.Equals(_stateAccessor(), value) == false)
                {
                    PreviousState = _stateAccessor();
                    _stateMutator(value);
                }
            }
        }
        public TState PreviousState { get; private set; }

        public IStateConfigurator<TState,TTrigger> ConfigureState(TState state)
        {
            if(_states.ContainsKey(state))
            {
                throw new InvalidOperationException($"{state} is already defined");
            }

            _states.Add(state, new StateInfo<TState, TTrigger>(state));

            return new StateConfigurator<TState,TTrigger>(_states[state]);
        }

        public bool ExecuteTransition(TTrigger trigger)
        {
            var transition = GetTransition(trigger);

            if (transition is null)
            {
                _logger.Warn($"No trasitions exists for the trigger : {trigger} from state : {CurrentState}");
            }

            _states[CurrentState].ExitActions.ForEach(action => action());

            CurrentState = transition.TargetState;

            _states[CurrentState].EntryActions.ForEach(action => action());

            return true;
        }

        private Transition<TState, TTrigger> GetTransition(TTrigger trigger)
        {
            return _states[CurrentState].Transitions.Find(x => EqualityComparer<TTrigger>.Default.Equals(x.Trigger, trigger) && EqualityComparer<TState>.Default.Equals(x.SourceState, CurrentState));
        }

    }
}

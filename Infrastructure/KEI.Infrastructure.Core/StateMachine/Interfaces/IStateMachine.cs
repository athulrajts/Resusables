using KEI.Infrastructure.StateMachine.Interfaces;

namespace KEI.Infrastructure.StateMachine
{
    public interface IStateMachine<TState,TTrigger>
    {
        TState CurrentState { get; }
        TState PreviousState { get; }
        IStateConfigurator<TState,TTrigger> ConfigureState(TState state);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEI.Infrastructure.StateMachine.Interfaces
{
    public interface IStateConfigurator<TState,TTrigger>
    {
        IStateConfigurator<TState, TTrigger> Allow(TTrigger trigger, TState targetState);
        IStateConfigurator<TState, TTrigger> OnEntry(Action entryAction);
        IStateConfigurator<TState, TTrigger> OnExit(Action exitAction);
    }
}

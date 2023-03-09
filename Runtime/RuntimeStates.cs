using SH.StateMachine.Nodes;

namespace SH.StateMachine
{
    public class RuntimeStates
    {
        public readonly StateNode DefaultState;
        public readonly StateNode StartState;
        public StateNode CurrentState { get; private set; }

        public event System.Action<StateNode, StateNode> OnCurrentStateChanged;

        public RuntimeStates(StateNode defaultState, StateNode startState)
        {
            DefaultState = defaultState;
            StartState = startState;
            CurrentState = startState;
        }

        public void SetCurrentState(StateNode stateNode)
        {
            OnCurrentStateChanged?.Invoke(CurrentState, stateNode);
            CurrentState = stateNode;
        }
    }
}
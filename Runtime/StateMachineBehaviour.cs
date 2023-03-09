using UnityEngine;
using SH.StateMachine.Nodes;

namespace SH.StateMachine
{
    public abstract class StateMachineBehaviour<T> : MonoBehaviour where T : Object
    {
        [SerializeField] private StateMachine stateMachine = null;
        [SerializeField] private T origin = null;

        private RuntimeStates runtimeStates;

        public void Init()
        {
            runtimeStates = stateMachine.GetRuntimeStateMachineInstance();
            runtimeStates.CurrentState.OnStart(origin);
        }

        public void Stop()
        {
            runtimeStates.CurrentState.OnEnd(origin);
            enabled = false;
        }

        public void Resume()
        {
            runtimeStates.CurrentState.OnStart(origin);
            enabled = true;
        }

        public void Restart()
        {
            runtimeStates.SetCurrentState(runtimeStates.StartState);
            runtimeStates.CurrentState.OnStart(origin);
            enabled = true;
        }

        private void Update()
        {
            bool status = runtimeStates.CurrentState.OnUpdate(origin);

            foreach (TransitionNode current in runtimeStates.CurrentState.Outputs)
                if ((status || current.ForceTransition) && current.CheckConditions(origin))
                {
                    ChangeState(current.Outputs[0] is StateNode state ? state : runtimeStates.DefaultState);
                    break;
                }
        }

        private void ChangeState(StateNode nextState)
        {
            runtimeStates.CurrentState.OnEnd(origin);
            runtimeStates.SetCurrentState(nextState);
            runtimeStates.CurrentState.OnStart(origin);
        }
    }
}
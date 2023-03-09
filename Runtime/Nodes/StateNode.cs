using System.Collections.Generic;
using UnityEngine;
using SH.StateMachine.Actions;

namespace SH.StateMachine.Nodes
{
    public class StateNode : BaseNode
    {
        [SerializeReference] private List<BaseAction> actions = new();

        private float startTime;

        public float ElapsedTime => Time.time - startTime;

        public void OnStart<T>(T origin)
        {
            startTime = Time.time;

            foreach (BaseAction action in actions)
                action.OnStart(origin);
        }

        public bool OnUpdate<T>(T origin)
        {
            bool status = true;

            foreach (BaseAction action in actions)
                status &= action.OnUpdate(origin, ElapsedTime);

            return status;
        }

        public void OnEnd<T>(T origin)
        {
            foreach (BaseAction action in actions)
                action.OnEnd(origin);
        }
    }
}
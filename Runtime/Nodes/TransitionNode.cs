using System.Collections.Generic;
using UnityEngine;
using SH.StateMachine.Conditions;

namespace SH.StateMachine.Nodes
{
    public class TransitionNode : BaseNode
    {
        [SerializeField] private bool forceTransition = false;
        [SerializeReference] private List<BaseCondition> conditions = new();

        public bool ForceTransition => forceTransition;

        public bool CheckConditions<T>(T origin)
        {
            foreach (BaseCondition condition in conditions)
                if (!condition.Check(origin))
                    return false;

            return true;
        }
    }
}
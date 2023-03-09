using System.Collections.Generic;
using UnityEngine;
using SH.StateMachine.Nodes;
using SH.StateMachine.Attributes;

namespace SH.StateMachine
{
    [CreateAssetMenu(menuName = "SH/State Machine", fileName = "New State Machine")]
    public class StateMachine : ScriptableObject
    {
        [SerializeField, ReadOnly] private StateNode defaultState = null;
        [SerializeField, ReadOnly] private StateNode startState = null;
        [SerializeField, HideInInspector] private Vector3 graphViewPosition = Vector3.zero;
        [SerializeField, HideInInspector] private Vector3 graphViewScale = Vector3.one;

        public StateNode DefaultState => defaultState;
        public StateNode StartState => startState;

        public RuntimeStates GetRuntimeStateMachineInstance()
        {
            Dictionary<BaseNode, BaseNode> nodes = new();
            BaseNode copiedDefaultState = CopyAllNodes(nodes, defaultState);
            BaseNode copiedStartState = (startState != null && !nodes.ContainsKey(startState)) ? CopyAllNodes(nodes, startState) : copiedStartState = copiedDefaultState; ;

            return new RuntimeStates((StateNode)copiedDefaultState, (StateNode)copiedStartState);
        }

        private BaseNode CopyAllNodes(Dictionary<BaseNode, BaseNode> nodes, BaseNode origin)
        {
            if (nodes.ContainsKey(origin))
                return null;

            BaseNode copiedParent = Instantiate(origin);
            copiedParent.name = $"{copiedParent.name}_Instance";
            nodes.Add(origin, copiedParent);

            BaseNode copiedChild = null;
            for (int i = 0; i < origin.Outputs.Count; i++)
            {
                copiedChild = CopyAllNodes(nodes, origin.Outputs[i]);
                if (copiedChild != null)
                    copiedParent.SetOutputNode(copiedChild, i);
                else
                    copiedParent.SetOutputNode(nodes[origin.Outputs[i]], i);
            }

            return copiedParent;
        }
    }
}
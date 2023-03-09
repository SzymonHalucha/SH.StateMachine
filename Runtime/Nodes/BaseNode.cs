using System.Collections.Generic;
using UnityEngine;

namespace SH.StateMachine.Nodes
{
    public class BaseNode : ScriptableObject
    {
        [SerializeField, HideInInspector] protected List<BaseNode> outputs = new();
        [SerializeField, HideInInspector] protected Vector2 nodePosition = Vector3.zero;

        public virtual IReadOnlyList<BaseNode> Outputs => outputs;

        public virtual void SetNodePosition(Vector2 position) => nodePosition = position;
        public virtual void AddOutputNode(BaseNode node) => outputs.Add(node);
        public virtual void SetOutputNode(BaseNode node, int index) => outputs[index] = node;
        public virtual void RemoveOutputNode(BaseNode node) => outputs.Remove(node);
    }
}
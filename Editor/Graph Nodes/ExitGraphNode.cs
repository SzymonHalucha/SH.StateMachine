using UnityEngine;
using UnityEngine.UIElements;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public class ExitGraphNode : BaseGraphNode
    {
        public ExitGraphNode(StateMachine parent, Vector2 position, bool isRuntime)
        {
            isInstancedRuntime = isRuntime;
            CreateScriptableObject<ExitNode>(position, parent);

            AddTitleInputField("Exit");

            CreateInputPort(typeof(TransitionGraphNode), "Result");
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(position, Vector2.zero));
            SaveScriptableObjectName();
        }

        public ExitGraphNode(ExitNode targetNode, bool isRuntime)
        {
            target = targetNode;
            isInstancedRuntime = isRuntime;

            AddTitleInputField();

            CreateInputPort(typeof(TransitionGraphNode), "Result");
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(GetNodePosition(), Vector2.zero));
            SaveScriptableObjectName();
        }
    }
}
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public class SeparatorGraphNode : BaseGraphNode
    {
        public SeparatorGraphNode(StateMachine parent, Vector2 position, bool isRuntime)
        {
            isInstancedRuntime = isRuntime;
            CreateScriptableObject<SeparatorNode>(position, parent);

            CreateInputPort(typeof(BaseGraphNode), "");
            CreateOutputPort(typeof(BaseGraphNode), "");
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(position, Vector2.zero));
            SaveScriptableObjectName();
        }

        public SeparatorGraphNode(SeparatorNode targetNode, bool isRuntime)
        {
            target = targetNode;
            isInstancedRuntime = isRuntime;

            CreateInputPort(typeof(BaseGraphNode), "");
            CreateOutputPort(typeof(BaseGraphNode), "");
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(GetNodePosition(), Vector2.zero));
            SaveScriptableObjectName();
        }

        public void SetInputOutputPortType(System.Type type)
        {
            (outputContainer[0] as Port).portType = type;
            (inputContainer[0] as Port).portType = type;

            (outputContainer[0] as Port).portName = "";
            (inputContainer[0] as Port).portName = "";
            RefreshExpandedState();
            RefreshPorts();
        }
    }
}
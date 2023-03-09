using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public class StartGraphNode : BaseGraphNode
    {
        public StartGraphNode(StateMachine parent, Vector2 position, bool isRuntime)
        {
            title = "Start";
            isInstancedRuntime = isRuntime;
            CreateScriptableObject<StartNode>(position, parent);
            outputsPortName = new string[] { "Default State", "Start State" };

            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;

            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(0));
            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(1));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(position, Vector2.zero));
            SaveScriptableObjectName(title);
        }

        public StartGraphNode(StartNode targetNode, bool isRuntime)
        {
            title = "Start";
            target = targetNode;
            isInstancedRuntime = isRuntime;
            outputsPortName = new string[] { "Default State", "Start State" };

            capabilities &= ~Capabilities.Movable;
            capabilities &= ~Capabilities.Deletable;

            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(0));
            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(1));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(GetNodePosition(), Vector2.zero));
            SaveScriptableObjectName(title);
        }

        public override void SaveScriptableObjectName()
        {
            SaveScriptableObjectName(title);
        }
    }
}
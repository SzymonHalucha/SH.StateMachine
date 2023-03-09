using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public class StateGraphNode : BaseGraphNode
    {
        public StateGraphNode(StateMachine parent, Vector2 position, bool isRuntime)
        {
            isInstancedRuntime = isRuntime;
            CreateScriptableObject<StateNode>(position, parent);

            AddSelectionButton();
            AddTitleInputField("New State");

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(target);
            extensionContainer.Add(new IMGUIContainer(() => editor.OnInspectorGUI()));

            CreateInputPort(typeof(TransitionGraphNode), "In", Port.Capacity.Multi);
            CreateOutputPort(typeof(TransitionGraphNode), GetOutputPortName(0));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(position, Vector2.zero));
            SaveScriptableObjectName();
        }

        public StateGraphNode(StateNode targetNode, bool isRuntime)
        {
            target = targetNode;
            isInstancedRuntime = isRuntime;

            AddSelectionButton();
            AddTitleInputField();

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(target);
            extensionContainer.Add(new IMGUIContainer(() => editor.OnInspectorGUI()));

            CreateInputPort(typeof(TransitionGraphNode), "In", Port.Capacity.Multi);
            CreateOutputPort(typeof(TransitionGraphNode), GetOutputPortName(0));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(GetNodePosition(), Vector2.zero));
            SaveScriptableObjectName();
        }

        public override void RemoveOutputPort(Port port)
        {
            if (outputContainer.childCount > 1)
                outputContainer.Remove(port);
        }

        public void UpdateOutputPort()
        {
            if (((Port)outputContainer[outputContainer.childCount - 1]).connected)
                CreateOutputPort(((Port)outputContainer[0]).portType, GetOutputPortName(outputContainer.childCount));
        }
    }
}
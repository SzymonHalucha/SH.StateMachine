using UnityEngine;
using UnityEngine.UIElements;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public class TransitionGraphNode : BaseGraphNode
    {
        public TransitionGraphNode(StateMachine parent, Vector2 position, bool isRuntime)
        {
            isInstancedRuntime = isRuntime;
            CreateScriptableObject<TransitionNode>(position, parent);

            AddSelectionButton();
            AddTitleInputField("New Transition");

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(target);
            extensionContainer.Add(new IMGUIContainer(() => editor.OnInspectorGUI()));

            CreateInputPort(typeof(StateGraphNode), "In");
            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(0));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(position, Vector2.zero));
            SaveScriptableObjectName();
        }

        public TransitionGraphNode(TransitionNode targetNode, bool isRuntime)
        {
            target = targetNode;
            isInstancedRuntime = isRuntime;

            AddSelectionButton();
            AddTitleInputField();

            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(target);
            extensionContainer.Add(new IMGUIContainer(() => editor.OnInspectorGUI()));

            CreateInputPort(typeof(StateGraphNode), "In");
            CreateOutputPort(typeof(StateGraphNode), GetOutputPortName(0));
            RefreshExpandedState();
            RefreshPorts();

            SetPosition(new Rect(GetNodePosition(), Vector2.zero));
            SaveScriptableObjectName();
        }
    }
}
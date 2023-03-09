using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using SH.StateMachine.Nodes;
using SH.StateMachine.Editor.GraphNodes;

namespace SH.StateMachine.Editor
{
    public class StateMachineGraphView : GraphView
    {
        private readonly StateMachine target;
        private readonly RuntimeStates runtimeStates;

        private bool IsRuntime => runtimeStates != null;

        public StateMachineGraphView(StateMachine stateMachine)
        {
            target = stateMachine;
            this.StretchToParentSize();
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new();
            Add(grid);
            grid.SendToBack();
            grid.StretchToParentSize();

            GenerateToolbar();
            RefreshGraphView();
        }

        public StateMachineGraphView(RuntimeStates states)
        {
            this.runtimeStates = states;
            this.StretchToParentSize();
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            GridBackground grid = new();
            Add(grid);
            grid.SendToBack();
            grid.StretchToParentSize();

            GenerateToolbar();
            RefreshGraphView();

            runtimeStates.OnCurrentStateChanged += SetColorForRuntimeCurrentState;
            SetColorForRuntimeCurrentState(null, runtimeStates.CurrentState);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent e)
        {
            if (IsRuntime)
                return;

            Vector2 mousePosition = this.viewTransform.matrix.inverse.MultiplyPoint(e.localMousePosition);
            e.menu.AppendAction("State", (e) => AddElement(new StateGraphNode(target, mousePosition, IsRuntime)));
            e.menu.AppendAction("Transition", (e) => AddElement(new TransitionGraphNode(target, mousePosition, IsRuntime)));
            // e.menu.AppendAction("Separator", (e) => AddElement(new SeparatorGraphNode(target, mousePosition, isRuntime)));
            e.menu.AppendAction("Exit", (e) => AddElement(new ExitGraphNode(target, mousePosition, IsRuntime)));
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.Where((port) => startPort != port
                                         && startPort.direction != port.direction
                                         && startPort.node != port.node
                                         && startPort.portType != port.portType)
                                         .ToList();
        }

        public void SaveData()
        {
            if (IsRuntime)
            {
                runtimeStates.OnCurrentStateChanged -= SetColorForRuntimeCurrentState;
                return;
            }

            SetGraphView();
            foreach (Node current in nodes)
            {
                ((BaseGraphNode)current).Target.SetNodePosition(current.GetPosition().position);
                ((BaseGraphNode)current).SaveScriptableObjectName();

                if (current is StartGraphNode startGraphNode && startGraphNode.Target.Outputs.Count > 0)
                {
                    SetVariableInStateMachine((StateNode)startGraphNode.Target.Outputs[0], "defaultState");
                    if (startGraphNode.Target.Outputs.Count > 1)
                        SetVariableInStateMachine((StateNode)startGraphNode.Target.Outputs[1], "startState");
                }
            }

            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void RefreshGraphView()
        {
            DeleteElements(graphElements);

            if (IsRuntime)
            {
                LoadRuntimeScriptableObjects(runtimeStates.DefaultState);
                LoadRuntimeScriptableObjects(runtimeStates.StartState);
            }
            else
            {
                LoadExistingScriptableObjects();
            }

            CreateStartGraphNode();
            graphViewChanged += OnGraphViewChanged;
        }

        private void LoadRuntimeScriptableObjects(BaseNode nodeToLoad)
        {
            if (nodes.Any(x => ((BaseGraphNode)x).Target == nodeToLoad))
                return;

            BaseGraphNode createdGraphNode = LoadSingleGraphNode(nodeToLoad);
            AddElement(createdGraphNode);

            foreach (BaseNode output in nodeToLoad.Outputs)
                LoadRuntimeScriptableObjects(output);

            if (nodeToLoad.Outputs.Count <= 0)
                return;

            CreateEdgesForGraphNode(createdGraphNode);
        }

        private void LoadExistingScriptableObjects()
        {
            Object[] loadedObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target));
            if (loadedObjects.Length <= 0)
                return;

            foreach (Object current in loadedObjects)
                if (current is StateMachine stateMachine)
                    UpdateViewTransform(GetGraphView(stateMachine).position, GetGraphView(stateMachine).scale);
                else
                    AddElement(LoadSingleGraphNode((BaseNode)current));

            foreach (Node current in nodes)
                if (current is BaseGraphNode graphNode && graphNode.Target.Outputs != null && graphNode.Target.Outputs.Count > 0)
                    CreateEdgesForGraphNode(graphNode);
        }

        private BaseGraphNode LoadSingleGraphNode(BaseNode node)
        {
            return node switch
            {
                TransitionNode x => new TransitionGraphNode(x, IsRuntime),
                StateNode x => new StateGraphNode(x, IsRuntime),
                StartNode x => new StartGraphNode(x, IsRuntime),
                ExitNode x => new ExitGraphNode(x, IsRuntime),
                SeparatorNode x => new SeparatorGraphNode(x, IsRuntime),
                _ => null
            };
        }

        private void SetVariableInStateMachine(StateNode state, string variableName)
        {
            FieldInfo variableField = GetFieldInfo(typeof(StateMachine), variableName);
            variableField.SetValue(target, state);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new();
            Add(toolbar);

            Label nameLabel = new(IsRuntime ? "Instanced State Machine" : $"Name: {target.name}");
            toolbar.Add(nameLabel);
            nameLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            nameLabel.StretchToParentSize();
        }

        private void CreateStartGraphNode()
        {
            if (IsRuntime)
            {
                StartGraphNode runtimeStart = new(null, Vector2.zero, IsRuntime);
                runtimeStart.Target.AddOutputNode(runtimeStates.DefaultState);
                runtimeStart.Target.AddOutputNode(runtimeStates.StartState);
                CreateEdgesForGraphNode(runtimeStart);
                AddElement(runtimeStart);
                return;
            }

            if (nodes.Any(x => x.GetType() == typeof(StartGraphNode)))
                return;

            StartGraphNode editorStart = new(target, Vector2.zero, IsRuntime);
            AddElement(editorStart);
        }

        private void CreateEdgesForGraphNode(BaseGraphNode graphNode)
        {
            foreach (Node relative in nodes)
            {
                for (int i = 0; i < graphNode.Target.Outputs.Count; i++)
                {
                    if (((BaseGraphNode)relative).Target == graphNode.Target.Outputs[i])
                    {
                        Port inputPort = ((BaseGraphNode)relative).GetAvailableInputPort();
                        Port outputPort = graphNode.GetAvailableOutputPort(i);
                        Edge newEdge = inputPort.ConnectTo(outputPort);
                        AddElement(newEdge);

                        // if (graphNode is SeparatorGraphNode separatorGraphNode)
                        //     separatorGraphNode.SetInputOutputPortType(newEdge.output.portType);

                        if (graphNode is StateGraphNode stateGraphNode)
                            stateGraphNode.UpdateOutputPort();
                    }
                }
            }
        }

        private (Vector3 position, Vector3 scale) GetGraphView(StateMachine from)
        {
            FieldInfo graphViewPositionField = GetFieldInfo(typeof(StateMachine), "graphViewPosition");
            FieldInfo graphViewScaleField = GetFieldInfo(typeof(StateMachine), "graphViewScale");
            return ((Vector3)graphViewPositionField.GetValue(from), (Vector3)graphViewScaleField.GetValue(from));
        }

        private void SetGraphView()
        {
            FieldInfo graphViewPositionField = GetFieldInfo(typeof(StateMachine), "graphViewPosition");
            FieldInfo graphViewScaleField = GetFieldInfo(typeof(StateMachine), "graphViewScale");
            graphViewPositionField.SetValue(target, viewTransform.position);
            graphViewScaleField.SetValue(target, viewTransform.scale);
        }

        private FieldInfo GetFieldInfo(System.Type type, string variableName)
        {
            return type.GetField(variableName, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private void SetColorForRuntimeCurrentState(StateNode current, StateNode next)
        {
            StateGraphNode currentGraphNode = nodes.Where(x => ((BaseGraphNode)x).Target == current).FirstOrDefault() as StateGraphNode;
            StateGraphNode nextGraphNode = nodes.Where(x => ((BaseGraphNode)x).Target == next).FirstOrDefault() as StateGraphNode;

            if (current != null)
                currentGraphNode.titleContainer.style.backgroundColor = nextGraphNode.titleContainer.style.backgroundColor;

            nextGraphNode.titleContainer.style.backgroundColor = new Color(0.15f, 0.48f, 0.13f, 0.8f);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange change)
        {
            if (change.elementsToRemove != null)
                foreach (GraphElement graphElement in change.elementsToRemove)
                {
                    if (graphElement is BaseGraphNode baseGraphNode)
                        baseGraphNode.DeleteScriptableObject();

                    if (graphElement is Edge edge)
                    {
                        ((BaseGraphNode)edge.output.node).Target.RemoveOutputNode(((BaseGraphNode)edge.input.node).Target);
                        edge.output.DisconnectAll();
                        edge.input.DisconnectAll();

                        if (edge.output.node is StateGraphNode stateGraphNode)
                            stateGraphNode.RemoveOutputPort(edge.output);
                    }
                }

            if (change.edgesToCreate != null)
                foreach (Edge edge in change.edgesToCreate)
                {
                    if (edge.input.node is BaseGraphNode baseGraphNode)
                        ((BaseGraphNode)edge.output.node).Target.AddOutputNode(baseGraphNode.Target);

                    if (edge.output.node is StateGraphNode && edge.input.node is TransitionGraphNode)
                        ((BaseGraphNode)edge.output.node).CreateOutputPort(typeof(TransitionGraphNode), "Out");

                    if (edge.input.node is SeparatorGraphNode separatorGraphNodeInput)
                        separatorGraphNodeInput.SetInputOutputPortType(edge.output.portType);

                    if (edge.output.node is SeparatorGraphNode separatorGraphNodeOutput)
                        separatorGraphNodeOutput.SetInputOutputPortType(edge.input.portType);
                }

            return change;
        }
    }
}
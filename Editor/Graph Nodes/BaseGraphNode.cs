using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using SH.StateMachine.Nodes;

namespace SH.StateMachine.Editor.GraphNodes
{
    public abstract class BaseGraphNode : Node
    {
        [SerializeField] protected BaseNode target = null;
        [SerializeField] protected bool isInstancedRuntime = false;

        public BaseNode Target => target;

        protected string[] outputsPortName = { "Out" };

        protected virtual void AddSelectionButton()
        {
            titleContainer.Add(new Button(() => Selection.activeObject = target) { text = "Select" });
        }

        protected virtual void AddTitleInputField(string inputName = null)
        {
            TextField titleInput = new();
            titleInput.value = inputName == null ? target.name : inputName;
            titleInput[0].style.backgroundColor = new Color(0, 0, 0, 0);
            titleInput[0].style.borderTopWidth = 0;
            titleInput[0].style.borderBottomWidth = 0;
            titleInput[0].style.borderLeftWidth = 0;
            titleInput[0].style.borderRightWidth = 0;
            titleContainer.Insert(0, titleInput);
        }

        protected virtual void CreateInputPort(System.Type portType, string name, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, capacity, portType);
            port.portName = name;
            inputContainer.Add(port);
        }

        public virtual string GetOutputPortName(int index)
        {
            if (index >= outputsPortName.Length)
                return outputsPortName[0];
            else
                return outputsPortName[index];
        }

        public virtual void CreateOutputPort(System.Type portType, string name, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, portType);
            port.portName = name;
            outputContainer.Add(port);
        }

        public virtual void RemoveOutputPort(Port port)
        {
            outputContainer.Remove(port);
        }

        public virtual Port GetAvailableOutputPort()
        {
            foreach (Port current in outputContainer.Children())
                if (!current.connected)
                    return current;

            return null;
        }

        public virtual Port GetAvailableOutputPort(int index)
        {
            if (index >= outputContainer.childCount)
                for (int i = outputContainer.childCount; i <= index; i++)
                    CreateOutputPort(((Port)outputContainer[0]).portType, GetOutputPortName(i));

            return (Port)outputContainer[index];
        }

        public virtual Port GetAvailableInputPort()
        {
            foreach (Port current in inputContainer.Children())
                if (!current.connected || current.capacity == Port.Capacity.Multi)
                    return current;

            return null;
        }

        public virtual void SaveScriptableObjectName()
        {
            target.name = ((TextField)titleContainer[0]).value;
            EditorUtility.SetDirty(target);
        }

        public virtual void SaveScriptableObjectName(string objectName)
        {
            target.name = objectName;
            EditorUtility.SetDirty(target);
        }

        protected virtual void CreateScriptableObject<T>(Vector2 position, ScriptableObject parent) where T : BaseNode
        {
            target = ScriptableObject.CreateInstance<T>();
            target.SetNodePosition(position);

            if (isInstancedRuntime)
                return;

            AssetDatabase.AddObjectToAsset(target, parent);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public virtual void DeleteScriptableObject()
        {
            if (isInstancedRuntime)
                return;

            Object loaded = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(target), target.GetType());
            if (loaded == null)
                return;

            AssetDatabase.RemoveObjectFromAsset(loaded);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        protected virtual Vector2 GetNodePosition()
        {
            FieldInfo nodePositionField = typeof(BaseNode).GetField("nodePosition", BindingFlags.Instance | BindingFlags.NonPublic);
            return (Vector2)nodePositionField.GetValue(target);
        }

        public override void UpdatePresenterPosition()
        {
            base.UpdatePresenterPosition();
            target.SetNodePosition(GetPosition().position);
        }
    }
}
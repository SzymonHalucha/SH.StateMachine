using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SH.StateMachine.Editor
{
    public class StateMachineGraph : EditorWindow
    {
        private StateMachineGraphView graphView;

        [MenuItem("Tools/State Machine")]
        private static void OpenWindow()
        {
            StateMachineGraph window = GetWindow<StateMachineGraph>();
            window.titleContent = new GUIContent("State Machine");
            Open(Selection.activeInstanceID, 0);
        }

        [OnOpenAsset(0)]
        public static bool Open(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is not StateMachine)
                return false;

            StateMachineGraph window = GetWindow<StateMachineGraph>();
            window.titleContent = new GUIContent("State Machine");
            window.graphView = new StateMachineGraphView((StateMachine)EditorUtility.InstanceIDToObject(id));
            window.rootVisualElement.Add(window.graphView);
            return true;
        }

        public static void Open(StateMachine target)
        {
            StateMachineGraph window = GetWindow<StateMachineGraph>();
            window.titleContent = new GUIContent("State Machine");
            window.graphView = new StateMachineGraphView(target);
            window.rootVisualElement.Add(window.graphView);
        }

        public static void Open(RuntimeStates runtimeStates)
        {
            StateMachineGraph window = GetWindow<StateMachineGraph>();
            window.titleContent = new GUIContent("State Machine");
            window.graphView = new StateMachineGraphView(runtimeStates);
            window.rootVisualElement.Add(window.graphView);
        }

        private void OnDestroy()
        {
            graphView.SaveData();
            rootVisualElement.Clear();
        }
    }
}
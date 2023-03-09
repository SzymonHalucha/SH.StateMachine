using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace SH.StateMachine.Editor
{
    [CustomEditor(typeof(StateMachineBehaviour))]
    public class StateMachineBehaviourEditor<T> : UnityEditor.Editor where T : Object
    {
        private FieldInfo stateMachineField = typeof(StateMachineBehaviour<T>).GetField("stateMachine", BindingFlags.Instance | BindingFlags.NonPublic);
        private FieldInfo runtimeStatesField = typeof(StateMachineBehaviour<T>).GetField("runtimeStates", BindingFlags.Instance | BindingFlags.NonPublic);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (stateMachineField.GetValue((StateMachineBehaviour<T>)target) != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Graph Editor"))
                    StateMachineGraph.Open((StateMachine)stateMachineField.GetValue(target));
            }

            if (runtimeStatesField.GetValue((StateMachineBehaviour<T>)target) != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Open Instanced Graph Editor"))
                    StateMachineGraph.Open((RuntimeStates)runtimeStatesField.GetValue(target));
            }
        }
    }
}
using UnityEngine;
using UnityEditor;

namespace SH.StateMachine.Editor
{
    [CustomEditor(typeof(StateMachine))]
    public class StateMachineEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Editor"))
                StateMachineGraph.Open((StateMachine)target);
        }
    }
}
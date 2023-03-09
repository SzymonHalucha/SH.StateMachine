using UnityEngine;

namespace SH.StateMachine.Actions
{
    public class DebugLogAction : BaseAction
    {
        [SerializeField] private string onStartText = "On Start";
        [SerializeField] private string onUpdateText = "On Update";
        [SerializeField] private string onEndText = "On End";
        [SerializeField] private bool returnValue = true;

        public override void OnStart<T>(T origin)
        {
            Debug.Log(onStartText);
        }

        public override bool OnUpdate<T>(T origin, float elapsedTime)
        {
            Debug.Log(onUpdateText);
            return returnValue;
        }

        public override void OnEnd<T>(T origin)
        {
            Debug.Log(onEndText);
        }
    }
}
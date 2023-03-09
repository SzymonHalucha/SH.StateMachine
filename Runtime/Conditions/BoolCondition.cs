using UnityEngine;

namespace SH.StateMachine.Conditions
{
    public class BoolCondition : BaseCondition
    {
        [SerializeField] private bool value = false;

        public override bool Check<T>(T origin)
        {
            return value;
        }
    }
}
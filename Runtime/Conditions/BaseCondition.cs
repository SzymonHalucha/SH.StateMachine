namespace SH.StateMachine.Conditions
{
    [System.Serializable]
    public abstract class BaseCondition
    {
        public abstract bool Check<T>(T origin);
    }
}
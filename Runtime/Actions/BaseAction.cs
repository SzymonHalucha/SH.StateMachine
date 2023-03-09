namespace SH.StateMachine.Actions
{
    [System.Serializable]
    public abstract class BaseAction
    {
        public abstract void OnStart<T>(T origin);
        public abstract bool OnUpdate<T>(T origin, float elapsedTime);
        public abstract void OnEnd<T>(T origin);
    }
}
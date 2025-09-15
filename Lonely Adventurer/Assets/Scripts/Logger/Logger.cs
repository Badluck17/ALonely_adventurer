namespace Zycalipse.GameLogger
{
    public interface ILogger
    {
        void Information(object msg);
        void Warning(object msg);
        void Error(object msg);
    }

    public class Logger : ILogger
    {
        public virtual void Error(object msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public virtual void Information(object msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public virtual void Warning(object msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }
    }

    public class GameLog : Logger
    {
        public GameLog(System.Type _type)
        {
            type = _type;
        }

        private string message;
        private System.Type type;
        public override void Error(object msg)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            string message = $"[{type}] {msg}";
            UnityEngine.Debug.LogError(message);
#endif
        }

        public override void Information(object msg)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            string message = $"[{type}] {msg}";
            UnityEngine.Debug.Log(message);
#endif
        }

        public override void Warning(object msg)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            string message = $"[{type}] {msg}";
            UnityEngine.Debug.LogWarning(message);
#endif
        }
    }

}
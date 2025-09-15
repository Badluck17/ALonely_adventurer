using Zycalipse.GameLogger;

namespace Zycalipse.Entitys
{
    public interface IStatus
    {
        void ChangeStatus(CharacterStatus status);
        void GetPoisoned();
        void GetParalized();
        void GetConfused();
        void GetBleed();
    }

    public class CharcterStatus : IStatus
    {
        public CharcterStatus()
        {
            gameLog = new GameLog(GetType());
        }

        public CharacterStatus currentStatus { get; private set; }
        private GameLog gameLog;

        public void ChangeStatus(CharacterStatus status)
        {
            gameLog.Information($"Status will be changed from {currentStatus} to {status}");
            currentStatus = status;
        }

        public void GetBleed()
        {
            throw new System.NotImplementedException();
        }

        public void GetConfused()
        {
            throw new System.NotImplementedException();
        }

        public void GetParalized()
        {
            throw new System.NotImplementedException();
        }

        public void GetPoisoned()
        {
            throw new System.NotImplementedException();
        }
    }
}
using UnityEngine;

namespace Zycalipse.Menus
{
    public interface IMenu
    {
        void OnPush();
        void OnPop();
        void OnFocusIn();
        void OnFocusOut();
        void RegisterListener();
        void UnRegisterListener();
        void RefreshMenu();
    }

    public class Menu : MonoBehaviour, IMenu
    {
        public virtual void OnFocusIn() { }
        public virtual void OnFocusOut() { }
        public virtual void RegisterListener() { }
        public virtual void UnRegisterListener() { }
        public virtual void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public virtual void OnPush()
        {
            RegisterListener();
            OnFocusIn();
        }

        public virtual void RefreshMenu()
        {

        }
    }
}
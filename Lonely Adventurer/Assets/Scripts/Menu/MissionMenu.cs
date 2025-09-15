using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Menus
{
    public class MissionMenu : Menu
    {
        [SerializeField]
        UnityEngine.UI.Button closeButton;

        // Start is called before the first frame update
        void Start()
        {
            closeButton.onClick.AddListener(OnClose);
        }
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnClose);
        }
        private void OnEnable()
        {
            MenusManager.Instance.currentPopUpMenu = this;
        }
        private void OnDisable()
        {
            MenusManager.Instance.currentPopUpMenu = null;
        }

        void OnClose()
        {
            gameObject.SetActive(false);
        }
    }
}

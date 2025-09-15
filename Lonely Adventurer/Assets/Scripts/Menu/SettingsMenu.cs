using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Menus
{
    public class SettingsMenu : Menu
    {
        [SerializeField]
        UnityEngine.UI.Button closeButton, tutorialButton, closeTutorialButton;

        public GameObject TutorialPanel;
        public Transform content;
        public int max, min;

        private void Update()
        {
            if (content.localPosition.x > max)
            {
                content.localPosition = new Vector3(max - 1, content.localPosition.y, content.localPosition.z);
            }else
            if (content.localPosition.x < min)
            {
                content.localPosition = new Vector3(min + 1, content.localPosition.y, content.localPosition.z);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            TutorialPanel.SetActive(false);
            closeButton.onClick.AddListener(OnClose);
            tutorialButton.onClick.AddListener(OnTutorialButton);
            closeTutorialButton.onClick.AddListener(OnCloseTutorialButton);
        }
        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(OnClose);
            tutorialButton.onClick.RemoveListener(OnTutorialButton);
            closeTutorialButton.onClick.RemoveListener(OnCloseTutorialButton);
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

        void OnTutorialButton()
        {
            TutorialPanel.SetActive(true);
        }

        void OnCloseTutorialButton()
        {
            TutorialPanel.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zycalipse.GameLogger;
using Zycalipse.Managers;

namespace Zycalipse.Menus
{
    public class InGameMenu : Menu
    {
        [SerializeField]
        private Button continueButton, backToMMButton;

        private GameLog logger;

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentPopUpMenu = this;
            GameManager.Instance.GamePaused = true;

            RefreshMenu();
        }

        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
            GameManager.Instance.GamePaused = false;
        }

        public override void RegisterListener()
        {
            continueButton.onClick.AddListener(OnContinueButtonClicked);
            backToMMButton.onClick.AddListener(OnBackToMMButtonClicked);
        }

        public override void UnRegisterListener()
        {
            continueButton.onClick.RemoveListener(OnContinueButtonClicked);
            backToMMButton.onClick.RemoveListener(OnBackToMMButtonClicked);
        }

        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnPush()
        {
            RegisterListener();

            logger = new GameLog(GetType());
        }

        private void OnEnable()
        {
            OnFocusIn();
        }

        private void OnDisable()
        {
            OnFocusOut();
        }

        private void Start()
        {
            OnPush();
        }

        private void Update()
        {

        }

        private void OnDestroy()
        {
            OnPop();
        }

        public override void RefreshMenu()
        {

        }

        private async void OnContinueButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            GameManager.Instance.GamePaused = false;
            gameObject.SetActive(false);
        }
        private async void OnSettingsButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);

        }
        private async void OnBackToMMButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            GameManager.Instance.GamePaused = false;
            Systems.LevelManager.Instance.ReturnToMM();
            gameObject.SetActive(false);
        }
    }
}

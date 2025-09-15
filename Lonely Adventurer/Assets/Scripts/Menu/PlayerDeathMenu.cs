using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Systems;
using Zycalipse.Managers;
using Zycalipse.GameLogger;

namespace Zycalipse.Menus
{
    public class PlayerDeathMenu : Menu
    {
        [SerializeField]
        private Button reviveButton, backToMMButton;
        [SerializeField]
        private TextMeshProUGUI deathText;

        private GameLog logger;

        private void Start()
        {
            OnPush();
            logger = new GameLog(GetType());
        }

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentPopUpMenu = this;

            if (!LevelManager.Instance.HasReviveChance)
            {
                SoundManager.PlayOneShotSound(DataManager.Instance.GameOver, transform.position);
                reviveButton.gameObject.SetActive(false);
            }
        }
        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
        }
        public override void RegisterListener()
        {
            reviveButton.onClick.AddListener(OnReviveButtonClicked);
            backToMMButton.onClick.AddListener(BackToMM);
        }
        public override void UnRegisterListener()
        {
            reviveButton.onClick.RemoveListener(OnReviveButtonClicked);
            backToMMButton.onClick.RemoveListener(BackToMM);
        }
        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnPush()
        {
            RegisterListener();
            OnFocusIn();
        }

        public override void RefreshMenu()
        {

        }

        private async void OnReviveButtonClicked()
        {
            EffectManager.Instance.ActivateEffect(Effect.PlayerRevive, CombatSystem.Instance.playerManager.transform.position);
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            MenusManager.Instance.ActiveMenu(MenuList.AdsMenu);
            gameObject.SetActive(false);
        }

        private async void OnEnable()
        {
            reviveButton.gameObject.SetActive(LevelManager.Instance.HasReviveChance);
            GameManager.Instance.ChangeGameState(GameState.PlayerDied);
            OnFocusIn();
        }

        private async void OnDisable()
        {
            OnFocusOut();
        }

        private async void BackToMM()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            gameObject.SetActive(false);
            LevelManager.Instance.ReturnToMM();
        }
    }
}

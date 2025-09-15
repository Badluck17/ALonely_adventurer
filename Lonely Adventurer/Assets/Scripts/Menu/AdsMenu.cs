using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.GameLogger;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Menus
{
    public class AdsMenu : Menu
    {
        [SerializeField]
        private Button closeButton;
        [SerializeField]
        private TextMeshProUGUI adsText;
        [SerializeField]
        private TextMeshProUGUI adsTimer;
        private bool startTimer = false;
        private float timer = 3f;

        private GameLog logger;

        private void Start()
        {
            OnPush();
            logger = new GameLog(GetType());
        }

        private async void Update()
        {
            if (startTimer)
            {
                if(timer <= 0)
                {
                    closeButton.gameObject.SetActive(true);
                    adsTimer.gameObject.SetActive(false);

                    timer = 3f;
                    startTimer = false;
                    return;
                }
                adsTimer.text = $"{(int)timer}";
                timer -= Time.deltaTime;
            }
        }

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentPopUpMenu = this;
            closeButton.gameObject.SetActive(false);
            adsTimer.gameObject.SetActive(true);
        }
        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
            closeButton.gameObject.SetActive(false);
            adsTimer.gameObject.SetActive(true);
        }
        public override void RegisterListener()
        {
            closeButton.onClick.AddListener(OnCloseButton);
        }
        public override void UnRegisterListener()
        {
            closeButton.onClick.RemoveListener(OnCloseButton);
        }
        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnPush()
        {
            adsText.text = "This is an ad\nPlease wait for a moments....";
            RegisterListener();
            OnFocusIn();
        }

        public override void RefreshMenu()
        {

        }

        private async void OnCloseButton()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
            LevelManager.Instance.HasReviveChance = false;
            int playerMaxHP = CombatSystem.Instance.playerManager.playerData.PlayerMaxHealth;
            await CombatSystem.Instance.playerManager.playerData.Healed(playerMaxHP);
            await CombatSystem.Instance.playerManager.RefreshUI();
            gameObject.SetActive(false);
        }

        private async void OnEnable()
        {
            OnFocusIn();
            startTimer = true;
        }

        private async void OnDisable()
        {
            OnFocusOut();
        }
    }
}

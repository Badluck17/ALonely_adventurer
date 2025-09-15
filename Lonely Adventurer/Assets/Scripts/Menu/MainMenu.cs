using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Zycalipse.GameLogger;
using Zycalipse.Systems;
using Zycalipse.Managers;
using Zycalipse.UI;

namespace Zycalipse.Menus
{
    public class MainMenu : Menu
    {
        private static MainMenu instance;
        public static MainMenu Instance
        {
            get
            {
                return instance;
            }
        }
        private GameLog gameLog;

        [SerializeField]
        private GameObject SettingsMenu;
        [SerializeField]
        private GameObject cheatObj;
        public Text energyText, energyTimerText;
        [SerializeField]
        private Text money, gem;
        [SerializeField]
        private ButtonSlideAnimation levelSelectionSlide, shopSlide, inventorySlide, characterSlide;
        [SerializeField]
        private Button levelSelectionButton, shopButton, inventoryButton, characterButton, settingButton;
        [SerializeField]
        private CanvasGroup levelCanvas, shopCamvas, inventoryCanvas, characterCanvas;
        [SerializeField]
        private LevelSelectionMenu levelSelectionMenu;
        [SerializeField]
        private float fadingSpeed = 1.5f;
        [SerializeField]
        private HorizontalLayoutGroup layoutGroup;

        private MainMenuSubMenus currentSubMenu;

        public override async void OnFocusIn()
        {
            MenusManager.Instance.currentMenu = this;
            levelSelectionButton.interactable = true;
            settingButton.interactable = true;
            inventoryButton.interactable = true;
            shopButton.interactable = true;
            characterButton.interactable = true;

            levelCanvas.gameObject.SetActive(false);
            shopCamvas.gameObject.SetActive(false);
            inventoryCanvas.gameObject.SetActive(false);
            characterCanvas.gameObject.SetActive(false);

            currentSubMenu = MainMenuSubMenus.LevelMenu;
            await ChangeSubMenu(MainMenuSubMenus.LevelMenu);
            RefreshMenu();
        }

        public override void OnFocusOut()
        {
            levelSelectionButton.interactable = false;
            settingButton.interactable = false;
            inventoryButton.interactable = false;
            shopButton.interactable = false;
            characterButton.interactable = false;
        }

        public override void RegisterListener()
        {
            settingButton.onClick.AddListener(OnSettingButtonClicked);

            levelSelectionButton.onClick.RemoveAllListeners();
            levelSelectionButton.onClick.AddListener(OnLevelButtonClicked);
            inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
            characterButton.onClick.AddListener(OnCharacterButtonClicked);
        }

        public override void UnRegisterListener()
        {
            settingButton.onClick.RemoveListener(OnSettingButtonClicked);

            levelSelectionButton.onClick.RemoveListener(OnLevelButtonClicked);
            shopButton.onClick.RemoveListener(OnShopButtonClicked);
            inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            characterButton.onClick.RemoveListener(OnCharacterButtonClicked);
        }

        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override async void OnPush()
        {
            RegisterListener();
            OnFocusIn();

            gameLog = new GameLog(GetType());
        }

        private async void OnEnable()
        {
            OnFocusIn();
            RefreshMenu();
        }

        private void OnDisable()
        {
            MenusManager.Instance.currentMenu = null;
            OnFocusOut();
        }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
        }

        private async void Start()
        {
            OnPush();

            await ButtonSlide(MainMenuSubMenus.LevelMenu);
        }

        private async void LateUpdate()
        {
            if (!levelSelectionSlide.Selected && !shopSlide.Selected && !inventorySlide.Selected && !characterSlide.Selected)
            {
                await ButtonSlide(currentSubMenu);
                await MenusManager.Instance.BlackscreenFade(false);
            }
        }

        private void OnDestroy()
        {
            OnPop();
        }

        public override async void RefreshMenu()
        {
            levelSelectionMenu.UpdateStatus();

            if (GameManager.Instance.Money > 999999999)
            {
                var displayAmount = GameManager.Instance.Money / 1000000000;
                money.text = $"{displayAmount}B";
            }
            else if (GameManager.Instance.Money > 999999)
            {
                var displayAmount = GameManager.Instance.Money / 1000000;
                money.text = $"{displayAmount}M";
            }
            else if (GameManager.Instance.Money > 999)
            {
                var displayAmount = GameManager.Instance.Money / 1000;
                money.text = $"{displayAmount}K";
            }
            else
            {
                money.text = $"{GameManager.Instance.Money}";
            }

            if (GameManager.Instance.Gem > 999999999)
            {
                var displayAmount = GameManager.Instance.Gem / 1000000000;
                gem.text = $"{displayAmount}B";
            }
            else if (GameManager.Instance.Gem > 999999)
            {
                var displayAmount = GameManager.Instance.Gem / 1000000;
                gem.text = $"{displayAmount}M";
            }
            else if (GameManager.Instance.Gem > 999)
            {
                var displayAmount = GameManager.Instance.Gem / 1000;
                gem.text = $"{displayAmount}K";
            }
            else
            {
                gem.text = $"{GameManager.Instance.Gem}";
            }

            await ButtonSlide(currentSubMenu);
        }

        private async Task ChangeSubMenu(MainMenuSubMenus subMenu)
        {
            layoutGroup.enabled = false;
            DisableOtherSubMenus();
            switch (subMenu)
            {
                case MainMenuSubMenus.LevelMenu:
                    levelCanvas.gameObject.SetActive(true);

                    MenusManager.Instance.Level3DModels?.GoToMenu(0);
                    while (levelCanvas.alpha < 1)
                    {
                        levelCanvas.alpha += Time.deltaTime * fadingSpeed;
                        FadeOutLastSubMenu(currentSubMenu);
                        await Task.Yield();
                    }
                    currentSubMenu = MainMenuSubMenus.LevelMenu;
                    break;
                case MainMenuSubMenus.ShopMenu:
                    shopCamvas.gameObject.SetActive(true);

                    MenusManager.Instance.Level3DModels?.GoToMenu(1);
                    while (shopCamvas.alpha < 1)
                    {
                        shopCamvas.alpha += Time.deltaTime * fadingSpeed;
                        FadeOutLastSubMenu(currentSubMenu);
                        await Task.Yield();
                    }
                    currentSubMenu = MainMenuSubMenus.ShopMenu;
                    break;
                case MainMenuSubMenus.InventoryMenu:
                    inventoryCanvas.gameObject.SetActive(true);

                    MenusManager.Instance.Level3DModels?.GoToMenu(2);
                    while (inventoryCanvas.alpha < 1)
                    {
                        inventoryCanvas.alpha += Time.deltaTime * fadingSpeed;
                        FadeOutLastSubMenu(currentSubMenu);
                        await Task.Yield();
                    }
                    currentSubMenu = MainMenuSubMenus.InventoryMenu;
                    break;
                case MainMenuSubMenus.CharacterMenu:
                    characterCanvas.gameObject.SetActive(true);

                    MenusManager.Instance.Level3DModels?.GoToMenu(3);
                    while (characterCanvas.alpha < 1)
                    {
                        characterCanvas.alpha += Time.deltaTime * fadingSpeed;
                        FadeOutLastSubMenu(currentSubMenu);
                        await Task.Yield();
                    }
                    currentSubMenu = MainMenuSubMenus.CharacterMenu;
                    break;
            }
            DisableOtherSubMenus();
            layoutGroup.enabled = true;
            await ButtonSlide(currentSubMenu);
        }

        private async void FadeOutLastSubMenu(MainMenuSubMenus subMenu)
        {
            switch (subMenu)
            {
                case MainMenuSubMenus.LevelMenu:
                    levelCanvas.alpha -= Time.deltaTime * fadingSpeed;
                    break;
                case MainMenuSubMenus.ShopMenu:
                    shopCamvas.alpha -= Time.deltaTime * fadingSpeed;
                    break;
                case MainMenuSubMenus.InventoryMenu:
                    inventoryCanvas.alpha -= Time.deltaTime * fadingSpeed;
                    break;
                case MainMenuSubMenus.CharacterMenu:
                    characterCanvas.alpha -= Time.deltaTime * fadingSpeed;
                    break;
            }
        }

        private async void DisableOtherSubMenus()
        {
            MenusManager.Instance.Toggle3DLevelMenu(currentSubMenu == MainMenuSubMenus.LevelMenu);

            levelCanvas.gameObject.SetActive(currentSubMenu == MainMenuSubMenus.LevelMenu);
            shopCamvas.gameObject.SetActive(currentSubMenu == MainMenuSubMenus.ShopMenu);
            inventoryCanvas.gameObject.SetActive(currentSubMenu == MainMenuSubMenus.InventoryMenu);
            characterCanvas.gameObject.SetActive(currentSubMenu == MainMenuSubMenus.CharacterMenu);

            switch (currentSubMenu)
            {
                case MainMenuSubMenus.LevelMenu:
                    levelCanvas.alpha = 1;
                    shopCamvas.alpha = 0;
                    inventoryCanvas.alpha = 0;
                    characterCanvas.alpha = 0;
                    break;
                case MainMenuSubMenus.ShopMenu:
                    levelCanvas.alpha = 0;
                    shopCamvas.alpha = 1;
                    inventoryCanvas.alpha = 0;
                    characterCanvas.alpha = 0;
                    break;
                case MainMenuSubMenus.InventoryMenu:
                    levelCanvas.alpha = 0;
                    shopCamvas.alpha = 0;
                    inventoryCanvas.alpha = 1;
                    characterCanvas.alpha = 0;
                    break;
                case MainMenuSubMenus.CharacterMenu:
                    levelCanvas.alpha = 0;
                    shopCamvas.alpha = 0;
                    inventoryCanvas.alpha = 0;
                    characterCanvas.alpha = 1;
                    break;
            }
        }
        public async void OnPlayButtonClicked(int id)
        {
            if (!EnergyManager.Instance.UseEnergy())
                return;

            MenusManager.Instance.ToggleMainMenuLookEffect(false);
            GameManager.Instance.CurrentLevelLoaded = id + 1;
            GameManager.Instance.Imortal = false;
            gameObject.SetActive(false);
            MenusManager.Instance.Toggle3DLevelMenu(false);
            LevelManager.Instance.Play(id);
        }

        public async void TutorialTriggered()
        {
            GameManager.Instance.Imortal = false;
            gameObject.SetActive(false);
            MenusManager.Instance.Toggle3DLevelMenu(false);
        }

        private async void OnLevelButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            await ButtonSlide(MainMenuSubMenus.LevelMenu);
            await ChangeSubMenu(MainMenuSubMenus.LevelMenu);
        }
        private async void OnInventoryButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            await ButtonSlide(MainMenuSubMenus.InventoryMenu);
            await ChangeSubMenu (MainMenuSubMenus.InventoryMenu);
        }
        private async void OnShopButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            await ButtonSlide(MainMenuSubMenus.ShopMenu);
            await ChangeSubMenu (MainMenuSubMenus.ShopMenu);
        }
        private async void OnCharacterButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            await ButtonSlide(MainMenuSubMenus.CharacterMenu);
            await ChangeSubMenu (MainMenuSubMenus.CharacterMenu);
        }
        private async void OnSettingButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            //cheatObj.SetActive(true);
            SettingsMenu.SetActive(true);
            gameLog.Information("SettingButton clicked!");
        }

        private async Task ButtonSlide(MainMenuSubMenus menuSelected)
        {
            // find the selected button, slide it and unslide the other
            // also enable disable depending on the selected button
            levelSelectionSlide.Selected = menuSelected == MainMenuSubMenus.LevelMenu;
            shopSlide.Selected = menuSelected == MainMenuSubMenus.ShopMenu;
            inventorySlide.Selected = menuSelected == MainMenuSubMenus.InventoryMenu;
            characterSlide.Selected = menuSelected == MainMenuSubMenus.CharacterMenu;

            levelSelectionSlide.DoSlide();
            shopSlide.DoSlide();
            inventorySlide.DoSlide();
            characterSlide.DoSlide();
        }
    }
}

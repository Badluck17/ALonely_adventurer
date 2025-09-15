using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Zycalipse.Systems;
using Zycalipse.Menus;
using Zycalipse.GameLogger;

namespace Zycalipse.Managers
{
    public class MenusManager : MonoBehaviour
    {
        private static MenusManager instance;
        public static MenusManager Instance
        {
            get
            {
                return instance;
            }
        }

        public Camera mainCam;
        [SerializeField]
        private Image blackscreen;
        [SerializeField]
        private Transform menuCanvas;
        [SerializeField]
        private AssetReferenceGameObject mainMenu, battleMenu, IGM, deathMenu, adsMenu, playerLevelUp;
        private GameObject MMGameObject, battleMenuGameObject, IGMGameObject, deathMenuGameObject, adsMenuGameObject, playerLevelUpGameObject;
        public LevelSelectionMenuModel Level3DModels;

        public GameObject heliPopup, firstPiece, secondPiece;
        public GameObject AquiredWeaponPopup, ShotGunAquiredPopup, SniperAquiredPopup;
        public Button BasicWeaponAquiredCloseButton, heliPieceCloseButton;

        public Menu currentMenu { get; set; }
        public Menu currentPopUpMenu { get; set; }
        private GameLog logger;

        [SerializeField]
        private GameObject mainMenuEffect;

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }

            logger = new GameLog(GetType());
        }

        private async void Start()
        {
            BasicWeaponAquiredCloseButton.onClick.AddListener(CloseAquiredWeaponPopup);
            heliPieceCloseButton.onClick.AddListener(CloseHeliPiece);

            MMGameObject = await AddressableManager.Instance.LoadMenuInstance(mainMenu, menuCanvas);
            battleMenuGameObject = await AddressableManager.Instance.LoadMenuInstance(battleMenu, menuCanvas);
            IGMGameObject = await AddressableManager.Instance.LoadMenuInstance(IGM, menuCanvas);
            deathMenuGameObject = await AddressableManager.Instance.LoadMenuInstance(deathMenu, menuCanvas);
            adsMenuGameObject = await AddressableManager.Instance.LoadMenuInstance(adsMenu, menuCanvas);
            playerLevelUpGameObject = await AddressableManager.Instance.LoadMenuInstance(playerLevelUp, menuCanvas);

            ActiveMenu(MenuList.MainMenu);
        }

        private void OnDestroy()
        {
            BasicWeaponAquiredCloseButton.onClick.RemoveListener(CloseAquiredWeaponPopup);
            heliPieceCloseButton.onClick.RemoveListener(CloseHeliPiece);
        }

        public void ToggleMainMenuLookEffect(bool enable)
        {
            mainMenuEffect.SetActive(enable);
        }

        public void ShowAquiredWeaponPopup(WeaponName name)
        {
            GameManager.Instance.GamePaused = true;
            AquiredWeaponPopup.SetActive(true);
            ShotGunAquiredPopup.SetActive(name == WeaponName.Shotgun);
            SniperAquiredPopup.SetActive(name == WeaponName.Sniper);
        }

        private void CloseAquiredWeaponPopup()
        {
            GameManager.Instance.GamePaused = false;
            AquiredWeaponPopup.SetActive(false);
        }
        public void ShowHeliPiece(bool first)
        {
            //GameManager.Instance.GamePaused = true;
            heliPopup.SetActive(true);
            firstPiece.SetActive(first);
            secondPiece.SetActive(!first);
        }
        public void CloseHeliPiece()
        {
            //GameManager.Instance.GamePaused = false;
            heliPopup.SetActive(false);
        }

        public async void RefreshMenu()
        {
            if (currentPopUpMenu != null)
            {
                currentPopUpMenu.RefreshMenu();
                return;
            }

            if (currentMenu != null)
                currentMenu.RefreshMenu();
        }

        public async void ActiveMenu(MenuList menu)
        {
            switch (menu)
            {
                case MenuList.MainMenu:
                    MMGameObject.SetActive(true);
                    if (LevelManager.Instance.fromFtue)
                    {
                        LevelManager.Instance.fromFtue = false;
                        ShowAquiredWeaponPopup(WeaponName.Sniper);
                    }
                    break;
                case MenuList.BattleMenu:
                    battleMenuGameObject.SetActive(true);
                    break;
                case MenuList.IGM:
                    IGMGameObject.SetActive(true);
                    break;
                case MenuList.DeathMenu:
                    deathMenuGameObject.SetActive(true);
                    break;
                case MenuList.AdsMenu:
                    adsMenuGameObject.SetActive(true);
                    break;
                case MenuList.PlayerLevelUpMenu:
                    playerLevelUpGameObject.SetActive(true);
                    break;
            }
        }

        public async Task BlackscreenFade(bool show)
        {
            if (!blackscreen.gameObject.activeSelf)
            {
                blackscreen.gameObject.SetActive(true);
            }

            if (show)
            {
                float alphaVal = 0f;
                if(blackscreen.color.a == 1)
                    blackscreen.color = new Color(0, 0, 0, alphaVal);

                while (alphaVal < 1)
                {
                    alphaVal += Time.deltaTime * 5f;
                    blackscreen.color = new Color(0,0,0,alphaVal);
                    await Task.Yield();
                }
            }
            else
            {
                float alphaVal = 1f;

                if (blackscreen.color.a == 0)
                    blackscreen.color = new Color(0, 0, 0, alphaVal);

                while (alphaVal > 0)
                {
                    alphaVal -= Time.deltaTime * 5f;
                    blackscreen.color = new Color(0, 0, 0, alphaVal);
                    await Task.Yield();
                }
            }

            blackscreen.gameObject.SetActive(blackscreen.color.a == 1 ? true : false);
        }

        public void Toggle3DLevelMenu(bool active)
        {
            //Level3DModels.gameObject.SetActive(active);
        }
    }
}

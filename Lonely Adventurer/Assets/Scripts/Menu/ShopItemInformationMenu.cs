using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zycalipse.Managers;
using Zycalipse.Items;
using TMPro;

namespace Zycalipse.Menus
{
    public class ShopItemInformationMenu : Menu
    {
        [SerializeField]
        private Button buyButton, closeButton;
        [SerializeField]
        private TextMeshProUGUI itemName, descriptionText, priceText;
        [SerializeField]
        private GameObject goldLogo, gemLogo, moneyLogo;
        private CurrencyType currencyType;
        private int price;
        private UnityEvent buyEvent;


        void Start()
        {
            OnPush();
        }

        void Update()
        {
        }

        private void OnDestroy()
        {
            OnPop();
        }

        private void OnEnable()
        {
            OnFocusIn();
            RegisterListener();
        }

        private void OnDisable()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentPopUpMenu = this;
        }
        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
        }
        public override void RegisterListener()
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        public override void UnRegisterListener()
        {
            buyButton.onClick.RemoveListener(OnBuyButtonClicked);
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
        public override void OnPop()
        {
            OnFocusOut();
        }

        public override void OnPush()
        {
            OnFocusIn();
        }

        public override void RefreshMenu()
        {

        }

        private void OnBuyButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            buyEvent.Invoke();
            ResetPopUp();
        }

        private void OnCloseButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            ResetPopUp();
        }

        private void ResetPopUp()
        {
            buyEvent = null;
            itemName.text = "";
            descriptionText.text = "";
            price = 0;
            priceText.text = price.ToString();

            goldLogo.SetActive(false);
            gemLogo.SetActive(false);

            gameObject.SetActive(false);
        }

        public void InitWeaponItemData(ShopWeaponItem weaponItem)
        {
            buyEvent = weaponItem.BuyEvent;
            itemName.text = weaponItem.IsBuyGold ? "GOLD" : weaponItem.isBuyGem ? "GEM" : weaponItem.weaponName.ToString();
            descriptionText.text = weaponItem.IsBuyGold ? $"Get {weaponItem.CurrencyBought} GOLD" : weaponItem.isBuyGem ? $"Get {weaponItem.CurrencyBought} GEM \n By transfer BNI to : 98712347" : weaponItem.ItemDescription;
            price = weaponItem.Price;
            priceText.text = price.ToString();
            currencyType = weaponItem.CurrencyType;

            goldLogo.SetActive(currencyType == CurrencyType.Gold);
            gemLogo.SetActive(currencyType == CurrencyType.Gem);
            moneyLogo.SetActive(currencyType == CurrencyType.Money);

            buyButton.gameObject.SetActive(!weaponItem.Owned && !weaponItem.Locked);

            if(weaponItem.IsBuyGold || weaponItem.isBuyGem)
                buyButton.gameObject.SetActive(weaponItem.IsBuyGold || weaponItem.isBuyGem);
        }
    }
}

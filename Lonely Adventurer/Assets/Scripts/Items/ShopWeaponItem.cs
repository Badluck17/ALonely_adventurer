using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Zycalipse.Menus;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Items
{
    public class ShopWeaponItem : ShopItem
    {
        public WeaponName weaponName;
        [SerializeField]
        private GameObject ownedStatusObject, lockedStatusObject, priceObject;
        [SerializeField]
        private ShopItemInformationMenu ItemInformationPopUp;
        [SerializeField]
        private TextMeshProUGUI priceText;
        [SerializeField]
        private GameObject goldLogo, gemLogo, moneyLogo;

        public int CurrencyBought;

        public bool IsBuyGold, isBuyGem;
        [SerializeField]
        private Image bg;
        [SerializeField]
        private Sprite ownedBg, notOwnedBg, lockedBg;
        public UnityEvent BuyEvent;

        // Start is called before the first frame update
        void Start()
        {
            // todo when owned save the data
            //when start, check if the data saved is available then change owned status accordingly
            BuyEvent = new UnityEvent();
            BuyEvent.AddListener(BuyingItem);

            Owned = DataManager.Instance.WeaponOwnedStatus[weaponName.ToString() + DataManager.ItemOwnedStatus] != "" ? true : false;

            Locked = weaponName == WeaponName.BarrelBomb? true:false;
            ItemButton.onClick.AddListener(OnButtonClicked);
            RefreshItem();
        }

        private void OnEnable()
        {
            RefreshItem();
        }

        private void OnDestroy()
        {
            ItemButton.onClick.RemoveListener(OnButtonClicked);
            BuyEvent.RemoveListener(BuyingItem);
        }

        private void OnButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);

            RefreshItem();

            if (isBuyGem || IsBuyGold)
            {
                ItemInformationPopUp.gameObject.SetActive(true);
                ItemInformationPopUp.InitWeaponItemData(this);
                return;
            }

            ItemInformationPopUp.gameObject.SetActive(true);
            ItemInformationPopUp.InitWeaponItemData(this);
        }

        private void BuyingItem()
        {
            if(isBuyGem || IsBuyGold)
            {
                if (IsBuyGold && GameManager.Instance.Gem >= Price)
                {
                    GameManager.Instance.UsingGem(Price);
                    GameManager.Instance.GainMoney(CurrencyBought);
                }
                RefreshItem();
                MenusManager.Instance.RefreshMenu();
                return;
            }

            switch (CurrencyType)
            {
                case CurrencyType.Gold:
                    if (GameManager.Instance.Money >= Price)
                    {
                        GameManager.Instance.UsingMoney(Price);

                        Owned = true;
                        PlayerPrefs.SetString(weaponName.ToString() + DataManager.ItemOwnedStatus, DataManager.ItemOwnedStatus);
                        DataManager.Instance.UpdateData();
                        RefreshItem();
                        MenusManager.Instance.RefreshMenu();
                    }
                    break;
                case CurrencyType.Gem:
                    if (GameManager.Instance.Gem >= Price)
                    {
                        GameManager.Instance.UsingGem(Price);

                        Owned = true;
                        PlayerPrefs.SetString(weaponName.ToString() + DataManager.ItemOwnedStatus, DataManager.ItemOwnedStatus);
                        DataManager.Instance.UpdateData();
                        RefreshItem();
                        MenusManager.Instance.RefreshMenu();
                    }
                    break;
                case CurrencyType.Money:
                    break;
            }
        }

        public void RefreshItem()
        {
            if(isBuyGem || IsBuyGold)
            {
                Locked = Owned = false;
            }

            bg.sprite = Locked ? lockedBg : Owned ? ownedBg : notOwnedBg;

            lockedStatusObject.SetActive(Locked);
            ownedStatusObject.SetActive(Owned && !Locked);
            priceObject.SetActive(!Owned && !Locked);

            lockedStatusObject.SetActive(Locked);
            ownedStatusObject.SetActive(Owned && !Locked);
            priceObject.SetActive(!Owned);

            goldLogo.SetActive(CurrencyType == CurrencyType.Gold);
            gemLogo.SetActive(CurrencyType == CurrencyType.Gem);
            moneyLogo.SetActive(CurrencyType == CurrencyType.Money);

            priceText.text = Price.ToString();
        }
    }

}
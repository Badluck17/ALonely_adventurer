using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Menus;
using Zycalipse.Managers;
using Zycalipse.Systems;
using UnityEngine.Events;

namespace Zycalipse.Items
{
    public class InventoryWeaponItem : InventoryItem
    {
        public Sprite skillSprite, areaSprite;

        public WeaponName weaponName;
        public WeaponType weaponType;
        private EquipedWeaponRangeType weaponRangeType;
        [SerializeField]
        private InventoryItemInformationMenu ItemInformationPopUp;
        [SerializeField]
        private GameObject lockedStatusObject, statusObject;
        [SerializeField]
        private TextMeshProUGUI powerText;
        public UnityEvent EquipEvent;

        // Start is called before the first frame update
        void Start()
        {
            // todo when owned save the data
            //when start, check if the data saved is available then change owned status accordingly

            EquipEvent = new UnityEvent();
            EquipEvent.AddListener(OnEquipButtonClicked);

            for (int i = 0; i < DataManager.Instance.playerAttackArea.Length; i++)
            {
                if (DataManager.Instance.playerAttackArea[i].Name == weaponName)
                {
                    ItemDescription = DataManager.Instance.playerAttackArea[i].Description;
                }
            }

            switch (weaponType)
            {
                case WeaponType.CloseRange:
                    weaponRangeType = EquipedWeaponRangeType.EquipedCloseRangeWeapon;
                    break;
                case WeaponType.MidRange:
                    weaponRangeType = EquipedWeaponRangeType.EquipedMidRangeWeapon;
                    break;
                case WeaponType.LongRange:
                    weaponRangeType = EquipedWeaponRangeType.EquipedLongRangeWeapon;
                    break;
            }

            // bug first load is always false
            if(weaponName == WeaponName.Pistol)
                ItemInformationPopUp.InitWeaponItemData(this);

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
            EquipEvent.RemoveListener(OnEquipButtonClicked);
        }

        private void OnButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            RefreshItem();
            ItemInformationPopUp.gameObject.SetActive(true);
            ItemInformationPopUp.InitWeaponItemData(this);
        }

        private void OnEquipButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            PlayerPrefs.SetString(weaponRangeType.ToString(), weaponName.ToString());
            DataManager.Instance.UpdateData();
            MenusManager.Instance.RefreshMenu();
            RefreshItem();
        }

        public void RefreshItem()
        {
            Equiped = false;
            for (int i = 0; i < DataManager.Instance.EquipedWeapons.Length; i++)
            {
                if (DataManager.Instance.EquipedWeapons[i] == weaponName)
                    Equiped = true;
            }

            Owned = DataManager.Instance.WeaponOwnedStatus[weaponName.ToString() + DataManager.ItemOwnedStatus] != "" ? true : false;

            Locked = CurrencyType == CurrencyType.Gem ? true : false;

            lockedStatusObject.SetActive(Locked);
            statusObject.SetActive(!Locked);

            powerText.text = Power.ToString();
        }
    }
}

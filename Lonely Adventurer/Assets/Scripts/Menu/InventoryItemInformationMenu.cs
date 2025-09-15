using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Zycalipse.Managers;
using Zycalipse.Systems;
using Zycalipse.Items;

namespace Zycalipse.Menus
{
    public class InventoryItemInformationMenu : Menu
    {
        [SerializeField]
        private Image skillImage, areaImage;

        [SerializeField]
        private InventoryMenu inventoryMenu;
        [SerializeField]
        private Button equipeButton, closeButton;
        [SerializeField]
        private TextMeshProUGUI itemName, descriptionText, skillName, weaponAreaType;
        [SerializeField]
        private TextMeshProUGUI[] powerText;
        [SerializeField]
        private TextMeshProUGUI[] skillEffectText;
        [SerializeField]
        private TextMeshProUGUI[] skillDurationText;
        [SerializeField]
        private RectTransform scrollTransform;

        private UnityEvent equipEvent;

        private int power;

        void Start()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (scrollTransform.anchoredPosition.y < 0)
                scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, 0);
            else if (scrollTransform.anchoredPosition.y > 400)
                scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, 400);
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
            scrollTransform.anchoredPosition = new Vector2(scrollTransform.anchoredPosition.x, 0);
        }
        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
        }
        public override void RegisterListener()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            equipeButton.onClick.AddListener(OnEquipButtonClicked);
        }
        public override void UnRegisterListener()
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            equipeButton.onClick.RemoveListener(OnEquipButtonClicked);
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
            // change info
        }

        private void OnCloseButtonClicked()
        {
            ResetPopUp();
        }

        private void OnEquipButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.EquipeWeapon, transform.position);
            equipEvent.Invoke();
            ResetPopUp();
        }
        private void ResetPopUp()
        {
            equipEvent = null;
            itemName.text = "";
            descriptionText.text = "";
            power = 0;

            for (int i = 0; i < powerText.Length; i++)
            {
                powerText[i].text = power.ToString();
            }

            inventoryMenu.RefreshMenu();
            gameObject.SetActive(false);
        }

        public void InitWeaponItemData(InventoryWeaponItem weaponItem)
        {
            WeaponSkills skill = WeaponSkills.MultiShot;

            skillImage.sprite = weaponItem.skillSprite;
            areaImage.sprite = weaponItem.areaSprite;

            equipEvent = weaponItem.EquipEvent;
            itemName.text = weaponItem.weaponName.ToString();
            descriptionText.text = weaponItem.ItemDescription;
            power = weaponItem.Power;
            for (int i = 0; i < powerText.Length; i++)
            {
                powerText[i].text = power.ToString();
            }

            for (int i = 0; i < DataManager.Instance.playerAttackArea.Length; i++)
            {
                if (DataManager.Instance.playerAttackArea[i].Name == weaponItem.weaponName)
                {
                    skillName.text = DataManager.Instance.playerAttackArea[i].Skill.ToString();
                    weaponAreaType.text = DataManager.Instance.playerAttackArea[i].Type.ToString();
                    skill = DataManager.Instance.playerAttackArea[i].Skill;
                }
            }

            for (int i = 0; i < skillEffectText.Length; i++)
            {
                switch (skill)
                {
                    case WeaponSkills.Burn:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.BurnSkillData.Damage[i].ToString();
                        break;
                    case WeaponSkills.Freeze:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.FreezeSkillData.DecreaseAttackPrecentage[i].ToString();
                        break;
                    case WeaponSkills.Lightning:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.LightningSkillData.DamagePrecentage[i].ToString();
                        break;
                    case WeaponSkills.Poison:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.PoisonSkillData.Damage[i].ToString();
                        break;
                    case WeaponSkills.Blind:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.BlindSkillData.AccuracyDecreasePrecentage[i].ToString();
                        break;
                    case WeaponSkills.Bleed:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.BleedSkillData.Damage[i].ToString();
                        break;
                    case WeaponSkills.Stun:
                        skillEffectText[i].text = DataManager.Instance.allWeaponSkillDatas.StunSkillData.StunChance[i].ToString();
                        break;
                }
            }

            for (int i = 0; i < skillDurationText.Length; i++)
            {
                switch (skill)
                {
                    case WeaponSkills.Burn:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.BurnSkillData.TurnLast[i].ToString() + " Turn";
                        break;
                    case WeaponSkills.Freeze:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.FreezeSkillData.TurnLast[i].ToString() + " Turn";
                        break;
                    case WeaponSkills.Lightning:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.LightningSkillData.EnemyEffected[i].ToString() + " Enemy";
                        break;
                    case WeaponSkills.Poison:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.PoisonSkillData.TurnLast[i].ToString() + " Turn";
                        break;
                    case WeaponSkills.Blind:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.BlindSkillData.TurnLast[i].ToString() + " Turn";
                        break;
                    case WeaponSkills.Bleed:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.BleedSkillData.TurnLast[i].ToString() + " Turn";
                        break;
                    case WeaponSkills.Stun:
                        skillDurationText[i].text = DataManager.Instance.allWeaponSkillDatas.StunSkillData.HealPrecentage[i].ToString() + " Heal";
                        break;
                }
            }

            equipeButton.gameObject.SetActive(weaponItem.Owned && !weaponItem.Locked && !weaponItem.Equiped);
        }
    }
}

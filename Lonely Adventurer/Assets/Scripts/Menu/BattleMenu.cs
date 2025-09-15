using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Managers;
using Zycalipse.Systems;

namespace Zycalipse.Menus
{
    public class BattleMenu : Menu
    {
        [SerializeField]
        private Sprite pistol, pistolSkill, shotgun, shotgunSkill, sniper, sniperSkill,
            gauntlet, gauntletSkill, energyGun, energyGunSkill, energySniper, energySniperSkill,
            barrel, barrelSkill, c4, c4Skill, launcher, launcherSkill;

        [SerializeField]
        private Button playerAttackButton, closeRangeButton, midRangeButton, longRangeButton, cheatExpButton;
        [SerializeField]
        private RectTransform closeRangeSafeArea, midRangeSafeArea, longRangeSafeArea;
        [SerializeField]
        private TextMeshProUGUI closeRangeAttackPower, midRangeAttackPower, longRangeAttackPower;
        // need better implementation
        [SerializeField]
        private Image pistolIcon, gauntletPistolIcon, barrelBombIcon;
        [SerializeField]
        private Image shotgunIcon, energyGunIcon, c4Icon;
        [SerializeField]
        private Image sniperIcon, energySniperIcon, launcherIcon;
        [SerializeField]
        private Text playerLevelIndicator;
        [SerializeField]
        private Image PlayerExpBar;
        [SerializeField]
        public Text level;
        [SerializeField]
        private Button pasuseButton;
        [SerializeField]
        private GameObject tutorialUI;
        [SerializeField]
        private RectTransform highlightTutorial;

        public bool isTutorialPopUpTriggered { get; set; }

        [SerializeField]
        private Color unUsedWeaponColor, usedWeaponColor;

        private WeaponType equipedWeaponType;
        private bool toTop = true;
        private float speed = 20f;

        private async void cheatexp()
        {
            await Systems.CombatSystem.Instance.playerManager.playerData.PlayerGainExp(20f);
        }

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentMenu = this;
            RefreshMenu();
            closeRangeButton.onClick.AddListener(LowRangeWeaponSelected);
            midRangeButton.onClick.AddListener(MidRangeWeaponSelected);
            longRangeButton.onClick.AddListener(LongRangeWeaponSelected);
            cheatExpButton.onClick.AddListener(cheatexp);
        }

        public override void OnFocusOut()
        {
            closeRangeButton.onClick.RemoveAllListeners();
            midRangeButton.onClick.RemoveAllListeners();
            longRangeButton.onClick.RemoveAllListeners();
            cheatExpButton.onClick.RemoveListener(cheatexp);
        }

        public override void RegisterListener()
        {
            playerAttackButton.onClick.AddListener(PlayerAttack);
            pasuseButton.onClick.AddListener(OnPauseButtonClicked);
        }

        public override void UnRegisterListener()
        {
            playerAttackButton.onClick.RemoveListener(PlayerAttack);
        }

        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnPush()
        {
            RegisterListener();
        }

        private void OnEnable()
        {
            RefreshMenu();
            OnFocusIn();
        }

        private void OnDisable()
        {
            RefreshMenu();
            OnFocusOut();
        }

        private void Start()
        {
            tutorialUI.SetActive(false);
            isTutorialPopUpTriggered = false;
            OnPush();
            RefreshMenu();
        }

        private void LateUpdate()
        {
            if (!GameManager.Instance.IsFTUE && tutorialUI.activeSelf)
            {
                tutorialUI.SetActive(false);
            }
            else

            if (GameManager.Instance.TimeToTriggerTutorialPopUp && GameManager.Instance.IsFTUE)
            {
                tutorialUI.SetActive(true);
                if(EnemysManager.Instance.enemyList.Count > 0)
                    highlightTutorial.position = MenusManager.Instance.mainCam.WorldToScreenPoint(EnemysManager.Instance.enemyList[0].transform.position);
            }
            switch (equipedWeaponType)
            {
                case WeaponType.CloseRange:
                    if (closeRangeSafeArea.anchoredPosition.y < 0)
                        toTop = true;
                    else if (closeRangeSafeArea.anchoredPosition.y > 50)
                        toTop = false;

                    if (toTop)
                        closeRangeSafeArea.anchoredPosition = 
                            new Vector2(closeRangeSafeArea.anchoredPosition.x, closeRangeSafeArea.anchoredPosition.y + speed * Time.deltaTime);
                    else if(!toTop)
                        closeRangeSafeArea.anchoredPosition = 
                            new Vector2(closeRangeSafeArea.anchoredPosition.x, closeRangeSafeArea.anchoredPosition.y + -speed * Time.deltaTime);
                    break;
                case WeaponType.MidRange:
                    if (midRangeSafeArea.anchoredPosition.y < 0)
                        toTop = true;
                    else if (midRangeSafeArea.anchoredPosition.y > 50)
                        toTop = false;

                    if (toTop)
                        midRangeSafeArea.anchoredPosition = 
                            new Vector2(midRangeSafeArea.anchoredPosition.x, midRangeSafeArea.anchoredPosition.y + speed * Time.deltaTime);
                    else if (!toTop)
                        midRangeSafeArea.anchoredPosition = 
                            new Vector2(midRangeSafeArea.anchoredPosition.x, midRangeSafeArea.anchoredPosition.y + -speed * Time.deltaTime);
                    break;
                case WeaponType.LongRange:
                    if (longRangeSafeArea.anchoredPosition.y < 0)
                        toTop = true;
                    else if (longRangeSafeArea.anchoredPosition.y > 50)
                        toTop = false;

                    if (toTop)
                        longRangeSafeArea.anchoredPosition = 
                            new Vector2(longRangeSafeArea.anchoredPosition.x, longRangeSafeArea.anchoredPosition.y + speed * Time.deltaTime);
                    else if (!toTop)
                        longRangeSafeArea.anchoredPosition = 
                            new Vector3(longRangeSafeArea.anchoredPosition.x, longRangeSafeArea.anchoredPosition.y + -speed * Time.deltaTime);
                    break;
            }
        }

        private void OnDestroy()
        {
            OnPop();
        }

        private void ResetWeaponButtonAnimation()
        {
            closeRangeSafeArea.anchoredPosition = new Vector2(closeRangeSafeArea.anchoredPosition.x, 0);
            midRangeSafeArea.anchoredPosition = new Vector2(midRangeSafeArea.anchoredPosition.x, 0);
            longRangeSafeArea.anchoredPosition = new Vector2(longRangeSafeArea.anchoredPosition.x, 0);
        }

        public async override void RefreshMenu()
        {
            while (CombatSystem.Instance.playerManager == null || !CombatSystem.Instance.playerManager.playerInited)
                await System.Threading.Tasks.Task.Yield();

            midRangeSafeArea.gameObject.SetActive(!GameManager.Instance.IsFTUE);
            longRangeSafeArea.gameObject.SetActive(!GameManager.Instance.IsFTUE);
            midRangeButton.gameObject.SetActive(!GameManager.Instance.IsFTUE);
            longRangeButton.gameObject.SetActive(!GameManager.Instance.IsFTUE);

            playerLevelIndicator.text = CombatSystem.Instance.playerManager.playerData.PlayerLevel.ToString();
            PlayerExpBar.fillAmount = CombatSystem.Instance.playerManager.playerData.PlayerExp / 
                CombatSystem.Instance.playerManager.playerData.ExpNeededEachLevel
                [CombatSystem.Instance.playerManager.playerData.PlayerLevel - 1];
            level.text = $"Level {LevelManager.Instance.CurrentLevel}";
            if(GameManager.Instance.IsFTUE)
                level.text = $"Level Tutorial";

            if (GameManager.Instance.ZycalipseGameState == GameState.PreparingCombat)
            {
                equipedWeaponType = WeaponType.CloseRange;
                ResetWeaponButtonAnimation();
            }

            ChangeWeaponButtonUI(equipedWeaponType);
            UpdateEquipedWeaponUI();
        }

        private void PlayerAttack()
        {
            if (GameManager.Instance.ZycalipseGameState == GameState.PlayerTurn)
            {
                CombatSystem.Instance.PlayerAttacking();
            }
        }

        private void ChangeWeaponButtonUI(WeaponType type)
        {
            if (equipedWeaponType == type)
            {
                PlayerAttack();
            }

            if (equipedWeaponType != type)
                ResetWeaponButtonAnimation();

            equipedWeaponType = type;

            closeRangeButton.GetComponent<Image>().color = equipedWeaponType == WeaponType.CloseRange ? usedWeaponColor : unUsedWeaponColor;
            midRangeButton.GetComponent<Image>().color = equipedWeaponType == WeaponType.MidRange ? usedWeaponColor : unUsedWeaponColor;
            longRangeButton.GetComponent<Image>().color = equipedWeaponType == WeaponType.LongRange ? usedWeaponColor : unUsedWeaponColor;
        }

        private void UpdateEquipedWeaponUI()
        {
            int closeRangeDamage = 0;
            int midRangeDamage = 0;
            int longRangeDamage = 0;

            WeaponName closeRangeName = WeaponName.Pistol;
            WeaponName midRangeName = WeaponName.Shotgun;
            WeaponName longRangeName = WeaponName.Sniper;

            for (int i = 0; i < CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped.Count; i++)
            {
                switch (CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i])
                {
                    case WeaponName.Pistol:
                        closeRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.Pistol].Damage;
                        closeRangeName = WeaponName.Pistol;
                        break;
                    case WeaponName.Shotgun:
                        midRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.Shotgun].Damage;
                        midRangeName = WeaponName.Shotgun;
                        break;
                    case WeaponName.Sniper:
                        longRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.Sniper].Damage;
                        longRangeName = WeaponName.Sniper;
                        break;


                    case WeaponName.GauntletPistol:
                        closeRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.GauntletPistol].Damage;
                        closeRangeName = WeaponName.GauntletPistol;
                        break;
                    case WeaponName.EnergyGun:
                        midRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.EnergyGun].Damage;
                        midRangeName = WeaponName.EnergyGun;
                        break;
                    case WeaponName.EnergySniper:
                        longRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.EnergySniper].Damage;
                        longRangeName = WeaponName.EnergySniper;
                        break;


                    case WeaponName.BarrelBomb:
                        closeRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.BarrelBomb].Damage;
                        closeRangeName = WeaponName.BarrelBomb;
                        break;
                    case WeaponName.C4:
                        midRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.C4].Damage;
                        midRangeName = WeaponName.C4;
                        break;
                    case WeaponName.Launcher:
                        longRangeDamage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[WeaponName.Launcher].Damage;
                        longRangeName = WeaponName.Launcher;
                        break;
                }
            }

            closeRangeAttackPower.text = closeRangeDamage.ToString();
            midRangeAttackPower.text = midRangeDamage.ToString();
            longRangeAttackPower.text = longRangeDamage.ToString();

            pistolIcon.gameObject.SetActive(closeRangeName == WeaponName.Pistol);
            shotgunIcon.gameObject.SetActive(midRangeName == WeaponName.Shotgun);
            sniperIcon.gameObject.SetActive(longRangeName == WeaponName.Sniper);

            gauntletPistolIcon.gameObject.SetActive(closeRangeName == WeaponName.GauntletPistol);
            energyGunIcon.gameObject.SetActive(midRangeName == WeaponName.EnergyGun);
            energySniperIcon.gameObject.SetActive(longRangeName == WeaponName.EnergySniper);

            barrelBombIcon.gameObject.SetActive(closeRangeName == WeaponName.BarrelBomb);
            c4Icon.gameObject.SetActive(midRangeName == WeaponName.C4);
            launcherIcon.gameObject.SetActive(longRangeName == WeaponName.Launcher);

            for (int i = 0; i < CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped.Count; i++)
            {
                var name = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];

                switch (name)
                {
                    case WeaponName.Pistol:
                        pistolIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? pistol : pistolSkill;
                        break;
                    case WeaponName.Shotgun:
                        shotgunIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? shotgun : shotgunSkill;
                        break;
                    case WeaponName.Sniper:
                        sniperIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? sniper : sniperSkill;
                        break;
                    case WeaponName.GauntletPistol:
                        gauntletPistolIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? gauntlet : gauntletSkill;
                        break;
                    case WeaponName.EnergyGun:
                        energyGunIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? energyGun : energyGunSkill;
                        break;
                    case WeaponName.EnergySniper:
                        energySniperIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? energySniper : energySniperSkill;
                        break;
                    case WeaponName.BarrelBomb:
                        barrelBombIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? barrel : barrelSkill;
                        break;
                    case WeaponName.C4:
                        c4Icon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? c4 : c4Skill;
                        break;
                    case WeaponName.Launcher:
                        launcherIcon.sprite = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[name].SkillLevel == 0 ? launcher : launcherSkill;
                        break;
                }
            }
        }

        private async void LowRangeWeaponSelected()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            if (GameManager.Instance.ZycalipseGameState != GameState.PlayerTurn)
                return;

            ChangeWeaponButtonUI(WeaponType.CloseRange);

            await CombatSystem.Instance.playerManager.PlayerChangedWeapon(DataManager.Instance.EquipedWeapons[0]);
        }
        private async void MidRangeWeaponSelected()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            if (GameManager.Instance.ZycalipseGameState != GameState.PlayerTurn)
                return;

            ChangeWeaponButtonUI(WeaponType.MidRange);

            await CombatSystem.Instance.playerManager.PlayerChangedWeapon(DataManager.Instance.EquipedWeapons[1]);
        }
        private async void LongRangeWeaponSelected()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            if (GameManager.Instance.ZycalipseGameState != GameState.PlayerTurn)
                return;

            ChangeWeaponButtonUI(WeaponType.LongRange);

            await CombatSystem.Instance.playerManager.PlayerChangedWeapon(DataManager.Instance.EquipedWeapons[2]);
        }

        private async void OnPauseButtonClicked()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            MenusManager.Instance.ActiveMenu(MenuList.IGM);
        }
    }
}

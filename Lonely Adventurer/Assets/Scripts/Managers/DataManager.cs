using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Menus;
using Zycalipse.GameLogger;

namespace Zycalipse.Managers
{
    public class DataManager : MonoBehaviour
    {
        private static DataManager instance;
        public static DataManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameLog logger;

        public PlayerAttackArea[] playerAttackArea;
        public Players.AllWeaponSkillDatas allWeaponSkillDatas;

        public WeaponName[] EquipedWeapons { get; private set; }

        public static string ItemOwnedStatus = "Owned";
        public Dictionary<string, string> WeaponOwnedStatus;

        [Header("All Sound List")]
        [Header("UI Sound")]
        public AudioClip ButtonCloseSound;
        public AudioClip ButtonOpenSound;

        [Header("Weapon Sound")]
        public AudioClip PistolSound;
        public AudioClip ShotgunSound;
        public AudioClip SniperSound;
        public AudioClip GauntletPistolSound;
        public AudioClip EnergyGunSound;
        public AudioClip EnergySniperSound;
        public AudioClip BarrelBombSound;
        public AudioClip C4Sound;
        public AudioClip LauncherSound;

        [Header("Weapon Sound")]
        public AudioClip MainMenuMusic;
        public AudioClip CombatMusic;

        [Header("Sounds Effect For Player")]
        public AudioClip[] PlayerWalkingSounds;

        [Header("Sounds Effect For Enemy")]
        public AudioClip[] EnemyHitSounds;

        [Header("Sound for zombies")]
        public AudioClip[] ZombieAttack;

        [Header("Others")]
        public AudioClip ExpColected;
        public AudioClip RollingSkill;
        public AudioClip WheelSkillStop;
        public AudioClip PlayerLevelup;
        public AudioClip EquipeWeapon;
        public AudioClip PlayerWin;
        public AudioClip GameOver;

        // Rule 101 :
        /*
         For item owned save name = itemName + ItemOwnedStatus
        For weapon equiped save name = equipedWeaponType
        For player char level save name = PlayerCharacterName

        example : 
        string PistolOwned = Owned // own a pistol
        string LauncherOwned = "" // doesn't own a launcher

        string EquipedCloseRangeWeapon = Pistol // pistol equiped
        string EquipedCloseRangeWeapon = "" // nothing is equiped, equip default weapon
        
        int PlayerCharacterName = 1 // level 1
        int PlayerCharacterName = 0 // level 0 new game
         */

        // Start is called before the first frame update
        void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
        }

        // Update is called once per frame
        void Start()
        {
            allWeaponSkillDatas = new Players.AllWeaponSkillDatas();
            EquipedWeapons = new WeaponName[3];
            WeaponOwnedStatus = new Dictionary<string, string>();
            UpdateData();
        }

        public async void UpdateData()
        {
            CheckOwnedWeapon();
            CheckEquipedWeaponData();
            if(MenusManager.Instance != null)
                MenusManager.Instance.RefreshMenu();
        }

        private async void CheckOwnedWeapon()
        {
            WeaponOwnedStatus.Clear();
            string defaultVal = "";
            WeaponName name = WeaponName.Pistol;

            var nameLength = System.Enum.GetValues(typeof(WeaponName)).Length;

            for (int i = 0; i < nameLength; i++, name++)
            {
                var ownedStatus = PlayerPrefs.GetString(name.ToString()+ItemOwnedStatus, defaultVal);

                if(name == WeaponName.Pistol || name == WeaponName.Shotgun || name == WeaponName.Sniper)
                {
                    // must own 3 basic weapon
                    PlayerPrefs.SetString(name.ToString() + ItemOwnedStatus, ItemOwnedStatus);
                    WeaponOwnedStatus.Add(name.ToString() + ItemOwnedStatus, ItemOwnedStatus);
                }
                else
                {
                    WeaponOwnedStatus.Add(name.ToString() + ItemOwnedStatus, ownedStatus);
                }
            }
            // todo make a funct to set and check when user buy or equip or upgrade something that need to be saved
        }

        private async void CheckEquipedWeaponData()
        {
            string defaultVal = "";
            EquipedWeaponRangeType typeList = EquipedWeaponRangeType.EquipedCloseRangeWeapon;

            for (int i = 0; i < EquipedWeapons.Length; i++)
            {
                string equipedWeapon = PlayerPrefs.GetString(typeList.ToString(), defaultVal);

                if (equipedWeapon == "")
                {
                    // nothing equiped
                    // 3 basic weapon
                    WeaponName name = i == 0 ? WeaponName.Pistol : i == 1 ? WeaponName.Shotgun : WeaponName.Sniper;

                    PlayerPrefs.SetString(typeList.ToString(), name.ToString());
                    EquipedWeapons[i] = name;
                }
                else
                {
                    // something equiped

                    WeaponName nameList = WeaponName.Pistol;
                    for (int inc = 0; inc < playerAttackArea.Length; inc++)
                    {
                        if (equipedWeapon == nameList.ToString())
                        {
                            EquipedWeapons[i] = nameList;
                        }

                        if (inc < playerAttackArea.Length - 1)
                            nameList++;
                    }
                }
                typeList++;
            }
        }
    }
}

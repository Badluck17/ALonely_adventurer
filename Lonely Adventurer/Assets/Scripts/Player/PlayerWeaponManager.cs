using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zycalipse.GameLogger;

namespace Zycalipse.Players
{
    public class WeaponStat
    {
        public WeaponStat()
        {
            SkillLevel = 0;
        }

        public WeaponName Name;
        public WeaponType Type;
        public WeaponAttackType AttackType;
        public WeaponKnockbackDirection KnockBackDirection;
        public int KnockBackPower;
        public int Damage;
        public int WeaponDelay;
        public WeaponSkills Skill;
        public int SkillLevel;
    }

    public class PlayerWeaponManager
    {
        public PlayerWeaponManager()
        {
            logger = new GameLog(GetType());
            PlayerWeapons = new Dictionary<WeaponName, GameObject>();
            PlayerWeaponsStat = new Dictionary<WeaponName, WeaponStat>();
            AllWeaponEquiped = new List<WeaponName>();
        }
        public PlayerWeaponManager(PlayerWeaponManager playerWeaponManager)
        {
            logger = new GameLog(GetType());
            PlayerWeapons = new Dictionary<WeaponName, GameObject>();
            PlayerWeaponsStat = playerWeaponManager.PlayerWeaponsStat;
            AllWeaponEquiped = new List<WeaponName>();
        }

        private GameLog logger;

        public Dictionary<WeaponName, GameObject> PlayerWeapons { get; private set; }
        public Dictionary<WeaponName, WeaponStat> PlayerWeaponsStat;
        public WeaponName CurrentWeapon { get; private set; }
        public List<WeaponName> AllWeaponEquiped { get; set; }

        public void ChangeWeapon(WeaponName weapon)
        {
            if (PlayerWeapons.Count <= 0)
                return;

            CurrentWeapon = weapon;

            for (int i = 0; i < PlayerWeapons.Count; i++)
            {
                PlayerWeapons.ElementAt(i).Value.SetActive(false);
            }

            PlayerWeapons[CurrentWeapon].SetActive(true);
        }

        public async void AddOwnedWeapon(WeaponName weaponName, GameObject weaponAttackArea)
        {
            // load weapon and area only the one that we use
            PlayerWeapons.Add(weaponName, weaponAttackArea);
            AllWeaponEquiped.Add(weaponName);
        }
    }
}

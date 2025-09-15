using Zycalipse.Systems;
using UnityEngine;

namespace Zycalipse.Items
{
    public class PlayerLevelUpItem : ZycalipseItems
    {
        public LevelUpItemSlotNumber ItemColoumnNumber { get; set; }
        public LevelUpItems ItemType;
        public bool isWeaponSkill;
        public WeaponName weaponSkillName;
        public GameObject desc;

        private void Start()
        {
            if(desc != null)
                desc.SetActive(false);
        }

        public void ShowDesc()
        {
            if (desc != null)
                desc.SetActive(true);
        }

        public void ApplyEffect()
        {
            switch (ItemType)
            {
                case LevelUpItems.SmallHeal:
                    SmallHealEffect();
                    break;
                case LevelUpItems.MedHeal:
                    MedHealEffect();
                    break;
                case LevelUpItems.GreatHeal:
                    GreatHealEffect();
                    break;
                case LevelUpItems.ExtraLive:
                    GainExtraLife();
                    break;
                case LevelUpItems.IncreaseMaxLevel:
                    IncreaseMaxLevel();
                    break;
                case LevelUpItems.SkillLightning:
                    GainSkillLightning();
                    break;
                case LevelUpItems.SkillFreeze:
                    GainSkillFreeze();
                    break;
                case LevelUpItems.SkillBurn:
                    GainSkillBurn();
                    break;
                case LevelUpItems.SkillPoison:
                    GainSkillPoison();
                    break;
                case LevelUpItems.SkillBleed:
                    GainSkillBleed();
                    break;
                case LevelUpItems.SkillStun:
                    GainSkillStun();
                    break;
                case LevelUpItems.SkillBlind:
                    GainSkillBlind();
                    break;
                case LevelUpItems.SkillShadowClone:
                    SkillShadowClone();
                    break;
                case LevelUpItems.Rage:
                    SkillRage();
                    break;
                case LevelUpItems.FullPower:
                    SkillFullPower();
                    break;
                case LevelUpItems.GreedLife:
                    SkillGreed();
                    break;
                case LevelUpItems.Meteor:
                    SkillMeteor();
                    break;
            }
        }

        private async void SmallHealEffect()
        {
            await CombatSystem.Instance.playerManager.Heal(10);
        }
        private async void MedHealEffect()
        {
            await CombatSystem.Instance.playerManager.Heal(20);
        }
        private async void GreatHealEffect()
        {
            await CombatSystem.Instance.playerManager.Heal(50);
        }
        private void GainExtraLife()
        {
            CombatSystem.Instance.IncreasePlayerLife();
        }
        private async void IncreaseMaxLevel()
        {
            CombatSystem.Instance.IncreasePlayerMaxLevel();
            await CombatSystem.Instance.playerManager.playerData.PlayerGainExp(100f);
        }

        // rework skill assignment

        private async void GainSkillLightning()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Lightning
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillFreeze()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Freeze
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillBurn()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Burn
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillPoison()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Poison
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillBleed()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Bleed
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillStun()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Stun
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void GainSkillBlind()
        {
            for (int i = 0; i < 3; i++)
            {
                var weaponEquipedName = CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[i];
                if (CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].Skill == WeaponSkills.Blind
                    && CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel < 5
                    && weaponEquipedName == weaponSkillName)
                {
                    CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[weaponEquipedName].SkillLevel++;
                }
            }
        }
        private async void SkillShadowClone()
        {
            CombatSystem.Instance.alreadyGettingKageBunshin = true;
            UnityEngine.Debug.Log("qwe getting kage");
        }
        private async void SkillRage()
        {
            CombatSystem.Instance.alreadyGettingRage = true;
            UnityEngine.Debug.Log("qwe getting rage");
        }
        private async void SkillFullPower()
        {
            CombatSystem.Instance.alreadyGettingFullPower = true;
            UnityEngine.Debug.Log("qwe getting full powa");
        }
        private async void SkillGreed()
        {
            CombatSystem.Instance.alreadyGettingGreedyLife = true;
            UnityEngine.Debug.Log("qwe getting greed");
        }
        private async void SkillMeteor()
        {
            CombatSystem.Instance.alreadyGettingMeteor = true;
            UnityEngine.Debug.Log("qwe getting meteor");
        }
    }
}

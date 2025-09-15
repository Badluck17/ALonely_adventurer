using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zycalipse.Players
{
    public class AllWeaponSkillDatas
    {
        public AllWeaponSkillDatas()
        {
            BurnSkillData = new BurnSkillData();
            PoisonSkillData = new PoisonSkillData();
            FreezeSkillData = new FreezeSkillData();
            StunSkillData = new StunSkillData();
            BlindSkillData = new BlindSkillData();
            BleedSkillData = new BleedSkillData();
            LightningSkillData = new LightningSkillData();
            MultiShotSkillData = new MultiShotSkillData();
        }

        public BurnSkillData BurnSkillData;
        public PoisonSkillData PoisonSkillData;
        public FreezeSkillData FreezeSkillData;
        public StunSkillData StunSkillData;
        public BlindSkillData BlindSkillData;
        public BleedSkillData BleedSkillData;
        public LightningSkillData LightningSkillData;
        public MultiShotSkillData MultiShotSkillData;
    }

    public class BurnSkillData
    {
        public BurnSkillData()
        {
            Damage = new int[5];
            TurnLast = new int[5];

            Damage[0] = 1;
            TurnLast[0] = 1;

            Damage[1] = 2;
            TurnLast[1] = 1;

            Damage[2] = 2;
            TurnLast[2] = 2;

            Damage[3] = 3;
            TurnLast[3] = 2;

            Damage[4] = 3;
            TurnLast[4] = 3;
        }

        // damage overtime according weapon attack
        public int[] Damage;
        public int[] TurnLast;
    }
    public class PoisonSkillData
    {
        public PoisonSkillData()
        {
            Damage = new int[5];
            TurnLast = new int[5];

            Damage[0] = 2;
            TurnLast[0] = 2;

            Damage[1] = 3;
            TurnLast[1] = 2;

            Damage[2] = 4;
            TurnLast[2] = 2;

            Damage[3] = 5;
            TurnLast[3] = 3;

            Damage[4] = 6;
            TurnLast[4] = 3;
        }

        // damage overtime according weapon attack
        public int[] Damage;
        public int[] TurnLast;
    }
    public class FreezeSkillData
    {
        public FreezeSkillData()
        {
            DecreaseAttackPrecentage = new int[5];
            TurnLast = new int[5];

            DecreaseAttackPrecentage[0] = 32;
            TurnLast[0] = 1;

            DecreaseAttackPrecentage[1] = 34;
            TurnLast[1] = 1;

            DecreaseAttackPrecentage[2] = 36;
            TurnLast[2] = 1;

            DecreaseAttackPrecentage[3] = 38;
            TurnLast[3] = 1;

            DecreaseAttackPrecentage[4] = 40;
            TurnLast[4] = 1;
        }

        // damage overtime according weapon attack
        public int[] DecreaseAttackPrecentage;
        public int[] TurnLast;
    }
    public class StunSkillData
    {
        public StunSkillData()
        {
            HealPrecentage = new int[5];
            StunChance = new int[5];

            HealPrecentage[0] = 1;
            StunChance[0] = 40;

            HealPrecentage[1] = 2;
            StunChance[1] = 40;

            HealPrecentage[2] = 3;
            StunChance[2] = 40;

            HealPrecentage[3] = 4;
            StunChance[3] = 40;

            HealPrecentage[4] = 5;
            StunChance[4] = 60;
        }

        // damage overtime according weapon attack
        public int[] HealPrecentage;
        public int[] StunChance;
    }
    public class BlindSkillData
    {
        public BlindSkillData()
        {
            AccuracyDecreasePrecentage = new int[5];
            TurnLast = new int[5];

            AccuracyDecreasePrecentage[0] = 32;
            TurnLast[0] = 1;

            AccuracyDecreasePrecentage[1] = 34;
            TurnLast[1] = 1;

            AccuracyDecreasePrecentage[2] = 36;
            TurnLast[2] = 1;

            AccuracyDecreasePrecentage[3] = 38;
            TurnLast[3] = 1;

            AccuracyDecreasePrecentage[4] = 40;
            TurnLast[4] = 1;
        }

        // damage overtime according weapon attack
        public int[] AccuracyDecreasePrecentage;
        public int[] TurnLast;
    }
    public class BleedSkillData
    {
        public BleedSkillData()
        {
            Damage = new int[5];
            TurnLast = new int[5];

            Damage[0] = 5;
            TurnLast[0] = 1;

            Damage[1] = 6;
            TurnLast[1] = 1;

            Damage[2] = 7;
            TurnLast[2] = 1;

            Damage[3] = 8;
            TurnLast[3] = 2;

            Damage[4] = 9;
            TurnLast[4] = 2;
        }

        // damage overtime according weapon attack
        public int[] Damage;
        public int[] TurnLast;
    }
    public class LightningSkillData
    {
        public LightningSkillData()
        {
            DamagePrecentage = new int[5];
            EnemyEffected = new int[5];

            DamagePrecentage[0] = 22;
            EnemyEffected[0] = 1;

            DamagePrecentage[1] = 22;
            EnemyEffected[1] = 1;

            DamagePrecentage[2] = 25;
            EnemyEffected[2] = 1;

            DamagePrecentage[3] = 27;
            EnemyEffected[3] = 1;

            DamagePrecentage[4] = 30;
            EnemyEffected[4] = 1;
        }

        // damage overtime according weapon attack
        public int[] DamagePrecentage;
        public int[] EnemyEffected;
    }
    public class MultiShotSkillData
    {
        public MultiShotSkillData()
        {
            DamagePrecentage = new int[5];
            ExtraHitCount = new int[5];

            DamagePrecentage[0] = 20;
            ExtraHitCount[0] = 3;

            DamagePrecentage[1] = 20;
            ExtraHitCount[1] = 4;

            DamagePrecentage[2] = 20;
            ExtraHitCount[2] = 5;

            DamagePrecentage[3] = 20;
            ExtraHitCount[3] = 6;

            DamagePrecentage[4] = 20;
            ExtraHitCount[4] = 7;
        }

        // damage overtime according weapon attack
        public int[] DamagePrecentage;
        public int[] ExtraHitCount;
    }
}

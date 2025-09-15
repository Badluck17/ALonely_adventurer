using System.Threading.Tasks;
using Zycalipse.GameLogger;
using Zycalipse.Systems;

namespace Zycalipse.Enemys
{
    [System.Serializable]
    public class SkillsData
    {
        public EnemySkills Skills;
        public int Effect;
    }

    [System.Serializable]
    public class EnemyBehavior
    {
        public int BehaviorChance;
        public EnemyMoveDirections[] MoveDirections;
        public bool HasSkill;
        public SkillsData[] Skills;
    }

    public class EnemyDatas
    {
        private GameLog logger;

        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public int Power { get; private set; }
        public int Defense { get; private set; }
        public int EvasionRate { get; private set; }
        public int CritChance { get; private set; }
        public int CritDamage { get; private set; }
        public int ExpDrop { get; private set; }
        public int GoldDrop { get; private set; }
        public int HealthDropPrecentage { get; private set; }
        public EnemyDropType HealthDropType { get; private set; }
        public EnemyAttackRange[] Range { get; private set; }
        public EnemyBehavior[] Behaviors { get; private set; }
        public EnemyBehavior DefaultBehavior { get; private set; }

        public EnemyDatas(EnemyBaseData baseData)
        {
            logger = new GameLog(GetType());
            Health = baseData.HealthPoint;
            Power = baseData.AttackPower;
            Range = baseData.Range;
            Defense = baseData.DefensePower;
            EvasionRate = baseData.EvasionRate;
            CritChance = baseData.CritChance;
            CritDamage = baseData.CritDamage;
            ExpDrop = baseData.ExpDrop;
            GoldDrop = baseData.GoldDrop;
            HealthDropPrecentage = baseData.HealthDropPrecentage;
            HealthDropType = baseData.HealthDropType;
            MaxHealth = Health;

            DefaultBehavior = new EnemyBehavior();
            DefaultBehavior.BehaviorChance = baseData.DefaultBehavior.BehaviorChance;

            DefaultBehavior.MoveDirections = new EnemyMoveDirections[baseData.DefaultBehavior.MoveDirections.Length];
            DefaultBehavior.MoveDirections = baseData.DefaultBehavior.MoveDirections;

            DefaultBehavior.HasSkill = baseData.DefaultBehavior.Skills.Length > 0 ? true : false;
            if (DefaultBehavior.HasSkill)
            {
                DefaultBehavior.Skills = new SkillsData[baseData.DefaultBehavior.Skills.Length];
                for (int i = 0; i < DefaultBehavior.Skills.Length; i++)
                {
                    DefaultBehavior.Skills[i].Skills = baseData.DefaultBehavior.Skills[i].Skills;
                    DefaultBehavior.Skills[i].Effect = baseData.DefaultBehavior.Skills[i].Effect;
                }
            }

            Behaviors = new EnemyBehavior[baseData.EnemyBehaviors.Length];
            for (int i = 0; i < Behaviors.Length; i++)
            {
                Behaviors[i] = new EnemyBehavior();
                Behaviors[i].BehaviorChance = baseData.EnemyBehaviors[i].BehaviorChance;

                Behaviors[i].MoveDirections = new EnemyMoveDirections[baseData.EnemyBehaviors[i].MoveDirections.Length];
                Behaviors[i].MoveDirections = baseData.EnemyBehaviors[i].MoveDirections;

                if (!baseData.EnemyBehaviors[i].HasSkill)
                {
                    Behaviors[i].HasSkill = false;
                    Behaviors[i].Skills = null;
                    continue;
                }

                Behaviors[i].HasSkill = baseData.EnemyBehaviors[i].HasSkill;
                Behaviors[i].Skills = new SkillsData[baseData.EnemyBehaviors[i].Skills.Length];

                for (int skillInc = 0; skillInc < Behaviors[i].Skills.Length; skillInc++)
                {
                    Behaviors[i].Skills[skillInc] = new SkillsData();
                    Behaviors[i].Skills[skillInc].Skills = baseData.EnemyBehaviors[i].Skills[skillInc].Skills;
                    Behaviors[i].Skills[skillInc].Effect = baseData.EnemyBehaviors[i].Skills[skillInc].Effect;
                }
            }
        }

        public async Task Healed(int amount)
        {
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;

            logger.Information($"healing {amount} hp");
        }

        public async Task TakingDamage(int amount)
        {
            Health -= amount;
            if (Health < 0)
                Health = 0;

            if (Health <= 0)
            {
                //died
            }
        }
    }
}


using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Threading.Tasks;
using Zycalipse.GameLogger;
using Zycalipse.Systems;
using Zycalipse.Managers;

namespace Zycalipse.Players
{
    public class PlayerDatas
    {
        private GameLog logger;

        public int PlayerMaxHealth { get; private set; }
        public int PlayerHealth { get; private set; }
        public int PlayerDefense { get; private set; }
        public int PlayerEvasiaon { get; private set; }
        public int PlayerLevel { get; private set; }
        public float PlayerExp { get; private set; }
        public int MaxLevel { get; private set; }
        public int[] ExpNeededEachLevel { get; private set; }

        public PlayerDatas(PlayerDatas playerDatas)
        {
            logger = new GameLog(GetType());
            this.PlayerHealth = playerDatas.PlayerHealth;
            this.PlayerMaxHealth = playerDatas.PlayerMaxHealth;
            this.PlayerDefense = playerDatas.PlayerDefense;
            this.PlayerEvasiaon = playerDatas.PlayerEvasiaon;
            this.PlayerLevel = playerDatas.PlayerLevel;
            this.PlayerExp = playerDatas.PlayerExp;
            this.MaxLevel = playerDatas.MaxLevel;
            this.ExpNeededEachLevel = new int[this.MaxLevel];

            for (int i = 0; i < MaxLevel; i++)
            {
                this.ExpNeededEachLevel[i] = playerDatas.ExpNeededEachLevel[i];
            }
        }

        public PlayerDatas(PlayerBaseData baseData)
        {
            logger = new GameLog(GetType());

            PlayerMaxHealth = GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].MaxHP;

            PlayerDefense = GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].Defense;

            PlayerEvasiaon = GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].EvasionRate;

            PlayerHealth = PlayerMaxHealth;
            PlayerLevel = 1;
            PlayerExp = 0f;
            MaxLevel = baseData.LevelingExpNeeded.Length;
            ExpNeededEachLevel = new int[MaxLevel];

            for (int i = 0; i < MaxLevel; i++)
            {
                ExpNeededEachLevel[i] = baseData.LevelingExpNeeded[i];
            }
        }

        public int GetHealthPrecentage()
        {
            return (int)(PlayerHealth/PlayerMaxHealth);
        }

        public async Task Healed(int precentage, bool isprecentage = true)
        {

            float precenta = precentage / 100f;
            float amount = (float)PlayerMaxHealth * precenta;

            PlayerHealth += isprecentage?(int)amount: precentage;
            if (PlayerHealth > PlayerMaxHealth)
                PlayerHealth = PlayerMaxHealth;

            logger.Information($"healing {amount} hp");
        }

        public async Task TakingDamage(int amount)
        {
            PlayerHealth -= amount;
            if (PlayerHealth < 0)
                PlayerHealth = 0;

            if (PlayerHealth <= 0)
            {
                CombatSystem.Instance.DecreasePlayerLife();
                if (CombatSystem.Instance.PlayerLife <= 0)
                {
                    Managers.MenusManager.Instance.ActiveMenu(MenuList.DeathMenu);
                }
                else if(CombatSystem.Instance.PlayerLife > 0)
                {
                    EffectManager.Instance.ActivateEffect(Effect.PlayerRevive, CombatSystem.Instance.playerManager.transform.position);
                    await Healed(PlayerMaxHealth);
                }
            }
        }

        public async Task PlayerGainExp(float amount)
        {
            PlayerExp += amount;
            if(PlayerLevel == MaxLevel)
            {
                PlayerExp = 0f;
                Managers.MenusManager.Instance.currentMenu.RefreshMenu();
                return;
            }

            if (PlayerExp >= ExpNeededEachLevel[PlayerLevel-1])
                PlayerLevelUp();

            Managers.MenusManager.Instance.currentMenu.RefreshMenu();
        }

        private async void PlayerLevelUp()
        {
            if (PlayerLevel == CombatSystem.Instance.PlayerMaxLevel)
            {
                PlayerExp = ExpNeededEachLevel[PlayerLevel - 1];
                return;
            }

            EffectManager.Instance.ActivateEffect(Effect.LevelUp, CombatSystem.Instance.playerManager.transform.position);
            PlayerExp -= ExpNeededEachLevel[PlayerLevel - 1];
            PlayerLevel++;

            Managers.MenusManager.Instance.ActiveMenu(MenuList.PlayerLevelUpMenu);
            await LevelManager.Instance.currentArena.ShowFirework();
        }

        public void MaxLevelIncreased()
        {
            MaxLevel = 13;
        }
    }
}

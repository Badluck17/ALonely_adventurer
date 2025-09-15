using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.GameLogger;
using Zycalipse.Systems;
using Zycalipse.Menus;

namespace Zycalipse.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameLog logger;
        public LevelManager levelManager { get; set; }
        public GameState ZycalipseGameState { get; private set; }

        public bool Imortal { get; set; }
        public int CheatLevel { get; set; }

        public int Energy { get; private set; }
        private int money;
        public int Money
        {
            get { return money; }
            private set
            {
                money = value;
                PlayerPrefs.SetInt("Money", money);
                MenusManager.Instance.RefreshMenu();
                MainMenu.Instance.RefreshMenu();
            }
        }
        private int gem;
        public int Gem
        {
            get { return gem; }
            private set
            {
                gem = value;
                PlayerPrefs.SetInt("Gem", gem);
                MenusManager.Instance.RefreshMenu();
                MainMenu.Instance.RefreshMenu();
            }
        }

        public bool LevelCleared { get; set; }
        private bool gamePaused = false;
        public bool GamePaused {
            get {
                return gamePaused;
            }
            set {
                gamePaused = value;
                Time.timeScale = gamePaused ? 0 : 1;
                logger.Information($"game paused is {gamePaused}");
            }
        }

        public int CurrentLevelLoaded = 0;

        public CharacterLevelData MaleChar1LevelData;
        private int currentCharLevel;
        public int CurrentCharLevel {
            get {
                return currentCharLevel;
            }
            set {
                currentCharLevel = value;
                PlayerPrefs.SetInt(PlayerCharacterName.MaleChar1.ToString(), currentCharLevel);
            }
        }

        private int killCount, diedCount;
        public int KillCountTotal {
            get { return killCount; }
            set { killCount = value; PlayerPrefs.SetInt("KillCount", killCount);
                if (killCount == 10)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill10Enemy.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill10Enemy.ToString(), Items.MissionItem.Claimable);
                }
                if (killCount == 20)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill20Enemy.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill20Enemy.ToString(), Items.MissionItem.Claimable);
                }
                if (killCount == 30)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill30Enemy.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill30Enemy.ToString(), Items.MissionItem.Claimable);
                }
                if (killCount == 1000)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill1000Enemy.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill1000Enemy.ToString(), Items.MissionItem.Claimable);
                }
            } }
        public int DiedCount
        {
            get { return diedCount; }
            set { diedCount = value; PlayerPrefs.SetInt("DiedCount", diedCount);

                if (diedCount == 50)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Died50Time.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Died50Time.ToString(), Items.MissionItem.Claimable);
                }
            }
        }

        private int miniBossKillCount, bossKillCount, iceZombieKillCount, minerZombieKillCount;

        public int MiniBossKillCount
        {
            get { return miniBossKillCount; }
            set
            {
                miniBossKillCount = value; PlayerPrefs.SetInt("DiedCount", miniBossKillCount);

                if (miniBossKillCount == 3)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill3MiniBoss.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill3MiniBoss.ToString(), Items.MissionItem.Claimable);
                }
            }
        }
        public int BossKillCount
        {
            get { return bossKillCount; }
            set
            {
                bossKillCount = value; PlayerPrefs.SetInt("DiedCount", bossKillCount);

                if (bossKillCount == 4)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill4Boss.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill4Boss.ToString(), Items.MissionItem.Claimable);
                }
            }
        }
        public int IceZombieKillCount
        {
            get { return iceZombieKillCount; }
            set
            {
                iceZombieKillCount = value; PlayerPrefs.SetInt("DiedCount", iceZombieKillCount);

                if (iceZombieKillCount == 3)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill3IceZombie.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill3IceZombie.ToString(), Items.MissionItem.Claimable);
                }
                if (iceZombieKillCount == 6)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill6IceZombie.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill6IceZombie.ToString(), Items.MissionItem.Claimable);
                }
                if (iceZombieKillCount == 10)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill10IceZombie.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill10IceZombie.ToString(), Items.MissionItem.Claimable);
                }
            }
        }
        public int MinerZombieKillCount
        {
            get { return minerZombieKillCount; }
            set
            {
                minerZombieKillCount = value; PlayerPrefs.SetInt("DiedCount", minerZombieKillCount);

                if (minerZombieKillCount == 10)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill10MinerZombie.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill10MinerZombie.ToString(), Items.MissionItem.Claimable);
                }
                if (minerZombieKillCount == 20)
                {
                    string missionStatus = PlayerPrefs.GetString(MissionNames.Kill20MinersZombie.ToString(), "");
                    if (missionStatus != Items.MissionItem.Claimed)
                        PlayerPrefs.SetString(MissionNames.Kill20MinersZombie.ToString(), Items.MissionItem.Claimable);
                }
            }
        }
        public int FTUE { get; set; }
        public bool IsFTUE { get; set; }
        public bool TimeToTriggerTutorialPopUp { get; set; }

        public int CurrentLevel { get; set; }

        private void Awake()
        {
            CurrentLevel = 0;
            TimeToTriggerTutorialPopUp = false;
            Imortal = false;
            CheatLevel = 1;
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }

            money = PlayerPrefs.GetInt("Money");
            gem = PlayerPrefs.GetInt("Gem");
            FTUE = PlayerPrefs.GetInt("FTUE", -1);

            if (FTUE > 0)
            {
                IsFTUE = false;
            }
            else
            {
                IsFTUE = true;
            }

            logger = new GameLog(GetType());
            LevelCleared = false;

            // check last level player
            CheckPlayerCharData();
        }

        private void CheckPlayerCharData()
        {
            int defVal = 0;
            int maleCharLevel = PlayerPrefs.GetInt(PlayerCharacterName.MaleChar1.ToString(), defVal);

            if (maleCharLevel > 0)
            {
                currentCharLevel = maleCharLevel;
            }
            else
            {
                CurrentCharLevel = 1;
            }
        }

        public void ChangeGameState(GameState state)
        {
            logger.Information($"change gamestate from {ZycalipseGameState} to {state}");
            ZycalipseGameState = state;
        }

        public void GainEnergy(int amount)
        {
            Energy += amount;
            logger.Information($"gained {amount} energy");
        }
        public void UsingEnergy(int amount)
        {
            if (Energy - amount < 0)
            {
                logger.Warning("something wrong, player not supposed to use energy");
                return;
            }
            Energy -= amount;
            logger.Information($"using {amount} energy");
        }

        public void GainGem(int amount)
        {
            Gem += amount;
        }

        public void UsingGem(int amount)
        {
            if (gem - amount < 0)
            {
                return;
            }
            Gem -= amount;
        }
        public void GainMoney(int amount)
        {
            Money += amount;
            logger.Information($"gained {amount} Money");
        }
        public void UsingMoney(int amount)
        {
            if (Money - amount < 0)
            {
                logger.Warning("something wrong, player not supposed to spend any Money");
                return;
            }
            Money -= amount;
            logger.Information($"spending {amount} Money");
        }

        public int PrecentageRandomizer()
        {
            int rand = Random.Range(0, 100);
            rand += (int)Time.deltaTime * 1000;
            rand %= 100;

            return rand;
        }
    }
}

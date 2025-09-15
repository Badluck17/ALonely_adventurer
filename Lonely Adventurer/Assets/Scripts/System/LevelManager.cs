using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zycalipse.Managers;
using Zycalipse.GameLogger;

namespace Zycalipse.Systems
{
    public class LevelData
    {
        public LevelData(LevelName name)
        {
            Name = name;
            Locked = true;
            CheckSavedData();
        }

        public LevelName Name { get; private set; }
        public ItemStatus Status { get; set; }
        public bool Locked { get; set; }
        public int ProgressLevel { get; set; }

        private void CheckSavedData()
        {
            // check if already unlocked and check level progress
            string status = PlayerPrefs.GetString(Name.ToString(), "");

            if (Name == LevelName.LevelOne && status != ItemStatus.Unlocked.ToString())
            {
                Status = ItemStatus.Unlocked;
                PlayerPrefs.SetString(Name.ToString(), ItemStatus.Unlocked.ToString());
                Locked = false;
            }
            else if (status == ItemStatus.Unlocked.ToString())
            {
                Status = ItemStatus.Unlocked;
                Locked = false;
                int levelProgress = PlayerPrefs.GetInt(Name.ToString() + "ProgressLevel", -1); ;
                if (levelProgress < 0)
                {
                    ProgressLevel = 0;
                }
                else
                {
                    ProgressLevel = levelProgress;
                }
            }
            else
            {
                Status = ItemStatus.Locked;
                Locked = true;
                ProgressLevel = 0;
                PlayerPrefs.SetInt(Name.ToString() + "ProgressLevel", 0);
                PlayerPrefs.SetString(Name.ToString(), Status.ToString());
            }
        }

        public void SaveData()
        {
            // save progress level

            PlayerPrefs.SetInt(Name.ToString() + "ProgressLevel", ProgressLevel);
            PlayerPrefs.SetString(Name.ToString(), Status.ToString());
        }
    }

    [System.Serializable]
    public class Levels
    {
        public string Name;
        public AssetReference[] levels;
    }

    public class LevelManager : MonoBehaviour
    {
        [Header("Level Assets")]
        [SerializeField]
        private Levels[] levels;
        [HideInInspector]
        public LevelData levelOneData;
        [HideInInspector]
        public LevelData levelTwoData;
        [HideInInspector]
        public int levelID = 0;
        public bool fromFtue { get; set; }

        public int CurrentLevel { get; private set; }
        public Arena currentArena { get; set; }
        public EnemySpawner enemySpawner { get; set; }
        public bool HasReviveChance { get; set; }
        private GameLog logger;
        public List<DropItemBehavior> drops { get; private set; }

        private static LevelManager instance;
        public static LevelManager Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            fromFtue = false;
            GameManager.Instance.levelManager = this;
            drops = new List<DropItemBehavior>();

            levelOneData = new LevelData(LevelName.LevelOne);
            levelTwoData = new LevelData(LevelName.LevelTwo);
        }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }

            logger = new GameLog(GetType());
            CurrentLevel = 0;
        }

        public void SaveLevelProgressData()
        {
            levelOneData.SaveData();
            levelTwoData.SaveData();
        }

        public async Task PrepareLevel()
        {
            //while (!CombatSystem.Instance.playerManager.playerInited)
            //{
            //    await Task.Yield();
            //}

            currentArena.CombatArena.SetActive(false);
            // Minimize null reference because data race
            //while (currentArena == null)
            //{
            //    await Task.Yield();
            //}

            //while (CombatSystem.Instance.playerManager == null)
            //{
            //    await Task.Yield();
            //}

            MenusManager.Instance.ActiveMenu(MenuList.BattleMenu);
            MenusManager.Instance.currentMenu.RefreshMenu();

            // Spawn enemys but disable it
            //while (enemySpawner == null)
            //{
            //    logger.Information("inside prepare level, enemy spawnner null");
            //    await Task.Yield();
            //}

            await enemySpawner.SpawnEnemys();

            while (EnemysManager.Instance.enemyList.Count <= 0)
            {
                await Task.Yield();
            }

            await enemySpawner.SpawnPlayer();

            // Wait until player walk to the middle of the map
            //await PlayerWalkingInProgress();

            while (CombatSystem.Instance.playerManager == null)
            {
                await Task.Yield();
            }

            while (!CombatSystem.Instance.playerManager.PlayerInPlayArea)
            {
                await Task.Yield();
            }

            await EnemysManager.Instance.ShowAllEnemy();
            currentArena.CombatArena.SetActive(true);

            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
            MenusManager.Instance.currentMenu?.RefreshMenu();
        }

        private async Task PlayerWalkingInProgress()
        {
            currentArena.CombatArena.SetActive(false);
            //while (!CombatSystem.Instance.playerManager.PlayerInPlayArea)
            //{
            //    logger.Information($"inside player walking in progress, is player in play area ? : {CombatSystem.Instance.playerManager.PlayerInPlayArea}");
            //    await Task.Yield();
            //}
            currentArena.CombatArena.SetActive(true);
        }

        public async void ReturnToMM()
        {
            CombatSystem.Instance.TurnCounter = 0;
            SaveLevelProgressData();
            currentArena = null;
            enemySpawner = null;
            await CombatSystem.Instance.ClearAllInstance();
            await EnemysManager.Instance.ClearEnemyList();
            ZycalipseSceneManager.Instance.ReturnToMM();
            MenusManager.Instance.RefreshMenu();
        }

        public async void ExitFTUE()
        {
            ReturnToMM();
        }

        public async void NextLevel(bool isPistol, bool iShotgun, bool isSniper)
        {
            CombatSystem.Instance.TurnCounter = 0;
            if (CurrentLevel == 20)
            {
                if (levelID == 0 && levelOneData.ProgressLevel < CurrentLevel)
                {
                    levelOneData.ProgressLevel = CurrentLevel;
                    levelTwoData.Status = ItemStatus.Unlocked;
                }
                else if (levelID == 1 && levelTwoData.ProgressLevel < CurrentLevel)
                {
                    levelTwoData.ProgressLevel = CurrentLevel;
                }
                SoundManager.PlayOneShotSound(DataManager.Instance.PlayerWin, transform.position);
                ReturnToMM();
                return;
            }

            await EnemysManager.Instance.ClearEnemyList();
            await ZycalipseSceneManager.Instance.UnloadPreviousAdditiveScene();

            var loadlevel = await ZycalipseSceneManager.Instance.LoadAdditiveScene(levels[levelID].levels[CurrentLevel]);
            loadlevel.Completed += async result =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    if (levelID == 0 && levelOneData.ProgressLevel < CurrentLevel)
                    {
                        levelOneData.ProgressLevel = CurrentLevel;
                    }else if(levelID == 1 && levelTwoData.ProgressLevel < CurrentLevel)
                    {
                        levelTwoData.ProgressLevel = CurrentLevel;
                    }

                    SaveLevelProgressData();
                    GameManager.Instance.LevelCleared = false;
                    GameManager.Instance.ChangeGameState(GameState.PreparingCombat);
                    CurrentLevel++;

                    await PrepareLevel();
                }
            };
        }

        public async void Play(int id = 0)
        {
            GameManager.Instance.IsFTUE = false;

            int levelLoaded = 0;
            if (GameManager.Instance.CheatLevel > 1)
                levelLoaded = GameManager.Instance.CheatLevel - 1;

            await EnemysManager .Instance.ClearEnemyList();
            levelID = id;
            var loadlevel = await ZycalipseSceneManager.Instance.LoadAdditiveScene(levels[levelID].levels[levelLoaded]);
            loadlevel.Completed += async result =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    HasReviveChance = true;
                    GameManager.Instance.ChangeGameState(GameState.PreparingCombat);
                    CurrentLevel = GameManager.Instance.CheatLevel > 1 ? GameManager.Instance.CheatLevel : 1;

                    GameManager.Instance.LevelCleared = false;
                    await PrepareLevel();
                }
            };
        }

        public async void AddDropToList(DropItemBehavior drop)
        {
            if(drop != null && drops != null)
                drops.Add(drop);
        }

        public async void RemoveDrop(DropItemBehavior drop)
        {
            drops.Remove(drop);
        }

        public async void CollectDrop()
        {
            for (int i = 0; i < drops.Count; i++)
            {
                drops[i].TimeToCollect = true;
                if (drops[i].type != EnemyDropType.Exp)
                    drops[i].ForceToCollect = true;
            }
        }

        public async void DestroyDrops()
        {
            for (int i = 0; i < drops.Count; i++)
            {
                RemoveDrop(drops[i]);
            }
        }
    }
}

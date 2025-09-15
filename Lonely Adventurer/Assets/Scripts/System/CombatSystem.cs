using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Managers;
using Zycalipse.Players;
using Zycalipse.UI;

namespace Zycalipse.Systems
{
    public class CombatSystem : MonoBehaviour
    {
        private static CombatSystem instance;
        public static CombatSystem Instance
        {
            get { return instance; }
        }
        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
            }
        }

        public PlayerManager playerManager { get; set; }
        public bool CombatStarting { get; set; }
        public PlayerDatas CurrentPlayerData { get; private set; }
        public PlayerWeaponManager CurrentPlayerWeaponManager { get; private set; }
        public int PlayerLife { get; private set; }
        public bool alreadyGettingExtraLife { get; set; }
        public bool alreadyGettingSmartLevel { get; set; }
        public bool alreadyGettingRage { get; set; }
        public bool alreadyGettingFullPower { get; set; }
        public bool alreadyGettingGreedyLife { get; set; }
        public bool alreadyGettingKageBunshin { get; set; }
        public bool alreadyGettingMeteor { get; set; }
        public int PlayerMaxLevel { get; private set; }

        public DamageTypeText[] DamageTypeObjects;

        public int TurnCounter { get; set; }

        int killCount = 0;

        private void Start()
        {
            TurnCounter = 0;
            CurrentPlayerData = null;
            CurrentPlayerWeaponManager = null;
            PlayerLife = 1;
            PlayerMaxLevel = 10;
            alreadyGettingExtraLife = alreadyGettingSmartLevel = alreadyGettingRage = alreadyGettingFullPower 
                = alreadyGettingKageBunshin = alreadyGettingGreedyLife = alreadyGettingMeteor = false;
        }

        public GameObject GetAppropriateDamagePopUp(DamageType type)
        {
            GameObject obj = null;

            for (int i = 0; i < DamageTypeObjects.Length; i++)
            {
                if (DamageTypeObjects[i].Type == type)
                {
                    obj = DamageTypeObjects[i].Reference;
                }
            }

            if (obj == null)
                obj = DamageTypeObjects[0].Reference;

            return obj;
        }

        public void IncreasePlayerLife()
        {
            alreadyGettingExtraLife = true;
            PlayerLife++;
        }

        public void DecreasePlayerLife()
        {
            PlayerLife--;
        }

        public void IncreasePlayerMaxLevel()
        {
            alreadyGettingSmartLevel = true;
            PlayerMaxLevel += 13;
            playerManager.playerData.MaxLevelIncreased();
        }

        public void BattleOver()
        {
            killCount = 0;
            TurnCounter = 0;
            CurrentPlayerData = null;
            CurrentPlayerWeaponManager = null;
            playerManager = null;
            PlayerLife = 1;
            PlayerMaxLevel = 10;
            alreadyGettingExtraLife = alreadyGettingSmartLevel = alreadyGettingRage = alreadyGettingFullPower
                = alreadyGettingKageBunshin = alreadyGettingGreedyLife = alreadyGettingMeteor = false;
            GameManager.Instance.LevelCleared = false;
        }

        public void SavePlayerData(PlayerDatas playerDatas, PlayerWeaponManager playerWeaponManager)
        {
            CurrentPlayerData = playerDatas;
            CurrentPlayerWeaponManager = playerWeaponManager;
        }

        public async Task ClearAllInstance()
        {
            var enemys = EnemysManager.Instance.enemyList;

            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] != null)
                    enemys[i].Died();
            }

            LevelManager.Instance.drops.Clear();

            var clearInstance = await AddressableManager.Instance.UnloadAllInstance();
        }

        public async Task<bool> AllEnemyDefeated()
        {
            var enemys = EnemysManager.Instance.enemyList;

            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] != null && !enemys[i].Alive)
                    enemys[i].Died();

                killCount++;
            }

            killCount = 0;

            // if ftue show popup got shotgun also stop time
            if (GameManager.Instance.IsFTUE && EnemysManager.Instance.enemyList.Count <= 0)
            {
                MenusManager.Instance.ShowAquiredWeaponPopup(WeaponName.Shotgun);
            }

            if (EnemysManager.Instance.enemyList.Count <= 0)
            {
                LevelManager.Instance.CollectDrop();

                await WaitForDropToCollect();

                LevelManager.Instance.currentArena.CombatArena.SetActive(false);
                LevelManager.Instance.currentArena.OpenGate();

                return true;
            }

            return false;
        }

        private async Task WaitForDropToCollect()
        {
            float timer = 0;
            while(LevelManager.Instance.drops.Count > 0)
            {
                // play
                timer += Time.deltaTime;
                if (timer > 0.5)
                {
                    timer = 0;
                    SoundManager.PlayOneShotSound(DataManager.Instance.ExpColected, transform.position);
                    await Task.Yield();
                }

                await Task.Yield();
            }

            LevelManager.Instance.drops.Clear();

            var clearInstance = await AddressableManager.Instance.UnloadAllInstance();

            if (EnemysManager.Instance.enemyList.Count <= 0)
            {

                if (GameManager.Instance.IsFTUE)
                {
                    GameManager.Instance.FTUE = 1;
                    PlayerPrefs.SetInt("FTUE", 1);
                    GameManager.Instance.IsFTUE = false;
                    LevelManager.Instance.fromFtue = true;
                }

                GameManager.Instance.LevelCleared = true;
            }
        }

        public async Task PlayerTurn()
        {
            playerManager.ChangePlayerAnimation("Idle");
            bool win = await AllEnemyDefeated();

            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);

            if (win)
                return;

            var enemys = EnemysManager.Instance.enemyList;
            TurnCounter++;

            if (alreadyGettingMeteor && TurnCounter > 0 && TurnCounter%5 == 0)
            {
                int rand = GameManager.Instance.PrecentageRandomizer();
                rand += (int)Time.deltaTime * 1000;
                rand %= enemys.Count;

                await enemys[rand].TakeDamage(true, 999, false, true, false);
            }

            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i] != null && !enemys[i].Alive)
                    enemys[i].foundDestination = false;
            }
        }

        public async Task EnemyTurn()
        {
            playerManager.ChangePlayerAnimation("Idle");
            bool enemyTurnDone = false;

            bool win = await AllEnemyDefeated();

            if (win)
            {
                return;
            }

            GameManager.Instance.ChangeGameState(GameState.EnemyTurn);

            var enemys = EnemysManager.Instance.enemyList;

            for (int i = 0; i < enemys.Count; i++)
            {
                if(!enemys[i].Alive || enemys[i] == null)
                {
                    continue;
                }

                await enemys[i].EnemyActions();
            }

            while (!enemyTurnDone)
            {
                for (int i = 0; i < enemys.Count; i++)
                {
                    if (!enemys[i].Alive || enemys[i] == null)
                    {
                        continue;
                    }

                    if (enemys[i].HasActions)
                    {
                        enemyTurnDone = false;
                        break;
                    }
                    else
                    {
                        enemyTurnDone = true;
                    }
                }
                await Task.Yield();
            }

            await PlayerTurn();
        }

        public async void PlayerAttacking()
        {
            if (playerManager == null)
                return;

            GameManager.Instance.ChangeGameState(GameState.PlayerAttacking);
            var collectible = LevelManager.Instance.drops;
            var enemys = EnemysManager.Instance.enemyList;

            for (int i = 0; i < collectible.Count; i++)
            {
                if (collectible[i].type == EnemyDropType.Exp)
                    continue;

                collectible[i].TimeToCollect = true;
            }

            bool isThisWeaponHasStunSkill = playerManager.WeaponManager.PlayerWeaponsStat[playerManager.WeaponManager.CurrentWeapon].Skill == WeaponSkills.Stun;
            List<Enemys.Enemy> enemyThatGotHit = new List<Enemys.Enemy>();

            int hitEnemyIndex = 0;
            // register enemy that got hit
            for (int i = 0; i < enemys.Count; i++)
            {
                if (enemys[i].BeingAttacked)
                {
                    enemyThatGotHit.Add(enemys[i]);
                }
            }

            if (enemyThatGotHit.Count > 0)
            {
                if (isThisWeaponHasStunSkill && playerManager.WeaponManager.PlayerWeaponsStat[playerManager.WeaponManager.CurrentWeapon].SkillLevel > 0)
                {
                    int stunGacha = GameManager.Instance.PrecentageRandomizer();
                    stunGacha *= 1000;
                    stunGacha += (int)Time.realtimeSinceStartup;
                    stunGacha %= enemyThatGotHit.Count;

                    if (enemyThatGotHit.Count == 1)
                        enemyThatGotHit[0].PlayerAttackWithSkills(WeaponSkills.Stun);
                    else
                        enemyThatGotHit[stunGacha].PlayerAttackWithSkills(WeaponSkills.Stun);
                }

                // apply skill effect first because or it will make an exception when enemy dies midway
                for (int i = 0; i < enemyThatGotHit.Count; i++)
                {
                    var weaponName = playerManager.WeaponManager.CurrentWeapon;
                    if (playerManager.WeaponManager.PlayerWeaponsStat[weaponName].SkillLevel > 0 && !isThisWeaponHasStunSkill)
                    {
                        enemyThatGotHit[i].PlayerAttackWithSkills(playerManager.WeaponManager.PlayerWeaponsStat[weaponName].Skill);
                    }
                }

                // then do the damage
                for (int i = 0; i < enemyThatGotHit.Count; i++)
                {
                    int damage = playerManager.WeaponManager.PlayerWeaponsStat[playerManager.WeaponManager.CurrentWeapon].Damage;
                    SoundManager.PlayOneShotSound(DataManager.Instance.EnemyHitSounds[hitEnemyIndex], transform.position);

                    if (alreadyGettingRage && playerManager.playerData.GetHealthPrecentage() < 30)
                    {
                        damage += (int)(20/100*damage);
                    }

                    hitEnemyIndex++;
                    if (hitEnemyIndex == DataManager.Instance.EnemyHitSounds.Length)
                    {
                        hitEnemyIndex--;
                    }
                    await enemyThatGotHit[i].PlayerAttackInProgress(damage);

                    playerManager.ChangePlayerAnimation("Idle");
                    await Task.Delay(100);
                }
            }

            playerManager.ChangePlayerAnimation("Idle");

            for (int i = 0; i < collectible.Count; i++)
            {
                if (collectible[i].type == EnemyDropType.Exp)
                    continue;

                collectible[i].TimeToCollect = false;
            }

            bool win = await AllEnemyDefeated();

            if (win)
            {
                return;
            }

            await EnemyTurn();
        }

        public async Task EnemyAttacking(int damage)
        {
            if (playerManager == null)
                return;

            await playerManager.TakeDamage(false, damage);
        }
    }
}

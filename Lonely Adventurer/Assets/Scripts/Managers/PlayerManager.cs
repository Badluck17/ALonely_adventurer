using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Zycalipse.Entitys;
using Zycalipse.GameLogger;
using Zycalipse.Systems;
using Zycalipse.UI;
using Zycalipse.Players;
using Spine.Unity;

namespace Zycalipse.Managers
{
    public class PlayerManager : MonoBehaviour, ICharacterBattleActions
    {
        [SerializeField]
        private PlayerBaseData playerBaseData;
        public PlayerDatas playerData { get; private set; }
        [SerializeField]
        private PlayerUI playerUI;
        [SerializeField]
        private Transform figureTransform;
        public PlayerWeaponManager WeaponManager { get; set; }
        private Transform AttackArea;
        private PlayerAttackArea[] playerWeapons;
        public AllWeaponSkillDatas AllWeaponSkillDatas { get; private set; }

        [SerializeField]
        private float spinSpeed = 5f;
        public bool PlayerInPlayArea { get; set; }

        private GameLog logger;

        [SerializeField]
        private SkeletonAnimation anim;
        private List<Spine.Attachment> guns;
        private WeaponName firstEquipedWeapon;
        private int playerWalkSoundIndex = 0;
        private float delayWalkingSound = 0;
        public bool playerInited { get; set; }

        // once load alot of level at the same time
        private bool alreadyLoadNextLevel = false;

        void Awake()
        {
            PlayerInPlayArea = playerInited = false;
        }

        async void Start()
        {
            alreadyLoadNextLevel = false;
            AttackArea = LevelManager.Instance.currentArena.PlayerAttackAreaSpawnPoint;
            AllWeaponSkillDatas = new AllWeaponSkillDatas();
            CombatSystem.Instance.playerManager = this;
            playerWeapons = new PlayerAttackArea[3];

            logger = new GameLog(GetType());
            await Init();

            // TODO : CHANGE WEAPON EQUIPED IN CHAR FIGURE
            //guns = new List<Spine.Attachment>();

            //foreach (var item in anim.Skeleton.Slots)
            //{
            //    Debug.Log($"qwe {item.Skeleton.Data.Name} {item.Data.Name}");
            //    if (item.Data.Name.Contains("gun"))
            //    {
            //        guns.Add(item.Attachment);
            //        item.Attachment = null;
            //    }
            //}
        }

        public async Task Init()
        {
            transform.position = LevelManager.Instance.currentArena.StartingPoint.position;

            if (CombatSystem.Instance.CurrentPlayerData != null && CombatSystem.Instance.CurrentPlayerWeaponManager != null)
            {
                logger.Information($"inside init, current player data not null, current weaponmanager not null, current level : {LevelManager.Instance.CurrentLevel}");
                playerData = new PlayerDatas(CombatSystem.Instance.CurrentPlayerData);
                WeaponManager = new PlayerWeaponManager(CombatSystem.Instance.CurrentPlayerWeaponManager);
            }
            else
            {
                logger.Information($"inside init, current player data null, current weaponmanager null, current level : {LevelManager.Instance.CurrentLevel}");
                playerData = new PlayerDatas(playerBaseData);
                WeaponManager = new PlayerWeaponManager();
            }

            await AssignWeapon();

            await RefreshUI();

            await LoadWeapon();

            // First time use close range weapon
            await PlayerChangedWeapon(firstEquipedWeapon);
            playerInited = true;
        }

        async void Update()
        {
            if (!playerInited ||
                GameManager.Instance.levelManager == null ||
                GameManager.Instance.levelManager.currentArena == null ||
                GameManager.Instance.levelManager.currentArena.FinishPoint == null)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerData.PlayerGainExp(20f);
            }

            if (GameManager.Instance.LevelCleared && GameManager.Instance.ZycalipseGameState != GameState.PowerUpSection)
            {
                if (Vector3.Distance(transform.position, GameManager.Instance.levelManager.currentArena.FinishPoint.position) < 1f)
                {
                    if (alreadyLoadNextLevel)
                        return;

                    alreadyLoadNextLevel = true;
                    CombatSystem.Instance.playerManager = null;
                    await PlayerChangedWeapon(firstEquipedWeapon);
                    CombatSystem.Instance.SavePlayerData(playerData, WeaponManager);
                    logger.Information($"level cleared not walking {GameManager.Instance.LevelCleared}, gamestate : {GameManager.Instance.ZycalipseGameState} already load next level : {alreadyLoadNextLevel}");

                    bool isPistol = WeaponManager.AllWeaponEquiped[0] == WeaponName.Pistol;
                    bool isShotgun = WeaponManager.AllWeaponEquiped[1] == WeaponName.Shotgun;
                    bool isSniper = WeaponManager.AllWeaponEquiped[2] == WeaponName.Sniper;

                    if (LevelManager.Instance.fromFtue)
                        LevelManager.Instance.ExitFTUE();
                    else
                        LevelManager.Instance.NextLevel(isPistol, isShotgun, isSniper);
                    return;
                }

                PlayWalkSound();
                ChangePlayerAnimation("Walk");

                transform.position = Vector3.MoveTowards(transform.position,
                    GameManager.Instance.levelManager.currentArena.FinishPoint.position, 10f * Time.deltaTime);

                // show popup complete if completing lvl 20
                if (LevelManager.Instance.CurrentLevel == 20)
                {
                    bool first = LevelManager.Instance.levelID == 0 ? true : false;
                    MenusManager.Instance.ShowHeliPiece(first);
                }

                return;
            }

            if (GameManager.Instance.ZycalipseGameState == GameState.PlayerTurn)
            {
                if (GameManager.Instance.TimeToTriggerTutorialPopUp && GameManager.Instance.IsFTUE)
                {

                    return;
                }

                AttackArea.Rotate(0.75f * spinSpeed * Time.deltaTime * Vector3.down);
            }
            else if(GameManager.Instance.ZycalipseGameState == GameState.PreparingCombat && !PlayerInPlayArea)
            {
                PlayWalkSound();
                ChangePlayerAnimation("Walk");
                if (Vector3.Distance(transform.position, LevelManager.Instance.currentArena.MiddlePoint.position) < 0.2f)
                {
                    logger.Information($"level prepared not walking, gamestate : {GameManager.Instance.ZycalipseGameState}, ");
                    ChangePlayerAnimation("Idle");
                    PlayerInPlayArea = true;
                    return;
                }
                logger.Information($"level prepared stil walking, gamestate : {GameManager.Instance.ZycalipseGameState}, ");
                transform.position = Vector3.MoveTowards(transform.position,
                    LevelManager.Instance.currentArena.MiddlePoint.position, 10f * Time.deltaTime);
            }
        }

        private async void PlayWalkSound()
        {
            delayWalkingSound += Time.deltaTime;

            if (delayWalkingSound < 0.2f)
            {
                return;
            }

            delayWalkingSound = 0;

            EffectManager.Instance.ActivateEffect(Effect.PlayerWalk, transform.position);
            SoundManager.PlayOneShotSound(DataManager.Instance.PlayerWalkingSounds[playerWalkSoundIndex], transform.position);

            playerWalkSoundIndex++;
            if (playerWalkSoundIndex == DataManager.Instance.PlayerWalkingSounds.Length)
            {
                playerWalkSoundIndex = 0;
            }
        }

        private async Task AssignWeapon()
        {
            for (int i = 0; i < DataManager.Instance.playerAttackArea.Length; i++)
            {
                if (DataManager.Instance.playerAttackArea[i].Name == DataManager.Instance.EquipedWeapons[0])
                {
                    playerWeapons[0] = DataManager.Instance.playerAttackArea[i];
                    firstEquipedWeapon = DataManager.Instance.playerAttackArea[i].Name;
                }

                if (DataManager.Instance.playerAttackArea[i].Name == DataManager.Instance.EquipedWeapons[1])
                {
                    playerWeapons[1] = DataManager.Instance.playerAttackArea[i];
                }

                if (DataManager.Instance.playerAttackArea[i].Name == DataManager.Instance.EquipedWeapons[2])
                {
                    playerWeapons[2] = DataManager.Instance.playerAttackArea[i];
                }
            }
        }

        private async Task LoadWeapon()
        {
            for (int i = 0; i < playerWeapons.Length; i++)
            {
                var weaponAssetID = AddressableManager.Instance.objectLoaded.Count;

                await AddressableManager.Instance.LoadInstance(playerWeapons[i].AttackAreaObject, AttackArea);

                WeaponManager.AddOwnedWeapon(playerWeapons[i].Name, AddressableManager.Instance.objectLoaded[weaponAssetID]);
                AddressableManager.Instance.objectLoaded[weaponAssetID].SetActive(false);

                if (CombatSystem.Instance.CurrentPlayerWeaponManager == null)
                {
                    var stat = new WeaponStat
                    {
                        Name = playerWeapons[i].Name,
                        Type = playerWeapons[i].Type,
                        AttackType = playerWeapons[i].AttackType,
                        Damage = playerWeapons[i].Damage,
                        KnockBackDirection = playerWeapons[i].KnockBackDirection,
                        KnockBackPower = playerWeapons[i].KnockBackPower,
                        WeaponDelay = playerWeapons[i].WeaponDelay,
                        Skill = playerWeapons[i].Skill
                };

                    WeaponManager.PlayerWeaponsStat.Add(playerWeapons[i].Name, stat);
                }
            }
        }

        public async Task PlayerChangedWeapon(WeaponName weaponName)
        {
            if (GameManager.Instance.LevelCleared)
                return;

            LevelManager.Instance.currentArena.EnableAttackAreaHelper(weaponName);
            WeaponManager.ChangeWeapon(weaponName);
        }

        public void ChangePlayerAnimation(string name)
        {
            //anim.AnimationName = name;
        }


        public async Task Heal(int amount, bool precentage = true)
        {
            EffectManager.Instance.ActivateEffect(Effect.Heal, transform.position);
            await playerData.Healed(amount, precentage);
            await RefreshUI();
        }

        public async Task TakeDamage(bool knockback, int damage, bool fromPoison = false, bool fromBurn = false, bool fromBleed = false)
        {
            if (GameManager.Instance.Imortal)
                damage = 0;

            damage -= playerData.PlayerDefense;
            if (damage < 0)
                damage = 0;

            int missChance = GameManager.Instance.PrecentageRandomizer();

            if (missChance > playerData.PlayerEvasiaon)
            {
                DamgeUIPopUp(damage.ToString(), CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Normal));
                await playerData.TakingDamage(damage);
            }
            else
            {
                DamgeUIPopUp("Miss!", CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Miss));
            }

            await RefreshUI();
        }
        private async void DamgeUIPopUp(string damage, GameObject damagePopUp)
        {
            EffectManager.Instance.ActivateEffect(Effect.PlayerHit, transform.position);

            var ui = Instantiate(damagePopUp, MenusManager.Instance.currentMenu.transform) as GameObject;
            DamageUI dmgUI = ui.GetComponent<DamageUI>();
            dmgUI.Damage = damage.ToString();
            dmgUI.position = new Vector3();
            dmgUI.position = MenusManager.Instance.mainCam.WorldToScreenPoint(this.transform.position);
        }

        public async void RotatePlayer(int positif)
        {
            figureTransform.localScale = new Vector3(positif, figureTransform.localScale.y, figureTransform.localScale.z);
        }

        public async void PlayerGotPowerUp()
        {
            for (int i = 0; i < WeaponManager.PlayerWeaponsStat.Count; i++)
            {
                WeaponManager.PlayerWeaponsStat[WeaponManager.AllWeaponEquiped[i]].Damage += 3;
            }

            MenusManager.Instance.RefreshMenu();
        }

        public async void PlayerGotHealthRestore()
        {
            await Heal(80, false);
        }

        public async Task Attacking(ICharacterBattleActions target)
        {
            throw new System.NotImplementedException();
        }

        public async Task Defending()
        {
            await RefreshUI();
        }

        public async Task RefreshUI()
        {
            playerUI.RefreshHealth();
        }
    }
}

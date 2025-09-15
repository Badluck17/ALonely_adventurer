using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Zycalipse.Systems;
using Zycalipse.Managers;
using Zycalipse.Entitys;
using Zycalipse.UI;
using Zycalipse.GameLogger;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Zycalipse.Enemys
{
    public class Enemy : MonoBehaviour, ICharacterBattleActions
    {
        [SerializeField]
        private Color zombieColor;
        private GameLog logger;
        [SerializeField]
        private EnemyBaseData baseData;
        public EnemyDatas data { get; private set; }
        [SerializeField]
        private EnemyUI enemyUI;
        [SerializeField]
        private Rigidbody rigidbody;
        [SerializeField]
        private Transform figureTransform;

        private float movSpeed = 5f;

        public int ID { get; set; }
        public bool BeingAttacked { get; private set; }
        private bool canAttack = false;
        private bool canWalk = false;
        public bool HasActions { get; private set; }
        public bool Alive { get; set; }

        public int currentLayer { get; set; }
        public int currentPosInLayer { get; set; }
        public bool foundDestination { get; set; }
        
        private EnemyMoveDirections directions;
        private Vector3 MovementDestination { get; set; }
        private int enemyBehaviorChoosen = -1;

        [SerializeField]
        private AssetReference xpDrop;
        [SerializeField]
        private AssetReference bigHealthPotion;
        [SerializeField]
        private AssetReference medHealthPotion;

        [SerializeField]
        private Spine.Unity.SkeletonAnimation animation;

        [SerializeField]
        private GameObject poisonStatus, burnedStatus, freezedStatus, stunnedStatus, blindedStatus, bleedingStatus;

        [SerializeField]
        public bool isBonusLevelObject, isPowerUp, isRestoreHealth;

        public bool isBonusObject { get { return this.isBonusLevelObject; } }

        public bool isPosoned { get; private set; }
        public bool isBurned { get; private set; }
        public bool isFreezed { get; private set; }
        public bool isStunned { get; private set; }
        public bool isBlinded { get; private set; }
        public bool isBleeding { get; private set; }

        public int BurnedTurn { get; private set; }
        public int FreezedTurn { get; private set; }
        public int PoisonedTurn { get; private set; }
        public int BlindedTurn { get; private set; }
        public int BleedingTurn { get; private set; }

        public int BurnedLevel { get; private set; }
        public int FreezeLevel { get; private set; }
        public int PoisonLevel { get; private set; }
        public int BlindedLevel { get; private set; }
        public int BleedLevel { get; private set; }

        async void Start()
        {
            BurnedLevel = FreezeLevel = PoisonLevel = BlindedLevel = BleedLevel = 0;
            BurnedTurn = FreezedTurn = PoisonedTurn = BlindedTurn = BleedingTurn = 0;
            BeingAttacked = isPosoned = isBurned = isFreezed = isStunned = isBlinded = isBleeding = false;
            data = new EnemyDatas(baseData);
            ID = currentLayer = currentPosInLayer = -1;
            Alive = true;
            HasActions = false;
            EnemysManager.Instance.RegisterEnemy(this);

            //if (baseData.Name == "EnemyX")
            //{
            //    animation.Skeleton.R = zombieColor.r;
            //    animation.Skeleton.G = zombieColor.g;
            //    animation.Skeleton.B = zombieColor.b;

            //    // L_Hand

            //    foreach (var item in animation.Skeleton.Slots)
            //    {
            //        if (item.Data.Name == "L_Hand")
            //        {
            //            item.Attachment = null;
            //        }
            //    }
            //}

            logger = new GameLog(GetType());
            enemyUI.enemy = this;
            await RefreshUI();
            ChangeEnemyAnimation("Idle");

            figureTransform.gameObject.SetActive(false);
            enemyUI.DisableUI();

            if (this.isBonusLevelObject)
            {
                await data.TakingDamage(data.MaxHealth - 2);
            }
        }

        public void ShowEnemy()
        {
            figureTransform.gameObject.SetActive(true);
            enemyUI.DisableUI(false);

            if (transform.position.x < CombatSystem.Instance.playerManager.transform.position.x)
            {
                RotatePlayer(true);
            }
            else
            {
                RotatePlayer(false);
            }
        }

        async void LateUpdate()
        {
            if (GameManager.Instance.IsFTUE && BeingAttacked)
            {
                // stop attack area movement
                // munculin ui

                GameManager.Instance.TimeToTriggerTutorialPopUp = true;
            }

            if (GameManager.Instance.ZycalipseGameState == GameState.EnemyTurn)
            {
                if (!Alive)
                {
                    return;
                }

                if (canAttack)
                {
                    int rand = GameManager.Instance.PrecentageRandomizer();
                    int damage = rand <= data.CritChance ? data.CritDamage : data.Power;

                    if (isFreezed)
                    {
                        int decreasePrecentage = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.FreezeSkillData.DecreaseAttackPrecentage[FreezeLevel];
                        damage *= decreasePrecentage / 100;
                    }

                    if (isBlinded)
                    {
                        int decreasePrecentage = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BlindSkillData.AccuracyDecreasePrecentage[BlindedLevel];
                        rand = GameManager.Instance.PrecentageRandomizer();

                        if(rand <= decreasePrecentage)
                        {
                            canAttack = false;
                            HasActions = false;
                            return;
                        }
                    }

                    rand += 1000;
                    rand %= DataManager.Instance.ZombieAttack.Length;
                    SoundManager.PlayOneShotSound(DataManager.Instance.ZombieAttack[rand], transform.position);
                    await CombatSystem.Instance.EnemyAttacking(damage);
                    canAttack = false;
                    HasActions = false;
                    await DoEnemyAttackAnimation();
                }

                if (canWalk)
                {
                    canWalk = false;
                    await Walking();
                }
            }
        }

        private async Task DoEnemyAttackAnimation()
        {
            if (transform.position.x < CombatSystem.Instance.playerManager.transform.position.x)
            {
                RotatePlayer(true);
            }
            else
            {
                RotatePlayer(false);
            }

            ChangeEnemyAnimation("Attack");

            await Task.Delay(1000);

            ChangeEnemyAnimation("Idle");
        }

        private void ToggleStatusIndicaor()
        {
            poisonStatus.SetActive(isPosoned);
            burnedStatus.SetActive(isBurned);
            bleedingStatus.SetActive(isBleeding);
            blindedStatus.SetActive(isBlinded);
            freezedStatus.SetActive(isFreezed);
            stunnedStatus.SetActive(isStunned);
        }

        private async Task CheckAbnormalStatus()
        {
            ToggleStatusIndicaor();

            if (isBurned)
            {
                if (BurnedTurn > 0)
                {
                    await TakeDamage(false, CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BurnSkillData.Damage[BurnedLevel], false, true, false);
                    BurnedTurn--;
                }
                else
                    isBurned = false;
            }
            if (isBleeding)
            {
                if (BleedingTurn > 0)
                {
                    EffectManager.Instance.ActivateEffect(Effect.Bleed, transform.position);
                    await TakeDamage(false, CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BleedSkillData.Damage[BleedLevel], false, false, true);
                    BleedingTurn--;
                }
                else
                    isBleeding = false;
            }
            if (isPosoned)
            {
                if (PoisonedTurn > 0)
                {
                    await TakeDamage(false, CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BleedSkillData.Damage[PoisonLevel], true, false, false);
                    PoisonedTurn--;
                }
                else
                    isPosoned = false;
            }

            if (isBlinded)
            {
                if (BlindedTurn > 0)
                    BlindedTurn--;
                else
                    isBlinded = false;
            }
            if (isFreezed)
            {
                if (FreezedTurn > 0)
                    FreezedTurn--;
                else
                    isFreezed = false;
            }
        }

        public async Task EnemyActions()
        {
            if (this.isBonusLevelObject)
                return;

            await CheckAbnormalStatus();
            if (isStunned)
            {
                isStunned = false;
                return;
            }
            HasActions = !GameManager.Instance.IsFTUE;
            int chance = GameManager.Instance.PrecentageRandomizer();
            if (chance < 70)
            {
                for (int i = 0; i < data.Range.Length; i++)
                {
                    switch (data.Range[i])
                    {
                        case EnemyAttackRange.CloseRange:
                            if (currentLayer == 0)
                            {
                                canAttack = true;
                            }
                            break;
                        case EnemyAttackRange.MidRange:
                            if (currentLayer == 2 || currentLayer == 3)
                            {
                                canAttack = true;
                            }
                            break;
                        case EnemyAttackRange.LongRange:
                            if (currentLayer == 5)
                            {
                                canAttack = true;
                            }
                            break;
                    }
                    if (canAttack)
                        break;
                }
            }

            if (canAttack)
                return;

            canWalk = true;

            // if after knocked back move to the nearest point first nothing else

            if(currentLayer == -1 || currentPosInLayer == -1)
            {
                enemyBehaviorChoosen = 99;
                MovementDestination = await LevelManager.Instance.currentArena.FindNextNearestPoint(directions, this);

                return;
            }

            int rand = GameManager.Instance.PrecentageRandomizer();

            int defaultBehaviorChance, behavio1Chance, behavior2Chance;
            defaultBehaviorChance = data.DefaultBehavior.BehaviorChance;
            behavio1Chance = data.Behaviors[0].BehaviorChance;
            behavior2Chance = data.Behaviors[1].BehaviorChance;

            if (rand < behavio1Chance)
                enemyBehaviorChoosen = 1;
            else if(rand < behavio1Chance + behavior2Chance)
                enemyBehaviorChoosen = 2;
            else
                enemyBehaviorChoosen = 0;
        }

        private async Task CanMoveForward()
        {
            if (directions == EnemyMoveDirections.Forward)
            {
                for (int inc = 0; inc < data.Range.Length; inc++)
                {
                    if (data.Range.Length == 1)
                    {
                        switch (data.Range[inc])
                        {
                            case EnemyAttackRange.MidRange:
                                if (currentLayer == 2)
                                {
                                    directions = EnemyMoveDirections.Stay;
                                }
                                break;
                            case EnemyAttackRange.LongRange:
                                if (currentLayer == 5)
                                {
                                    directions = EnemyMoveDirections.Stay;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (data.Range[inc] == EnemyAttackRange.MidRange && currentLayer == 2)
                        {
                            directions = EnemyMoveDirections.Stay;
                        }
                    }
                }
            }
        }

        private async Task Walking()
        {
            if (this.isBonusLevelObject || GameManager.Instance.IsFTUE || LevelManager.Instance.currentArena == null)
                return;

            ChangeEnemyAnimation("Walk");
            switch (enemyBehaviorChoosen)
            {
                case 0:
                    for (int i = 0; i < data.DefaultBehavior.MoveDirections.Length; i++)
                    {
                        directions = data.DefaultBehavior.MoveDirections.Length > 0 ? data.DefaultBehavior.MoveDirections[i] : EnemyMoveDirections.Stay;
                        foundDestination = true;

                        await CanMoveForward();

                        MovementDestination = await LevelManager.Instance.currentArena.FindNextNearestPoint(directions, this);

                        while (Vector3.Distance(transform.position, MovementDestination) > 0.1f)
                        {
                            if (transform.position.x < MovementDestination.x)
                            {
                                RotatePlayer(true);
                            }
                            else
                            {
                                RotatePlayer(false);
                            }

                            transform.position = Vector3.MoveTowards(transform.position, MovementDestination, movSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                    }
                    break;
                case 1:
                    for (int i = 0; i < data.Behaviors[0].MoveDirections.Length; i++)
                    {
                        directions = data.Behaviors[0].MoveDirections.Length > 0 ? data.Behaviors[0].MoveDirections[i] : EnemyMoveDirections.Stay;
                        foundDestination = true;

                        await CanMoveForward();

                        MovementDestination = await LevelManager.Instance.currentArena.FindNextNearestPoint(directions, this);

                        while (Vector3.Distance(transform.position, MovementDestination) > 0.1f)
                        {
                            if (transform.position.x < MovementDestination.x)
                            {
                                RotatePlayer(true);
                            }
                            else
                            {
                                RotatePlayer(false);
                            }

                            transform.position = Vector3.MoveTowards(transform.position, MovementDestination, movSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < data.Behaviors[1].MoveDirections.Length; i++)
                    {
                        directions = data.Behaviors[1].MoveDirections.Length > 0 ? data.Behaviors[1].MoveDirections[i] : EnemyMoveDirections.Stay;
                        foundDestination = true;

                        await CanMoveForward();

                        MovementDestination = await LevelManager.Instance.currentArena.FindNextNearestPoint(directions, this);

                        while (Vector3.Distance(transform.position, MovementDestination) > 0.1f)
                        {
                            if (transform.position.x < MovementDestination.x)
                            {
                                RotatePlayer(true);
                            }
                            else
                            {
                                RotatePlayer(false);
                            }

                            transform.position = Vector3.MoveTowards(transform.position, MovementDestination, movSpeed * Time.deltaTime);
                            await Task.Yield();
                        }
                    }
                    break;
                default:
                    while (Vector3.Distance(transform.position, MovementDestination) > 0.1f)
                    {
                        if (transform.position.x < MovementDestination.x)
                        {
                            RotatePlayer(true);
                        }
                        else
                        {
                            RotatePlayer(false);
                        }

                        transform.position = Vector3.MoveTowards(transform.position, MovementDestination, movSpeed * Time.deltaTime);
                        await Task.Yield();
                    }
                    break;
            }
            ChangeEnemyAnimation("Idle");

            HasActions = false;
        }

        public async Task PlayerAttackInProgress(int damage)
        {
            if (this.BeingAttacked)
            {
                if (this.isBonusLevelObject)
                {
                    if (this.isPowerUp && this.BeingAttacked)
                    {
                        CombatSystem.Instance.playerManager.PlayerGotPowerUp();
                    }
                    else if (this.isRestoreHealth && this.BeingAttacked)
                    {
                        CombatSystem.Instance.playerManager.PlayerGotHealthRestore();
                    }

                    await this.TakeDamage(true, damage);
                    return;
                }
                int rand = GameManager.Instance.PrecentageRandomizer();

                if (rand <= data.EvasionRate && !GameManager.Instance.IsFTUE)
                {
                    DamgeUIPopUp("Miss!", CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Miss));
                    logger.Information($"enemy id ({ID}) evade player attack");
                    return;
                }

                await TakeDamage(true, damage);
            }
        }

        public async void PlayerAttackWithSkills(WeaponSkills weaponSkills)
        {
            if (!this.BeingAttacked)
                return;

            var currentWeapon = CombatSystem.Instance.playerManager.WeaponManager.CurrentWeapon;
            int skillLevel = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[currentWeapon].SkillLevel;

            if (skillLevel < 1)
                return;

            skillLevel--;

            switch (weaponSkills)
            {
                case WeaponSkills.Burn:
                    if (isBurned)
                        return;
                    isBurned = true;
                    BurnedTurn = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BurnSkillData.TurnLast[skillLevel];
                    BurnedLevel = skillLevel;
                    break;
                case WeaponSkills.Freeze:
                    if (isFreezed)
                        return;
                    isFreezed = true;
                    FreezedTurn = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.FreezeSkillData.TurnLast[skillLevel];
                    FreezeLevel = skillLevel;
                    EffectManager.Instance.ActivateEffect(Effect.Freeze, transform.position);
                    break;
                case WeaponSkills.Lightning:
                    var enemys = EnemysManager.Instance.enemyList;
                    for (int i = 0; i < enemys.Count; i++)
                    {
                        if (enemys[i].ID == ID || enemys[i] == null || this == null)
                            continue;

                        bool isEnemyInSide = enemys[i].currentLayer == currentLayer && (enemys[i].currentPosInLayer - currentPosInLayer < 2 && enemys[i].currentPosInLayer - currentPosInLayer > -2);
                        bool isEnemyInTopOrDown = enemys[i].currentPosInLayer == currentPosInLayer && (enemys[i].currentLayer - currentLayer < 2 && enemys[i].currentLayer - currentLayer > -2);

                        if (isEnemyInSide || isEnemyInTopOrDown)
                        {
                            int damage = CombatSystem.Instance.playerManager.WeaponManager.PlayerWeaponsStat[CombatSystem.Instance.playerManager.WeaponManager.CurrentWeapon].Damage;
                            damage *= CombatSystem.Instance.playerManager.AllWeaponSkillDatas.LightningSkillData.DamagePrecentage[skillLevel - 1];
                            await enemys[i].TakeDamage(false, damage);
                        }
                    }
                    break;
                case WeaponSkills.Poison:
                    if (isPosoned)
                        return;
                    isPosoned = true;
                    PoisonedTurn = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.PoisonSkillData.TurnLast[skillLevel];
                    PoisonLevel = skillLevel;
                    break;
                case WeaponSkills.Blind:
                    if (isBlinded)
                        return;
                    isBlinded = true;
                    BlindedTurn = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BlindSkillData.TurnLast[skillLevel];
                    BlindedLevel = skillLevel;
                    EffectManager.Instance.ActivateEffect(Effect.Blind, transform.position);
                    break;
                case WeaponSkills.Bleed:
                    if (isBleeding)
                        return;
                    isBleeding = true;
                    BleedingTurn = CombatSystem.Instance.playerManager.AllWeaponSkillDatas.BleedSkillData.TurnLast[skillLevel];
                    BleedLevel = skillLevel;
                    break;
                case WeaponSkills.Stun:
                    await CombatSystem.Instance.playerManager.Heal(CombatSystem.Instance.playerManager.AllWeaponSkillDatas.StunSkillData.HealPrecentage[skillLevel]);

                    int precentage = GameManager.Instance.PrecentageRandomizer();
                    precentage += (int)Time.realtimeSinceStartup;
                    precentage %= 100;

                    if (precentage > CombatSystem.Instance.playerManager.AllWeaponSkillDatas.StunSkillData.StunChance[skillLevel])
                        return;

                    DamgeUIPopUp("Stun!", CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Miss));
                    isStunned = true;
                    EffectManager.Instance.ActivateEffect(Effect.Stun, transform.position);
                    break;
                case WeaponSkills.MultiShot:
                    break;
            }
            ToggleStatusIndicaor();
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.BeingAttacked = true;
            }
        }
        private void OnTriggerStay(Collider collision)
        {
            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.BeingAttacked = true;
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.CompareTag("PlayerAttackArea"))
            {
                this.BeingAttacked = false;
            }
        }

        public async void RotatePlayer(bool positif)
        {
            //int multiplier = -1;
            //if ((positif && figureTransform.localScale.x < 0) || (!positif && figureTransform.localScale.x > 0))
            //{
            //    multiplier *= -1;
            //}

            //figureTransform.localScale = new Vector3(figureTransform.localScale.x * multiplier, figureTransform.localScale.y, figureTransform.localScale.z);
        }

        private async void DamgeUIPopUp(string damage, GameObject damagePopUp)
        {
            var ui = Instantiate(damagePopUp, MenusManager.Instance.currentMenu.transform) as GameObject;
            DamageUI dmgUI = ui.GetComponent<DamageUI>();
            dmgUI.Damage = damage.ToString();
            dmgUI.position = new Vector3();
            dmgUI.position = MenusManager.Instance.mainCam.WorldToScreenPoint(this.transform.position);
        }

        public async Task TakeDamage(bool knockback ,int damage, bool fromPoison = false, bool fromBurn = false, bool fromBleed = false)
        {
            if (this.isBonusLevelObject)
            {
                CombatSystem.Instance.playerManager.ChangePlayerAnimation("shooting");
                await data.TakingDamage(99);
                Alive = false;
                bool win = await CombatSystem.Instance.AllEnemyDefeated();
                EnemysManager.Instance.ClearEnemyList(false);

                return;
            }

            if (transform.position.x < CombatSystem.Instance.playerManager.transform.position.x)
            {
                CombatSystem.Instance.playerManager.RotatePlayer(1);
            }
            else
            {
                CombatSystem.Instance.playerManager.RotatePlayer(-1);
            }

            switch (CombatSystem.Instance.playerManager.WeaponManager.CurrentWeapon)
            {
                case WeaponName.Pistol:
                    SoundManager.PlayOneShotSound(DataManager.Instance.PistolSound, transform.position);
                    break;
                case WeaponName.Shotgun:
                    SoundManager.PlayOneShotSound(DataManager.Instance.ShotgunSound, transform.position);
                    break;
                case WeaponName.Sniper:
                    SoundManager.PlayOneShotSound(DataManager.Instance.SniperSound, transform.position);
                    break;
                case WeaponName.GauntletPistol:
                    SoundManager.PlayOneShotSound(DataManager.Instance.GauntletPistolSound, transform.position);
                    break;
                case WeaponName.EnergyGun:
                    SoundManager.PlayOneShotSound(DataManager.Instance.EnergyGunSound, transform.position);
                    break;
                case WeaponName.EnergySniper:
                    SoundManager.PlayOneShotSound(DataManager.Instance.EnergySniperSound, transform.position);
                    break;
                case WeaponName.BarrelBomb:
                    SoundManager.PlayOneShotSound(DataManager.Instance.BarrelBombSound, transform.position);
                    break;
                case WeaponName.C4:
                    SoundManager.PlayOneShotSound(DataManager.Instance.C4Sound, transform.position);
                    break;
                case WeaponName.Launcher:
                    SoundManager.PlayOneShotSound(DataManager.Instance.LauncherSound, transform.position);
                    break;
            }

            if (damage < 0)
                damage = 0;

            if (fromPoison)
            {
                EffectManager.Instance.ActivateEffect(Effect.Poison, transform.position);
                DamgeUIPopUp(damage.ToString(), CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Poison));
            }
            else if (fromBurn)
            {
                EffectManager.Instance.ActivateEffect(Effect.Burn, transform.position);
                DamgeUIPopUp(damage.ToString(), CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Burn));
            }
            else
            {
                damage -= data.Defense;
                if (damage < 0)
                    damage = 0;

                EffectManager.Instance.ActivateEffect(Effect.EnemyHit, transform.position);
                DamgeUIPopUp(damage.ToString(), CombatSystem.Instance.GetAppropriateDamagePopUp(DamageType.Normal));
            }

            CombatSystem.Instance.playerManager.ChangePlayerAnimation("shooting");
            ChangeEnemyAnimation("Hurt");
            await data.TakingDamage(damage);

            await RefreshUI();

            Vector3 difference = (transform.position - CombatSystem.Instance.playerManager.transform.position).normalized;
            Vector3 force = difference * 15f;

            if (data.Health <= 0)
            {
                // check enemy type then increase kill count
                var zombie = baseData.Type;
                var name = baseData.Name;

                if (name.Contains("EnemyC"))
                    GameManager.Instance.IceZombieKillCount++;
                if (name.Contains("EnemyB"))
                    GameManager.Instance.MinerZombieKillCount++;

                switch (zombie)
                {
                    case EnemyType.MiniBoss:
                        GameManager.Instance.MiniBossKillCount++;
                        break;
                    case EnemyType.Boss:
                        GameManager.Instance.BossKillCount++;
                        break;
                }

                GameManager.Instance.KillCountTotal++;
                int goldDropAmount = CombatSystem.Instance.alreadyGettingGreedyLife ? data.GoldDrop + 1 : data.GoldDrop;
                GameManager.Instance.GainMoney(goldDropAmount);

                var expAmountToDrop = data.ExpDrop * 2;
                for (int i = 0; i < expAmountToDrop; i++)
                {
                    await AddressableManager.Instance.LoadInstance(xpDrop, enemyUI.transform.position);
                }

                int dropHealthPotion = GameManager.Instance.PrecentageRandomizer();
                if(dropHealthPotion <= data.HealthDropPrecentage && data.HealthDropPrecentage > 0)
                {
                    switch (data.HealthDropType)
                    {
                        case EnemyDropType.SmallHealthPotion:
                            break;
                        case EnemyDropType.MediumHealthPotion:
                            await AddressableManager.Instance.LoadInstance(medHealthPotion, transform.position);
                            break;
                        case EnemyDropType.BigHealthPotion:
                            await AddressableManager.Instance.LoadInstance(bigHealthPotion, transform.position);
                            break;
                    }
                }

                Alive = false;

                rigidbody.AddForce(force * 10f, ForceMode.Impulse);
                await Task.Delay(200);
                rigidbody.linearVelocity = Vector3.zero;

                if (!GameManager.Instance.IsFTUE)
                {
                    bool win = await CombatSystem.Instance.AllEnemyDefeated();
                    if (win && GameManager.Instance.ZycalipseGameState != GameState.PowerUpSection)
                    {
                        GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
                    }
                }
            }
            else
            {
                int knockbackPrecentage = GameManager.Instance.PrecentageRandomizer();

                if (knockbackPrecentage < 40 && knockback)
                {
                    rigidbody.AddForce(force, ForceMode.Impulse);
                    await Task.Delay(200);
                    rigidbody.linearVelocity = Vector3.zero;

                    currentLayer = currentPosInLayer = -1;
                }
            }

            ChangeEnemyAnimation("Idle");
        }

        public void ChangeEnemyAnimation(string name)
        {
            //animation.AnimationName = name;
        }

        public async void Died()
        {
            MenusManager.Instance.currentMenu.RefreshMenu();

            if (!Alive)
                EnemysManager.Instance.UnregisterEnemy(this);
        }

        public async Task DealDamage(int damage)
        {
        }

        public async Task Attacking(ICharacterBattleActions target)
        {
        }

        public async Task Defending()
        {
            throw new System.NotImplementedException();
        }

        public async Task RefreshUI()
        {
            await enemyUI.RefreshHealth();
        }
    }
}
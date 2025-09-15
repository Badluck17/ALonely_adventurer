using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zycalipse.Managers;
using Zycalipse.GameLogger;

namespace Zycalipse.Systems
{
    [System.Serializable]
    public class AreaLayer
    {
        public string Name;
        public Transform[] transforms;
    }
    public class Arena : MonoBehaviour
    {
        public Transform[] fireworks;

        public Transform PlayerAttackAreaSpawnPoint;
        public Transform FinishPoint;
        public Transform MiddlePoint;
        public Transform StartingPoint;
        public GameObject DropCollectPoint;
        public GameObject CombatArena;
        public GameObject GateClosed;
        public GameObject GateOpened;
        public AreaLayer[] pointTransform;

        [SerializeField]
        private Transform bgSpawnPoint;
        [SerializeField]
        private GameObject[] level1BGs;
        [SerializeField]
        private GameObject[] level2BGs;

        [SerializeField]
        private GameObject AttackAreaFull;
        [SerializeField]
        private GameObject AttackAreaFullShort;
        [SerializeField]
        private GameObject AttackAreaFullC4;
        [SerializeField]
        private GameObject AttackAreaLong;
        [SerializeField]
        private GameObject AttackAreaMid;


        private GameLog logger;

        void Start()
        {
            LevelManager.Instance.currentArena = this;
            CombatArena.SetActive(false);
            GateClosed.SetActive(true);
            GateOpened.SetActive(false);
            logger = new GameLog(GetType());

            // no different level for now
            //if (GameManager.Instance.CurrentLevelLoaded == 1)
            //{
            //    int rand = Managers.GameManager.Instance.PrecentageRandomizer();
            //    rand %= level1BGs.Length;

            //    Instantiate(level1BGs[rand], bgSpawnPoint);
            //}
            //else
            //{
            //    int rand = Managers.GameManager.Instance.PrecentageRandomizer();
            //    rand %= level2BGs.Length;

            //    Instantiate(level2BGs[rand], bgSpawnPoint);
            //}
        }

        public async Task ShowFirework()
        {
            for (int i = 0; i < fireworks.Length; i++)
            {
                float timer = 0.2f;
                while(timer > 0)
                {
                    timer -= Time.deltaTime;
                    await Task.Yield();
                }
                EffectManager.Instance.ActivateEffect(Effect.Firework, fireworks[i].position);
            }
        }

        public void OpenGate()
        {
            GateClosed.SetActive(false);
            GateOpened.SetActive(true);
        }

        public async Task<Vector3> FindNextNearestPoint(EnemyMoveDirections direction, Enemys.Enemy enemy)
        {
            int currentLayer = -1;
            int currentPosInLayer = -1;
            Vector3 currentPosition = Vector3.zero;
            var destination = Vector3.zero;

            if (enemy != null)
            {
                currentLayer = enemy.currentLayer;
                currentPosInLayer = enemy.currentPosInLayer;
                currentPosition = enemy.transform.position;
            }

            // first time spawn loop all to find the nearest
            if (currentLayer < 0 || currentPosInLayer < 0)
            {
                float distance = 100f;

                for (int i = 0; i < pointTransform.Length; i++)
                {
                    for (int inc = 0; inc < pointTransform[i].transforms.Length; inc++)
                    {
                        var checkDist = Vector3.Distance(pointTransform[i].transforms[inc].position, currentPosition);

                        if (checkDist < distance)
                        {
                            destination = pointTransform[i].transforms[inc].position;
                            distance = checkDist;
                            currentLayer = i;
                            currentPosInLayer = inc;
                        }
                    }
                }

                enemy.currentLayer = currentLayer;
                enemy.currentPosInLayer = currentPosInLayer;
                return destination;
            }

            // not first time, already has current layer and position
            switch (direction)
            {
                case EnemyMoveDirections.Forward:
                    if (currentLayer == 0)
                    {
                        enemy.currentLayer = currentLayer;
                        enemy.currentPosInLayer = currentPosInLayer;
                        return currentPosition;
                    }
                    currentLayer--;
                    destination = pointTransform[currentLayer].transforms[currentPosInLayer].position;
                    break;
                case EnemyMoveDirections.Backward:
                    if (currentLayer == 5)
                    {
                        enemy.currentLayer = currentLayer;
                        enemy.currentPosInLayer = currentPosInLayer;
                        return currentPosition;
                    }
                    currentLayer++;
                    destination = pointTransform[currentLayer].transforms[currentPosInLayer].position;
                    break;
                case EnemyMoveDirections.Right:
                    currentPosInLayer++;
                    if (currentPosInLayer == pointTransform[currentLayer].transforms.Length)
                        currentPosInLayer = 0;

                    destination = pointTransform[currentLayer].transforms[currentPosInLayer].position;
                    break;
                case EnemyMoveDirections.Left:
                    currentPosInLayer--;
                    if (currentPosInLayer < 0)
                        currentPosInLayer = pointTransform[currentLayer].transforms.Length - 1;

                    destination = pointTransform[currentLayer].transforms[currentPosInLayer].position;
                    break;
                case EnemyMoveDirections.Stay:
                    destination = pointTransform[currentLayer].transforms[currentPosInLayer].position;
                    break;
            }

            enemy.currentLayer = currentLayer;
            enemy.currentPosInLayer = currentPosInLayer;
            return destination;
        }

        public void EnableAttackAreaHelper(WeaponName name)
        {
            bool shortArea, fullArea, c4Area, longArea, midArea;
            shortArea = fullArea = c4Area = longArea = midArea = false;

            switch (name)
            {
                case WeaponName.Pistol:
                case WeaponName.GauntletPistol:
                    shortArea = true;
                    break;
                case WeaponName.Shotgun:
                case WeaponName.EnergyGun:
                    midArea = true;
                    break;
                case WeaponName.Sniper:
                case WeaponName.EnergySniper:
                    longArea = true;
                    break;
                case WeaponName.C4:
                    c4Area = true;
                    break;
                case WeaponName.Launcher:
                    fullArea = true;
                    break;
                case WeaponName.BarrelBomb:
                    break;
            }

            AttackAreaFull.SetActive(fullArea);
            AttackAreaFullShort.SetActive(shortArea);
            AttackAreaFullC4.SetActive(c4Area);
            AttackAreaMid.SetActive(midArea);
            AttackAreaLong.SetActive(longArea);
        }
    }
}

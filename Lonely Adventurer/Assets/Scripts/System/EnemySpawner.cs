using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Zycalipse.Systems
{
    [System.Serializable]
    public class EnemyReference
    {
        [Header("Enemy Assets")]
        [SerializeField]
        public AssetReference enemys;
        public int AmountToSpawn;
        public EnemyLayer[] PreferedSpawnLayer;
    }
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private AssetReference playerReference;

        [SerializeField]
        private EnemyReference[] enemys;
        public Vector3[] spawnPoints { get; set; }

        public AudioClip[] spawnEffects;
        public bool BonusLevel;
        public bool TutorialLevel;
        [SerializeField]
        private Transform leftPoint, rightPoint;
        [SerializeField]
        private GameObject[] bonusItems;

        private void Start()
        {
            LevelManager.Instance.enemySpawner = this;
        }

        private void OnDestroy()
        {
            enemys = null;
        }

        public async Task SpawnPlayer()
        {
            playerReference.InstantiateAsync(LevelManager.Instance.currentArena.MiddlePoint.position, Quaternion.identity, gameObject.transform);
        }

        public async Task SpawnEnemys()
        {
            if (BonusLevel)
            {
                Instantiate(bonusItems[0], leftPoint);
                Instantiate(bonusItems[1], rightPoint);

                return;
            }

            spawnPoints = new Vector3[LevelManager.Instance.currentArena.pointTransform[0].transforms.Length * 3];
            for (int i = 3; i < 6; i++)
            {
                int trueNmber = i - 3;
                for (int inc = 0; inc < LevelManager.Instance.currentArena.pointTransform[0].transforms.Length; inc++)
                {
                    int xtra = trueNmber * LevelManager.Instance.currentArena.pointTransform[0].transforms.Length;
                    spawnPoints[inc + xtra] = new Vector3();
                    spawnPoints[inc + xtra] = LevelManager.Instance.currentArena.pointTransform[i].transforms[inc].position;
                }
            }

            if (TutorialLevel)
            {
                var handler = enemys[0].enemys.InstantiateAsync(LevelManager.Instance.currentArena.pointTransform[1].transforms[4].position, Quaternion.identity, transform);

                return;
            }

            for (int i = 0; i < enemys.Length; i++)
            {
                for (int inc = 0; inc < enemys[i].AmountToSpawn; inc++)
                {
                    int rand = Random.Range(0, spawnPoints.Length);
                    if(enemys[i].PreferedSpawnLayer.Length > 0)
                    {
                        rand %= LevelManager.Instance.currentArena.pointTransform[0].transforms.Length;

                        switch (enemys[i].PreferedSpawnLayer[0])
                        {
                            case EnemyLayer.Four:
                                break;
                            case EnemyLayer.Five:
                                rand += LevelManager.Instance.currentArena.pointTransform[0].transforms.Length;
                                break;
                            case EnemyLayer.Six:
                                rand += 2 * LevelManager.Instance.currentArena.pointTransform[0].transforms.Length;
                                break;
                        }
                    }

                    var handler = enemys[i].enemys.InstantiateAsync(spawnPoints[rand], Quaternion.identity, transform);

                    await Task.Delay(200);
                }
            }
        }
    }
}

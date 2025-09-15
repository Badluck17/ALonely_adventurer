using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zycalipse.Enemys;

namespace Zycalipse.Systems
{
    public class EnemysManager : MonoBehaviour
    {
        private static EnemysManager instance;
        public static EnemysManager Instance
        {
            get { return instance; }
        }

        public List<Enemy> enemyList { get; set; }

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this.gameObject);
            else
            {
                instance = this;
            }
            enemyList = new List<Enemy>();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public void RegisterEnemy(Enemy enemy)
        {
            enemyList.Add(enemy);
            enemy.ID = enemyList.Count;
        }

        public async void UnregisterEnemy(Enemy enemy, bool deactivated = true)
        {
            Enemy deletedEnemy = enemy;
            enemyList.Remove(enemy);
            deletedEnemy.gameObject.SetActive(!deactivated);
        }

        public async Task ClearEnemyList(bool deactivated = true)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                UnregisterEnemy(enemyList[i], deactivated);
            }

            enemyList = null;
            enemyList = new List<Enemy>();
        }

        public async Task ShowAllEnemy()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].ShowEnemy();

                int soundRand = Managers.GameManager.Instance.PrecentageRandomizer();
                soundRand %= LevelManager.Instance.enemySpawner.spawnEffects.Length;

                Managers.SoundManager.PlayOneShotSound(LevelManager.Instance.enemySpawner.spawnEffects[soundRand], enemyList[i].gameObject.transform.position);

                await Task.Delay(150);
            }
            Managers.GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Zycalipse.GameLogger;

namespace Zycalipse.Managers
{
    public class ZycalipseSceneManager : MonoBehaviour
    {
        private static ZycalipseSceneManager instance;
        public static ZycalipseSceneManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameLog logger;
        private AsyncOperationHandle<SceneInstance> loadHandle;
        private AsyncOperationHandle<SceneInstance> additiveLoadHandle;
        private AsyncOperationHandle<SceneInstance> additiveUnloadHandle;

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
        }

        private void SceneAsyncResult(AsyncOperationHandle<SceneInstance> result)
        {
            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                logger.Information($"Load scene {result.Result.Scene.name} is done!");
            }
            else
            {
                logger.Error($"Load scene {result.Result.Scene.name} is failed!");
            }
        }

        public async Task LoadNewScene(AssetReference scenePath)
        {
            loadHandle = Addressables.LoadSceneAsync(scenePath, LoadSceneMode.Single);
            //loadHandle.Completed += result => { SceneAsyncResult(result); };
        }

        public async Task<AsyncOperationHandle<SceneInstance>> LoadAdditiveScene(AssetReference scenePath)
        {
            additiveLoadHandle = Addressables.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            //additiveLoadHandle.Completed += result => { SceneAsyncResult(result); };
            return additiveLoadHandle;
        }

        private async Task<AsyncOperationHandle<SceneInstance>> UnloadAdditiveScene()
        {
            additiveUnloadHandle = Addressables.UnloadSceneAsync(additiveLoadHandle);
            //additiveUnloadHandle.Completed += result => { SceneAsyncResult(result); };
            return additiveUnloadHandle;
        }

        public async Task UnloadPreviousAdditiveScene()
        {
            var result = await UnloadAdditiveScene();
            result.Completed += async(handler) =>
            {
                if (result.Status == AsyncOperationStatus.Failed)
                    logger.Error($"unload {additiveUnloadHandle.Result.Scene.name} is failed");
            };
        }

        public async void ReturnToMM()
        {
            Systems.LevelManager.Instance.drops.Clear();
            Systems.LevelManager.Instance.HasReviveChance = true;
            var clearInstance = await Systems.AddressableManager.Instance.UnloadAllInstance();
            await UnloadPreviousAdditiveScene();

            Systems.CombatSystem.Instance.BattleOver();
            MenusManager.Instance.currentMenu.gameObject.SetActive(false);
            MenusManager.Instance.ActiveMenu(MenuList.MainMenu);

            MenusManager.Instance.ToggleMainMenuLookEffect(true);
        }
    }
}

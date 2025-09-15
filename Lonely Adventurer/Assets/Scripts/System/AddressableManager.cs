using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Zycalipse.GameLogger;

namespace Zycalipse.Systems
{
    public class AddressableManager : MonoBehaviour
    {
        private static AddressableManager instance;
        public static AddressableManager Instance
        {
            get
            {
                return instance;
            }
        }

        private GameLog logger;
        public Dictionary<int, GameObject> objectLoaded { get; set; }
        public Dictionary<int, GameObject> menuLoaded { get; set; }

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
            objectLoaded = new Dictionary<int, GameObject>();
            menuLoaded = new Dictionary<int, GameObject>();
        }

        private async void OnDestroy()
        {
            await UnloadAllInstance();
        }

        public async Task<GameObject> LoadMenuInstance(AssetReference reference, Transform parent)
        {
            var handler = reference.InstantiateAsync(parent);
            handler.Completed += (resultHandle) => {
                if (resultHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    resultHandle.Result.SetActive(false);
                    menuLoaded.Add(menuLoaded.Count, resultHandle.Result);
                }
                else
                {
                    logger.Error($"load menu {reference.GetType()} failed");
                }
            };

            while (!handler.IsDone)
            {
                await Task.Yield();
            }

            return handler.Result.gameObject;
        }

        public async Task LoadInstance(AssetReference reference, Transform parent)
        {
            var handler = reference.InstantiateAsync(parent);
            handler.Completed += (resultHandle) => {
                if (resultHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    objectLoaded.Add(objectLoaded.Count, resultHandle.Result);
                }
                else
                {
                    logger.Error($"load {reference.GetType()} failed");
                }
            };

            await handler.Task;
        }

        public async Task LoadInstance(AssetReference reference, Vector3 position)
        {
            var handler = reference.InstantiateAsync(position, Quaternion.identity);
            handler.Completed += (resultHandle) => {
                if (resultHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    objectLoaded.Add(objectLoaded.Count, resultHandle.Result);
                }
                else
                {
                    logger.Error($"load {reference.GetType()} failed");
                }
            };

            await handler.Task;
        }

        private async Task<bool> UnloadInstance(GameObject objectReference)
        {
            return Addressables.ReleaseInstance(objectReference);
        }

        public async Task<bool> UnloadAllInstance()
        {
            if (objectLoaded.Count < 1)
                return true;

            for (int i = 0; i < objectLoaded.Count; i++)
            {
                if (objectLoaded[i] == null)
                    continue;

                var completed = await UnloadInstance(objectLoaded[i]);
                while (!completed)
                {
                    await Task.Yield();
                }
            }

            objectLoaded.Clear();

            return true;
        }
    }
}

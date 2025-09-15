using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zycalipse.GameLogger;
using Zycalipse.Managers;

namespace Zycalipse.Systems
{
    public class Boot : MonoBehaviour
    {
        [SerializeField]
        private Sprite teamLogo;
        [SerializeField]
        private AssetReference mainMenuScene, prologueScene;

        private static string FirstTime = "FirstTime";

        private float timer = 0.4f;
        bool loaded = false;

        private GameLog logger;

        private void Awake()
        {
            logger = new GameLog(GetType());
        }

        async void Start()
        {

            //var firstTime = PlayerPrefs.GetInt(FirstTime, 0);

            //if (firstTime > 0)
            //{
            //    logger.Information("Otw load MM");
            //    await LoadMainMenu();
            //}
            //else
        }

        private async void Update()
        {
            timer -= Time.deltaTime;

            if (timer < 0 && !loaded)
            {
                loaded = true;

                logger.Information("Otw load prologue");
                await LoadPrologue();
            }
        }

        private async Task LoadMainMenu()
        {
            while (ZycalipseSceneManager.Instance == null || mainMenuScene == null)
            {
                await Task.Yield();
            }

            await ZycalipseSceneManager.Instance.LoadNewScene(mainMenuScene);
        }
        private async Task LoadPrologue()
        {
            while (ZycalipseSceneManager.Instance == null || prologueScene == null)
            {
                await Task.Yield();
            }
            await ZycalipseSceneManager.Instance.LoadNewScene(prologueScene);
        }

        private async Task LoadAddressable()
        {
            //    var handlr = Addressables.LoadAssetsAsync<Spine.SkeletonData>(playerAssets, (asyncHandler) => 
            //    {
            //        logger.Information($"load {asyncHandler.Name} success");
            //    }
            //    );

            //    handlr = Addressables.LoadAssetsAsync<Text>(playerAssets, (asyncHandler) =>
            //    {
            //        logger.Information($"load {asyncHandler.Name} success");
            //    }
            //    );

        }
    }
}

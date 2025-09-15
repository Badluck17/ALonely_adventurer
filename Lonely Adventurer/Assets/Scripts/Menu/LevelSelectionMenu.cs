using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using Zycalipse.Managers;


namespace Zycalipse.Menus
{
    public class LevelSelectionMenu : MonoBehaviour
    {
        [SerializeField]
        private Button missionButton;
        [SerializeField]
        private GameObject missionMenu;
        [SerializeField]
        private Scrollbar scrollBar;
        [SerializeField]
        private Transform content;
        private float scrollPos;
        private float[] pos;

        [SerializeField]
        private Button levelOnePlayButton, levelTwoPlayButton;
        [SerializeField]
        private TextMeshProUGUI levelOneProgress, levelTwoProgress;
        [SerializeField]
        private Image levelOneBar, levelTwoBar;

        [SerializeField]
        private GameObject Lvl2LockedStatus;
        [SerializeField]
        private AssetReference tutorial;

        private void OnDestroy()
        {
            missionButton.onClick.RemoveListener(OnMissionButtonClicked);
        }

        async void Start()
        {
            // TODO : UPDATE LEVEL PROGRESS STATUS
            UpdateStatus();

            missionMenu.SetActive(false);
            missionButton.onClick.AddListener(OnMissionButtonClicked);

            if (Managers.GameManager.Instance.IsFTUE)
            {
                // load tutorial

                var loadlevel = await Managers.ZycalipseSceneManager.Instance.LoadAdditiveScene(tutorial);
                loadlevel.Completed += async result =>
                {
                    if (result.Status == AsyncOperationStatus.Succeeded)
                    {
                        MainMenu.Instance.TutorialTriggered();
                        Systems.LevelManager.Instance.HasReviveChance = true;
                        GameManager.Instance.ChangeGameState(GameState.PreparingCombat);

                        GameManager.Instance.LevelCleared = false;
                        await Systems.EnemysManager.Instance.ClearEnemyList();
                        await Systems.LevelManager.Instance.PrepareLevel();
                    }
                };
            }
        }

        // Update is called once per frame
        void Update()
        {
            pos = new float[content.childCount];
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }

            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                scrollPos = scrollBar.value;
                //Managers.MenusManager.Instance.Level3DModels.ModelGroup.position = new Vector3(scrollPos * -50, 0, 0);
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scrollPos < pos[i] + distance / 2 && scrollPos > pos[i] - distance / 2)
                    {
                        if (scrollPos < pos[i])
                        {

                        }
                        scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], 0.1f);
                        //Managers.MenusManager.Instance.Level3DModels.ModelGroup.position = new Vector3(scrollBar.value * -50, 0, 0);
                    }
                }

                if (scrollBar.value < 0)
                {
                    scrollBar.value = 0;
                }
                if (scrollBar.value > 1)
                {
                    scrollBar.value = 1;
                }
            }

            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + distance / 2 && scrollPos > pos[i] - distance / 2)
                {
                    // this map is chosen do something
                    content.GetChild(i).localScale = Vector2.Lerp(content.GetChild(i).localScale, new Vector2 (1f,1f), 0.1f);
                }
                else
                {
                    content.GetChild(i).localScale = Vector2.Lerp(content.GetChild(i).localScale, new Vector2(0.5f, 0.5f), 0.1f);
                }
            }
        }

        private void OnMissionButtonClicked()
        {
            missionMenu.SetActive(true);
        }

        public void UpdateStatus()
        {
            levelOneBar.fillAmount = Systems.LevelManager.Instance.levelOneData.ProgressLevel / 20;
            levelTwoBar.fillAmount = Systems.LevelManager.Instance.levelTwoData.ProgressLevel / 20;

            levelOneProgress.text = $"Progress : {Systems.LevelManager.Instance.levelOneData.ProgressLevel}/20";
            levelTwoProgress.text = $"Progress : {Systems.LevelManager.Instance.levelTwoData.ProgressLevel}/20";

            levelTwoPlayButton.gameObject.SetActive(Systems.LevelManager.Instance.levelTwoData.Status == ItemStatus.Unlocked);
            Lvl2LockedStatus.gameObject.SetActive(Systems.LevelManager.Instance.levelTwoData.Status != ItemStatus.Unlocked);
        }
    }
}
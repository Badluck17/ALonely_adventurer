using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Zycalipse.GameLogger;
using Zycalipse.Managers;
using TMPro;

namespace Zycalipse.Systems
{
    public class PrologManager : MonoBehaviour
    {
        [SerializeField]
        private Image skipIndicator;
        private RectTransform skipIndicatorTransform;

        [SerializeField]
        GameObject cam;

        [SerializeField]
        private AssetReference mainMenuScene;

        private GameLog logger;

        public PlayableDirector director;
        float timer = 0f;

        [SerializeField]
        private TextMeshProUGUI skipHint;
        bool down = true;

        void OnEnable()
        {
            director.stopped += OnPlayableDirectorStopped;
            skipIndicator.fillAmount = 0;
        }

        async void OnPlayableDirectorStopped(PlayableDirector aDirector)
        {
            if (director == aDirector)
            {
                await LoadMainMenu();
            }
        }

        void OnDisable()
        {
            director.stopped -= OnPlayableDirectorStopped;
        }

        private void Awake()
        {
            logger = new GameLog(GetType());
        }

        private void Start()
        {
            skipIndicatorTransform = skipIndicator.gameObject.GetComponent<RectTransform>();
        }

        async void Update()
        {
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                timer += Time.deltaTime * 2;
                if (timer > 3)
                    timer = 3;
                float amount = timer / 3f;

                skipIndicator.fillAmount = amount;
                if (Input.touchCount > 0)
                {
                    if (skipIndicator.color.a < 1)
                        skipIndicator.color = new Color(skipIndicator.color.r, skipIndicator.color.g, skipIndicator.color.b, skipIndicator.color.a + Time.deltaTime);
                    for (var i = 0; i < Input.touchCount; i++)
                    {

                        if (Input.GetTouch(i).phase == TouchPhase.Began)
                        {
                            // assign new position to where finger was pressed
                            skipIndicatorTransform.position = new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, transform.position.z);

                        }

                    }
                }
                else if(Input.mousePresent)
                {
                    if (skipIndicator.color.a < 1)
                        skipIndicator.color = new Color(skipIndicator.color.r, skipIndicator.color.g, skipIndicator.color.b, skipIndicator.color.a + Time.deltaTime);

                    if(Camera.current != null)
                        skipIndicatorTransform.position = Camera.current.ScreenToWorldPoint(Input.mousePosition);
                }

                if (timer >= 3 && timer < 99)
                {
                    timer = 100f;
                    director.Stop();
                }
            }
            else if(timer < 3f)
            {
                if(skipIndicator.color.a > 0)
                    skipIndicator.color = new Color(skipIndicator.color.r, skipIndicator.color.g, skipIndicator.color.b, 0);
                skipIndicator.fillAmount = 0f;
                timer = 0f;
            }

            if ((skipHint.color.a < 0.2 && down) || (skipHint.color.a > 0.8 && !down))
            {
                down = !down;
            }
            int up = down ? -1 : 1; ;

            skipHint.color = new Color(1, 1, 1, skipHint.color.a + Time.deltaTime * up);
        }

        private async Task LoadMainMenu()
        {
            Destroy(cam);
            while (ZycalipseSceneManager.Instance == null || mainMenuScene == null)
            {
                await Task.Yield();
            }

            await ZycalipseSceneManager.Instance.LoadNewScene(mainMenuScene);
        }
    }

}
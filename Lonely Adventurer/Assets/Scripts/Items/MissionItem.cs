using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Managers;
using Zycalipse.Systems;
using Zycalipse.Menus;

namespace Zycalipse.Items
{
    public class MissionItem : MonoBehaviour
    {
        public static string Claimable = "Claim!";
        public static string Claimed = "Claimed";
        [SerializeField]
        private TextMeshProUGUI totalPrize, desc, claimText;
        public GameObject claimedObject;

        public MissionNames name;
        public int GemObtained;

        [SerializeField]
        private Button claimButton;
        [SerializeField]
        private Image buttonImage;

        // Start is called before the first frame update
        void Start()
        {
            claimButton.onClick.AddListener(OnClaimed);
        }

        private void OnDestroy()
        {
            claimButton.onClick.RemoveListener(OnClaimed);
        }

        private void OnEnable()
        {
            RefreshItem();
        }

        public void OnClaimed()
        {
            GameManager.Instance.GainGem(GemObtained);
            PlayerPrefs.SetString(name.ToString(), Claimed);
            RefreshItem();
        }

        public void RefreshItem()
        {
            string status = PlayerPrefs.GetString(name.ToString(), "");
            claimButton.gameObject.SetActive(status != "");
            totalPrize.text = $"{GemObtained}";

            string mission = name.ToString();
            switch (name)
            {
                case MissionNames.Kill10Enemy:
                    mission = "Kill 10 Enemy";
                    break;
                case MissionNames.Kill20Enemy:
                    mission = "Kill 20 Enemy";
                    break;
                case MissionNames.Kill30Enemy:
                    mission = "Kill 30 Enemy";
                    break;
                case MissionNames.Kill1000Enemy:
                    mission = "Kill 1000 Enemy";
                    break;
                case MissionNames.Kill3MiniBoss:
                    mission = "Kill 3 Mini Boss";
                    break;
                case MissionNames.Kill4Boss:
                    mission = "Kill 4 Boss";
                    break;
                case MissionNames.Died50Time:
                    mission = "Died 50 Times";
                    break;
                case MissionNames.Kill3IceZombie:
                    mission = "Kill 3 Ice Zombie";
                    break;
            }
            desc.text = mission;

            if (status != "")
            {
                if (status == Claimed)
                {
                    buttonImage.enabled = false;
                    claimText.text = Claimed;
                    claimText.gameObject.SetActive(false);
                    claimedObject.SetActive(true);
                }
                else
                {
                    buttonImage.enabled = true;
                    claimText.text = Claimable;
                    claimedObject.SetActive(false);
                }
            }
            else
            {
                // not claimable
            }
        }
    }
}

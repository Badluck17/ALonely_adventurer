using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Managers;

namespace Zycalipse.Menus
{
    [System.Serializable]
    public class CharacterStatInfo
    {
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI defenseText;
        public TextMeshProUGUI evasionText;
    }

    public class CharacterMenu : MonoBehaviour
    {
        [SerializeField]
        private Scrollbar scrollBar;
        [SerializeField]
        private Transform content;
        private float scrollPos;
        private float[] pos;

        public CharacterStatInfo CurrentStatInfo;
        public CharacterStatInfo UpgradedStatInfo;

        private int upgradeCost;
        [SerializeField]
        private TextMeshProUGUI costText;
        [SerializeField]
        private Button upgradeButton;
        // Start is called before the first frame update
        void Start()
        {
            RefreshMenu();
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
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
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scrollPos < pos[i] + distance / 2 && scrollPos > pos[i] - distance / 2)
                    {
                        scrollBar.value = Mathf.Lerp(scrollBar.value, pos[i], 0.1f);
                    }
                }
            }

            for (int i = 0; i < pos.Length; i++)
            {
                if (scrollPos < pos[i] + distance / 2 && scrollPos > pos[i] - distance / 2)
                {
                    content.GetChild(i).localScale = Vector2.Lerp(content.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                }
                else
                {
                    content.GetChild(i).localScale = Vector2.Lerp(content.GetChild(i).localScale, new Vector2(0.5f, 0.5f), 0.1f);
                }
            }
        }

        private void OnEnable()
        {
            // update current and next level
            // if level = 20 cant upgrade

            RefreshMenu();
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }

        public void RefreshMenu()
        {
            bool max = GameManager.Instance.CurrentCharLevel == 19;
            if (max)
            {
                //max
                upgradeButton.gameObject.SetActive(false);


                CurrentStatInfo.defenseText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].Defense.ToString();
                CurrentStatInfo.healthText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].MaxHP.ToString();
                CurrentStatInfo.evasionText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].EvasionRate.ToString();


                UpgradedStatInfo.defenseText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].Defense.ToString();
                UpgradedStatInfo.healthText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].MaxHP.ToString();
                UpgradedStatInfo.evasionText.text =
                    GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].EvasionRate.ToString();

                upgradeCost = GameManager.Instance.MaleChar1LevelData.statData
                    [GameManager.Instance.CurrentCharLevel].GoldToUpgrade;
                costText.text = upgradeCost.ToString();

                MenusManager.Instance.RefreshMenu();
                return;
            }

            CurrentStatInfo.defenseText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].Defense.ToString();
            CurrentStatInfo.healthText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].MaxHP.ToString();
            CurrentStatInfo.evasionText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel - 1].EvasionRate.ToString();


            UpgradedStatInfo.defenseText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel].Defense.ToString();
            UpgradedStatInfo.healthText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel].MaxHP.ToString();
            UpgradedStatInfo.evasionText.text =
                GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel].EvasionRate.ToString();

            upgradeCost = GameManager.Instance.MaleChar1LevelData.statData
                [GameManager.Instance.CurrentCharLevel].GoldToUpgrade;
            costText.text = upgradeCost.ToString();

            MenusManager.Instance.RefreshMenu();
        }

        private void OnUpgradeButtonClicked()
        {
            if (GameManager.Instance.CurrentCharLevel == 19)
            {
                //max
                upgradeButton.gameObject.SetActive(false);
                return;
            }


            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            if (GameManager.Instance.Money >= upgradeCost)
            {
                GameManager.Instance.UsingMoney(upgradeCost);
                GameManager.Instance.CurrentCharLevel++;
            }

            RefreshMenu();
            MenusManager.Instance.RefreshMenu();
        }
    }
}
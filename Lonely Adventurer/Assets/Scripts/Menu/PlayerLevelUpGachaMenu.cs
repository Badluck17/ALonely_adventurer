using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zycalipse.Items;
using Zycalipse.Managers;
using Zycalipse.Systems;
using TMPro;

namespace Zycalipse.Menus
{
    [System.Serializable]
    public class LevelUpItem
    {
        public GameObject prefabItem;
        public bool isWeaponSkill;
        public bool isOneTimeShow;
        public WeaponName weaponSkillName;
        public LevelUpItems itemType;
    }

    public class PlayerLevelUpGachaMenu : Menu
    {
        [SerializeField]
        private TextMeshProUGUI levelupText;

        [SerializeField]
        private Button claimLeft, claimMiddle, claimRight;
        [SerializeField]
        private GameObject buttonPanel;

        public LevelUpItem[] LevelUpItems;
        [SerializeField]
        private AutoScrollElementSystem leftGroup, middleGroup, rightGroup;
        private int maxItems = 6;
        private List<GameObject> leftWheelItems = new List<GameObject>();
        private List<GameObject> middleWheelItems = new List<GameObject>();
        private List<GameObject> rightWheelItems = new List<GameObject>();

        async void Start()
        {
            OnPush();
            buttonPanel.SetActive(false);
            await ResetWheels();
        }

        void Update()
        {
            if (leftGroup.Done && middleGroup.Done && rightGroup.Done && !buttonPanel.activeSelf)
            {
                buttonPanel.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            OnPop();
        }

        private async void OnEnable()
        {
            string ftueText = "You will has the chance to gacha on skill when Leveling Up!";
            string nonftueText = "Level Up!\n\nYou may choose one of these 3 reward!";

            levelupText.text = GameManager.Instance.IsFTUE ? ftueText : nonftueText;

            GameManager.Instance.ChangeGameState(GameState.PowerUpSection);
            await ResetWheels();
        }

        private async void OnDisable()
        {
            GameManager.Instance.ChangeGameState(GameState.PlayerTurn);
        }

        public override void OnFocusIn()
        {
            MenusManager.Instance.currentPopUpMenu = this;
        }
        public override void OnFocusOut()
        {
            MenusManager.Instance.currentPopUpMenu = null;
        }
        public override void RegisterListener()
        {

            claimLeft.onClick.AddListener(ClaimLeft);
            claimMiddle.onClick.AddListener(ClaimMiddle);
            claimRight.onClick.AddListener(ClaimRight);
        }
        public override void UnRegisterListener()
        {
            claimLeft.onClick.RemoveListener(ClaimLeft);
            claimMiddle.onClick.RemoveListener(ClaimMiddle);
            claimRight.onClick.RemoveListener(ClaimRight);
        }
        public override void OnPop()
        {
            OnFocusOut();
            UnRegisterListener();
        }

        public override void OnPush()
        {
            RegisterListener();
            OnFocusIn();
        }

        public override void RefreshMenu()
        {

        }

        public async Task Init()
        {
            if (CombatSystem.Instance.playerManager == null)
                return;

            await InitLeftWheel();
            await InitMiddleWheel();
            await InitRightWhell();
            StartSpinning();
        }

        public async Task ResetWheels()
        {
            for (int i = 0; i < leftWheelItems.Count; i++)
            {
                Destroy(leftWheelItems[i]);
            }
            for (int i = 0; i < middleWheelItems.Count; i++)
            {
                Destroy(middleWheelItems[i]);
            }
            for (int i = 0; i < rightWheelItems.Count; i++)
            {
                Destroy(rightWheelItems[i]);
            }

            leftWheelItems.Clear();
            middleWheelItems.Clear();
            rightWheelItems.Clear();

            leftGroup.ResetSpin();
            middleGroup.ResetSpin();
            rightGroup.ResetSpin();
            buttonPanel.SetActive(false);
            await Init();
        }

        private void StartSpinning()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.PlayerLevelup, transform.position);
            leftGroup.Starting = middleGroup.Starting = rightGroup.Starting = true;
            leftGroup.timer = 3f;
            middleGroup.timer = 3.5f;
            rightGroup.timer = 4;

            OnGachaEnd();
        }

        private async void OnGachaEnd()
        {
            GameObject itemHolder = null;

            while (!leftGroup.Done || !middleGroup.Done || !rightGroup.Done)
            {
                await Task.Yield();
            }

            int itemIndex = leftGroup.SelectedItemsIndex();
            if (itemIndex > 0 && itemIndex > leftWheelItems.Count)
                itemIndex = 0;
            itemHolder = leftWheelItems[itemIndex];

            for (int i = 0; i < leftWheelItems.Count; i++)
            {
                if(i != itemIndex)
                    Destroy(leftWheelItems[i]);
            }
            leftWheelItems.Clear();
            leftWheelItems.Add(itemHolder);
            itemHolder = null;

            itemIndex = middleGroup.SelectedItemsIndex();
            if (itemIndex > 0 && itemIndex > middleWheelItems.Count)
                itemIndex = 0;
            itemHolder = middleWheelItems[itemIndex];

            for (int i = 0; i < middleWheelItems.Count; i++)
            {
                if (i != itemIndex)
                    Destroy(middleWheelItems[i]);
            }
            middleWheelItems.Clear();
            middleWheelItems.Add(itemHolder);
            itemHolder = null;

            itemIndex = rightGroup.SelectedItemsIndex();
            if (itemIndex > 0 && itemIndex > rightWheelItems.Count)
                itemIndex = 0;
            itemHolder = rightWheelItems[itemIndex];

            for (int i = 0; i < rightWheelItems.Count; i++)
            {
                if (i != itemIndex)
                    Destroy(rightWheelItems[i]);
            }
            rightWheelItems.Clear();
            rightWheelItems.Add(itemHolder);

            leftGroup.rectTransform.anchoredPosition = new Vector2(0, 0);
            rightGroup.rectTransform.anchoredPosition = new Vector2(0, 0);
            middleGroup.rectTransform.anchoredPosition = new Vector2(0, 0);

            leftWheelItems[0].GetComponent<PlayerLevelUpItem>().ShowDesc();
            middleWheelItems[0].GetComponent<PlayerLevelUpItem>().ShowDesc();
            rightWheelItems[0].GetComponent<PlayerLevelUpItem>().ShowDesc();
        }

        private async Task InitLeftWheel()
        {
            GameObject firstItem = null;

            for (int i = 0; i < maxItems; i++)
            {
                if (i == maxItems - 1 && firstItem != null)
                {
                    leftWheelItems.Add(Instantiate(firstItem, leftGroup.transform));
                    return;
                }

                int number = GameManager.Instance.PrecentageRandomizer();
                number += (int)Time.realtimeSinceStartup;
                number %= LevelUpItems.Length;

                bool isWeaponAvailable = false;
                if (LevelUpItems[number].isWeaponSkill)
                {
                    for (int inc = 0; inc < 3; inc++)
                    {
                        if (LevelUpItems[number].weaponSkillName == CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[inc])
                        {
                            isWeaponAvailable = true;
                        }
                    }

                    if (!isWeaponAvailable)
                    {
                        i--;
                        continue;
                    }
                }

                if (i == 0)
                {
                    firstItem = LevelUpItems[number].prefabItem;
                }

                bool isExtraLife = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.ExtraLive;
                bool isExtraLevl = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.IncreaseMaxLevel;
                bool isMeteor = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Meteor;
                bool isRage = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Rage;
                bool isShadow = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SkillShadowClone;
                bool isGreed = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreedLife;
                bool isFullPower = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.FullPower;


                bool isHeal = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SmallHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.MedHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreatHeal;

                if (isHeal)
                {
                    float healPrecentage = GameManager.Instance.PrecentageRandomizer();
                    healPrecentage += 1000;
                    healPrecentage %= 2;

                    if (healPrecentage == 0)
                    {
                        i--;
                        continue;
                    }
                }
                if (CombatSystem.Instance.playerManager.playerData.PlayerLevel < 9 && isExtraLevl)
                {
                    i--;
                    continue;
                }

                if (LevelUpItems[number].isOneTimeShow &&
                    (isExtraLife && CombatSystem.Instance.alreadyGettingExtraLife) ||
                    (isExtraLevl && CombatSystem.Instance.alreadyGettingSmartLevel) ||
                    (isMeteor && CombatSystem.Instance.alreadyGettingMeteor) ||
                    (isRage && CombatSystem.Instance.alreadyGettingRage) ||
                    (isShadow && CombatSystem.Instance.alreadyGettingKageBunshin) ||
                    (isGreed && CombatSystem.Instance.alreadyGettingGreedyLife) ||
                    (isFullPower && CombatSystem.Instance.alreadyGettingFullPower))
                {
                    i--;
                    continue;
                }
                else
                {
                    leftWheelItems.Add(Instantiate(LevelUpItems[number].prefabItem, leftGroup.transform));
                }
            }
        }
        private async Task InitMiddleWheel()
        {
            GameObject firstItem = null;

            for (int i = 0; i < maxItems; i++)
            {
                if (i == maxItems - 1 && firstItem != null)
                {
                    middleWheelItems.Add(Instantiate(firstItem, middleGroup.transform));
                    return;
                }

                int number = GameManager.Instance.PrecentageRandomizer();
                number += (int)Time.realtimeSinceStartup;
                number %= LevelUpItems.Length;

                bool isWeaponAvailable = false;
                if (LevelUpItems[number].isWeaponSkill)
                {
                    for (int inc = 0; inc < 3; inc++)
                    {
                        if (LevelUpItems[number].weaponSkillName == CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[inc])
                        {
                            isWeaponAvailable = true;
                        }
                    }

                    if (!isWeaponAvailable)
                    {
                        i--;
                        continue;
                    }
                }

                if (i == 0)
                {
                    firstItem = LevelUpItems[number].prefabItem;
                }

                bool isExtraLife = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.ExtraLive;
                bool isExtraLevl = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.IncreaseMaxLevel;
                bool isMeteor = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Meteor;
                bool isRage = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Rage;
                bool isShadow = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SkillShadowClone;
                bool isGreed = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreedLife;
                bool isFullPower = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.FullPower;


                bool isHeal = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SmallHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.MedHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreatHeal;

                if (isHeal)
                {
                    float healPrecentage = GameManager.Instance.PrecentageRandomizer();
                    healPrecentage += 1000;
                    healPrecentage %= 2;

                    if (healPrecentage == 0)
                    {
                        i--;
                        continue;
                    }
                }

                if (CombatSystem.Instance.playerManager.playerData.PlayerLevel < 9 && isExtraLevl)
                {
                    i--;
                    continue;
                }
                if (LevelUpItems[number].isOneTimeShow &&
                    (isExtraLife && CombatSystem.Instance.alreadyGettingExtraLife) ||
                    (isExtraLevl && CombatSystem.Instance.alreadyGettingSmartLevel) ||
                    (isMeteor && CombatSystem.Instance.alreadyGettingMeteor) ||
                    (isRage && CombatSystem.Instance.alreadyGettingRage) ||
                    (isShadow && CombatSystem.Instance.alreadyGettingKageBunshin) ||
                    (isGreed && CombatSystem.Instance.alreadyGettingGreedyLife) ||
                    (isFullPower && CombatSystem.Instance.alreadyGettingFullPower))
                {
                    i--;
                    continue;
                }
                else
                {
                    middleWheelItems.Add(Instantiate(LevelUpItems[number].prefabItem, middleGroup.transform));
                }
            }
        }
        private async Task InitRightWhell()
        {
            GameObject firstItem = null;

            for (int i = 0; i < maxItems; i++)
            {
                if (i == maxItems - 1 && firstItem != null)
                {
                    rightWheelItems.Add(Instantiate(firstItem, rightGroup.transform));
                    return;
                }

                int number = GameManager.Instance.PrecentageRandomizer();
                number += (int)Time.realtimeSinceStartup;
                number %= LevelUpItems.Length;

                bool isWeaponAvailable = false;
                if (LevelUpItems[number].isWeaponSkill)
                {
                    for (int inc = 0; inc < 3; inc++)
                    {
                        if (LevelUpItems[number].weaponSkillName == CombatSystem.Instance.playerManager.WeaponManager.AllWeaponEquiped[inc])
                        {
                            isWeaponAvailable = true;
                        }
                    }

                    if (!isWeaponAvailable)
                    {
                        i--;
                        continue;
                    }
                }

                if (i == 0)
                {
                    firstItem = LevelUpItems[number].prefabItem;
                }

                bool isExtraLife = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.ExtraLive;
                bool isExtraLevl = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.IncreaseMaxLevel;
                bool isMeteor = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Meteor;
                bool isRage = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.Rage;
                bool isShadow = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SkillShadowClone;
                bool isGreed = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreedLife;
                bool isFullPower = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.FullPower;

                bool isHeal = LevelUpItems[number].itemType == Zycalipse.LevelUpItems.SmallHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.MedHeal
                    || LevelUpItems[number].itemType == Zycalipse.LevelUpItems.GreatHeal;

                if (isHeal)
                {
                    float healPrecentage = GameManager.Instance.PrecentageRandomizer();
                    healPrecentage += 1000;
                    healPrecentage %= 2;

                    if(healPrecentage == 0)
                    {
                        i--;
                        continue;
                    }
                }

                if (CombatSystem.Instance.playerManager.playerData.PlayerLevel < 9 && isExtraLevl)
                {
                    i--;
                    continue;
                }
                if (LevelUpItems[number].isOneTimeShow &&
                    (isExtraLife && CombatSystem.Instance.alreadyGettingExtraLife) ||
                    (isExtraLevl && CombatSystem.Instance.alreadyGettingSmartLevel) ||
                    (isMeteor && CombatSystem.Instance.alreadyGettingMeteor) ||
                    (isRage && CombatSystem.Instance.alreadyGettingRage) ||
                    (isShadow && CombatSystem.Instance.alreadyGettingKageBunshin) ||
                    (isGreed && CombatSystem.Instance.alreadyGettingGreedyLife) ||
                    (isFullPower && CombatSystem.Instance.alreadyGettingFullPower))
                {
                    i--;
                    continue;
                }
                else
                {
                    rightWheelItems.Add(Instantiate(LevelUpItems[number].prefabItem, rightGroup.transform));
                }
            }
        }

        private void ClaimLeft()
        {
            leftWheelItems[0].GetComponent<PlayerLevelUpItem>().ApplyEffect();

            OnClose();
        }
        private void ClaimMiddle()
        {
            middleWheelItems[0].GetComponent<PlayerLevelUpItem>().ApplyEffect();

            OnClose();
        }
        private void ClaimRight()
        {
            rightWheelItems[0].GetComponent<PlayerLevelUpItem>().ApplyEffect();

            OnClose();
        }

        private async void OnClose()
        {
            SoundManager.PlayOneShotSound(DataManager.Instance.ButtonOpenSound, transform.position);
            await CombatSystem.Instance.playerManager.RefreshUI();
            gameObject.SetActive(false);
        }
    }
}
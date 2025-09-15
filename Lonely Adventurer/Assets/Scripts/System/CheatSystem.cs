using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zycalipse.Managers;
using Zycalipse.Menus;

namespace Zycalipse.Systems
{
    public class CheatSystem : MonoBehaviour
    {
        [SerializeField]
        private Button addMoneyButton, imortalToggleButton, setLevelButton, closeButton;
        [SerializeField]
        private InputField levelInputField;

        // Start is called before the first frame update
        void Start()
        {
            addMoneyButton.onClick.AddListener(AddMoney);
            imortalToggleButton.onClick.AddListener(ImortalToggle);
            setLevelButton.onClick.AddListener(cheatLevel);
            closeButton.onClick.AddListener(Close);
        }

        private void AddMoney()
        {
            GameManager.Instance.GainMoney(50000);
            MenusManager.Instance.RefreshMenu();
        }

        private void ImortalToggle()
        {
            GameManager.Instance.Imortal = !GameManager.Instance.Imortal;
        }

        private void cheatLevel()
        {
            int level = int.Parse(levelInputField.text);

            GameManager.Instance.CheatLevel = level;
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}

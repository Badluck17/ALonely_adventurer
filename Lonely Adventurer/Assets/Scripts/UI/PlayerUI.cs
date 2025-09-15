using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Systems;

namespace Zycalipse.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Image helathBar;
        [SerializeField]
        private TextMeshProUGUI helathPoint;

        public async void RefreshHealth()
        {
            helathBar.fillAmount = (float)CombatSystem.Instance.playerManager.playerData.PlayerHealth /
                (float)CombatSystem.Instance.playerManager.playerData.PlayerMaxHealth;
            helathPoint.text = CombatSystem.Instance.playerManager.playerData.PlayerHealth.ToString();
        }
    }
}

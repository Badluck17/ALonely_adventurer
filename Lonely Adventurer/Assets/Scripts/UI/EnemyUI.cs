using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zycalipse.Systems;
using Zycalipse.Enemys;

namespace Zycalipse.UI
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField]
        private Image helathBar, outerHealthBar;
        [SerializeField]
        private TextMeshProUGUI helathPoint;
        public Enemy enemy { get; set; }

        public async Task RefreshHealth()
        {
            helathBar.fillAmount = (float)enemy.data.Health / (float)enemy.data.MaxHealth;
            helathPoint.text = enemy.data.Health.ToString();
        }

        public void DisableUI(bool disable = true)
        {
            outerHealthBar.gameObject.SetActive(!disable);
            helathBar.gameObject.SetActive(!disable);
            helathPoint.gameObject.SetActive(!disable);
        }
    }
}

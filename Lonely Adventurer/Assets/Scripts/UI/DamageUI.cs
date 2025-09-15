using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Zycalipse.UI
{
    [System.Serializable]
    public class DamageTypeText
    {
        public DamageType Type;
        public GameObject Reference;
    }


    public class DamageUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI damageText;
        [HideInInspector]
        public string Damage = "";
        [HideInInspector]
        public Vector3 position;
        private float timer = 2f;
        private bool positionSeted;

        [SerializeField]
        private CanvasGroup canvasGroup;

        // Start is called before the first frame update
        void Start()
        {
            canvasGroup.alpha = 0;
            positionSeted = false;
            damageText.text = Damage;
        }

        // Update is called once per frame
        async void Update()
        {
            damageText.fontSize -= Time.deltaTime * 150;

            if (position != Vector3.zero && !positionSeted)
            {
                positionSeted = true;
                transform.position = position;
                canvasGroup.alpha = 1;
            }

            if (damageText.fontSize <= 10)
            {
                Destroy(gameObject);
            }

            if (Damage != "")
            {
                if (timer < 0)
                {
                    damageText.fontSize -= Time.deltaTime * 300;
                }
                timer -= Time.deltaTime;
            }
        }
    }
}

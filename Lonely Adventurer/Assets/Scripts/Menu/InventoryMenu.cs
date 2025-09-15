using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zycalipse.Managers;

namespace Zycalipse.Menus
{
    [System.Serializable]
    public class WeaponSprite
    {
        public WeaponName Name;
        public Sprite Icon;
    }

    public class InventoryMenu : MonoBehaviour
    {
        //[SerializeField]
        //private Transform content;

        [SerializeField]
        private Image closeRangeWeaponImage, midRangeWeaponImage, longRangeWeaponImage;
        [SerializeField]
        private WeaponSprite[] closeWeapon, midWeapon, longWeapon;

        private void Start()
        {
            /*
             * check equiped weapon
             * if there is equiped then use that
             * 
             * if not, check owned weapon
             * if there is atleast 1 in each category
             * equip it
             * if not make the equiped blank (but its imposible and if happen than this is bug)
            */

            UpdateEquipedWeaponUI();
        }

        private void Update()
        {
            //if (content.localPosition.y > 350f)
            //{
            //    content.localPosition = new Vector3(content.localPosition.x, 350f, content.localPosition.z);
            //}
            //else if (content.localPosition.y < -100f)
            //{
            //    content.localPosition = new Vector3(content.localPosition.x, -100f, content.localPosition.z);
            //}
        }

        public void RefreshMenu()
        {
            UpdateEquipedWeaponUI();
        }

        private void UpdateEquipedWeaponUI()
        {
            for (int i = 0; i < closeWeapon.Length; i++)
            {
                if (closeWeapon[i].Name == DataManager.Instance.EquipedWeapons[0])
                {
                    // this weapon is equiped
                    closeRangeWeaponImage.sprite = closeWeapon[i].Icon;
                }
            }

            for (int i = 0; i < midWeapon.Length; i++)
            {
                if (midWeapon[i].Name == DataManager.Instance.EquipedWeapons[1])
                {
                    // this weapon is equiped
                    midRangeWeaponImage.sprite = midWeapon[i].Icon;
                }
            }

            for (int i = 0; i < longWeapon.Length; i++)
            {
                if (longWeapon[i].Name == DataManager.Instance.EquipedWeapons[2])
                {
                    // this weapon is equiped
                    longRangeWeaponImage.sprite = longWeapon[i].Icon;
                }
            }
        }
    }

}
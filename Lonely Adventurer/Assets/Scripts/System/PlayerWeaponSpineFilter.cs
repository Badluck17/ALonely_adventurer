using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

namespace Zycalipse.Systems
{
    public class PlayerWeaponSpineFilter : SpineMeshFilter
    {
        public string SlotNameToKeep;

        void Start()
        {
            FilterWeapon();
        }
        void OnEnable()
        {
            FilterWeapon();
        }

        private void FilterWeapon()
        {
            foreach (var item in Animation.Skeleton.Slots)
            {
                if (item.Data.Name != SlotNameToKeep)
                {
                    item.Attachment = null;
                }
            }
        }
    }
}

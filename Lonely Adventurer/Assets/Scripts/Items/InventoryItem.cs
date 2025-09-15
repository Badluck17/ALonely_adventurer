using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zycalipse.Items
{
    public class InventoryItem : ZycalipseItems
    {
        public CurrencyType CurrencyType;
        public int Power;
        public Button ItemButton;
        public bool Equiped { get; set; }
        public bool Owned { get; set; }
        public bool Locked { get; set; }
    }

}
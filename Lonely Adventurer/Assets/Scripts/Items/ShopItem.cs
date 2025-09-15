using UnityEngine.UI;

namespace Zycalipse.Items
{
    public class ShopItem : ZycalipseItems
    {
        public CurrencyType CurrencyType;
        public int Price;
        public Button ItemButton;
        public bool Owned { get; set; }
        public bool Locked { get; set; }
    }
}

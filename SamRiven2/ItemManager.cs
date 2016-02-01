using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace SamRiven2
{
    public class ItemManager
    {
        public static Item Hydra;
        public static Item Titanic;
        public static void Init()
        {
            UpdateItems();
            Shop.OnBuyItem += delegate { UpdateItems(); };
            Shop.OnSellItem += delegate { UpdateItems(); };
            Shop.OnUndo += delegate { UpdateItems(); };
            Player.OnSwapItem += delegate { UpdateItems(); };
        }

        public static void UpdateItems()
        {
            Core.DelayAction(() =>
            {
                var titanicId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == (ItemId)3748);
                var hydraId =
                    Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Ravenous_Hydra_Melee_Only || a.Id == ItemId.Tiamat_Melee_Only);
                if (hydraId != null)
                {
                    Hydra = new Item(hydraId.Id, 300);
                    Titanic = null;
                    return;
                }
                if (titanicId != null)
                {
                    Titanic = new Item(titanicId.Id);
                    Hydra = null;
                    return;
                }
                Hydra = null;
                Titanic = null;
            }, 200);
        } 
    }
}
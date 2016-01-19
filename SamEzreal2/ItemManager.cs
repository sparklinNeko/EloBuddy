using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace SamEzreal2
{
    public class ItemManager
    {
        public static Item Hydra;
        public static Item Titanic;
        public static Item TrinityForce;
        public static Item Gauntlet;
        public static Item Sheen;
        public static Item LichBane;
        public static Item Muramana;
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
            //Chat.Print("updating items");
            Core.DelayAction(() =>
            {
                var titanicId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == (ItemId)3748);
                var hydraId =
                    Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Ravenous_Hydra_Melee_Only || a.Id == ItemId.Tiamat_Melee_Only);
                var gauntletId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Iceborn_Gauntlet);
                var trinityId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Trinity_Force);
                var sheenId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Sheen);
                var lichbaneId = Player.Instance.InventoryItems.FirstOrDefault(
                        a => a.Id == ItemId.Lich_Bane);
                var muramanaId = Player.Instance.InventoryItems.FirstOrDefault(
                    a => a.Id == (ItemId)3042);
                if (gauntletId != null)
                {
                    Gauntlet = new Item(gauntletId.Id);
                }
                if (trinityId != null)
                {
                    TrinityForce = new Item(trinityId.Id);
                }
                if (sheenId != null)
                {
                    Sheen = new Item(sheenId.Id);
                }
                if (lichbaneId != null)
                {
                    LichBane = new Item(lichbaneId.Id);
                }
                if (muramanaId != null)
                {
                    //Chat.Print("Found muramana");
                    Muramana = new Item(muramanaId.Id);
                }
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
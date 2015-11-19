using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
//using EloBuddy.SDK.Events;

namespace SamSivir
{
    class ItemTargetHelper
    {
        private static readonly ItemId[] TargetItems = new[] { ItemId.Bilgewater_Cutlass, ItemId.Blade_of_the_Ruined_King};
        private static List<ItemId> _inventoryItems;
        private static int _keyPressed = 0;
        static ItemTargetHelper()
        {
            Game.OnTick += Game_OnTick;
            Shop.OnBuyItem += Shop_OnBuyItem;
            Shop.OnSellItem += Shop_OnSellItem;
            Game.OnWndProc += Game_OnWndProc;
        }

        public static void Initialize()
        {
            // Let the static initializer do the job, this way we avoid multiple init calls aswell
        }
        public static void Game_OnTick(EventArgs args)
        {
            if (_keyPressed + 200 > Environment.TickCount)
            {
                var item = ObjectManager.Player.InventoryItems.FirstOrDefault(a => TargetItems.Contains(a.Id));
                if (item != null && item.CanUseItem())
                {
                    var target = TargetSelector.GetTarget(450, DamageType.Physical);
                    if (target != null)
                    {
                        item.Cast(target);
                    }

                }

            }
        }
        public static void Shop_OnBuyItem(AIHeroClient sender, ShopActionEventArgs args)
        {
            if (!sender.IsMe)
                return;
            var itemId = TargetItems.FirstOrDefault(a => (ItemId)args.Id == a);
            if (itemId != null)
            {
                _inventoryItems.Add(itemId);
            }
            CleanInventoryItems();
        }
        public static void Shop_OnSellItem(AIHeroClient sender, ShopActionEventArgs args)
        {
            
            if (!sender.IsMe)
                return;
            //var itemId = inventoryItems.FirstOrDefault(a => (ItemId)args.Id == a);
            //if (itemId != null)
            //{
            //    inventoryItems.Remove(itemId);
            //}
            CleanInventoryItems();
        }
        public static void CleanInventoryItems()
        {
            _inventoryItems = _inventoryItems.FindAll(itemId => (ObjectManager.Player.InventoryItems.FirstOrDefault(slot => slot.Id == itemId) != null));
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint) WindowMessages.KeyDown) return;
            // Work around of getting
            const uint escapeKey = (uint)'1';
            if (escapeKey != args.WParam) return;
            Chat.Print("Lel");
            _keyPressed = Environment.TickCount;
        }
    }
}

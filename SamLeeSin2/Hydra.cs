using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace SamLeeSin2
{
    class Hydra
    {
        public static long LastETime = 0;
        public static Menu HydraMenu;
        public static bool _useHydra{
            get { return HydraMenu["useHydraAfterE"].Cast<CheckBox>().CurrentValue; }
        }
        public static void Init()
        {
            HydraMenu = Program.menu.AddSubMenu("Hydra Usage");
            HydraMenu.Add("useHydraAfterE", new CheckBox("Use Hydra After E", true));
            Game.OnTick += Game_OnTick;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;

        }
        private static void Game_OnTick(EventArgs args)
        {
            if (LastETime + 1000 > Environment.TickCount && _useHydra)
            {
                var hydraSlot = GetHydraSlot();
                if (hydraSlot != null && hydraSlot.CanUseItem())
                {
                    hydraSlot.Cast();
                }

            }
        }
        public static InventorySlot GetHydraSlot()
        {
            ItemId titanicHydra = (ItemId)3748;
            var hydraIds = new[] { ItemId.Ravenous_Hydra_Melee_Only, ItemId.Tiamat_Melee_Only, titanicHydra };
            return ObjectManager.Player.InventoryItems.FirstOrDefault(a => hydraIds.Contains(a.Id) && a.CanUseItem());
        }
        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;
            if (args.SData.Name == Program.Spells["E1"])
                LastETime = Environment.TickCount;
        }

    }
}

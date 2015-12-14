using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace SamKogmaw
{
    public static class ItemManager
    {
        public static void Init()
        {
            
        }

        public static bool HasBotrkOrCutlass()
        {
            return Player.Instance.InventoryItems.Any(
                    i => i.Id == ItemId.Blade_of_the_Ruined_King || i.Id == ItemId.Bilgewater_Cutlass);    
        }
        public static void CastBotrkOrCutlass(AIHeroClient target)
        {
            if (target == null || !target.IsValidTarget(450)) return;
            var item =
                Player.Instance.InventoryItems.FirstOrDefault(
                    i => i.Id == ItemId.Blade_of_the_Ruined_King || i.Id == ItemId.Bilgewater_Cutlass);
            if (item == null || !item.CanUseItem()) return;
            item.Cast(target);
        }
    }
}
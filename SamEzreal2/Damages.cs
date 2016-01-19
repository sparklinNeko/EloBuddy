using EloBuddy;

namespace SamEzreal2
{
    public static class Damages
    {
        public static double GetQDamage(this Obj_AI_Base target)
        {
            double sheen = 0;
            double lichbane = 0;
            double Mdamage = 0;
            if (ItemManager.Sheen != null && ItemManager.Sheen.IsReady())
                sheen = Player.Instance.BaseAttackDamage;
            if (ItemManager.TrinityForce != null && ItemManager.TrinityForce.IsReady())
                sheen = 2 * Player.Instance.BaseAttackDamage;
            if (ItemManager.Gauntlet != null && ItemManager.Gauntlet.IsReady())
                sheen = 1.25 * Player.Instance.BaseAttackDamage;
            if (ItemManager.LichBane != null && ItemManager.LichBane.IsReady())
                lichbane = .75 * Player.Instance.BaseAttackDamage + .5 * Player.Instance.TotalMagicalDamage;
            double ADdamage = 20*Logic.Q.Level + 15 + 1.1*Player.Instance.TotalAttackDamage +
                           .4*Player.Instance.TotalMagicalDamage;
            double armor = target.Armor*Player.Instance.PercentArmorPenetrationMod -
                           Player.Instance.FlatArmorPenetrationMod;
            double mres = target.SpellBlock*Player.Instance.PercentMagicPenetrationMod -
                          Player.Instance.FlatMagicPenetrationMod;
            mres = mres < 0 ? 0 : mres;
            armor = armor < 0 ? 0 : armor;
            double armorPercent = armor/(100 + armor);
            double mresPercent = mres/(100 + mres);
            ADdamage += sheen;
            Mdamage += lichbane;
            return ADdamage*(1-armorPercent) + Mdamage*(1 - mresPercent);
        }
    }
}
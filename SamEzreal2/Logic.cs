using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace SamEzreal2
{
    public static class Logic
    {
        private static AIHeroClient _Player
        {
            get { return Player.Instance; }
        }
        public static Spell.Skillshot Q;
            

        public static Spell.Skillshot W; 
        public static Spell.Skillshot E; 
        public static Spell.Skillshot R; 

        public static void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 2000, 65)
            {
                MinimumHitChance = HitChance.High
            };
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1550, 80)
            {
                AllowedCollisionCount = -1
            };
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Circular, 250, int.MaxValue, 10);
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1000, 2000, 160)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = -1
            };
            Game.OnTick += Game_OnTick;
        }

        private static void Game_OnTick(System.EventArgs args)
        {
            if (Player.HasBuff("Muramana") &&
                (_Player.ManaPercent < 25 || Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.Combo))
                ItemManager.Muramana.Cast();
            if (Config.config.GetKeyBind("etomouse") && E.IsReady() && Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None)
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
                return;
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
                return;
            }
            CheckDStuned();
        }

        private static void Harass()
        {
            if (Q.IsReady() && Config.harassMenu.GetCheckBox("Q.Enabled") &&
                Config.harassMenu.GetSlider("Q.Mana") < _Player.ManaPercent)
            {
                var QT = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (QT != null && QT.IsValidTarget())
                {
                    var qpred = Q.GetPrediction(QT);
                    if (qpred.HitChancePercent >= Config.harassMenu.GetSlider("Q.Hitchance"))
                    {
                        Player.CastSpell(SpellSlot.Q, qpred.CastPosition);
                        return;
                    }
                }
            }
            if (W.IsReady() && Config.harassMenu.GetCheckBox("W.Enabled") &&
                Config.harassMenu.GetSlider("W.Mana") < _Player.ManaPercent)
            {
                var WT = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (WT != null && WT.IsValidTarget())
                {
                    var wpred = W.GetPrediction(WT);
                    if (wpred.HitChancePercent >= Config.harassMenu.GetSlider("Q.Hitchance"))
                    {
                        Player.CastSpell(SpellSlot.W, wpred.CastPosition);
                        return;
                    }
                }
            }
        }

        private static void LastHit()
        {

            if (!Q.IsReady() || !Config.lasthitMenu.GetCheckBox("Q.Enabled") || Config.lasthitMenu.GetSlider("Q.Mana") > _Player.ManaPercent) return;
            var mob =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                    m => m != null && m.IsValidTarget(Q.Range - 50) && m.GetQDamage() > Prediction.Health.GetPrediction(m, (int)(1000 * m.Distance(_Player) / Q.Speed)) && Q.GetPrediction(m).HitChance >= HitChance.High);
            if (mob != null && mob.IsValidTarget(Q.Range - 50))
            {
                //Chat.Print(mob.Health + " - " + mob.GetQDamage() + "  or " + (int)(1000*mob.Distance(_Player) / Q.Speed));
                Player.CastSpell(SpellSlot.Q, mob.Position);

            }
        }

        private static void LaneClear()
        {
            if (!Q.IsReady() || !Config.laneclearMenu.GetCheckBox("Q.Enabled") || Config.laneclearMenu.GetSlider("Q.Mana") > _Player.ManaPercent) return;
            var mob =
                EntityManager.MinionsAndMonsters.EnemyMinions.FirstOrDefault(
                    m => m != null && m.IsValidTarget(Q.Range - 50) && Q.GetPrediction(m).HitChance >= HitChance.High);
            if (mob != null && mob.IsValidTarget(Q.Range - 50))
            {
                //Chat.Print(mob.Health + " - " + mob.GetQDamage() + "  or " + _Player.GetSpellDamage(mob, SpellSlot.Q));
                Player.CastSpell(SpellSlot.Q, mob.Position);
            }
        }

        private static void Combo()
        {
            if (ItemManager.Muramana != null && ItemManager.Muramana.IsReady() && !Player.HasBuff("Muramana") && _Player.ManaPercent > 25)
                ItemManager.Muramana.Cast();
            //Player.Instance.Buffs.ForEach(b => Chat.Print(b.Name));
            if (Q.IsReady() && Config.comboMenu.GetCheckBox("Q.Enabled") && 
                Config.comboMenu.GetSlider("Q.Mana") < _Player.ManaPercent)
            {
                var QT = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (QT != null && QT.IsValidTarget())
                {
                    var qpred = Q.GetPrediction(QT);
                    if (qpred.HitChancePercent >= Config.comboMenu.GetSlider("Q.Hitchance"))
                    {
                        Player.CastSpell(SpellSlot.Q, qpred.CastPosition);
                        return;
                    }
                }
            }
            if (W.IsReady() && Config.comboMenu.GetCheckBox("W.Enabled") &&
                Config.comboMenu.GetSlider("W.Mana") < _Player.ManaPercent)
            {
                var WT = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (WT != null && WT.IsValidTarget())
                {
                    var wpred = W.GetPrediction(WT);
                    if (wpred.HitChancePercent >= Config.comboMenu.GetSlider("Q.Hitchance"))
                    {
                        Player.CastSpell(SpellSlot.W, wpred.CastPosition);
                        return;
                    }
                }
            }
            
            //var ET = TargetSelector.GetTarget(E.Range, DamageType.Magical);
        }

        private static void CheckDStuned()
        {
            //throw new System.NotImplementedException();
        }
    }
}
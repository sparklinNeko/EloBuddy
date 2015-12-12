using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
// ReSharper disable InconsistentNaming

namespace SamKogmaw
{
    public static class Kogmaw
    {
        private static Spell.Skillshot Q, E, R;
        private static uint[] wRange = {590, 620, 650, 680, 710};
        private static uint[] rRange = {1200, 1500, 1800};
        private static Spell.Active W;

        private static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static void Init()
        {
            W = new Spell.Active(SpellSlot.W, 590);
            W.OnSpellCasted += W_OnSpellCasted;
            Q = new Spell.Skillshot(SpellSlot.Q, 1175, SkillShotType.Linear)
            {
                AllowedCollisionCount = 0
            };
            E = new Spell.Skillshot(SpellSlot.E, 1280, SkillShotType.Linear)
            {
                AllowedCollisionCount = -1
            };
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Circular)
            {
                AllowedCollisionCount = -1
            };
            Game.OnTick += Game_OnTick;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Obj_AI_Base.OnTeleport += Obj_AI_Base_OnTeleport;
            Game.OnEnd += Game_OnEnd;
            Orbwalker.DisableMovement = false;
        }

        private static void Game_OnEnd(GameEndEventArgs args)
        {
            Orbwalker.DisableMovement = false;
        }

        private static void W_OnSpellCasted(Spell.SpellBase spell, GameObjectProcessSpellCastEventArgs args)
        {
        }

        private static void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, GameObjectTeleportEventArgs args)
        {
            // todo CC
            // todo onrecall
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            // todo CC
            // todo onrecall
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (_Player.IsDead || !Config.gapcloseMenu.GetCheckBox("antigapcloseenabled")) return;
            if (!Config.gapcloseMenu.GetCheckBox("eantigapclose") ||
                (_Player.ManaPercent < Config.gapcloseMenu.GetSlider("eantigapclosemana"))) return;
            if (sender == null || !e.Sender.IsValidTarget(E.Range)) return;
            //if (e.Start.Distance(_Player) < e.End.Distance(_Player)) return;

            
            var edelay = Config.gapcloseMenu.GetSlider("eantigapclosedelay");
            if (edelay > 0)
                Core.DelayAction(() => CastE(sender, HitChance.High), edelay);
            else
                CastE(sender, HitChance.High);
        }
        private static void CastR(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            //Utils.Debug("Casting R");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = R.MinimumHitChance;
                R.MinimumHitChance = minimumHitchance;
            }
            R.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                R.MinimumHitChance = hitchance;
        }
        private static void CastQ(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            //Utils.Debug("Casting Q");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = Q.MinimumHitChance;
                Q.MinimumHitChance = minimumHitchance;
            }
            Q.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                Q.MinimumHitChance = hitchance;
        }
        private static void CastE(Obj_AI_Base target, HitChance minimumHitchance = HitChance.Unknown)
        {
            //Utils.Debug("Casting E");
            var hitchance = HitChance.High;
            if (minimumHitchance != HitChance.Unknown)
            {
                hitchance = E.MinimumHitChance;
                E.MinimumHitChance = minimumHitchance;
            }
            E.Cast(target);
            if (minimumHitchance != HitChance.Unknown)
                E.MinimumHitChance = hitchance;
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (_Player.IsDead) return;
            ChangeSkills();
            LimitOrbwalker();
            AutoHarass();
            //Utils.Debug(W.State);
        }

        private static void ChangeSkills()
        {
            if (W.State == SpellState.Surpressed && E.CastDelay == 250)
            {
                Q.CastDelay = 125;
                E.CastDelay = 125;
                R.CastDelay = 125;
            }
            if (W.State != SpellState.Surpressed && E.CastDelay == 125)
            {
                Q.CastDelay = 250;
                E.CastDelay = 250;
                R.CastDelay = 250;
            }
        }

        private static bool CheckBetween(int max)
        {
            //Utils.Debug(min+ " "+ (1/_Player.AttackDelay)+" "+ max);
            return ((int)((1/_Player.AttackDelay)*1000) < max);
        }

        private static void LimitOrbwalker()
        {
            // if w is not active (as <2.5) and forcelimit is off
            if (W.State != SpellState.Surpressed && !Config.limitMenu.GetCheckBox("forcelimit"))
            {

                Orbwalker.DisableMovement = false;
                return;
            }
            var num = Config.miscMenu.GetSlider("delaynum");

            var newDelay = Config.limitMenu.GetSlider("mmd_1");
            var allowed = CheckBetween(Config.limitMenu.GetSlider("maxas_1"));
            if (num == 1)
            {
                if (allowed)
                {
                    Orbwalker.DisableMovement = false;
                    return;
                }
            }
            else
            {
                for (var i = num; i > 1; i--)
                {
                    if (Config.limitMenu.GetSlider("maxas_" + (i)) < Config.limitMenu.GetSlider("maxas_" + (i - 1)))
                    {
                        Utils.Print("Max Attack Speed (" + i + "): " + Config.limitMenu.GetSlider("maxas_" + (i)) + " must be more than previous one: " + Config.limitMenu.GetSlider("maxas_" + (i-1)));
                        break;
                    }
                    allowed = CheckBetween(Config.limitMenu.GetSlider("maxas_" + (i)));
                    if (!allowed)
                    {
                        newDelay = Config.limitMenu.GetSlider("mmd_" + (i));
                        break;
                    }
                }
            }

            if (allowed)
            {
                Orbwalker.DisableMovement = false;
                return;
            }
            // && W.State != SpellState.Surpressed && !Config.limitMenu.GetCheckBox("forcelimit")
            if (newDelay > Orbwalker.MovementDelay)
            {
                Orbwalker.DisableMovement = Orbwalker.LastMovementSent + newDelay > Core.GameTickCount;
            }
        }

        private static void AutoHarass()
        {
           
            if (!Config.autoharassMenu.GetCheckBox("autoharassenabled")) return;
            // some mode is active
            if (Orbwalker.ActiveModesFlags != Orbwalker.ActiveModes.None) return;
            var enemies = EntityManager.Heroes.Enemies.Where(
                h => h.Distance(_Player) < R.Range && Config.autoharassMenu.GetCheckBox(h.ChampionName+"autoharass")
                ).OrderBy(h => h.Health);
            // Prioritize R over E over Q, one at a time
            foreach (AIHeroClient enemy in enemies)
            {

                var pred = R.GetPrediction(enemy);
                if (pred.HitChance >= HitChance.High && _Player.ManaPercent > Config.autoharassMenu.GetSlider("rautoharassmana"))
                {
                    Player.CastSpell(SpellSlot.R, pred.CastPosition);
                    return;
                }
                pred = E.GetPrediction(enemy);
                if (pred.HitChance >= HitChance.High && _Player.ManaPercent > Config.autoharassMenu.GetSlider("eautoharassmana"))
                {
                    Player.CastSpell(SpellSlot.E, pred.CastPosition);
                    return;
                }
                pred = Q.GetPrediction(enemy);
                if (pred.HitChance >= HitChance.High && _Player.ManaPercent > Config.autoharassMenu.GetSlider("qautoharassmana"))
                {
                    Player.CastSpell(SpellSlot.Q, pred.CastPosition);
                    return; 
                }

            }
        }

        private static void Combo()
        {
            var aarange = W.State == SpellState.Ready ? wRange[W.Level] : _Player.AttackRange;
            var target = TargetSelector.GetTarget(aarange, DamageType.Mixed);
            if (target != null && target.IsValidTarget() && _Player.ManaPercent > Config.comboMenu.GetSlider("wcombomana"))
                W.Cast();
            
        }

        // todo when onkillable fixed?
        private static void Lasthit()
        {
        }
    }
}
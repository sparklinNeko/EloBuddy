using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace ChampionPlugins.Plugins
{
    public class Ezreal : Plugin
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }
        public override void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (E.IsReady() && Player.Instance.CountEnemiesInRange(800) == 1 &&
               target.HealthPercent < Player.Instance.HealthPercent - 20)
            {
                E.Cast(Player.Instance.Position.Extend(target.Position, E.Range).To3D());
            }
            target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (W.IsReady() && target.IsValidTarget(W.Range) && Player.Instance.ManaPercent > 40)
            {
                W.Cast(target);
            }
            
            if (R.IsReady() && Player.Instance.CountEnemiesInRange(W.Range) <= 1)
            {
                var hero =
                    EntityManager.Heroes.Enemies.OrderByDescending(e => e.Health)
                        .FirstOrDefault(
                            e => e.Health < Player.Instance.GetSpellDamage(e, SpellSlot.R) && e.IsValidTarget(2500));
                if (hero == null) return;

                R.Cast(hero);
            }
        }

        public override void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && 20 <= Player.Instance.ManaPercent)
            {
                Q.Cast(target);
            }
            target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if (W.IsReady() && target.IsValidTarget(W.Range) && 30 <= Player.Instance.ManaPercent)
            {
                W.Cast(target);
            }
        }

        public override void Flee()
        {
            
        }

        public override void LaneClear()
        {
            var laneMinion =
                    EntityManager.MinionsAndMonsters.GetLaneMinions()
                        .OrderByDescending(m => m.Health)
                        .FirstOrDefault(
                            m => m.IsValidTarget(Q.Range) && m.Health <= Player.Instance.GetSpellDamage(m, SpellSlot.Q));
            if (laneMinion == null) return;

            if (Q.IsReady() && 40 <= Player.Instance.ManaPercent)
            {
                Q.Cast(laneMinion);
            }
        }

        public override void JungleClear()
        {
            
        }

        public override void LastHit()
        {
            var lastMinion =
                        EntityManager.MinionsAndMonsters.GetLaneMinions()
                            .OrderByDescending(m => m.Health)
                            .FirstOrDefault(
                                m =>
                                    m.IsValidTarget(Q.Range) &&
                                    !m.IsInRange(Player.Instance, Player.Instance.GetAutoAttackRange()) &&
                                    m.Health <= Player.Instance.GetSpellDamage(m, SpellSlot.Q));
            if (lastMinion == null) return;

            if (Q.IsReady() && 20 <= Player.Instance.ManaPercent)
            {
                Q.Cast(lastMinion);
            }
        }

        public override void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1160, SkillShotType.Linear, 350, 2000, 65)
            {
                MinimumHitChance = HitChance.High
            };
            W = new Spell.Skillshot(SpellSlot.W, 970, SkillShotType.Linear, 350, 1550, 80)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 470, SkillShotType.Circular, 450, int.MaxValue, 10);
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 1, 2000, 160)
            {
                MinimumHitChance = HitChance.High,
                AllowedCollisionCount = int.MaxValue
            };
        }

        public override void Perma()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && Q.GetRealDamage(target) <= target.Health && 20 <= Player.Instance.ManaPercent)
            {
                Q.Cast(target);
            }
        }
    }
}
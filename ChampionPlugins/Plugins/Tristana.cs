using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;

namespace ChampionPlugins.Plugins
{
    public class Tristana : Plugin
    {
        public static Spell.Active Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E, R;

        public override void Combo()
        {
            var target = TargetSelector.GetTarget(1200, DamageType.Physical);
            if (target == null || !target.IsValidTarget(Player.Instance.AttackRange)) return;



            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
                Orbwalker.ForcedTarget = target;
            }

            if (target.IsValidTarget(Player.Instance.AttackRange))
            {
                Q.Cast();

            }

            if (W.IsReady() && target.IsValidTarget(1200) && target.CountEnemiesInRange(700) <= 2 &&
                (target.HealthPercent < (Player.Instance.HealthPercent - 15)) &&
                !target.IsInRange(Player.Instance, Player.Instance.AttackRange))
            {
                var castpos = Player.Instance.Position.Extend(target.Position, W.Range).To3D();
                if (!castpos.Tower())
                {
                    W.Cast(castpos);
                }
            }
        }

        public override void Harass()
        {
            var target = TargetSelector.GetTarget(Player.Instance.AttackRange, DamageType.Physical);
            if (target == null) return;

  

            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
                Orbwalker.ForcedTarget = target;
            }

            if (target.IsValidTarget(Q.Range) && target.GetBuffCount("tristanaecharge") > 0)
            {
                Q.Cast();
            }
        }

        public override void Flee()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget(R.Range)) return;

            if (Player.Instance.HealthPercent <= 10 && Player.Instance.HealthPercent < target.HealthPercent)
            {
                R.Cast(target);
            }
            //todo: W to current waypoint
        }

        

        public override void LaneClear()
        {
            if (Player.Instance.ManaPercent <= 60) return;
            var minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .FirstOrDefault(m => m.IsValidTarget(Player.Instance.AttackRange));
            if (minion != null)
            {

                if (minion.IsValidTarget(E.Range) && E.IsReady())
                {
                    E.Cast(minion);
                }

                if (minion.IsValidTarget(Q.Range) && Q.IsReady())
                {
                    Q.IsReady();
                }

                var minionE =
                    EntityManager.MinionsAndMonsters.GetLaneMinions()
                        .FirstOrDefault(
                            m => m.IsValidTarget(Player.Instance.AttackRange) && m.GetBuffCount("tristanaecharge") > 0);
                if (minionE != null)
                {
                    Orbwalker.ForcedTarget = minionE;
                }
            }

            var tower = EntityManager.Turrets.Enemies.FirstOrDefault(t => !t.IsDead && t.IsInRange(Player.Instance, 800));
            if (tower != null)
            {

                if (tower.IsInRange(Player.Instance, E.Range) && E.IsReady())
                {
                    E.Cast(tower);
                }

                if (tower.IsInRange(Player.Instance, Q.Range) && Q.IsReady())
                {
                    Q.Cast();
                }
            }
        }

        public override void JungleClear()
        {
        }

        public override void LastHit()
        {
        }

        public override void Init()
        {
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 450, int.MaxValue, 180);
            E = new Spell.Targeted(SpellSlot.E, 550);
            R = new Spell.Targeted(SpellSlot.R, 550);
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
        }

        private void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            E = new Spell.Targeted(SpellSlot.E, 543 + (7*(uint) Player.Instance.Level));
            R = new Spell.Targeted(SpellSlot.R, 543 + (7*(uint) Player.Instance.Level));
        }

        public override void Perma()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (target == null || !target.IsValidTarget(R.Range)) return;

            if (R.IsReady())
            {
                var stacks = target.GetBuffCount("tristanaecharge");
                if (stacks > 0)
                {
                    if (target.Health <= (SpellSlot.E.GetRealDamage(target)*((0.29*stacks) + 1) +
                                          SpellSlot.R.GetRealDamage(target)))
                    {
                        R.Cast(target);
                    }
                }
            }

            if (R.IsReady())
            {
                if (target.Health <= (SpellSlot.R.GetRealDamage(target)) &&
                    target.Health > Player.Instance.TotalAttackDamage)
                {
                    R.Cast(target);
                }
            }

            if (R.IsReady())
            {
                if (target.RPos().AllyTower())
                {
                    R.Cast(target);
                }
            }
        }
    }

    public static class TristanaSpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            var Q = Tristana.Q;
            var E = Tristana.E;
            var W = Tristana.W;
            var R = Tristana.R;
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (Q.IsReady())
            {
                damage += Q.GetRealDamage(target);
            }

            // W
            if (W.IsReady())
            {
                damage += W.GetRealDamage(target);
            }

            // E
            if (E.IsReady())
            {
                damage += E.GetRealDamage(target);
            }

            // R
            if (R.IsReady())
            {
                damage += R.GetRealDamage(target);
            }

            return damage;
        }

        public static float GetRealDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetRealDamage(target);
        }

        public static float GetRealDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            // Helpers
            var spellLevel = Player.Instance.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Magical;
            float damage = 0;

            // Validate spell level
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    damage = new float[] {0, 0, 0, 0, 0}[spellLevel] + 0.0f*Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.W:

                    damage = new float[] {80, 105, 130, 155, 180}[spellLevel] + 0.5f*Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.E:

                    damage =
                        new[]
                        {
                            60 + (0.4f*Player.Instance.TotalAttackDamage),
                            70 + (0.5f*Player.Instance.TotalAttackDamage),
                            80 + (0.7f*Player.Instance.TotalAttackDamage),
                            90 + (0.8f*Player.Instance.TotalAttackDamage),
                            100 + (0.9f*Player.Instance.TotalAttackDamage)
                        }[spellLevel] +
                        0.75f*Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.R:

                    damage = new float[] {300, 400, 500}[spellLevel] + 1f*Player.Instance.TotalMagicalDamage;
                    break;
            }

            if (damage <= 0)
            {
                return 0;
            }

            return Player.Instance.CalculateDamageOnUnit(target, damageType, damage) - 50;
        }

        public static bool Tower(this Vector3 pos)
        {
            return EntityManager.Turrets.Enemies.Where(t => !t.IsDead).Any(d => d.Distance(pos) < 950);
        }

        public static bool AllyTower(this Vector3 pos)
        {
            return EntityManager.Turrets.Allies.Where(t => !t.IsDead).Any(d => d.Distance(pos) < 700);
        }

        public static Vector3 RPos(this Obj_AI_Base unit)
        {
            return unit.Position.Extend(Prediction.Position.PredictUnitPosition(unit, 300), 600).To3D();
        }
    }
}
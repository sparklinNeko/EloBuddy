using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;

namespace ChampionPlugins.Plugins
{
    public class Sivir : Plugin
    {
        public int LastAATime { get; set; }
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Active E { get; private set; }
        public static Spell.Active R { get; private set; }
        public static int LastAggersiveSpell { get; set; }
        public override void Combo()
        {
            if (LastAATime + ObjectManager.Player.AttackDelay * 1000 / 2 > Environment.TickCount)
            {
                var target2 = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
                if (target2 != null && W.IsReady())
                {
                    W.Cast();
                }
                target2 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target2 != null && Q.IsReady())
                {
                    Q.Cast(target2);
                }
            } 
        }

        public override void Harass()
        {
            var target2 = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target2 != null && Q.IsReady())
            {
                Q.Cast(target2);
            }
        }

        public override void Flee()
        {
            if (R.IsReady())
                R.Cast();
        }

        public override void LaneClear()
        {
            int minMinions = 3;
            int laneClearMana = 60;
            var minions = EntityManager.MinionsAndMonsters.GetLineFarmLocation(EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.Position, Q.Range, true), Q.Width, (int)Q.Range);
            if (Q.IsReady() && minions.HitNumber >= minMinions &&
                ObjectManager.Player.ManaPercent >= laneClearMana )
            {
                Q.Cast(minions.CastPosition);
            }
            if (W.IsReady() && EntityManager.MinionsAndMonsters.GetLaneMinions().Count(a => a.Distance(ObjectManager.Player.Position) <= ObjectManager.Player.GetAutoAttackRange()) >= minMinions &&
                laneClearMana < ObjectManager.Player.ManaPercent)
            {
                W.Cast();
            }
        }

        public override void JungleClear()
        {
                
        }

        public override void LastHit()
        {
            
        }

        public override void Perma()
        {
            if (LastAggersiveSpell + 500 > Environment.TickCount && E.IsReady())
            {
                E.Cast();
            }
        }

        public override void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1250, EloBuddy.SDK.Enumerations.SkillShotType.Linear, 250, 1350, 90);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R);
            Q.MinimumHitChance = HitChance.High;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
        }

        void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs args)
        {
            if (Orbwalker.IsAutoAttacking) return;
            if (!W.IsReady()) return;
            if (!target.IsValidTarget(ObjectManager.Player.AttackRange))
            W.Cast();
            Player.IssueOrder(GameObjectOrder.AttackUnit, target);
        }

        void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && !args.SData.IsAutoAttack() && args.Target != null && args.Target.NetworkId == ObjectManager.Player.NetworkId)
            {
                LastAggersiveSpell = Environment.TickCount;
            }
            if (sender.IsMe && args.SData.Name == W.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
            if (sender.IsMe && args.SData.Name == Q.Name)
            {
                Orbwalker.ResetAutoAttack();
            }
        }

        void Orbwalker_OnPostAttack(AttackableUnit target, System.EventArgs args)
        {
            LastAATime = Environment.TickCount;
            var target2 = TargetSelector.GetTarget(ObjectManager.Player.AttackRange, DamageType.Physical);
            if (target2 != null && W.IsReady() && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                W.Cast();
            }
        }

        
    }
}
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;

namespace ChampionPlugins.Plugins
{
    public class Ashe : Plugin
    {
        public static Spell.Active Q;
        public static Spell.Skillshot W, E, R;

        private AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public override void Combo()
        {
            if (Q.IsReady())
            {
                var targetq = TargetSelector.GetTarget(600, DamageType.Physical);

                if (targetq.IsValidTarget(600))
                {
                    if (_Player.GetBuffCount("asheqcastready") >= 5)
                    {
                        Q.Cast();
                    }

                }
            }

            if (W.IsReady())
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                var predW = W.GetPrediction(targetw);

                if (targetw.IsValidTarget(W.Range))
                {
                    if (predW.HitChance >= HitChance.Medium)
                    {
                        W.Cast(predW.CastPosition);
                    }
                }
            }

            if (R.IsReady())
            {
                var targetr = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                var predR = R.GetPrediction(targetr);
                {
                    if (targetr.IsValidTarget(R.Range))
                    {
                        if (targetr.HealthPercent <= 50)
                        {
                            if (predR.HitChance >= HitChance.Medium)
                            {
                                R.Cast(predR.CastPosition);
                            }
                        }
                    }
                }
            }
        }

        public override void Harass()
        {
            if (Q.IsReady())
            {
                var targetq = TargetSelector.GetTarget(_Player.GetAutoAttackRange(), DamageType.Physical);

                if (_Player.GetBuffCount("asheqcastready") >= 5 && targetq.IsValidTarget(_Player.GetAutoAttackRange() - 50))
                {
                    Q.Cast();
                }
            }

            if (W.IsReady())
            {
                var targetw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (targetw.IsValidTarget(W.Range))
                {
                    W.Cast(targetw.Position);
                }
            }
        }

        public override void Flee()
        {
            
        }

        public override void LaneClear()
        {
            int minMinions = 3;
            int laneClearMana = 60;
            var minions = EntityManager.MinionsAndMonsters.EnemyMinions.OrderBy(a => a.Health).FirstOrDefault(
                        a => a.Distance(Player.Instance) <= W.Range && !a.IsDead && !a.IsInvulnerable);
            if (Q.IsReady() && minions.CountEnemiesInRange(_Player.AttackRange) >= minMinions && _Player.ManaPercent >= laneClearMana)
            {
                if (_Player.GetBuffCount("asheqcastready") >= 5)
                {
                    Q.Cast();
                }
            }

            if (W.IsReady() && minions.CountEnemiesInRange(W.Range) >= minMinions && _Player.ManaPercent >= laneClearMana)
            {
                W.Cast(minions);
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
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone);
            E = new Spell.Skillshot(SpellSlot.E, 2500, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 250, 1600, 130);
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;

        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs args)
        {
            var intTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            {
                var prediction = R.GetPrediction(intTarget);
                if (R.IsReady() && sender.IsValidTarget(R.Range) && prediction.HitChance >= HitChance.Medium)
                    R.Cast(prediction.CastPosition);
            }
        }

        private static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            if (ObjectManager.Player.Distance(gapcloser.Sender, true) <
                W.Range*W.Range && sender.IsValidTarget() && sender.IsEnemy)
            {
                W.Cast(gapcloser.Sender);
            }
        }

        public override void Perma()
        {
            
        }
    }
}
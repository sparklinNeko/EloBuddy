using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace ChampionPlugins.Plugins
{
    public class Ryze : Plugin
    {
        public static Spell.Skillshot Q;
        public static Spell.Targeted W, E;
        public static Spell.Active R;
        public static int GetPassiveBuff
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                if (data != null)
                {
                    return data.Count == -1 ? 0 : data.Count == 0 ? 1 : data.Count;
                }
                return 0;
            }
        }
        public override void Combo()
        {
            var target = TargetSelector.GetTarget(570, DamageType.Magical);
            if (target != null)
            {
                var qpred = Q.GetPrediction(target);
                if (target.IsValidTarget(Q.Range))
                {
                    if (GetPassiveBuff <= 2 || !ObjectManager.Player.HasBuff("RyzePassiveStack"))
                    {
                        if (target.IsValidTarget(Q.Range) && Q.IsReady()) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(W.Range) && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(E.Range) && E.IsReady()) E.Cast(target);

                        if (R.IsReady())
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (Q.GetRealDamage(target) + E.GetRealDamage(target)))
                            {
                                if (target.HasBuff("RyzeW")) R.Cast();
                            }
                        }
                    }


                    if (GetPassiveBuff == 3)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (R.IsReady())
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (Q.GetRealDamage(target) + E.GetRealDamage(target)))
                            {
                                if (target.HasBuff("RyzeW")) R.Cast();
                            }
                        }
                    }

                    if (GetPassiveBuff == 4)
                    {
                        if (target.IsValidTarget(W.Range) && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(Q.Range) && Q.IsReady()) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(E.Range) && E.IsReady()) E.Cast(target);

                        if (R.IsReady())
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (Q.GetRealDamage(target) + E.GetRealDamage(target)))
                            {
                                if (target.HasBuff("RyzeW")) R.Cast();
                            }
                        }
                    }

                    if (ObjectManager.Player.HasBuff("ryzepassivecharged"))
                    {
                        if (W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (R.IsReady())
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (Q.GetRealDamage(target) + E.GetRealDamage(target)))
                            {
                                if (target.HasBuff("RyzeW")) R.Cast();
                                if (!E.IsReady() && !Q.IsReady() && !W.IsReady()) R.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                    if (Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                    if (E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);
                }
                if (!R.IsReady() || GetPassiveBuff != 4) return;

                if (Q.IsReady() || W.IsReady() || E.IsReady()) return;

                R.Cast();

            }
        }

        public override void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
        }

        public override void Flee()
        {
            
        }

        public override void LaneClear()
        {
            if (ObjectManager.Player.ManaPercent < 60) return;
            Obj_AI_Base minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
                    600).FirstOrDefault();
            if (minion != null && Player.Instance.ManaPercent > 60)
            {
                if (Q.IsReady())
                {
                    var qpred = Q.GetPrediction(minion);
                    Q.Cast(qpred.UnitPosition);
                }
                if (E.IsReady())
                {
                    E.Cast(minion);
                }
                if (W.IsReady())
                {
                    W.Cast(minion);
                }
                if (R.IsReady() && (GetPassiveBuff >= 4 || ObjectManager.Player.HasBuff("ryzepassivecharged")))
                {
                    R.Cast();
                }
            }
        }

        public override void JungleClear()
        {
            
        }

        public override void LastHit()
        {
            if (ObjectManager.Player.ManaPercent < 60) return;
            Obj_AI_Base minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
                    Q.Range).FirstOrDefault(m => m.Health <= Q.GetRealDamage(m));
            if (Q.IsReady() && minion != null)
                Q.Cast(minion);
            minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
                    W.Range).FirstOrDefault(m => m.Health <= W.GetRealDamage(m));
            if (Q.IsReady() && minion != null)
                W.Cast(minion);
            minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
                    E.Range).FirstOrDefault(m => m.Health <= E.GetRealDamage(m));
            if (Q.IsReady() && minion != null)
                E.Cast(minion);
        }

        public override void Init()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);
        }

        public override void Perma()
        {
            
        }
    }
}